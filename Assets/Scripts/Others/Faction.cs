public enum Faction { Player, Environment, Enemy };

public static class FactionHelpers
{
    public static bool IsHostile(this Faction f1, Faction f2)
    {
        switch (f1)
        {
            case Faction.Player:
                if (f2 == Faction.Environment) return false;
                return (f2 == Faction.Enemy);
            case Faction.Environment:
                return true;
            case Faction.Enemy:
                return (f2 == Faction.Player);
            default:
                break;
        }

        return false;
    }
}