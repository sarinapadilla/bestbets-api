#!/bin/sh
# Stops indexers from running. When this script exists, no indexers are running.

export SCRIPT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Block new indexer instances from launching.
# Assumes that this script lives in the same folder as the ones which run the indexers.
touch "${SCRIPT_PATH}/bestbets.indexer.STOP"


# Wait for existing indexer instances to finish.
# TODO: Make this list configurable.
indexers=("bestbets.indexer.live" "bestbets.indexer.preview")
for container in "${indexers[@]}"
do
    containerID=$(docker ps --filter name=$container -q -a)
    while [ "$containerID" != "" ]
    do
        echo "Waiting for ${container}"
        sleep 30
        containerID=$(docker ps --filter name=$container -q -a)
    done
done
