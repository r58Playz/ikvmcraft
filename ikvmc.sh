#!/bin/bash

CORLIB_DLL_IMPORTS=$(find 'statics/dotnet/runtimes/browser-wasm/lib/' -type f -name '*.dll' -exec printf -- '-r %s ' {} +)

OUTPUT="$1"
shift

if [ -f "$OUTPUT" ]; then
	echo "[ikvmc] skipping compilation since '$OUTPUT' exists"
	exit
fi

echo "[ikvmc] compiling to '$OUTPUT'"
dotnet statics/ikvm/ikvm-tools/ikvmc.dll \
	-r statics/dotnet/runtimes/browser-wasm/native/System.Private.CoreLib.dll \
	-runtime statics/ikvm/IKVM.Runtime.dll \
	-r statics/ikvm/IKVM.ByteCode.dll \
	-r statics/ikvm/IKVM.CoreLib.dll \
	-r statics/ikvm/IKVM.Java.dll \
	-r statics/ikvm/IKVM.Runtime.dll \
	${CORLIB_DLL_IMPORTS:+$CORLIB_DLL_IMPORTS} \
	-o "$OUTPUT" \
	"$@"
