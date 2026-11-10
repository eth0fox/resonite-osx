#!/bin/zsh
unset DYLD_LIBRARY_PATH

if [ -n "$WINEPATH" ]; then
	$WINEPATH --env "WINEDLLPATH=$WINEDLLPATH" "$RENDERER_PATH" "$@"
else
	"$RENDERER_PATH" "$@"
fi;
