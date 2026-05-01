namespace NeedBasements.Domain.Substances
{
    internal readonly struct CombatModifier
    {
        internal readonly string StatID;
        internal readonly int    Amount;

        internal CombatModifier(string statId, int amount)
        {
            StatID = statId;
            Amount = amount;
        }
    }
}
