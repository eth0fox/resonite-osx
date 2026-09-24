#!/bin/zsh
set -exo pipefail
SCRIPT_PATH=$(realpath "$(dirname "$0")")
declare -a EXTRA_LAUNCH_ARGS=("$@")



# You might need to change these!
export FROOXENGINE_PATH="$SCRIPT_PATH/resonite"
export NATIVES_PATH="$SCRIPT_PATH/binaries"
export RENDERIDE_PATH="$SCRIPT_PATH/renderide/target/release/renderide-renderer"

EXTRA_LAUNCH_ARGS+=-forcealtaudio # Force use of SoundFlow audio driver. Seems to work (at least for a bit) on macOS where NAudio doesn't at all.
EXTRA_LAUNCH_ARGS+=-donotautoloadhome # Don't automatically open the cloud home. Makes launching a bit faster and less resource intensive.


export CX_APP_BUNDLE_PATH=/Applications/CrossOver.app
export CX_BOTTLE_PATH="$HOME/Library/Application Support/CrossOver/Bottles"
export CX_BOTTLE=Resonite
export CX_MANAGED_BOTTLE_PATH="/Library/Application Support/CrossOver/Bottles"
export CX_ROOT="/Applications/CrossOver.app/Contents/SharedSupport/CrossOver"
export PATH="/Applications/CrossOver.app/Contents/SharedSupport/CrossOver/bin:$PATH"

# You shouldn't need to change these!
export QUEUE_NAME=reso
# Share the renderer's backing files, not just its named semaphores.
export TMPDIR="$CX_BOTTLE_PATH/$CX_BOTTLE/drive_c/users/crossover/AppData/Local/Temp/"
export RENDERIDE_INTERPROCESS_DIR="$TMPDIR"
export DYLD_LIBRARY_PATH=$NATIVES_PATH:$FROOXENGINE_PATH/runtimes/osx-arm64/native:$FROOXENGINE_PATH/runtimes/osx-universal/native:$FROOXENGINE_PATH/runtimes/osx/native:/opt/homebrew/lib
export OPENLIPSYNC_MODEL_PATH=$FROOXENGINE_PATH/RuntimeData/OpenLipSync

# check if RML exists next to the script.
if [ -f "$SCRIPT_PATH/ResoniteModLoader.dll" ]; then
	EXTRA_LAUNCH_ARGS+=-loadassembly 
	EXTRA_LAUNCH_ARGS+=$SCRIPT_PATH/ResoniteModLoader.dll
fi


function cleanup {
	# clean up old SHM files
	rm -rf "$RENDERIDE_INTERPROCESS_DIR/$QUEUE_NAME"* || true;
}
function on_exit {
	cleanup
	pkill Renderite.exe ResoniteOSXRunner;
}

trap on_exit EXIT
trap on_exit INT
cleanup



dotnet run --project ./ResoniteOSXRunner -- ${EXTRA_LAUNCH_ARGS[@]} \
	-shmprefix $QUEUE_NAME \
	-rendererpath $(realpath "$(dirname "$0")/renderite_wrapper.sh")
	
