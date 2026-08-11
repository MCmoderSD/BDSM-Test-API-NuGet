using System.IO.Compression;
using System.Reflection;
using System.Text.Json;

using MCmoderSD.BdsmTestApi.Enums;
using MCmoderSD.BdsmTestApi.Exceptions;
using MCmoderSD.BdsmTestApi.Internal;

namespace MCmoderSD.BdsmTestApi.Data;

public static class Documentation
{
    private const string ResourceName = "data.json.gz";

    private static readonly Lazy<IReadOnlyDictionary<Language, IReadOnlyDictionary<Kink, KinkDocumentation>>> LazyData = new(Load, LazyThreadSafetyMode.ExecutionAndPublication);

    public static IReadOnlyDictionary<Language, IReadOnlyDictionary<Kink, KinkDocumentation>> Data => LazyData.Value;

    public static KinkDocumentation Get(Kink kink, Language language)
    {
        return !TryGet(kink, language, out var documentation) ? throw new ArgumentException($"No documentation for kink {kink} in language {language}.", nameof(kink)) : documentation;
    }

    public static bool TryGet(Kink kink, Language language, out KinkDocumentation documentation)
    {
        if (Data.TryGetValue(language, out var byKink) && byKink.TryGetValue(kink, out var found))
        {
            documentation = found;
            return true;
        }

        documentation = null!;
        return false;
    }

    private static IReadOnlyDictionary<Language, IReadOnlyDictionary<Kink, KinkDocumentation>> Load()
    {
        Dictionary<string, Dictionary<string, DocumentationDto>>? raw;

        try
        {
            var assembly = typeof(Documentation).GetTypeInfo().Assembly;

            using var stream = assembly.GetManifestResourceStream(ResourceName) ?? throw new BdsmTestApiException($"Embedded resource '{ResourceName}' is missing from the assembly.");
            using var gzip = new GZipStream(stream, CompressionMode.Decompress);

            raw = JsonSerializer.Deserialize(gzip, BdsmJsonContext.Default.DocumentationData);
        }
        catch (Exception exception) when (exception is IOException or InvalidDataException or JsonException)
        {
            throw new BdsmTestApiException("Failed to load the embedded kink documentation.", exception);
        }

        if (raw is null) throw new BdsmTestApiException("The embedded kink documentation is empty.");

        var data = new Dictionary<Language, IReadOnlyDictionary<Kink, KinkDocumentation>>(raw.Count);

        foreach (var (languageCode, entries) in raw)
        {
            if (!LanguageExtensions.TryFromCode(languageCode, out var language)) continue;

            var byKink = new Dictionary<Kink, KinkDocumentation>(entries.Count);

            foreach (var (kinkId, entry) in entries)
            {
                if (!int.TryParse(kinkId, out var id) || !KinkExtensions.TryFromId(id, out var kink)) continue;

                byKink[kink] = new KinkDocumentation(
                    entry.Name ?? string.Empty,
                    entry.PairDesc ?? string.Empty,
                    entry.Description ?? string.Empty);
            }

            data[language] = byKink;
        }

        return data;
    }
}