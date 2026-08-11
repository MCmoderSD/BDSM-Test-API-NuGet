namespace MCmoderSD.BdsmTestApi.Enums;

public enum AgeGroup
{
    Under20 = 0,
    Age20To22 = 1,
    Age23To25 = 2,
    Age26To30 = 3,
    Age31To35 = 4,
    Age36To40 = 5,
    Age41To50 = 6,
    Age51To60 = 7,
    Age61To75 = 8,
    Above75 = 9
}

public static class AgeGroupExtensions
{
    private static readonly AgeGroup[] AllAgeGroups = Enum.GetValues<AgeGroup>();

    public static IReadOnlyList<AgeGroup> All => AllAgeGroups;

    extension(AgeGroup ageGroup)
    {
        public int GetId() => (int) ageGroup;

        public int GetMinAge() => ageGroup switch
        {
            AgeGroup.Under20 => 0,
            AgeGroup.Age20To22 => 20,
            AgeGroup.Age23To25 => 23,
            AgeGroup.Age26To30 => 26,
            AgeGroup.Age31To35 => 31,
            AgeGroup.Age36To40 => 36,
            AgeGroup.Age41To50 => 41,
            AgeGroup.Age51To60 => 51,
            AgeGroup.Age61To75 => 61,
            AgeGroup.Above75 => 76,
            _ => throw new ArgumentOutOfRangeException(nameof(ageGroup), ageGroup, "Invalid age group.")
        };

        public int GetMaxAge() => ageGroup switch
        {
            AgeGroup.Under20 => 19,
            AgeGroup.Age20To22 => 22,
            AgeGroup.Age23To25 => 25,
            AgeGroup.Age26To30 => 30,
            AgeGroup.Age31To35 => 35,
            AgeGroup.Age36To40 => 40,
            AgeGroup.Age41To50 => 50,
            AgeGroup.Age51To60 => 60,
            AgeGroup.Age61To75 => 75,
            AgeGroup.Above75 => int.MaxValue,
            _ => throw new ArgumentOutOfRangeException(nameof(ageGroup), ageGroup, "Invalid age group.")
        };
    }

    public static AgeGroup FromId(int id)
    {
        return !TryFromId(id, out var ageGroup) ? throw new ArgumentException($"Invalid age group id: {id}", nameof(id)) : ageGroup;
    }

    public static bool TryFromId(int id, out AgeGroup ageGroup)
    {
        if (Enum.IsDefined((AgeGroup) id))
        {
            ageGroup = (AgeGroup) id;
            return true;
        }

        ageGroup = default;
        return false;
    }

    public static AgeGroup FromAge(int age)
    {
        if (age < 0) throw new ArgumentOutOfRangeException(nameof(age), age, "Age cannot be negative.");

        foreach (var ageGroup in AllAgeGroups)
        {
            if (age >= ageGroup.GetMinAge() && age <= ageGroup.GetMaxAge()) return ageGroup;
        }

        // Unreachable: the ranges cover every non-negative age.
        throw new ArgumentOutOfRangeException(nameof(age), age, "Invalid age.");
    }
}