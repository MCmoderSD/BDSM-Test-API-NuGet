namespace MCmoderSD.BdsmTestApi.Enums;

public enum Language
{
    English,
    Spanish,
    Portuguese,
    French,
    German,
    Italian,
    Polish,
    Dutch,
    Russian,
    Turkish,
    Chinese,
    Japanese,
    Ukrainian,
    Hungarian,
    Czech,
    Korean,
    Thai,
    Vietnamese
}

public static class LanguageExtensions
{
    private static readonly Language[] AllLanguages = Enum.GetValues<Language>();

    public static IReadOnlyList<Language> All => AllLanguages;

    public static string GetCode(this Language language) => language switch
    {
        Language.English => "en",
        Language.Spanish => "es",
        Language.Portuguese => "pt",
        Language.French => "fr",
        Language.German => "de",
        Language.Italian => "it",
        Language.Polish => "pl",
        Language.Dutch => "nl",
        Language.Russian => "ru",
        Language.Turkish => "tr",
        Language.Chinese => "zh",
        Language.Japanese => "ja",
        Language.Ukrainian => "uk",
        Language.Hungarian => "hu",
        Language.Czech => "cs",
        Language.Korean => "ko",
        Language.Thai => "th",
        Language.Vietnamese => "vi",
        _ => throw new ArgumentOutOfRangeException(nameof(language), language, "Invalid language.")
    };

    public static Language FromCode(string code)
    {
        return !TryFromCode(code, out var language) ? throw new ArgumentException($"Invalid language code: {code}", nameof(code)) : language;
    }

    public static bool TryFromCode(string? code, out Language language)
    {
        if (!string.IsNullOrWhiteSpace(code))
        {
            var trimmed = code.Trim();
            foreach (var candidate in AllLanguages)
            {
                if (!string.Equals(candidate.GetCode(), trimmed, StringComparison.OrdinalIgnoreCase)) continue;
                language = candidate;
                return true;
            }
        }

        language = default;
        return false;
    }
}