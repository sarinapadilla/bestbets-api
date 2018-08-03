## This image has the tools
FROM node:8-jessie AS build

## Allows user/jenkins to pass a specific branch in as ENV var
ARG BB_LOADER_BRANCH=master

## Download and install loader
WORKDIR /usr/src
RUN git clone --single-branch -b ${BB_LOADER_BRANCH} https://github.com/nciocpl/bestbets-loader app
WORKDIR /usr/src/app
RUN npm install

FROM node:8-alpine
RUN apk update && apk add ca-certificates curl && rm -rf /var/cache/apk/*
COPY --from=build /usr/src/app /usr/src/app
COPY loader/run-loader.sh /usr/src/app
COPY loader/local.json /usr/src/app/config
RUN chmod +x /usr/src/app/run-loader.sh
COPY _shared/certs/cancergov/cancergov.crt /usr/local/share/ca-certificates
RUN chmod 644 /usr/local/share/ca-certificates/cancergov.crt
COPY _shared/certs/preview-cancergov/preview-cancergov.crt /usr/local/share/ca-certificates
RUN chmod 644 /usr/local/share/ca-certificates/preview-cancergov.crt
RUN update-ca-certificates
WORKDIR /usr/src/app
CMD [ "/usr/src/app/run-loader.sh" ]
