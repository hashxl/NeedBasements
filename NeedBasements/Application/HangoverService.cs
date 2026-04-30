using Asuna.CharManagement;
using Asuna.Items;
using NeedBasements.Domain.Addiction;

namespace NeedBasements.Application
{
    // When the pleasure LimbEffect ends (decay or sleep wipe), show the active substance's
    // withdrawal/hangover line if it has one, then clear the addiction state.
    internal class HangoverService
    {
        private readonly AddictionState _state;

        internal HangoverService(AddictionState state)
        {
            _state = state;
        }

        internal void OnPleasureEnded()
        {
            var ending = _state.ActiveSubstance;
            _state.ClearActive();

            if (ending == null || !ending.HasHangover) return;

            var jenna = Character.Get(ModConstants.CharacterJenna);
            if (jenna == null) return;

            Item.GenerateErrorDialogue(jenna, ending.HangoverLine, ending.HangoverEmotion);
        }
    }
}
