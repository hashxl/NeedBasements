using UnityEngine;

namespace NeedBasements
{
    internal static class CravingLines
    {
        // 0–30% addiction: infrequent, subtle
        private static readonly string[] Mild =
        {
            "...a smoke sounds really good right now.",
            "Hmm... I could go for something to take the edge off.",
            "One little something wouldn't hurt.",
            "...I keep thinking about it.",
            "My mind won't let it go. Just a little.",
            "Could really use something right about now.",
            "...it would make this so much easier.",
        };

        // 30–70%: noticeable, intrusive
        private static readonly string[] Moderate =
        {
            "*stops* ...I need something. Now.",
            "I can't stop thinking about it.",
            "*sighs* ...just one. It'll help.",
            "I'm anxious. This would fix it right now.",
            "There's no point pretending I'm not craving this.",
            "*fidgets* ...where did I put it?",
            "My body is telling me it needs something.",
            "I keep losing focus. I just need a little.",
            "Every few minutes my mind goes back to it.",
            "*rubs her arm* ...I should have brought more.",
        };

        // 70–100%: compulsive, physical
        private static readonly string[] Intense =
        {
            "*hands shake* ...I need it. Right now.",
            "*stops mid-step* I can't go on without something.",
            "No. No no no. I need one now.",
            "*breathes hard, hands restless* ...just one hit. Just one.",
            "I'm sick. Really sick. I can't function like this.",
            "*trembling* ...why didn't I bring more?",
            "Everything hurts without it. Every single thing.",
            "*clutches herself* Make it stop. I just need one.",
            "*sweating* ...I promised myself I'd cut back. After this one.",
            "My skin is crawling. I need it. I need it now.",
            "*grits teeth* Fine. Fine. I'll find some. I have to.",
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
