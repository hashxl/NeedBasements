using Asuna.Items;
using NeedBasements.Domain.Substances;
using UnityEngine;

namespace NeedBasements.Infrastructure
{
    // Registers each Substance as a Consumable in the game's item registry.
    internal static class SubstanceItemRegistry
    {
        internal static void RegisterAll(SubstanceCatalog catalog)
        {
            foreach (var substance in catalog.All)
            {
                if (Item.All.ContainsKey(substance.ItemKey))
                    continue;

                var consumable = ScriptableObject.CreateInstance<Consumable>();
                consumable.Name = substance.ItemName;
                Item.All.Add(substance.ItemKey, consumable);
            }
        }
    }
}
