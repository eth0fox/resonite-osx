#!/bin/zsh
set -exo pipefail
SCRIPT_PATH=$(realpath "$(dirname "$0")")

# You might need to change these!
export FROOXENGINE_PATH="$SCRIPT_PATH/resonite"
export NATIVES_PATH="$SCRIPT_PATH/binaries"
export RENDERIDE_PATH="$SCRIPT_PATH/renderide/target/dev-fast/renderide-renderer"


# You shouldn't need to change these!
export QUEUE_NAME=reso
export RENDERIDE_INTERPROCESS_DIR="$(dirname $(getconf DARWIN_USER_DIR))/T/"
export DYLD_LIBRARY_PATH=$NATIVES_PATH:$FROOXENGINE_PATH/runtimes/osx-arm64/native:$FROOXENGINE_PATH/runtimes/osx-universal/native:$FROOXENGINE_PATH/runtimes/osx/native:/opt/homebrew/lib


function cleanup {
	# clean up old SHM files
	rm -rf $RENDERIDE_INTERPROCESS_DIR/$QUEUE_NAME* || true;
}
function on_exit {
	cleanup
	pkill renderide-renderer ResoniteOSXRunner;
}

trap on_exit EXIT
cleanup

dotnet run --project ./ResoniteOSXRunner -- \
	-shmprefix $QUEUE_NAME \
	-rendererpath $(realpath "$(dirname "$0")/renderide_wrapper.sh") \
	-forcealtaudio \
	-donotautoloadhome
