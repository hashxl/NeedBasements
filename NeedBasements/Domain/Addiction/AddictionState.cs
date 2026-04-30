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

        internal Substance ActiveSubstance => _activeSubstance;
        internal int PurchaseCount => _purchaseCount;
        internal bool HasMetVendor => _hasMetVendor;
        internal void MarkVendorMet() => _hasMetVendor = true;
        internal bool HasSeenPriceHike => _hasSeenPriceHike;
        internal void MarkPriceHikeSeen() => _hasSeenPriceHike = true;

        // Returns true exactly once per craving window.
        internal bool TickCraving(float deltaTime)
        {
            _cravingTimer += deltaTime;
            if (_cravingTimer < _nextCravingInterval)
                return false;
            _cravingTimer = 0f;
            return true;
        }

        internal void ScheduleNextCraving(float interval) => _nextCravingInterval = interval;

        internal void Consume(Substance substance)
        {
            _activeSubstance = substance;
            _cravingTimer = 0f;
        }

        // Called when the pleasure LimbEffect ends (decay OR sleep wiping it).
        // Drops the active substance and resets craving accumulator so abstinence can begin.
        internal void ClearActive()
        {
            _activeSubstance = null;
            _cravingTimer = 0f;
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
