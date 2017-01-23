# Managing Best Bets Containers

**Scripts in this directory should be deployed together and on the path for the resource account responsbile for running the system.**

The Best Bets start up scripts each take a single parameter consisting of the name of a configuration
file. One of the values in the configuration files is the container name to use for that particular
instance. This allows multiple containers to run from a single image.

For example:
```bash
bestbets.api.sh bestbets.api.config.live
bestbets.api.sh bestbets.api.config.preview
```

## Stopping containers

`halt-container.sh` is provided as a convenience to stop and unload a container in a single command.  The usage
is `halt-container.sh <container_name>`

```bash
./halt-container.sh bestbets-api-live
```

## Best Bets API

Run the Bets API by running the `bestbets.api.sh` script.
* The `bestbets.indexer.sh` script optionally takes the name of a configuration file as an argument.
* If the configuration file argument is omitted, `bestbets.api.configuration.default` is loaded from the
    directory where `bestbets.api.sh` is located.

The Best Bets API uses these configuration values.  All values are required.

container_name=bestbets-api
* **currentTag -** The indexer version number to run. (**NOTE:** If the image for the configured version is not present,
    the run script will fail. This is by design. Only the deployment script should be used to deploy new image versions.)
* **elasticsearch\_user -** An Elasticsearch userid with access to create and modify indices.
* **elasticsearch\_password -** The password corresponding to _elasticsearch\_user_.
* **elasticsearch\_servers -** A comma-separated list of Elasticsearch servers.
* **displayservice\_host -** A server containing Best Bets display data.
* **container\_port -** The container's port number to expose externally (must match the Dockerfile's EXPOSE directive).
* **host\_port -** The host port number which container\_port is mapped to. If the host has multiple IP addresses, host\_port
    may include an IP address in the usual format of IP\_ADDRESS:PORT.


**Sample Configuration File**
```
container_name=bestbets-indexer
currentTag=0.1.22
elasticsearch\_user=ELASTICSEARCH\_READWRITE\_USERID
elasticsearch\_password=ELASTICSEARCH\_READWRITE\_PASSWORD
elasticsearch\_servers=https://localhost/
displayservice\_host=https://www.cancer.gov/
container_port=5006
host_port=5006
```

## Best Bets Indexer

Run the Bets Indexer by running the `bestbets.indexer.sh` script.
* The `bestbets.indexer.sh` script optionally takes the name of a configuration file as an argument.
* If the configuration file argument is omitted, `bestbets.indexer.configuration.default` is loaded from the
    directory where `bestbets.indexer.sh` is located.
* If the file `bestbets.indexer.STOP` exists in the script's directory, then the indexer will not be executed.

The Best Bets Indexer is configured by supplying values in bestbets.indexer.config.  All values are required.

container_name=bestbetsindexer
* **currentTag -** The indexer version number to run. (**NOTE:** If the image for the configured version is not present,
    the run script will fail. This is by design. Only the deployment script should be used to deploy new image versions.)
* **elasticsearch\_user -** An Elasticsearch userid with access to create and modify indices.
* **elasticsearch\_password -** The password corresponding to _elasticsearch\_user_.
* **elasticsearch\_servers -** A comma-separated list of Elasticsearch servers.
* **listingservice\_host -** A server exposing the Published Content Listing service.
* **displayservice\_host -** A server containing Best Bets data.


**Sample Configuration File**
```
container_name=bestbetsindexer
currentTag=0.1.22
elasticsearch\_user=ELASTICSEARCH\_READWRITE\_USERID
elasticsearch\_password=ELASTICSEARCH\_READWRITE\_PASSWORD
elasticsearch\_servers=https://localhost/
listingservice\_host=https://www.cancer.gov/
displayservice\_host=https://www.cancer.gov/
```