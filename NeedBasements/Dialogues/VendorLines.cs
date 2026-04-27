namespace NeedBasements
{
    internal static class VendorLines
    {
        internal const string LeaveChoice = "No thanks.";
        internal const string PayFail     = "You don't have enough fB for that.";

        internal static string BuyOption(SubstanceDef substance) =>
            $"Buy {substance.ItemName} ({substance.ShopCost} fB)";

        internal static string AddedToInventory(SubstanceDef substance) =>
            $"*pockets it* {substance.ItemName} added to inventory.";

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

        internal static string GetSuccessLine(int purchaseCount, SubstanceDef substance)
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
