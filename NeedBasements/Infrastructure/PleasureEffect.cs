using System;
using Asuna.CharManagement;
using NeedBasements.Domain.Substances;

namespace NeedBasements.Infrastructure
{
    // Wraps the per-substance pleasure LimbEffect on a character's head. The applied
    // LimbEffect is the single source of truth for "is the substance still active" —
    // never mirror its state in a parallel timer, since the game can wipe the effect
    // (e.g. on sleep) at any time.
    internal static class PleasureEffect
    {
        internal static void ApplyTo(Character character, Substance substance, float duration)
        {
            var effect = LimbEffect.Get(substance.LimbEffectID);
            if (effect == null) return;

            effect.CanDecay  = true;
            effect.DecayTime = duration;

            var head = character.Limbs.GetLimb(LimbType.Head);
            head?.ApplyEffect(effect);
        }

        internal static bool IsActive(Character character)
        {
            var head = character.Limbs.GetLimb(LimbType.Head);
            if (head == null) return false;

            foreach (var effect in head.GetAllEffects(new[] { LimbEffectType.Mental }))
            {
                if (effect != null && IsPleasureEffect(effect.name))
                    return true;
            }
            return false;
        }

        // Subscribes to the holder's OnEffectRemoved and invokes the callback whenever any
        // substance pleasure effect leaves the character (decay or external removal — e.g.
        // sleep). Matches by name prefix so all four substance variants are covered.
        internal static void SubscribeToRemoved(Character character, Action onRemoved)
        {
            character.Limbs.OnEffectRemoved.AddListener(info =>
            {
                if (info?.Effect == null) return;
                if (IsPleasureEffect(info.Effect.name))
                    onRemoved();
            });
        }

        private static bool IsPleasureEffect(string effectName) =>
            effectName != null && effectName.StartsWith(ModConstants.PleasureLimbEffectPrefix);
    }
}
