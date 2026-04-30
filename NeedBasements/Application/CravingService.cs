using Asuna.CharManagement;
using Asuna.Items;
using NeedBasements.Domain.Addiction;
using NeedBasements.Infrastructure;

namespace NeedBasements.Application
{
    // Per-frame craving driver: fires craving lines when Jenna is unsatisfied and due.
    // Satisfaction is read from the live LimbEffect, never from a mirrored timer.
    internal class CravingService
    {
        private readonly AddictionState _state;
        private readonly HangoverService _hangover;

        internal CravingService(AddictionState state, HangoverService hangover)
        {
            _state = state;
            _hangover = hangover;
        }

        internal void Tick(float deltaTime)
        {
            var jenna = Character.Get(ModConstants.CharacterJenna);
            if (jenna == null) return;

            bool effectActive = PleasureEffect.IsActive(jenna);

            // Polled hangover fallback: sleep can wipe the LimbEffect without firing
            // OnEffectRemoved, and the event subscription may be bound to a stale Jenna
            // instance after scene reloads. Detect the transition here too.
            if (!effectActive && _state.ActiveSubstance != null)
                _hangover.OnPleasureEnded();

            if (effectActive) return;

            if (!_state.TickCraving(deltaTime)) return;

            var stat = jenna.GetStat(ModConstants.StatAddiction);
            if (stat == null || stat.BaseValue < CravingSchedule.AddictionThreshold)
            {
                _state.ScheduleNextCraving(CravingSchedule.DormantInterval);
                return;
            }

            float addictionPercent = stat.BaseValue / AddictionStatFactory.MaxValue;
            _state.ScheduleNextCraving(CravingSchedule.NextInterval(addictionPercent));

            Item.GenerateErrorDialogue(
                jenna,
                CravingLines.RandomLine(addictionPercent),
                CravingLines.EmotionForLevel(addictionPercent));
        }
    }
}
