using Asuna.CharManagement;
using UnityEngine;

namespace NeedBasements.Infrastructure
{
    internal static class AddictionStatFactory
    {
        internal const float MaxValue = 200f;

        internal static void RegisterFor(Character character)
        {
            var stat = new Stat
            {
                Name            = "Addicted to substances",
                BaseMax         = (int)MaxValue,
                NotifyOwnerName = true,
                NotifyChanges   = true,
                Description     = "Addicted to substances",
                ID              = ModConstants.StatAddiction,
                DisplayColor    = Color.gray
            };
            stat.Initialize();
            stat.AddToCharacter(character);
            character.GetStat(stat.ID).BaseValue = 0;
        }

        internal static void UnregisterFrom(Character character)
        {
            if (character == null) return;
            var stat = character.GetStat(ModConstants.StatAddiction);
            if (stat != null)
            {
                stat.RemoveFromCharacter();
                // Also remove from global Stat registry
                Stat.All.Remove(ModConstants.StatAddiction);
            }
        }
    }
}
