using System.Collections.Generic;

namespace NeedBasements.Domain.Substances
{
    internal class SubstanceCatalog
    {
        private static readonly Substance Cigar = new Substance
        {
            ItemName        = "Cigar",
            ItemKey         = "cigar",
            ShopCost        = 50,
            AddictionGain   = 2,
            SatisfactionMax = 90f,
            SatisfactionMin = 15f,
            NegativeMild    = "*cough* ...still worth it.",
            NegativeSevere  = "*heavy cough* My lungs can't keep taking this...",
            PurchaseReaction = "Just to try it, right?",
            Stages = new[]
            {
                new ProgressionStage(10,  "Hmm... *lights the cigar* ...not bad at all.",                                "Think"),
                new ProgressionStage(20,  "There's something to it... I can't quite tell if I like it.",                  "Think"),
                new ProgressionStage(30,  "Just a few more won't hurt...",                                                "Happy"),
                new ProgressionStage(40,  "It calms me down a bit. Nothing serious.",                                    "Happy"),
                new ProgressionStage(50,  "I'm starting to enjoy this. Very relaxing.",                                  "Happy"),
                new ProgressionStage(60,  "One a day never hurt anyone... right?",                                       "Troubled"),
                new ProgressionStage(70,  "Could be worse. At least it's not drinking.",                                 "Troubled"),
                new ProgressionStage(80,  "I'm smoking more than I should... but it tastes so good.",                    "Sad"),
                new ProgressionStage(90,  "Every time I try to quit I get irritable. Better not even try.",              "Distressed"),
                new ProgressionStage(100, "I don't know... I think I need this now.",                                    "Eyeroll"),
                new ProgressionStage(110, "*cough* ...it passes.",                                                       "Distressed"),
                new ProgressionStage(120, "My lungs are complaining but I don't care anymore.",                          "Seductive_1"),
                new ProgressionStage(130, "Without a cigar I get anxious. With one I'm fine.",                           "Seductive_2"),
                new ProgressionStage(140, "I've tried to quit three times this month. It doesn't work.",                 "Seductive_2"),
                new ProgressionStage(150, "*heavy cough* ...I need this.",                                               "Angry"),
                new ProgressionStage(160, "My throat is wrecked but I can't stop.",                                      "Drunk"),
                new ProgressionStage(170, "Whatever happens, I light one up. Simple as that.",                           "Drunk"),
                new ProgressionStage(180, "I'm not addicted. I only smoke when I want to. ...Which is all the time.",    "Drunk"),
                new ProgressionStage(190, "*hands shake as I light it* ...ahh. Better.",                                 "Seductive_4"),
                new ProgressionStage(199, "I can't even taste it anymore. But without it... I can't exist.",             "Seductive_4"),
                new ProgressionStage(int.MaxValue, "...This hollowed me out. But I can't stop. Not ever again.",         "Seductive_4"),
            }
        };

        // +4/use — habitual, quick fix; satisfies for less time than a cigar
        private static readonly Substance Cigarette = new Substance
        {
            ItemName        = "Cigarette",
            ItemKey         = "cigarette",
            ShopCost        = 30,
            AddictionGain   = 4,
            SatisfactionMax = 70f,
            SatisfactionMin = 10f,
            NegativeMild    = "*cough* This smoke is getting to me.",
            NegativeSevere  = "*coughs hard* I can barely breathe... doesn't matter.",
            PurchaseReaction = "I can quit whenever I want.",
            Stages = new[]
            {
                new ProgressionStage(10,  "Just this one. It helps with the nerves.",                                    "Think"),
                new ProgressionStage(20,  "A little something to take the edge off.",                                    "Think"),
                new ProgressionStage(30,  "Not bad. Quick and easy.",                                                    "Happy"),
                new ProgressionStage(50,  "I can go through a whole pack without thinking about it.",                    "Happy"),
                new ProgressionStage(70,  "I smoke when I'm stressed. Which is... often.",                               "Troubled"),
                new ProgressionStage(90,  "I told myself just one. That was six ago.",                                   "Sad"),
                new ProgressionStage(110, "My fingers smell like smoke but I stopped noticing.",                         "Distressed"),
                new ProgressionStage(130, "Without one I can't focus on anything. Not a single thing.",                  "Distressed"),
                new ProgressionStage(150, "*coughs* I need to quit. Tomorrow.",                                          "Angry"),
                new ProgressionStage(170, "My hands are shaking. Where did I put my cigarettes?",                        "Drunk"),
                new ProgressionStage(190, "Each one burns a little more. I light the next one off the last.",            "Seductive_4"),
                new ProgressionStage(int.MaxValue, "I don't remember what breathing without smoke feels like.",          "Seductive_4"),
            }
        };

        // +12/use — mellow to paranoid; long high but strong grip
        private static readonly Substance Cannabis = new Substance
        {
            ItemName = "Cannabis",
            ItemKey  = "cannabis",
            ShopCost = 80,
            AddictionGain   = 12,
            SatisfactionMax = 150f,
            SatisfactionMin = 25f,
            NegativeMild    = "Getting a little fuzzy... this is fine.",
            NegativeSevere  = "*paranoid* Everything feels wrong when I'm not high. When does it kick in?",
            PurchaseReaction = "I've always been... curious.",
            Stages = new[]
            {
                new ProgressionStage(15,  "That's... different. Kind of nice, actually.",                                "Think"),
                new ProgressionStage(30,  "*giggles* Okay. I see why people like this.",                                 "Happy"),
                new ProgressionStage(60,  "Everything's a bit softer. I like it here.",                                 "Happy"),
                new ProgressionStage(90,  "I function better when I'm high. That's just... true.",                      "Troubled"),
                new ProgressionStage(120, "I used to worry about things. Now I worry about running out.",               "Troubled"),
                new ProgressionStage(150, "I can't sleep without it. Can barely sleep with it either.",                  "Sad"),
                new ProgressionStage(170, "*hands shake* The world is too loud when I'm sober.",                        "Distressed"),
                new ProgressionStage(190, "I don't feel anything anymore unless I smoke first.",                         "Seductive_4"),
                new ProgressionStage(int.MaxValue, "I can't tell if I'm high or if this is just... me now.",            "Seductive_4"),
            }
        };

        // +22/use — intense euphoria to desperate crash; fastest addiction spiral
        private static readonly Substance Pills = new Substance
        {
            ItemName        = "Pills",
            ItemKey         = "pills",
            ShopCost        = 150,
            AddictionGain   = 22,
            SatisfactionMax = 45f,
            SatisfactionMin = 5f,
            NegativeMild    = "*grinding teeth* The crash is getting worse every time.",
            NegativeSevere  = "*shaking violently* I need another one. Now. Please.",
            PurchaseReaction = "Everyone uses these. It's fine.",
            HangoverLine    = "*winces* God, my head is pounding... I shouldn't have taken those.",
            HangoverEmotion = "Distressed",
            Stages = new[]
            {
                new ProgressionStage(22,  "*swallows* Okay. Let's see what the fuss is about.",                         "Think"),
                new ProgressionStage(44,  "Oh. Oh, that's... that's something.",                                        "Happy"),
                new ProgressionStage(66,  "Everything is sharp. I feel invincible.",                                    "Happy"),
                new ProgressionStage(88,  "I need more to get the same feeling. Fine. I'll take two.",                  "Seductive_1"),
                new ProgressionStage(110, "The crash is brutal. I just take another before it hits.",                   "Seductive_2"),
                new ProgressionStage(132, "I can't remember the last time I felt okay without one.",                    "Sad"),
                new ProgressionStage(154, "*hands shake* I need to find more. I need them now.",                        "Seductive_4"),
                new ProgressionStage(176, "My heart is pounding. It does that all the time now.",                       "Angry"),
                new ProgressionStage(198, "*barely coherent* Just... one more. Hold it together.",                      "Seductive_4"),
                new ProgressionStage(int.MaxValue, "I don't know who I was before these. I don't want to know.",        "Seductive_4"),
            }
        };

        private readonly Substance[] _all = { Cigar, Cannabis, Cigarette, Pills };

        internal IReadOnlyList<Substance> All => _all;

        internal Substance FindByItemName(string itemName)
        {
            foreach (var substance in _all)
            {
                if (substance.ItemName == itemName)
                    return substance;
            }
            return null;
        }
    }
}
