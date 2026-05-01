using Asuna.CharManagement;
using Asuna.Items;
using NeedBasements.Domain.Addiction;
using NeedBasements.Infrastructure;

namespace NeedBasements.Application
{
    // Per-frame craving driver: fires craving lines when Jenna is unsatisfied and due.
    // Satisfaction is read from the live LimbEffect, never from a mirrored timer.
    // Also handles abstinence decay: addiction decreases over time when not consuming.
    internal class CravingService
    {
        private readonly AddictionState _state;
        private readonly HangoverService _hangover;
        private float _abstinenceTimer;

        internal CravingService(AddictionState state, HangoverService hangover)
        {
            _state = state;
            _hangover = hangover;
        }

        internal void Tick(float deltaTime)
        {
            var jenna = Character.Get(ModConstants.CharacterJenna);
            if (jenna == null) return;

            var stat = jenna.GetStat(ModConstants.StatAddiction);
            if (stat == null) return;

            bool effectActive = PleasureEffect.IsActive(jenna);

            // Polled hangover fallback: sleep can wipe the LimbEffect without firing
            // OnEffectRemoved, and the event subscription may be bound to a stale Jenna
            // instance after scene reloads. Detect the transition here too.
            if (!effectActive && _state.ActiveSubstance != null)
                _hangover.OnPleasureEnded();

            // Tick abstinence decay while not using (and not in the middle of a pleasure effect)
            if (!effectActive)
                TickAbstinenceDecay(stat, deltaTime);

            if (effectActive) return;

            if (!_state.TickCraving(deltaTime)) return;

            if (stat.BaseValue < CravingSchedule.AddictionThreshold)
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

        private void TickAbstinenceDecay(Stat addictionStat, float deltaTime)
        {
            _abstinenceTimer += deltaTime;

            // Determine decay interval based on current addiction level
            float decayInterval = GetDecayInterval(addictionStat.BaseValue);
            if (_abstinenceTimer < decayInterval)
                return;

            // Decay: subtract 1 point per interval
            addictionStat.BaseValue = UnityEngine.Mathf.Max(0, addictionStat.BaseValue - 3);
            _abstinenceTimer = 0f;
        }

        private float GetDecayInterval(float addiction)
        {
            if (addiction < 70f)
                return 5f * 60f;   // 5 minutes = 300 seconds
            if (addiction < 120f)
                return 8f * 60f;   // 8 minutes = 480 seconds
            return 10f * 60f;      // 10 minutes = 600 seconds
        }
    }
}
