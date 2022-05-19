# DotnetEnvironment

[![Build Status](https://dev.azure.com/denniswitt/DotnetEnvironment/_apis/build/status/wittdennis.DotnetEnvironment?branchName=master)](https://dev.azure.com/denniswitt/DotnetEnvironment/_build/latest?definitionId=8&branchName=master) 
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=wittdennis_DotnetEnvironment&metric=coverage)](https://sonarcloud.io/summary/new_code?id=wittdennis_DotnetEnvironment)

DotnetEnvironment is an opinionated package to retrieve information about the current environment an application is running in.

## How do I get started?

DotnetEnvironment is a static class that can be used to retrieve information like if the application is in development mode:

```csharp
if (DotnetEnvironment.IsDevelopment)
{
    // do stuff
}
```

Or get a directory where your application can store its log files (cross-platform):

```csharp
string logDirectory = DotnetEnvironment.GetLogDirectory();
```

## Where can I get it?

First, [install NuGet](https://docs.microsoft.com/en-us/nuget/install-nuget-client-tools). Then, install the package by either using dotnet:

```bash
> dotnet add package Pseud0R4ndom.DotnetEnvironment
```

or the package manager console:

```bash
PM> Install-Package Pseud0R4ndom.DotnetEnvironment
```

## License, etc

DotnetEnvironment is Copyright © 2022 Dennis Witt under the [MIT license](https://github.com/wittdennis/DotnetEnvironment/blob/master/LICENSE)