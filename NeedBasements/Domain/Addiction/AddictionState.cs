using NeedBasements.Domain.Substances;

namespace NeedBasements.Domain.Addiction
{
    // Ephemeral per-character addiction state: craving timing, currently active substance, purchase count.
    // Satisfaction itself is NOT tracked here — the LimbEffect on Jenna's head is the single source of truth
    // (so sleep/heal that wipes the effect can't desync from a parallel timer here).
    internal class AddictionState
    {
        private float _cravingTimer;
        private float _nextCravingInterval = 60f;
        private Substance _activeSubstance;
        private int _purchaseCount;
        private bool _hasMetVendor;
        private bool _hasSeenPriceHike;
        private int _relapsCount;
        private Substance _lastSubstance;
        private bool _cravingFired;
        private float _maxAddictionReached;
        private const float RelapsThreshold = 120f;  // Only counts as relapse if was at 50+ addiction

        internal Substance ActiveSubstance => _activeSubstance;
        internal int PurchaseCount => _purchaseCount;
        internal bool HasMetVendor => _hasMetVendor;
        internal void MarkVendorMet() => _hasMetVendor = true;
        internal bool HasSeenPriceHike => _hasSeenPriceHike;
        internal void MarkPriceHikeSeen() => _hasSeenPriceHike = true;
        internal int RelapsCount => _relapsCount;

        // Returns true exactly once per craving window, then false until next interval.
        internal bool TickCraving(float deltaTime)
        {
            _cravingTimer += deltaTime;
            if (_cravingTimer < _nextCravingInterval)
                return false;

            // Fire exactly once when interval is reached
            if (!_cravingFired)
            {
                _cravingFired = true;
                return true;
            }

            return false;
        }

        // Reset the craving fire flag when scheduling next craving
        internal void ScheduleNextCraving(float interval)
        {
            _nextCravingInterval = interval;
            _cravingTimer = 0f;
            _cravingFired = false;
        }


        internal void Consume(Substance substance)
        {
            // Check for relapse: consuming after abstinence from a HIGH addiction.
            // Only counts if previous peak addiction was >= threshold.
            if (_lastSubstance != null && _lastSubstance.ItemKey != substance.ItemKey && _maxAddictionReached >= RelapsThreshold)
            {
                // Switched substances after real abstinence (from high addiction) — increment relaps count.
                _relapsCount++;
            }

            _activeSubstance = substance;
            _lastSubstance = substance;
            _cravingTimer = 0f;
        }

        // Track the maximum addiction reached to determine if relapse counts
        internal void UpdateMaxAddiction(float currentAddiction)
        {
            if (currentAddiction > _maxAddictionReached)
                _maxAddictionReached = currentAddiction;
        }

        // Called when the pleasure LimbEffect ends (decay OR sleep wiping it).
        // Drops the active substance and resets craving accumulator so abstinence can begin.
        internal void ClearActive()
        {
            _activeSubstance = null;
            _cravingTimer = 0f;
        }

        // Get the addiction multiplier based on relaps count (2x, 3x, capped).
        internal float GetAddictionMultiplier() =>
            _relapsCount switch
            {
                0 => 1f,
                1 => 2f,
                _ => 3f  
            };

        // Reset relaps count and max addiction when addiction drops to 0 (successful abstinence).
        internal void ResetRelapsCount()
        {
            _relapsCount = 0;
            _maxAddictionReached = 0f;
        }

        internal void RegisterPurchase() => _purchaseCount++;

        // Mixing rule: while satisfied, only the same substance can be re-consumed.
        internal bool CanConsume(Substance substance, bool isPleasureActive)
        {
            if (!isPleasureActive) return true;
            if (_activeSubstance == null) return true;
            return _activeSubstance.ItemKey == substance.ItemKey;
        }
    }
}
