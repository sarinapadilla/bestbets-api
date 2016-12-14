@echo off
setlocal
set opencover=%localAppData%\Apps\OpenCover\OpenCover.Console.exe
set filters=+[NCI.OCPL.*]* -[NCI.OCPL.*.Tests]*
echo on
%opencover% -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test ..\test\NCI.OCPL.Api.BestBets.Indexer.Tests" -register:user -output:coverage1.xml -oldstyle  -filter:"%filters%"
%opencover% -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test ..\test\NCI.OCPL.Api.BestBets.Tests" -register:user -output:coverage2.xml -oldstyle  -filter:"%filters%"
%opencover% -target:"C:\Program Files\dotnet\dotnet.exe" -targetargs:"test ..\test\NCI.OCPL.Services.CDE.PublishedContentListing.Tests" -register:user -output:coverage3.xml -oldstyle  -filter:"%filters%"

set reportgenerator=%localAppData%\Apps\ReportGenerator\ReportGenerator.exe
set report_out=reports
if exist %report_out% rd %report_out% /q/set
mkdir %report_out%
%reportgenerator% -reports:coverage1.xml;coverage2.xml;coverage3.xml -targetdir:%report_out% 

