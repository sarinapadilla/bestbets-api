FROM microsoft/dotnet:2.0-sdk AS bestbets-build-env
COPY ./src /bestbets-api/src
COPY ./test /bestbets-api/test
COPY ./bestbets-api.sln /bestbets-api
WORKDIR /bestbets-api
RUN dotnet publish -c Release -o /bestbets-api/output

# Build runtime image
FROM microsoft/dotnet:2.0-runtime
COPY ./tools/integration-tests/Docker/_shared/certs/cancergov/cancergov.crt /usr/local/share/ca-certificates
RUN chmod 644 /usr/local/share/ca-certificates/cancergov.crt
COPY ./tools/integration-tests/Docker/_shared/certs/preview-cancergov/preview-cancergov.crt /usr/local/share/ca-certificates
RUN chmod 644 /usr/local/share/ca-certificates/preview-cancergov.crt
RUN update-ca-certificates
COPY --from=bestbets-build-env /bestbets-api/output .
COPY ./tools/integration-tests/Docker/bestbets-api/appsettings.inttest.json .
COPY ./tools/integration-tests/Docker/bestbets-api/hosting.json .
## This does not need to wait for the loader or other resources.
## Any integration tests will need to wait for the API to report being healthy
EXPOSE 5000
ENV ASPNETCORE_ENVIRONMENT=inttest
ENV ASPNETCORE_URLS="http://*:5000"
ENTRYPOINT ["dotnet", "NCI.OCPL.Api.BestBets.dll"]
