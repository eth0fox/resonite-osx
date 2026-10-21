#!/bin/zsh
unset DYLD_LIBRARY_PATH

renderer_dir="$(dirname "$0")/resonite/Renderer"
wine --env "WINEDLLPATH=$renderer_dir:$CX_ROOT/lib/wine" \
	"$renderer_dir/Renderite.Renderer.exe" "$@"
