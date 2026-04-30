using UnityEngine;

namespace NeedBasements.Domain.Substances
{
    internal class Substance
    {
        internal string             ItemName;
        internal string             ItemKey;
        internal int                ShopCost;
        internal int                AddictionGain;
        internal float              SatisfactionMax;
        internal float              SatisfactionMin;
        internal ProgressionStage[] Stages;
        internal string             NegativeMild;
        internal string             NegativeSevere;
        internal string             PurchaseReaction;
        internal string             HangoverLine;
        internal string             HangoverEmotion;

        internal bool HasHangover => !string.IsNullOrEmpty(HangoverLine);

        internal float SatisfactionAt(float addictionPercent) =>
            Mathf.Lerp(SatisfactionMax, SatisfactionMin, addictionPercent);

        internal ProgressionStage StageFor(int level)
        {
            foreach (var stage in Stages)
            {
                if (level <= stage.MaxLevel)
                    return stage;
            }
            return Stages[Stages.Length - 1];
        }
    }
}
