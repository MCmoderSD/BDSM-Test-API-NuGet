using MCmoderSD.BdsmTestApi.Data;

namespace MCmoderSD.BdsmTestApi.Enums;

// Ids are not contiguous: 2, 9, 15 and 16 are not used by BDSMTest.org.
public enum Kink
{
    Ageplayer = 1,
    Brat = 3,
    BratTamer = 4,
    DaddyMommy = 5,
    Degrader = 6,
    Dominant = 7,
    Degradee = 8,
    Little = 10,
    Masochist = 11,
    MasterMistress = 12,
    NonMonogamist = 13,
    Owner = 14,
    PrimalHunter = 17,
    Pet = 18,
    PrimalPrey = 19,
    Rigger = 20,
    RopeBunny = 21,
    Sadist = 22,
    Slave = 23,
    Submissive = 24,
    Switch = 25,
    Vanilla = 26,
    Voyeur = 27,
    Exhibitionist = 28,
    Experimentalist = 29
}

public static class KinkExtensions
{
    private static readonly Kink[] AllKinks = Enum.GetValues<Kink>();

    public static IReadOnlyList<Kink> All => AllKinks;

    extension(Kink kink)
    {
        public int GetId() => (int) kink;
        public string GetName(Language language) => kink.GetDocumentation(language).Name;
        public string GetPairDescription(Language language) => kink.GetDocumentation(language).PairDescription;
        public string GetDescription(Language language) => kink.GetDocumentation(language).Description;
        public KinkDocumentation GetDocumentation(Language language) => Documentation.Get(kink, language);
    }

    public static Kink FromId(int id)
    {
        return !TryFromId(id, out var kink) ? throw new ArgumentException($"Invalid kink id: {id}", nameof(id)) : kink;
    }

    public static bool TryFromId(int id, out Kink kink)
    {
        if (Enum.IsDefined((Kink) id))
        {
            kink = (Kink) id;
            return true;
        }

        kink = default;
        return false;
    }
}