#!/bin/bash

DOTNET_ROOT="$(dirname "$(readlink -f "$(command -v dotnet)")")"
# Compile bundles against the targeting/ref pack so Java Throwable params resolve to
# [System.Runtime]System.Exception — matching IKVM.Java and the loader. Referencing the runtime
# pack's System.Private.CoreLib instead bakes in [System.Private.CoreLib]System.Exception, which the
# loader (compiled against the ref pack) can't bind to.
REF_DIR="$(ls -d "$DOTNET_ROOT"/packs/Microsoft.NETCore.App.Ref/*/ref/net10.0 2>/dev/null | sort -V | tail -1)"
CORLIB_DLL_IMPORTS=$(find "$REF_DIR" -type f -name '*.dll' -exec printf -- '-r %s ' {} +)

OUTPUT="$1"
shift

if [ -f "$OUTPUT" ]; then
	echo "[ikvmc] skipping compilation since '$OUTPUT' exists"
	exit
fi

echo "[ikvmc] compiling to '$OUTPUT'"
dotnet statics/ikvm/ikvm-tools/ikvmc.dll \
	-runtime statics/ikvm/IKVM.Runtime.dll \
	-r statics/ikvm/IKVM.ByteCode.dll \
	-r statics/ikvm/IKVM.CoreLib.dll \
	-r statics/ikvm/IKVM.Java.dll \
	-r statics/ikvm/IKVM.Runtime.dll \
	${CORLIB_DLL_IMPORTS:+$CORLIB_DLL_IMPORTS} \
	-o "$OUTPUT" \
	"$@"
