#!/bin/zsh
set -exo pipefail
declare -a extraLaunchArgs=("$@")



# You might need to change these!
export FROOXENGINE_PATH="${FROOXENGINE_PATH:-$SCRIPT_PATH/resonite}"
export NATIVES_PATH="${NATIVES_PATH:-$SCRIPT_PATH/binaries}"
reso_tempdir="${RESO_TEMP_DIR:-$TMPDIR}"

extraLaunchArgs+=-forcealtaudio # Force use of SoundFlow audio driver. Seems to work (at least for a bit) on macOS where NAudio doesn't at all.
extraLaunchArgs+=-donotautoloadhome # Don't automatically open the cloud home. Makes launching a bit faster and less resource intensive.


# You shouldn't need to change these!
export QUEUE_NAME="${QUEUE_NAME:-reso}"
export OPENLIPSYNC_MODEL_PATH="${OPENLIPSYNC_MODEL_PATH:-$FROOXENGINE_PATH/RuntimeData/OpenLipSync}"
export DYLD_LIBRARY_PATH=$NATIVES_PATH:$FROOXENGINE_PATH/runtimes/osx-arm64/native:$FROOXENGINE_PATH/runtimes/osx-universal/native:$FROOXENGINE_PATH/runtimes/osx/native:/opt/homebrew/lib

if [ "${USE_TMPFS:-0}" = "1" ]; then

	# Check to see if we've already got a mount at /private/tmp/reso-tmpfs
	tmpfs_folder="${RESO_TEMP_DIR:-/private/tmp/reso-tmpfs/$(id -u)}"
	if ! mount | grep -q $tmpfs_folder; then
		sudo mkdir -p $tmpfs_folder
		sudo chown $(id -u):$(id -g) $tmpfs_folder
		sudo mount_tmpfs $tmpfs_folder
		sudo chown $(id -u):$(id -g) $tmpfs_folder
	fi
	reso_tempdir="$tmpfs_folder"

fi;

# check if RML exists next to the script.
if [ -f "$SCRIPT_PATH/ResoniteModLoader.dll" ]; then
	extraLaunchArgs+=-loadassembly 
	extraLaunchArgs+=$SCRIPT_PATH/ResoniteModLoader.dll
fi


function cleanup {
	# clean up old SHM files
	rm -rf "$reso_tempdir/$QUEUE_NAME"* || true;
}
function on_exit {
	cleanup
	pkill $(basename "$RENDERER_PATH") ResoniteOSXRunner;
}

trap on_exit EXIT
trap on_exit INT
cleanup


dotnet build ./ResoniteOSXRunner

TMPDIR=$reso_tempdir \
RENDERIDE_INTERPROCESS_DIR=$reso_tempdir \
	./ResoniteOSXRunner/bin/Debug/net10.0/ResoniteOSXRunner \
	-rendererpath "$SCRIPT_PATH/scripts/renderer-wrapper.sh" \
	-shmprefix $QUEUE_NAME \
	${extraLaunchArgs[@]} 
	
	
