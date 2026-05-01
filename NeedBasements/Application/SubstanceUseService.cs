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
            float addictionMultiplier = _state.GetAddictionMultiplier();
            int addictionGain = (int)(substance.AddictionGain * addictionMultiplier);
            stat.BaseValue += addictionGain;

            // Reset relaps if addiction drops to 0 (successful abstinence cycle).
            if (stat.BaseValue <= 0)
            {
                stat.BaseValue = 0;
                _state.ResetRelapsCount();
            }

            float addictionPercent = stat.BaseValue / AddictionStatFactory.MaxValue;
            float satisfactionDuration = substance.SatisfactionAt(addictionPercent);

            _state.Consume(substance);
            PleasureEffect.ApplyTo(jenna, substance, satisfactionDuration);

            ShowProgressionLine(jenna, substance, (int)stat.BaseValue);
            ShowCombatModifiers(jenna, substance);
            ShowNegativeReaction(jenna, substance, addictionPercent);

            jenna.Inventory.Remove(item);
        }

        private static void ShowProgressionLine(Character jenna, Substance substance, int level)
        {
            var stage = substance.StageFor(level);
            Item.GenerateErrorDialogue(jenna, stage.Text, stage.Emotion);
        }

        private static void ShowCombatModifiers(Character jenna, Substance substance)
        {
            if (substance.CombatModifiers == null || substance.CombatModifiers.Length == 0)
                return;

            var stat = jenna.GetStat(ModConstants.StatAddiction);
            float addictionPercent = stat?.BaseValue / AddictionStatFactory.MaxValue ?? 0f;

            // Combat modifier multiplier scales with addiction: 2x at medium, 3x at high
            float modifierMultiplier = 1f;
            if (addictionPercent >= 0.66f)      // high addiction (66%+)
                modifierMultiplier = 3f;
            else if (addictionPercent >= 0.33f) // medium addiction (33%+)
                modifierMultiplier = 2f;

            var lines = new System.Collections.Generic.List<string>();
            foreach (var mod in substance.CombatModifiers)
            {
                int scaledAmount = (int)(mod.Amount * modifierMultiplier);
                string prefix = scaledAmount > 0 ? "+" : "";
                string effect = scaledAmount > 0 ? "BUFF" : "DEBUFF";
                string multiplierNote = modifierMultiplier > 1f ? $" (x{modifierMultiplier})" : "";
                lines.Add($"[{effect}] {prefix}{scaledAmount} {StatDisplayName(mod.StatID)}{multiplierNote}");
            }

            string modifiersText = string.Join(" | ", lines);
            Item.GenerateErrorDialogue(jenna, modifiersText, "Happy");
        }

        private static string StatDisplayName(string statId) => statId switch
        {
            ModConstants.StatLustDefense    => "Lust Defense",
            ModConstants.StatLustPower      => "Lust Power",
            ModConstants.StatPhysicalPower  => "Physical Power",
            ModConstants.StatPhysicalDef    => "Physical Defense",
            ModConstants.StatEnergy         => "Energy",
            ModConstants.StatMovementSpeed  => "Movement Speed",
            _ => statId
        };

        private static void ShowNegativeReaction(Character jenna, Substance substance, float addictionPercent)
        {
            if (addictionPercent <= 0.5f) return;

            string line = addictionPercent > 0.8f ? substance.NegativeSevere : substance.NegativeMild;
            Item.GenerateErrorDialogue(jenna, line, "Sad");
        }
    }
}
