# NL.Serverless.AspNetCore
This repo provides code for hosting an AspNet Core App inside an Azure Function V2 HTTP Trigger without the use of TestServer.

I combined the solution of <a href="https://github.com/NicklausBrain">NicklausBrain</a> (original repo can be found <a href="https://github.com/NicklausBrain/serverless-core-api">here</a>) with the recently added <a href="https://docs.microsoft.com/en-US/azure/azure-functions/functions-dotnet-dependency-injection">DI support for azure functions</a>. This provides a way to avoid initalizing the AspNet Core App with every incoming request.

## Instructions
TODO

## Credits
Most of the code is copy pasta from this repo:
https://github.com/NicklausBrain/serverless-core-api

Thanks <a href="https://github.com/NicklausBrain">NicklausBrain</a>.
