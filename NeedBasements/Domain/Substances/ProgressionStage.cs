namespace NeedBasements.Domain.Substances
{
    internal readonly struct ProgressionStage
    {
        internal readonly int    MaxLevel;
        internal readonly string Text;
        internal readonly string Emotion;

        internal ProgressionStage(int maxLevel, string text, string emotion)
        {
            MaxLevel = maxLevel;
            Text     = text;
            Emotion  = emotion;
        }
    }
}
