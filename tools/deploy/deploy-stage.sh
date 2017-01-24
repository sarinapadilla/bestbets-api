#!/bin/sh
# Deploy to staging

export SCRIPT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
export RUN_SCRIPTS="../run"
export RUN_LOCATION="bestbets-run"

# Determine what configuration file to use.
configuration="${SCRIPT_PATH}/deploy-stage.config"

if [ ! -f $configuration ]; then
    echo "Configuration file '${configuration}' not found"
fi


#   Need to import:
#       - Version number to run
#       - SSH credentials

# Read configuration
export configData=`cat $configuration`
while IFS='=' read -r name value || [ -n "$name" ]
do
    if [ "$name" = "indexer_server" ];then export indexer_server="$value"; fi
    if [ "$name" = "server_list" ];then export server_list="$value"; fi
    if [ "$name" = "release_version" ];then export release_version="$value"; fi
done <<< "$configData"

IFS=', ' read -r -a server_list <<< "$server_list"

# Check for required config values
if [ -z "$indexer_server" ]; then echo "indexer_server not set, aborting."; exit 1; fi
if [ -z "$server_list" ]; then echo "server_list not set, aborting."; exit 1; fi
if [ -z "$release_version" ]; then echo "release_version not set, aborting."; exit 1; fi

# Check for required environment variables
if [ -z "$DOCKER_USER" ]; then echo "DOCKER_USER not set, aborting."; exit 1; fi
if [ -z "$DOCKER_PASS" ]; then echo "DOCKER_PASS not set, aborting."; exit 1; fi


# Deploy support script collection.
for server in "${server_list[@]}"
do
    echo "Deploying run scripts to ${server}"
    ssh -q ${server} mkdir -p bestbets-run
    scp -q ${RUN_SCRIPTS}/* ${server}:bestbets-run
done

##################################################################
#   Suspend Indexer
##################################################################
echo "Suspending indexers on ${indexer_server}"
ssh -q ${indexer_server} ${RUN_LOCATION}/stop-indexers.sh

##################################################################
#   Per server steps.
##################################################################
for server in "${server_list[@]}"
do


#        Deploy configuration (Write a persistent something or other telling the system which tag it's going to use)

    # Stop existing API container
    ssh -q ${server} ${RUN_LOCATION}/stop-api.sh

    # Pull image for new version (pull version-specific tag)
    imageName="nciwebcomm/bestbets-api:runtime-${release_version}"
    ssh -q ${server} bestbets-run/pull-image.sh $imageName $DOCKER_USER $DOCKER_PASS

#        Pull image for new version (pull version-specific tag)
#            When we run the image, possibly run the indexer first.
#               tools/run/bestbets-indexer.sh bestbets.indexer.config.live (or .preview)
#            This is something we'd want when changing the schema, probably involves introducing a new alias at the same time (said new alias would have no data until the indexer runs)
#
#        Start API via tools/run/bestbets-api.sh bestbets.api.config.live (or .preview)
#
#        Test API availability
#            If all is well,
#                Remove old image
#                Continue on next server.
#            Error:
#                Roll back to previous image

done

#    After all servers are updated, report that deployment has completed.
#
#    Run indexer(?) (Command line switch?)
#

##################################################################
#   Resume Indexer
##################################################################
echo "Resuming indexers on ${indexer_server}"
ssh -q ${indexer_server} ${RUN_LOCATION}/resume-indexers.sh
