using Asuna.CharManagement;
using Modding;
using UnityEngine;

namespace NeedBasements.Infrastructure.LimbEffects
{
    internal static class ModLimbEffects
    {
        internal static void RegisterAll(ModManifest manifest)
        {
            var icon = manifest.SpriteResolver.ResolveAsResource(ModConstants.EffectSpritePath);

            Register(ModConstants.PleasureLimbEffectID, "Pleasure", decayTime: 280f, icon: icon);
        }

        private static LimbEffect Register(string id, string displayName, float decayTime, Sprite icon)
        {
            if (LimbEffect.All.TryGetValue(id, out var existing) && existing != null)
                return existing;

            var effect = ScriptableObject.CreateInstance<LimbEffect>();
            effect.name              = id;
            effect.DisplayName       = displayName;
            effect.DisplaySprite     = icon;
            effect.EffectType        = LimbEffectType.Mental;
            effect.CanDecay          = true;
            effect.DecayTime         = decayTime;
            effect.CanStack          = false;
            effect.RemoveOnCombatEnd = false;
            effect.ForceToCertainLimb = true;
            effect.LimbToForce        = LimbType.Head;

            LimbEffect.All[id] = effect;
            return effect;
        }
    }
}
