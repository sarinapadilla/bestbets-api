
## Running the Tests in Docker
### Requirements
* Docker for XXX or Docker Toolkit (specifically docker-compose)

### Images
The images are located in the folders under the Docker folder
* bestbets-api - This is the API image
* cancergov - This is a fake version of 
* elasticsearch
* loader
* tester

#### SSL
The best bets loader only allows for SSL sources, and as that is how production will be configured, we are going to keep it that way. In the _shared/certs folder is a self-signed cert that can be installed in the API and loader images. Luckily, the API does not need SSL and so we won't need to modify the java keystore of the tester image.

### Executing
1. 

## Developing & Testing Tests
### Requirements
* JDK 8
* Maven
* A local running Best Bets API on http://localhost:5000

### Executing
1. 

