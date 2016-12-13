# bestbets-svc
## Build
```
cd bestbets-api
dotnet restore #install NuGet packages
cd test/NCI.OCPL.Api.BestBets.Tests
dotnet build # builds all projects (test are dependent on src)
dotnet test #runs unit tests
```

