using NeedBasements.Domain.Substances;

namespace NeedBasements
{
    internal static class VendorLines
    {
        internal const string LeaveChoice = "No thanks.";
        internal const string PayFail     = "You don't have enough fB for that.";

        // First-meeting intro lines — vendor is mysterious, Jenna starts skeptical and warms up.
        internal const string IntroVendor1 = "Well, well... a new face. Don't see many of those around here. *steps closer* You look like someone who could use... a little something to take the edge off.";
        internal const string IntroJenna1  = "...Who are you? I've never seen you down here before.";
        internal const string IntroVendor2 = "Names? *chuckles* Names aren't important. What matters is what I can offer. Premium goods. Highest quality. Things that make the day a little softer.";
        internal const string IntroJenna2  = "This feels off. You're standing in an alley pitching me whatever's under that coat — that doesn't scream 'trustworthy.'";
        internal const string IntroVendor3 = "Trustworthy? *smiles warmly* I'm offering relief. Peace. People come to me on their worst days and leave smiling. Nothing shady about that. And first-timers — first-timers always get my honest word on the price.";
        internal const string IntroJenna3  = "I really shouldn't be doing this...";
        internal const string IntroVendor4 = "But you're still standing here. *grins* That tells me everything I need to know. Try a little. Just to see. If it's not for you, walk away — no hard feelings, no questions.";
        internal const string IntroJenna4  = "*sighs* ...Fine. Just this once. What've you got?";
        internal const string IntroVendor5 = "*spreads coat open* Now we're talking. Take your pick.";

        // One-shot price-hike scene — triggers the first time Jenna comes back after her
        // addiction crosses the threshold. She tries to argue him down; he doesn't budge.
        internal const string PriceHikeVendor1 = "*tilts his head and smiles* Well, look who's back. Before we get started... a small heads up. The prices have... gone up a bit.";
        internal const string PriceHikeJenna1  = "Wait — what? Up? Since *when*? That's nearly *double* what I paid last time!";
        internal const string PriceHikeVendor2 = "*shrugs lazily* Supply, demand. Risk. The usual story. And, well... a loyalty premium for clients who can't really go elsewhere.";
        internal const string PriceHikeJenna2  = "Loyalty premium?! That's not a premium, that's a *robbery*. Come on. We've been doing this for months — give me the old price. Just for me.";
        internal const string PriceHikeVendor3 = "*chuckles* I'd love to, sweetheart. I really would. But the price is the price. Of course... you're free to walk away. Try the other guy. Oh, wait. There is no other guy.";
        internal const string PriceHikeJenna3  = "...you piece of shit. *clenches her jaw* You know I can't.";
        internal const string PriceHikeVendor4 = "*grins* I know. So... what'll it be?";
        internal const string PriceHikeJenna4  = "*sighs, defeated* ...fine. Fine. I'll pay your damn price.";

        internal static string BuyOption(Substance substance, int unitPrice) =>
            $"Buy {substance.ItemName} ({unitPrice} fB)";

        internal static string QuantityPrompt(Substance substance) =>
            $"How many {substance.ItemName}s do you want?";

        internal static string QuantityOption(Substance substance, int quantity, int totalPrice) =>
            $"{quantity}x ({totalPrice} fB)";

        internal static string AddedToInventory(Substance substance, int quantity) =>
            quantity > 1
                ? $"*pockets them* {substance.ItemName} x{quantity} added to inventory."
                : $"*pockets it* {substance.ItemName} added to inventory.";

        internal static string GetGreeting(int purchaseCount)
        {
            if (purchaseCount == 0)
                return "Psst... looking for something to take the edge off? I've got quite the selection.";
            if (purchaseCount < 3)
                return "Back already? Good. I've got what you need.";
            if (purchaseCount < 7)
                return "*nods* My best client. What'll it be today? my favorite addict";
            if (purchaseCount < 15)
                return "*grins* I knew you'd be back. You always come back, don't you?";
            if (purchaseCount < 25)
                return "*laughs softly* Look who's here again. Couldn't stay away, hm? ...addict.";
            if (purchaseCount < 40)
                return "*leans back, smiling* I don't even need to ask what you want anymore. It's nice having regulars.";
            return "*laughs openly* You know, I almost feel bad about this line of work. Almost. What do you need?";
        }

        internal static string GetSuccessLine(int purchaseCount, Substance substance)
        {
            if (purchaseCount <= 2)
                return substance.PurchaseReaction;
            if (purchaseCount <= 5)
                return "Enjoy. See you soon.";
            if (purchaseCount <= 10)
                return "*winks* I'll be here when you need more. You know where to find me.";
            if (purchaseCount <= 20)
                return "*counts the fB* Always a pleasure doing business with you.";
            if (purchaseCount <= 35)
                return "*smirks* Same time next week? Or sooner. Probably sooner.";
            return "*laughs* Go on. Don't let me keep you from your habit.";
        }
    }
}
