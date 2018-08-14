## Information about this test suite configuration

### Containers
These are the containers that run the integration tests.
* bestbets-api - This is the API. 
  * Dockerfile building expects context to be root of solution.
* cancergov - This is a fake version of www.cancer.gov and preview.cancer.gov. 
  * No Dockerfile, uses httpd:2.4-alpine image.
* elasticsearch - This is the elasticsearch container that will store the data.
  * Dockerfile is empty for now as we may need to add plugins. 
* loader - This container runs the bestbets-loader in order to populate data in the elasticsearch container
  * The Dockerfile takes an argument, BB_LOADER_BRANCH, to determine the branch/tag to clone from. The default branch is master.
* tester - This container runs the integration tests.

#### SSL
The best bets loader only allows for SSL source and as that is how production will be configured, we are going to keep it that way. The API can use both, but once again production will be SSL, let's keep it that way. In the _shared/certs folder is a self-signed cert that can be installed in the API and loader images. The cancergov container uses the cancergov and preview-cancergov certs for exposing SSL.

## Running the Tests in Docker
### Requirements
* Docker for Mac/Windows or just Docker for Linux
  * Whatever it is, you need a working docker-compose (1.22.0 or later)

### Executing
1. 

## Developing & Testing Tests
The integration tests have been written using Karate as the testing framework. See https://github.com/intuit/karate for more details.

### Requirements
* JDK 8
* Maven
* Docker for Mac/Windows or just Docker for Linux
  * Whatever it is, you need a working docker-compose (1.22.0 or later)

### Executing
1. Start up the "swarm" by running `docker-compose -f tools/integration-test-docker/docker-compose.test-dev.yml up -d --build`
1. Modify the karate tests in integration-tests
1. Run the tests
   1. `cd integration-tests/bestbets-integration-tests`
   1. `mvn clean test -DargLine="-Dkarate.env=local"`
1. Shut down the "swarm" by running `docker-compose -f tools/integration-test-docker/docker-compose.test-dev.yml down`
1. Try the complete tests by following the instructions above


#### Forcing Rebuild of Containers
The following command will be needed if you modify the loader source in github. Normally the --build option to docker-compose will rebuild the images if the API source or integration tests have changed. Basically, if the files are local and have changed, you do NOT need this. If you are cloning from GitHub or doing an npm install, then you definately need to make sure you have rebuilt the images.
`docker-compose -f tools/integration-test-docker/docker-compose.test-dev.yml build --no-cache --force-rm --pull`





