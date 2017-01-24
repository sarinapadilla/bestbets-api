#!/bin/sh
# Allows indexers to run again.

export SCRIPT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Allow new indexer instances to launch.
# Assumes that this script lives in the same folder as the ones which run the indexers.
rm "${SCRIPT_PATH}/bestbets.indexer.STOP"
