using Asuna.CharManagement;
using Modding;
using NeedBasements.Domain.Substances;
using UnityEngine;

namespace NeedBasements.Infrastructure.LimbEffects
{
    internal static class ModLimbEffects
    {
        // One pleasure LimbEffect per substance, each carrying its own combat StatModifiers
        // (see README "Combat modifiers" section). Registering distinct effects — instead of
        // one shared effect mutated at apply-time — keeps the SDK's clone-on-Get contract
        // straightforward: each Get returns a clone that already has the right modifiers.
        internal static void RegisterAll(ModManifest manifest, SubstanceCatalog catalog)
        {
            var icon = manifest.SpriteResolver.ResolveAsResource(ModConstants.EffectSpritePath);

            foreach (var substance in catalog.All)
                Register(substance, icon, decayTime: 280f);
        }

        internal static void UnregisterAll(SubstanceCatalog catalog)
        {
            var jenna = Character.Get("Jenna");

            // Remove any active pleasure effects from Jenna
            if (jenna != null)
            {
                var head = jenna.Limbs.GetLimb(LimbType.Head);
                if (head != null)
                {
                    foreach (var substance in catalog.All)
                    {
                        var effect = LimbEffect.Get(substance.LimbEffectID);
                        if (effect != null && head.HasEffect(effect))
                            head.RemoveEffect(effect);
                    }
                }
            }

            // Remove only the mod's pleasure effects from global registry
            var keysToRemove = new System.Collections.Generic.List<string>();
            foreach (var key in LimbEffect.All.Keys)
            {
                if (key.StartsWith(ModConstants.PleasureLimbEffectPrefix))
                    keysToRemove.Add(key);
            }
            foreach (var key in keysToRemove)
                LimbEffect.All.Remove(key);
        }

        private static LimbEffect Register(Substance substance, Sprite icon, float decayTime)
        {
            var id = substance.LimbEffectID;
            if (LimbEffect.All.TryGetValue(id, out var existing) && existing != null)
                return existing;

            var effect = ScriptableObject.CreateInstance<LimbEffect>();
            effect.name              = id;
            effect.DisplayName       = "Pleasure";
            effect.DisplaySprite     = icon;
            effect.EffectType        = LimbEffectType.Mental;
            effect.CanDecay          = true;
            effect.DecayTime         = decayTime;
            effect.CanStack          = false;
            effect.RemoveOnCombatEnd = false;
            effect.ForceToCertainLimb = true;
            effect.LimbToForce        = LimbType.Head;

            if (substance.CombatModifiers != null)
            {
                foreach (var mod in substance.CombatModifiers)
                {
                    effect.StatModifiers.Add(new StatModifierInfo
                    {
                        Type         = StatModifierType.Value,
                        ModifierID   = id + "_" + mod.StatID,
                        StatName     = mod.StatID,
                        ModifyAmount = mod.Amount,
                    });
                }
            }

            LimbEffect.All[id] = effect;
            return effect;
        }
    }
}
