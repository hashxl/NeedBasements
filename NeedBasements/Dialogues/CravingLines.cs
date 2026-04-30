using UnityEngine;

namespace NeedBasements
{
    internal static class CravingLines
    {
        // 0–30% addiction: infrequent, subtle
        private static readonly string[] Mild =
        {
            "...a smoke sounds really good right now.",
            "Hmm... I could go for a drag. Just a quick one.",
            "One little pill wouldn't hurt anybody.",
            "...I keep thinking about lighting one up.",
            "My mind won't let go of that pack I had.",
            "Could really use a hit right about now.",
            "...a smoke would make this so much easier.",
        };

        // 30–70%: noticeable, intrusive
        private static readonly string[] Moderate =
        {
            "*stops* ...I need a smoke. Now.",
            "I can't stop thinking about my next fix.",
            "*sighs* ...just one drag. It'll help.",
            "I'm anxious. A pill would fix this right now.",
            "There's no point pretending I'm not craving a hit.",
            "*fidgets* ...where did I put my pack?",
            "My body is telling me it needs the stuff.",
            "I keep losing focus. I just need to light one up.",
            "Every few minutes my mind goes back to the stash.",
            "*rubs her arm* ...I should have brought more pills.",
        };

        // 70–100%: compulsive, physical
        private static readonly string[] Intense =
        {
            "*hands shake* ...I need a hit. Right now.",
            "*stops mid-step* I can't go on without a smoke.",
            "No. No no no. I need a pill now.",
            "*breathes hard, hands restless* ...just one drag. Just one.",
            "I'm sick. Really sick. I need something in me.",
            "*trembling* ...why didn't I bring more pills?",
            "Everything hurts without a smoke. Every single thing.",
            "*clutches herself* Make it stop. I just need one hit.",
            "*sweating* ...I promised I'd cut back. After this last pill.",
            "My skin is crawling. I need to light up. Now.",
            "*grits teeth* Fine. Fine. I'll find some dope. I have to.",
        };

        internal static string[] ForLevel(float addictionPercent)
        {
            if (addictionPercent < 0.3f) return Mild;
            if (addictionPercent < 0.7f) return Moderate;
            return Intense;
        }

        internal static string EmotionForLevel(float addictionPercent)
        {
            if (addictionPercent < 0.3f) return "Think";
            if (addictionPercent < 0.7f) return "Troubled";
            return "Distressed";
        }

        internal static string RandomLine(float addictionPercent)
        {
            var pool = ForLevel(addictionPercent);
            return pool[Random.Range(0, pool.Length)];
        }
    }
}
