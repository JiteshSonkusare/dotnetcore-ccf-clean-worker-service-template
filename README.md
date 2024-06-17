[![Build](https://github.com/JiteshSonkusare/dotnetcore-ccf-clean-worker-service-template/actions/workflows/release.yml/badge.svg)](https://github.com/JiteshSonkusare/dotnetcore-ccf-clean-worker-service-template/actions/workflows/release.yml) [![license](https://img.shields.io/github/license/JiteshSonkusare/dotnetcore-ccf-clean-worker-service-template?color=blue&label=license&logo=Github&style=flat-square)](https://github.com/JiteshSonkusare/dotnetcore-ccf-clean-worker-service-template/blob/master/README.md) [![nuget](https://img.shields.io/nuget/v/CCF.Clean.Dotnet.WorkerService?label=version&logo=NuGet&style=flat-square)](https://www.nuget.org/packages/CCF.Clean.Dotnet.WorkerService) [![nuget](https://img.shields.io/nuget/dt/CCF.Clean.Dotnet.WorkerService?color=blue&label=downloads&logo=NuGet&style=flat-square)](https://www.nuget.org/packages/CCF.Clean.Dotnet.WorkerService)

# CCF Clean Dotnet Worker Service Template

CCF Clean Dotnet Worker Service nuget template is ready-to-use project template for creating dotnet core worker service using Clean Architecture, leveraging ccf clean worker service features.

## Key features

- [x] Clean Architecture
    - [x] MediatR
    - [x] Repositories (Generic Repositories)
    - [x] Model Mapping (Automapper)
- [x] Dotnet Core Worker Service
- [x] Entity Framework Core (Database)
- [x] Exception Handling
- [x] Structured Logging with NLog
- [x] Dependency Injection
- [x] Generic API Http Client Handler
- [x] Option Pattern
- [x] Result Pattern
- [x] Qaurtz.Net
- [x] Polly

## Supported Versions

- [x] .NET 8.0

## Getting started

1. Install CCF Clean Worker Service Template

    ```
    dotnet new install CCF.Clean.Dotnet.WorkerService
    ```
    > NOTE: The template only needs to be installed once. Running this command again will update your version of the template. Specify the version to get specific version of template.

2. Create a new directory

    ```    
    mkdir CCFDemoWorkerService
    cd CCFDemoWorkerService
    ```

3. Create a new solution

    ```
    dotnet new CCFClean.WorkerService --name {{SolutionName}} --output .\
    ```
    > NOTE: Specify {{SolutionName}}, this will be used as the solution name and project namespaces.
