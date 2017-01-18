# Configuration 
# Best Bets Indexer

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