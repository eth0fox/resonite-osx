#!/bin/zsh
set -exo pipefail
export SCRIPT_PATH=$(realpath "$(dirname "$0")")

export RENDERER_PATH="${RENDERER_PATH:-$SCRIPT_PATH/renderide/target/release/renderide-renderer}"

$SCRIPT_PATH/scripts/run-common.sh $@