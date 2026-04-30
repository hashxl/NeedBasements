using Asuna.CharManagement;
using Asuna.Dialogues;
using Asuna.Items;
using Modding;
using NeedBasements.Application;
using NeedBasements.Domain.Addiction;
using NeedBasements.Domain.Substances;
using NeedBasements.Infrastructure;
using NeedBasements.Infrastructure.LimbEffects;
using UnityEngine;

namespace NeedBasements
{
    // ITCMod entry point. Composition root only — wiring + event delegation.
    public class NeedBasements : ITCMod
    {
        private CravingService _cravingService;
        private SubstanceUseService _substanceUseService;
        private VendorService _vendorService;
        private HangoverService _hangoverService;

        public void OnModLoaded(ModManifest manifest)
        {
            Debug.Log("NeedBasements installed");
        
            var catalog = new SubstanceCatalog();
            var state   = new AddictionState();
            var jenna   = Character.Get(ModConstants.CharacterJenna);

            ModLimbEffects.RegisterAll(manifest);
            AddictionStatFactory.RegisterFor(jenna);
            SubstanceItemRegistry.RegisterAll(catalog);

            _hangoverService     = new HangoverService(state);
            _cravingService      = new CravingService(state, _hangoverService);
            _substanceUseService = new SubstanceUseService(state, catalog);
            _vendorService       = new VendorService(state, catalog, manifest);

            // Sleep (and any other path that wipes limb effects) removes the pleasure effect.
            // The hangover service speaks the substance's withdrawal line and clears state so
            // cravings can resume immediately.
            PleasureEffect.SubscribeToRemoved(jenna, _hangoverService.OnPleasureEnded);

            Item.OnItemUsed.AddListener(_substanceUseService.OnItemUsed);
        }

        public void OnModUnLoaded()
        {
            Item.OnItemUsed.RemoveListener(_substanceUseService.OnItemUsed);
        }

        public void OnFrame(float deltaTime) => _cravingService.Tick(deltaTime);
       
        public void OnLevelChanged(string oldLevel, string newLevel) =>
            _vendorService.OnLevelChanged(oldLevel, newLevel);

        public void OnDialogueStarted(Dialogue dialogue) { }
        public void OnLineStarted(DialogueLine line) { }
    }
}
