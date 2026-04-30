using Asuna.CharManagement;
using Asuna.Items;
using NeedBasements.Domain.Addiction;
using NeedBasements.Domain.Substances;
using NeedBasements.Infrastructure;

namespace NeedBasements.Application
{
    // Reacts to any item used: if it is a known substance, applies its effects to Jenna.
    internal class SubstanceUseService
    {
        private readonly AddictionState _state;
        private readonly SubstanceCatalog _catalog;

        internal SubstanceUseService(AddictionState state, SubstanceCatalog catalog)
        {
            _state = state;
            _catalog = catalog;
        }

        internal void OnItemUsed(Item item)
        {
            var substance = _catalog.FindByItemName(item.Name);
            if (substance == null) return;

            Consume(item, substance);
        }

        private void Consume(Item item, Substance substance)
        {
            var jenna = Character.Get(ModConstants.CharacterJenna);
            bool isPleasureActive = PleasureEffect.IsActive(jenna);

            if (!_state.CanConsume(substance, isPleasureActive))
            {
                Item.GenerateErrorDialogue(jenna, BlockedLines.Get(_state.ActiveSubstance.ItemName), "Troubled");
                return;
            }

            var stat = jenna.GetStat(ModConstants.StatAddiction);
            stat.BaseValue += substance.AddictionGain;

            float addictionPercent = stat.BaseValue / AddictionStatFactory.MaxValue;
            float satisfactionDuration = substance.SatisfactionAt(addictionPercent);

            _state.Consume(substance);
            PleasureEffect.ApplyTo(jenna, satisfactionDuration);

            ShowProgressionLine(jenna, substance, (int)stat.BaseValue);
            ShowNegativeReaction(jenna, substance, addictionPercent);

            jenna.Inventory.Remove(item);
        }

        private static void ShowProgressionLine(Character jenna, Substance substance, int level)
        {
            var stage = substance.StageFor(level);
            Item.GenerateErrorDialogue(jenna, stage.Text, stage.Emotion);
        }

        private static void ShowNegativeReaction(Character jenna, Substance substance, float addictionPercent)
        {
            if (addictionPercent <= 0.5f) return;

            string line = addictionPercent > 0.8f ? substance.NegativeSevere : substance.NegativeMild;
            Item.GenerateErrorDialogue(jenna, line, "Sad");
        }
    }
}
