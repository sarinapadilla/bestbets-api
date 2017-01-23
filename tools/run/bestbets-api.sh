#!/bin/sh
export SCRIPT_PATH="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
export BASE_INDEXER_IMAGE="nciwebcomm/bestbets-api:runtime"

# Is indexing enabled?
if [ -f "${SCRIPT_PATH}/bestbets.indexer.STOP" ]; then echo "Best bets indexer disabled. Exiting."; exit 0; fi

# Determine what configuration file to use.
if [ -n "$1" ]; then
    configuration=$1
else
    configuration="${SCRIPT_PATH}/bestbets.indexer.config.default"
fi
if [ ! -f $configuration ]; then
    echo "Configuration file '${configuration}' not found"
fi


# Read configuration
export configData=`cat $configuration`
while IFS='=' read -r name value || [ -n "$name" ]
do
    if [ $name = "container_name" ];then export container_name="$value"; fi
    if [ $name = "currentTag" ];then export currentTag="$value"; fi
    if [ $name = "elasticsearch_user" ];then export elasticsearch_user="$value"; fi
    if [ $name = "elasticsearch_password" ];then export elasticsearch_password="$value"; fi
    if [ $name = "elasticsearch_servers" ];then export elasticsearch_servers="$value"; fi
    if [ $name = "displayservice_host" ];then export displayservice_host="$value"; fi
done <<< "$configData"

# Check for required config values
if [ -z "$container_name" ]; then echo "container_name not set, aborting."; exit 1; fi
if [ -z "$currentTag" ]; then echo "currentTag not set, aborting."; exit 1; fi
if [ -z "$elasticsearch_user" ]; then echo "elasticsearch_user not set, aborting."; exit 1; fi
if [ -z "$elasticsearch_password" ]; then echo "elasticsearch_password not set, aborting."; exit 1; fi
if [ -z "$elasticsearch_servers" ]; then echo "elasticsearch_servers not set, aborting."; exit 1; fi
if [ -z "$displayservice_host" ]; then echo "displayservice_host not set, aborting."; exit 1; fi

# Does the image exist?
export imageName="${BASE_INDEXER_IMAGE}-${currentTag}"
export imageID=$(docker images -q $imageName)
if [ -z "$imageID" ]; then
    echo "Image $imageName not available. Aborting."
    exit 1
fi

docker run --name ${container_name} \
    -d -p 5006:5006 \
    -e CGBestBetsDisplayService__Host="${displayservice_host}" \
    -e Elasticsearch__Servers="${elasticsearch_servers}" \
    -e Elasticsearch__Userid="${elasticsearch_user}" \
    -e Elasticsearch__Password="${elasticsearch_password}" \
    $imageName
