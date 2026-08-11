# BDSM Test API

## Description
A simple .NET wrapper for fetching results from [BDSMTest.org](https://bdsmtest.org)

This is the .NET port of the [Java version](https://github.com/MCmoderSD/BDSM-Test-Api).

## Features
- Fetch any BDSMTest.org result by its ID
- Multiple language support
- Typed results (no manual JSON parsing)
- Works out of the box, no setup needed
- Fetch a match between two results
- No third party dependencies, trimming and AOT friendly

## Usage

### NuGet
Make sure you have my Sonatype Nexus OSS repository added, either in a `NuGet.config`
next to your solution:
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
    <packageSources>
        <add key="Nexus" value="https://mcmodersd.de/nexus/repository/nuget-hosted/index.json" />
    </packageSources>
</configuration>
```
or from the command line:
```bash
dotnet nuget add source https://mcmodersd.de/nexus/repository/nuget-hosted/index.json --name Nexus
```

Add the package to your project:
```bash
dotnet add package BDSM-Test-API --version 1.0.0
```
or in your `.csproj` file:
```xml
<ItemGroup>
    <PackageReference Include="BDSM-Test-API" Version="1.0.0" />
</ItemGroup>
```

### Usage Example

### Fetch a result by its ID
```csharp
using MCmoderSD.BdsmTestApi.Core;
using MCmoderSD.BdsmTestApi.Enums;

// Initialize the API
var api = new BdsmTestApi();
var result = await api.FetchResultAsync("your_result_id_here"); // Replace with your actual result ID
var language = Language.English;

// Print the result
Console.WriteLine($"Result ID: {result.Id}");
Console.WriteLine($"Version: {result.Version}");
Console.WriteLine($"Gender: {result.Gender}");
Console.WriteLine($"Age Group: {result.AgeGroup}");
Console.WriteLine($"Timestamp: {result.Timestamp}");
Console.WriteLine($"Language: {result.Language}");

// Print scores
foreach (var score in result.Scores)
{
    Console.WriteLine($"{score.GetName(language)}: {score.Value}%");
    Console.WriteLine($"- {score.GetPairDescription(language)}");
    Console.WriteLine($"- {score.GetDescription(language)}");
    Console.WriteLine();
}
```

### Fetch a match between two results
```csharp
using MCmoderSD.BdsmTestApi.Core;

// Initialize the API
var api = new BdsmTestApi();

// Get user input for result IDs
Console.Write("Enter your result ID: ");
var yourId = Console.ReadLine()!.Trim();

Console.Write("Enter your partner's result ID: ");
var partnerId = Console.ReadLine()!.Trim();

// Fetch and display the match result
var match = await api.FetchMatchAsync(yourId, partnerId);
Console.WriteLine($"Your Compatibility Score: {match.Score}%");
```

Runnable versions of both examples live under [`Sample/`](Sample):
```bash
dotnet run --project Sample/FetchExample
dotnet run --project Sample/MatchExample
```

## Notes

### Package ID and namespace
The package is published as `BDSM-Test-API`, matching the Java artifact, while the code
lives under `MCmoderSD.BdsmTestApi.*`. The `MCmoderSD.` prefix is required: a bare
`BdsmTestApi` namespace would shadow the `BdsmTestApi` client type in every consuming
project, so `new BdsmTestApi()` would not compile.

### Async only
Every call hits the network, so the public API is async. There is no synchronous
wrapper on purpose: blocking on the returned `Task` deadlocks in some sync contexts.

### Reusing an `HttpClient`
`new BdsmTestApi()` uses a shared, static `HttpClient`, so creating clients freely is
safe. In an ASP.NET Core app you can hand it a client from `IHttpClientFactory`:
```csharp
builder.Services.AddHttpClient<BdsmTestApi>();
```
The library never disposes a client it did not create.

### Translating without extra requests
The localized kink names and descriptions for all 18 languages ship inside the package
as a gzipped JSON resource, so `score.GetName(Language.German)` and friends never cause
another request. The same data is reachable directly:
```csharp
using MCmoderSD.BdsmTestApi.Data;
using MCmoderSD.BdsmTestApi.Enums;

var documentation = Documentation.Get(Kink.RopeBunny, Language.German);
Console.WriteLine(documentation.Name);
```

### Custom auth signature
The default auth signature is anonymous and enough for reading results. If you need a
different one:
```csharp
var api = new BdsmTestApi("your_auth_signature_here");
```

### Errors
Invalid arguments throw `ArgumentException`. Anything that goes wrong while talking to
BDSMTest.org, including unknown result IDs, throws
`MCmoderSD.BdsmTestApi.Exceptions.BdsmTestApiException`, which carries the HTTP status code and
the raw response body when it has them.

## License
BSD 3-Clause, see [LICENSE](LICENSE).
