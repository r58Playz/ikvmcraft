#!/usr/bin/env python3
"""Drives downloading source JARs, running ikvmc per bundle, and emitting the
runtime manifest + MSBuild include consumed by the loader.

Single source of truth for which Java libraries are AOT-compiled into managed
assemblies. Loader picks bundles at MC-launch time by exact-GAV match against
the version JSON's libraries; mismatches fall back to URLClassLoader (JIT).
"""

from __future__ import annotations

import argparse
import json
import os
import subprocess
import sys
import urllib.request
from dataclasses import dataclass, field
from pathlib import Path

REPO_ROOT = Path(__file__).resolve().parent

MAVEN_CENTRAL = "https://repo1.maven.org/maven2"


@dataclass
class Bundle:
    name: str
    output_dir: str          # "jars" or "statics" — relative to repo root
    output_dll: str          # filename ikvmc writes (and the assembly's name)
    jars: list[str]          # "group:artifact:version" or "file:path/relative/to/repo"
    prefixes: list[str]      # class-name prefixes this DLL serves
    aot: bool = True
    always_replace: bool = False


BUNDLES: list[Bundle] = [
    # used in the loader
    Bundle(
        name="asm",
        output_dir="jars",
        output_dll="ikvmc_asm-9.7.1.dll",
        jars=[
            "org.ow2.asm:asm:9.7.1",
            "org.ow2.asm:asm-tree:9.7.1",
            "org.ow2.asm:asm-analysis:9.7.1",
            "org.ow2.asm:asm-commons:9.7.1",
        ],
        prefixes=["org.objectweb.asm"],
        aot=True,
    ),

    # used by minecraft
    Bundle(
        name="lwjgl3",
        output_dir="statics",
        output_dll="ikvmc_lwjgl3-3.2.2.dll",
        jars=["file:statics/lwjgl3-3.2.2.jar"],
        prefixes=["org.lwjgl."],
        aot=True,
        always_replace=True,
    ),
    Bundle(
        name="log4j",
        output_dir="jars",
        output_dll="ikvmc_log4j-2.17.1.dll",
        jars=[
            "org.apache.logging.log4j:log4j-api:2.17.1",
            "org.apache.logging.log4j:log4j-core:2.17.1",
        ],
        prefixes=["org.apache.logging.log4j."],
        aot=False,  # log4j has plenty of probes that misbehave under AOT
    ),
    Bundle(
        name="guava",
        output_dir="jars",
        output_dll="ikvmc_guava-21.0.dll",
        jars=[
            "com.google.guava:guava:21.0",
        ],
        prefixes=["com.google.common."],
        aot=True,
    ),
    Bundle(
        name="fastutil",
        output_dir="jars",
        output_dll="ikvmc_fastutil-8.2.1.dll",
        jars=[
            "it.unimi.dsi:fastutil:8.2.1",
        ],
        prefixes=["it.unimi.dsi.fastutil."],
        aot=True,
    ),
]


@dataclass
class ResolvedJar:
    """One entry in a bundle's jar list, resolved to local + (optional) maven GAV."""
    local_path: Path                # absolute path
    gav: tuple[str, str, str] | None  # None for file: refs
    relative_path: str | None       # maven-layout path (matches MC version json), None for file: refs
    url: str | None                 # download URL, None for file: refs


def parse_jar_ref(ref: str) -> ResolvedJar:
    if ref.startswith("file:"):
        local = (REPO_ROOT / ref[len("file:") :]).resolve()
        return ResolvedJar(local_path=local, gav=None, relative_path=None, url=None)

    parts = ref.split(":")
    if len(parts) != 3:
        raise ValueError(f"bad maven coord '{ref}' — expected group:artifact:version")
    group, artifact, version = parts
    relative = f"{group.replace('.', '/')}/{artifact}/{version}/{artifact}-{version}.jar"
    local = REPO_ROOT / "jars" / f"{artifact}-{version}.jar"
    url = f"{MAVEN_CENTRAL}/{relative}"
    return ResolvedJar(
        local_path=local,
        gav=(group, artifact, version),
        relative_path=relative,
        url=url,
    )


def fetch(bundles: list[Bundle]) -> None:
    """Download every maven-sourced jar into jars/. file: refs are checked but never downloaded."""
    for bundle in bundles:
        for ref in bundle.jars:
            resolved = parse_jar_ref(ref)
            if resolved.local_path.exists():
                continue
            if resolved.url is None:
                print(
                    f"[fetch] missing non-maven jar {resolved.local_path} for bundle '{bundle.name}'",
                    file=sys.stderr,
                )
                sys.exit(1)
            resolved.local_path.parent.mkdir(parents=True, exist_ok=True)
            print(f"[fetch] {resolved.url} -> {resolved.local_path}")
            urllib.request.urlretrieve(resolved.url, resolved.local_path)


def ikvmc(bundles: list[Bundle]) -> None:
    """Invoke ikvmc.sh once per bundle. The script itself skips if the output already exists."""
    ikvmc_sh = REPO_ROOT / "ikvmc.sh"
    if not ikvmc_sh.exists():
        print(f"[ikvmc] missing {ikvmc_sh}", file=sys.stderr)
        sys.exit(1)

    for bundle in bundles:
        out_path = REPO_ROOT / bundle.output_dir / bundle.output_dll
        out_path.parent.mkdir(parents=True, exist_ok=True)

        input_paths = [str(parse_jar_ref(ref).local_path) for ref in bundle.jars]
        cmd = ["bash", str(ikvmc_sh), str(out_path), *input_paths]
        print(f"[ikvmc] {bundle.name}: {' '.join(cmd)}")
        subprocess.run(cmd, check=True, cwd=REPO_ROOT)


def emit_manifest(bundles: list[Bundle]) -> None:
    """Emit loader/ikvmc-manifest.g.json — runtime side reads this as an embedded resource."""
    out_path = REPO_ROOT / "loader" / "ikvmc-manifest.g.json"

    manifest = {"bundles": []}
    for bundle in bundles:
        jar_entries = []
        for ref in bundle.jars:
            resolved = parse_jar_ref(ref)
            if resolved.gav is None:
                continue
            group, artifact, version = resolved.gav
            jar_entries.append({
                "group": group,
                "artifact": artifact,
                "version": version,
                "relativePath": resolved.relative_path,
            })

        assembly_name = bundle.output_dll
        if assembly_name.endswith(".dll"):
            assembly_name = assembly_name[: -len(".dll")]

        manifest["bundles"].append({
            "name": bundle.name,
            "assemblyName": assembly_name,
            "prefixes": list(bundle.prefixes),
            "alwaysReplace": bundle.always_replace,
            "jars": jar_entries,
        })

    out_path.parent.mkdir(parents=True, exist_ok=True)
    out_path.write_text(json.dumps(manifest, indent=2) + "\n")
    print(f"[manifest] wrote {out_path}")


def emit_targets(bundles: list[Bundle]) -> None:
    """Emit loader/Generated.targets — MSBuild include with Reference + AOTWhitelist + EmbeddedResource."""
    out_path = REPO_ROOT / "loader" / "Generated.targets"

    references: list[str] = []
    aot_whitelist: list[str] = []
    for bundle in bundles:
        rel = f"..\\{bundle.output_dir}\\{bundle.output_dll}"
        references.append(f'    <Reference Include="{rel}" />')

        assembly_name = bundle.output_dll
        if assembly_name.endswith(".dll"):
            assembly_name = assembly_name[: -len(".dll")]
        if bundle.aot:
            aot_whitelist.append(f'    <AOTWhitelist Include="{assembly_name}" />')

    lines = [
        "<!-- Generated by scripts/build-ikvmc.py. Do not edit by hand. -->",
        "<Project>",
        "  <ItemGroup>",
        *references,
        "",
        *aot_whitelist,
        "",
        '    <EmbeddedResource Include="ikvmc-manifest.g.json">',
        "      <LogicalName>IkvmWasm.ikvmc-manifest.json</LogicalName>",
        "    </EmbeddedResource>",
        "  </ItemGroup>",
        "</Project>",
        "",
    ]
    out_path.write_text("\n".join(lines))
    print(f"[targets] wrote {out_path}")


def cmd_all() -> None:
    fetch(BUNDLES)
    ikvmc(BUNDLES)
    emit_manifest(BUNDLES)
    emit_targets(BUNDLES)


def cmd_emit_only() -> None:
    """Re-emit manifest + targets without running ikvmc (useful when only metadata changed)."""
    emit_manifest(BUNDLES)
    emit_targets(BUNDLES)


def main(argv: list[str]) -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument(
        "command",
        nargs="?",
        default="all",
        choices=["all", "fetch", "ikvmc", "manifest", "targets", "emit"],
    )
    args = parser.parse_args(argv)

    dispatch = {
        "all": cmd_all,
        "fetch": lambda: fetch(BUNDLES),
        "ikvmc": lambda: ikvmc(BUNDLES),
        "manifest": lambda: emit_manifest(BUNDLES),
        "targets": lambda: emit_targets(BUNDLES),
        "emit": cmd_emit_only,
    }
    dispatch[args.command]()
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv[1:]))
