#!/bin/zsh
set -exo pipefail
export SCRIPT_PATH=$(realpath "$(dirname "$0")")



export CX_APP_BUNDLE_PATH="${CX_APP_BUNDLE_PATH:-/Applications/CrossOver.app}" 
export CX_ROOT="$CX_APP_BUNDLE_PATH/Contents/SharedSupport/CrossOver" 
export CX_BOTTLE_PATH="$HOME/Library/Application Support/CrossOver/Bottles" 
export CX_BOTTLE=${CX_BOTTLE:-Resonite} 

export RENDERER_PATH="${RENDERER_PATH:-$SCRIPT_PATH/resonite/Renderer/Renderite.Renderer.exe}"
export RESO_TEMP_DIR="${RESO_TEMP_DIR:-$CX_BOTTLE_PATH/$CX_BOTTLE/drive_c/users/crossover/AppData/Local/Temp/}"

export WINEPATH="$CX_ROOT/bin/wine" 
export WINEDLLPATH="$(dirname $RENDERER_PATH):$CX_ROOT/lib/wine" 


$SCRIPT_PATH/scripts/run-common.sh $@