#!/bin/sh
# Stops API instances from running.

# TODO: Make this list configurable.
apiInstances=("bestbets.api.live" "bestbets.api.preview")
for container in "${apiInstances[@]}"
do
    ./halt-container.sh $container
done