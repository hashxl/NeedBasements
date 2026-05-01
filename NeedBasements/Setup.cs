using ANToolkit;
using ANToolkit.UI;
using Asuna.CharManagement;
using Asuna.Dialogues;
using Asuna.Items;
using Modding;
using UnityEngine.SceneManagement;
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
        private SubstanceCatalog _catalog;

        public void OnModLoaded(ModManifest manifest)
        {
            Debug.Log("NeedBasements installed");

            _catalog = new SubstanceCatalog();
            var state   = new AddictionState();
            var jenna   = Character.Get(ModConstants.CharacterJenna);

            ModLimbEffects.RegisterAll(manifest, _catalog);
            AddictionStatFactory.RegisterFor(jenna);
            SubstanceItemRegistry.RegisterAll(_catalog);

            _hangoverService     = new HangoverService(state);
            _cravingService      = new CravingService(state, _hangoverService);
            _substanceUseService = new SubstanceUseService(state, _catalog);
            _vendorService       = new VendorService(state, _catalog, manifest);

            // Sleep (and any other path that wipes limb effects) removes the pleasure effect.
            // The hangover service speaks the substance's withdrawal line and clears state so
            // cravings can resume immediately.
            PleasureEffect.SubscribeToRemoved(jenna, _hangoverService.OnPleasureEnded);

            Item.OnItemUsed.AddListener(_substanceUseService.OnItemUsed);

            // Spawn vendor immediately if already in Carceburg
            if (SceneManager.GetActiveScene().name == ModConstants.LevelToSell)
                _vendorService.OnLevelChanged("", ModConstants.LevelToSell);


            Notification.Create("Mod installed successfully : NeedBasements", Color.green, 3f);
          
        }

        public void OnModUnLoaded()
        {
            Debug.Log("NeedBasements uninstalling, cleaning up...");

            Item.OnItemUsed.RemoveListener(_substanceUseService.OnItemUsed);

            var jenna = Character.Get(ModConstants.CharacterJenna);

            // Remove all substance items from Jenna's inventory
            foreach (var substance in _catalog.All)
            {
                // Keep removing while items exist
                while (true)
                {
                    var item = jenna.Inventory.GetItemByName(substance.ItemName);
                    if (item == null) break;
                    jenna.Inventory.Remove(item);
                }
            }

            // Remove addiction stat
            AddictionStatFactory.UnregisterFrom(jenna);

            // Remove substance items from the game registry
            SubstanceItemRegistry.UnregisterAll(_catalog);

            // Remove LimbEffects from the game registry
            ModLimbEffects.UnregisterAll(_catalog);

            // Remove vendor NPC if present
            _vendorService.RemoveVendorNpc();
            Notification.Create("Mod uninstalled successfully : NeedBasements", Color.green, 3f);
            Debug.Log("NeedBasements cleanup complete");

        }

        public void OnFrame(float deltaTime) => _cravingService.Tick(deltaTime);
       
        public void OnLevelChanged(string oldLevel, string newLevel) =>
            _vendorService.OnLevelChanged(oldLevel, newLevel);

        public void OnDialogueStarted(Dialogue dialogue) { }
        public void OnLineStarted(DialogueLine line) { }
    }
}
