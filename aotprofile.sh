#!/bin/bash

OUT="$(realpath "$1")"
shift
IN="$(realpath "$1")"
shift

if [ -f "$OUT" ]; then
	echo "[aotprofile] skipping generation since '$OUT' exists"
	exit
fi

cd aotprofile || exit 1

dotnet run -- "$OUT" "$IN" "$@" >/dev/null
