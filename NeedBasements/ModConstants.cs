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


        // Custom mental LimbEffects registered at mod load.
        // One pleasure effect per substance — each carries its own combat StatModifiers.
        // The shared prefix lets us detect "any substance pleasure is active" without
        // hard-coding the four IDs in every check.
        internal const string PleasureLimbEffectPrefix    = "LmbEffect_SubstancePleasure_";
        internal const string CigarPleasureLimbEffectID     = "LmbEffect_SubstancePleasure_Cigar";
        internal const string CigarettePleasureLimbEffectID = "LmbEffect_SubstancePleasure_Cigarette";
        internal const string CannabisPleasureLimbEffectID  = "LmbEffect_SubstancePleasure_Cannabis";
        internal const string PillsPleasureLimbEffectID     = "LmbEffect_SubstancePleasure_Pills";

        // Built-in stat IDs used as targets for the substance combat modifiers.
        internal const string StatLustDefense    = "stat_lust_defense";
        internal const string StatLustPower      = "stat_lust_power";
        internal const string StatPhysicalPower  = "stat_physical_power";
        internal const string StatPhysicalDef    = "stat_physical_defense";
        internal const string StatEnergy         = "stat_energy";
        internal const string StatMovementSpeed  = "stat_speed";

        // Built-in game effect (note: prefix is "LimbEffect_", not "LmbEffect_")
        internal const string GropedLimbEffectID         = "LimbEffect_Groped";

        // Vendor doubles his prices once Jenna's addiction crosses this threshold.
        internal const float  PriceHikeAddictionThreshold = 120f;
        internal const float  PriceHikeMultiplier         = 2f;
    }
}
