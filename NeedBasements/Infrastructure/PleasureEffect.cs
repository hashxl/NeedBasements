using System;
using Asuna.CharManagement;

namespace NeedBasements.Infrastructure
{
    // Wraps the pleasure LimbEffect on a character's head. The applied LimbEffect is the
    // single source of truth for "is the substance still active" — never mirror its state
    // in a parallel timer, since the game can wipe the effect (e.g. on sleep) at any time.
    internal static class PleasureEffect
    {
        internal static void ApplyTo(Character character, float duration)
        {
            var effect = LimbEffect.Get(ModConstants.PleasureLimbEffectID);
            if (effect == null) return;

            effect.CanDecay  = true;
            effect.DecayTime = duration;

            var head = character.Limbs.GetLimb(LimbType.Head);
            head?.ApplyEffect(effect);
        }

        internal static bool IsActive(Character character)
        {
            var effect = LimbEffect.Get(ModConstants.PleasureLimbEffectID);
            if (effect == null) return false;

            var head = character.Limbs.GetLimb(LimbType.Head);
            return head != null && head.HasEffect(effect);
        }

        // Subscribes to the holder's OnEffectRemoved and invokes the callback whenever the
        // pleasure effect leaves the character (decay or external removal — e.g. sleep).
        internal static void SubscribeToRemoved(Character character, Action onRemoved)
        {
            character.Limbs.OnEffectRemoved.AddListener(info =>
            {
                if (info?.Effect == null) return;
                if (info.Effect.name == ModConstants.PleasureLimbEffectID)
                    onRemoved();
            });
        }
    }
}
