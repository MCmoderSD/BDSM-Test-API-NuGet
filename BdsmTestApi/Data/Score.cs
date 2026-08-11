using MCmoderSD.BdsmTestApi.Enums;

namespace MCmoderSD.BdsmTestApi.Data;

public sealed record Score(Kink Kink, string Name, string PairDescription, string Description, int Value)
{
    public string GetName(Language language) => Documentation.Get(Kink, language).Name;

    public string GetPairDescription(Language language) => Documentation.Get(Kink, language).PairDescription;

    public string GetDescription(Language language) => Documentation.Get(Kink, language).Description;
}