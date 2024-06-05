
public enum Faction { Player, Environment, Enemy };

public static class FactionHelpers
{
    public static bool IsHostile(this Faction f1, Faction f2)
    {
        switch (f1)
        {
            case Faction.Player:
                return (f2 == Faction.Enemy);
            case Faction.Environment:
                return (f2 != Faction.Environment);
            case Faction.Enemy:
                return (f2 == Faction.Player);
            default:
                break;
        }

        return false;
    }
}