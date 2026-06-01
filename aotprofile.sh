#!/bin/bash

OUT="$(realpath "$1")"
shift
IN="$(realpath "$1")"
shift

if [ -f "$OUT" ]; then
	echo "[aotprofile] skipping generation since '$OUT' exists"
	exit
fi

# Resolve "@<path>" filter args to absolute paths while still in the repo root,
# since the program runs from aotprofile/ where the relative path would break.
ARGS=()
for arg in "$@"; do
	if [[ "$arg" == @* ]]; then
		ARGS+=("@$(realpath "${arg:1}")")
	else
		ARGS+=("$arg")
	fi
done

cd aotprofile || exit 1

dotnet run -- "$OUT" "$IN" "${ARGS[@]}" >/dev/null
