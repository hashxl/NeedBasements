using UnityEngine;

namespace NeedBasements.Domain.Addiction
{
    internal static class CravingSchedule
    {
        internal const float DormantInterval = 600f;
        internal const float AddictionThreshold = 10f;

        // Interval between cravings: 600s at 0% addiction → 45s at 100%, with random variance
        internal static float NextInterval(float addictionPercent)
        {
            float baseInterval = Mathf.Lerp(600f, 45f, addictionPercent);
            return baseInterval * Random.Range(0.6f, 1.4f);
        }
    }
}
