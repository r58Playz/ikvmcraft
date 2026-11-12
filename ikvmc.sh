#!/bin/bash

DOTNET_ROOT="$(dirname "$(readlink -f "$(command -v dotnet)")")"
REF_DIR="$(ls -d "$DOTNET_ROOT"/packs/Microsoft.NETCore.App.Ref/*/ref/net11.0 2>/dev/null | sort -V | tail -1)"
CORLIB_DLL_IMPORTS=$(find "$REF_DIR" -type f -name '*.dll' -exec printf -- '-r %s ' {} +)

OUTPUT="$1"
shift

if [ -f "$OUTPUT" ]; then
	echo "[ikvmc] skipping compilation since '$OUTPUT' exists"
	exit
fi

echo "[ikvmc] compiling to '$OUTPUT'"
dotnet statics/ikvm/ikvm-tools/ikvmc.dll \
	-optimize \
	-runtime statics/ikvm/IKVM.Runtime.dll \
	-r statics/ikvm/IKVM.ByteCode.dll \
	-r statics/ikvm/IKVM.CoreLib.dll \
	-r statics/ikvm/IKVM.Java.dll \
	-r statics/ikvm/IKVM.Runtime.dll \
	${CORLIB_DLL_IMPORTS:+$CORLIB_DLL_IMPORTS} \
	-o "$OUTPUT" \
	"$@"
