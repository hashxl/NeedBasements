namespace NeedBasements
{
    internal static class ModConstants
    {
        internal const string CharacterJenna     = "Jenna";
        internal const string CharacterVendorMan = "Man";
        internal const string StatAddiction      = "stat_unique_modded_smoke";
        internal const string LevelToSell        = "Carceburg";
        internal const string SpritePath         = "assets\\sprites\\npc\\base.png";
        internal const string EffectSpritePath   = "assets\\sprites\\effects\\base.png";


        // Custom mental LimbEffects registered at mod load
        internal const string PleasureLimbEffectID       = "LmbEffect_SubstancePleasure";

        // Built-in game effect (note: prefix is "LimbEffect_", not "LmbEffect_")
        internal const string GropedLimbEffectID         = "LimbEffect_Groped";

        // Vendor doubles his prices once Jenna's addiction crosses this threshold.
        internal const float  PriceHikeAddictionThreshold = 120f;
        internal const float  PriceHikeMultiplier         = 2f;
    }
}
