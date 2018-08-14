#!/bin/sh

is_ready() {
    [ $(curl --write-out %{http_code} --silent --output /dev/null http://elasticsearch:9200/_cat/health?h=st) = 200 ] \
    && [ $(curl --write-out %{http_code} --silent --output /dev/null https://cancergov/PublishedContent/List) = 200 ] \
    && [ $(curl --write-out %{http_code} --silent --output /dev/null https://preview-cancergov/PublishedContent/List) = 200 ]
}

# wait until is ready
i=0
while ! is_ready; do
    i=`expr $i + 1`
    if [ $i -ge 10 ]; then
        echo "$(date) - still not ready, giving up"
        exit 1
    fi
    echo "$(date) - waiting to be ready"
    sleep 10
done

#start the script
echo "Loading Live..."
NODE_TLS_REJECT_UNAUTHORIZED=0 BBSOURCEHOST=cancergov BBALIAS=bestbets_live_v1 node index.js
echo "Loading Preview..."
NODE_TLS_REJECT_UNAUTHORIZED=0 BBSOURCEHOST=preview-cancergov BBALIAS=bestbets_preview_v1 node index.js
tail -f /dev/null