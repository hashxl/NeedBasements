using UnityEngine;

namespace NeedBasements
{
    internal static class BlockedLines
    {
        private static readonly string[] Generic =
        {
            "*shakes head* One at a time. I'm not that far gone.",
            "My body hasn't finished with the last one. I'll wait.",
            "Mixing these... even I have some limits left.",
            "Not yet. Give it some time.",
            "*pauses* ...no. Not while I'm still feeling the other one.",
            "I know what I'm doing. And mixing isn't it.",
            "*sets it aside* Later. Not now.",
        };
       
        internal static string Get(string activeName)
        {
            if (Random.value < 0.4f)
                return $"The {activeName} hasn't worn off yet. I can't mix these.";
            return Generic[Random.Range(0, Generic.Length)];
        }
    }
}
