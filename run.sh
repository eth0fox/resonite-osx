#!/bin/zsh
set -exo pipefail
SCRIPT_PATH=$(realpath "$(dirname "$0")")
declare -a EXTRA_LAUNCH_ARGS=("$@")



# You might need to change these!
export FROOXENGINE_PATH="$SCRIPT_PATH/resonite"
export NATIVES_PATH="$SCRIPT_PATH/binaries"
export RENDERIDE_PATH="$SCRIPT_PATH/renderide/target/dev-fast/renderide-renderer"

EXTRA_LAUNCH_ARGS+=-forcealtaudio # Force use of SoundFlow audio driver. Seems to work (at least for a bit) on macOS where NAudio doesn't at all.
EXTRA_LAUNCH_ARGS+=-donotautoloadhome # Don't automatically open the cloud home. Makes launching a bit faster and less resource intensive.




# You shouldn't need to change these!
export QUEUE_NAME=reso
export RENDERIDE_INTERPROCESS_DIR="$(dirname $(getconf DARWIN_USER_DIR))/T/"
export DYLD_LIBRARY_PATH=$NATIVES_PATH:$FROOXENGINE_PATH/runtimes/osx-arm64/native:$FROOXENGINE_PATH/runtimes/osx-universal/native:$FROOXENGINE_PATH/runtimes/osx/native:/opt/homebrew/lib

# check if RML exists next to the script.
if [ -f "$SCRIPT_PATH/ResoniteModLoader.dll" ]; then
	EXTRA_LAUNCH_ARGS+=-loadassembly 
	EXTRA_LAUNCH_ARGS+=$SCRIPT_PATH/ResoniteModLoader.dll
fi


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



dotnet run --project ./ResoniteOSXRunner -- ${EXTRA_LAUNCH_ARGS[@]} \
	-shmprefix $QUEUE_NAME \
	-rendererpath $(realpath "$(dirname "$0")/renderide_wrapper.sh") 
	