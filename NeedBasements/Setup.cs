using ANToolkit.Controllers;
using Asuna.CharManagement;
using Asuna.Dialogues;
using Asuna.Items;
using Asuna.Trading;
using Modding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace NeedBasements
{
    public class NeedBasements : ITCMod
    {
        private ModManifest _manifest;
        private ItemShopCatalogue _shopCatalogue;

        private float _frameTimer;
        private Vector3 _jennaWorldPosition;

        // Suppresses cravings while active (refills on each substance use)
        private float _satisfactionTimer;

        // Craving accumulator and dynamic interval based on addiction level
        private float _cravingTimer;
        private float _nextCravingInterval = 60f;

        // Tracks which substance is currently active (blocks mixing)
        private SubstanceDef _activeSubstance;

        // How many times Jenna has bought from the vendor (drives evolving dialogue)
        private int _purchaseCount;

        public void OnDialogueStarted(Dialogue dialogue) { }

        public void OnFrame(float deltaTime)
        {
            _frameTimer += deltaTime;
            if (_frameTimer >= 3f)
            {
                _frameTimer = 0f;
                var handler = Character.Get(ModConstants.CharacterJenna).Handlers.FirstOrDefault();
                if (handler != null)
                    _jennaWorldPosition = handler.transform.position;
            }

            if (_satisfactionTimer > 0f)
            {
                _satisfactionTimer -= deltaTime;
                return;
            }

            _cravingTimer += deltaTime;
            if (_cravingTimer < _nextCravingInterval)
                return;

            _cravingTimer = 0f;

            var stat = Character.Get(ModConstants.CharacterJenna).GetStat(ModConstants.StatAddiction);
            if (stat == null || stat.BaseValue < 10f)
            {
                _nextCravingInterval = 180f;
                return;
            }

            float addictionPercent = stat.BaseValue / 200f;
            _nextCravingInterval = CalculateCravingInterval(addictionPercent);
            TriggerCraving(addictionPercent);
        }

        // Interval between cravings: 180s at 0% addiction → 12s at 100%, with random variance
        private static float CalculateCravingInterval(float addictionPercent)
        {
            float baseInterval = Mathf.Lerp(180f, 12f, addictionPercent);
            return baseInterval * UnityEngine.Random.Range(0.6f, 1.4f);
        }

        private static void TriggerCraving(float addictionPercent)
        {
            var jenna = Character.Get(ModConstants.CharacterJenna);
            Item.GenerateErrorDialogue(jenna,
                CravingLines.RandomLine(addictionPercent),
                CravingLines.EmotionForLevel(addictionPercent));
        }

        // Applies the "relaxed" visual via LimbEffect; decays automatically after 'duration' seconds
        private static void ApplyRelaxedEffect(float duration)
        {
            var effect = LimbEffect.Get(ModConstants.RelaxedLimbEffectID);
            if (effect == null) return;

            effect.CanDecay  = true;
            effect.DecayTime = duration;

            var jenna = Character.Get(ModConstants.CharacterJenna);
            foreach (var limb in jenna.Limbs.GetAll())
            {
                if (limb.Type == LimbType.Head)
                {
                    limb.ApplyEffect(effect);
                    break;
                }
            }
        }

        public void OnLevelChanged(string oldLevel, string newLevel)
        {
            if (newLevel == ModConstants.LevelToSell)
                SpawnVendorNpc();
        }

        public void OnLineStarted(DialogueLine line) { }

        public void OnModLoaded(ModManifest manifest)
        {
            Debug.Log("NeedBasements installed");
            _manifest = manifest;

            var stat = new Stat
            {
                Name             = "Addicted to substances",
                BaseMax          = 200,
                NotifyOwnerName  = true,
                NotifyChanges    = true,
                Description      = "Addicted to substances",
                ID               = ModConstants.StatAddiction,
                DisplayColor     = Color.gray
            };
            stat.Initialize();

            var jenna = Character.Get(ModConstants.CharacterJenna);
            stat.AddToCharacter(jenna);
            jenna.GetStat(stat.ID).BaseValue = 0;

            var shopItems = new List<ShopItemInfo>();
            foreach (var substance in AllSubstances.All)
            {
                var consumable = ScriptableObject.CreateInstance<Consumable>();
                consumable.Name = substance.ItemName;
                if (!Item.All.ContainsKey(substance.ItemKey))
                    Item.All.Add(substance.ItemKey, consumable);

                shopItems.Add(new ShopItemInfo { Item = consumable, Cost = substance.ShopCost });
            }

            _shopCatalogue = ScriptableObject.CreateInstance<ItemShopCatalogue>();
            _shopCatalogue.Items = shopItems;

            Item.OnItemUsed.AddListener(OnAnyItemUsed);
        }

        public void OnModUnLoaded()
        {
            Item.OnItemUsed.RemoveListener(OnAnyItemUsed);
        }

        private void SpawnVendorNpc()
        {
            var npc = new GameObject();
            npc.transform.position = new Vector3(16.73f, -9.30f);

            var collider = npc.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.5f, 1f);

            var spriteObj = new GameObject();
            spriteObj.transform.position = new Vector3(16.03f, -10f);

            var spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _manifest.SpriteResolver.ResolveAsResource(ModConstants.SpritePath);
            spriteRenderer.transform.localScale = new Vector3(1f, 1f);

            var interactable = npc.AddComponent<Interactable>();
            interactable.TypeOfInteraction = InteractionType.Talk;

            var seller = Character.Get(ModConstants.CharacterVendorMan);
            interactable.OnInteracted.AddListener(_ => StartVendorDialogue(seller));
        }

        private void StartVendorDialogue(Character seller)
        {
            var dialogue = ScriptableObject.CreateInstance<Dialogue>();

            var greeting = new DialogueLine
            {
                LineID    = "vendor_greeting",
                Text      = VendorLines.GetGreeting(_purchaseCount),
                TextColor = Color.red,
                Character = seller,
                NextID    = ""
            };

            var jenna = Character.Get(ModConstants.CharacterJenna);
            foreach (var substance in AllSubstances.All)
            {
                var sub = substance;
                var choice = new DialogueChoice
                {
                    Text     = VendorLines.BuyOption(sub),
                    TargetID = "",
                    ChoiceID = $"vendor_buy_{sub.ItemKey}"
                };
                choice.OnChosen.AddListener(() => ExecutePurchase(sub, jenna));
                greeting.AddChoice(choice);
            }

            greeting.AddChoice(new DialogueChoice
            {
                Text     = VendorLines.LeaveChoice,
                TargetID = "",
                ChoiceID = "vendor_leave"
            });

            dialogue.Lines.Add(greeting);
            DialogueManager.StartDialogue(dialogue);
        }

        private void ExecutePurchase(SubstanceDef substance, Character jenna)
        {
            CurrencyHelper.Pay(substance.ShopCost, jenna,
                onSuccess: () =>
                {
                    _purchaseCount++;
                    var successDialogue = ScriptableObject.CreateInstance<Dialogue>();
                    successDialogue.Lines.Add(new DialogueLine
                    {
                        LineID    = "vendor_success",
                        Text      = VendorLines.GetSuccessLine(_purchaseCount, substance),
                        Character = jenna,
                        NextID    = ""
                    });
                    DialogueManager.StartDialogue(successDialogue);

                    GiveItems.GiveToCharacter(jenna, false, false, Item.Create<Consumable>(substance.ItemName));

                    var line = new DialogueLine()
                    {
                        LineID = "substance_add_inventory",
                        Text = VendorLines.AddedToInventory(substance),
                        TextColor = Color.yellow,
                        NextID = "" 
                    };

                    var addedDialogue = ScriptableObject.CreateInstance<Dialogue>();
                    addedDialogue.Lines.Add(line);
                    DialogueManager.StartDialogue(addedDialogue);
                },
                onFail: () => Item.GenerateErrorDialogue(jenna, VendorLines.PayFail, "Think")
            );
        }

        private void OnAnyItemUsed(Item item)
        {
            foreach (var substance in AllSubstances.All)
            {
                if (item.Name == substance.ItemName)
                {
                    OnSubstanceUsed(item, substance);
                    return;
                }
            }
        }

        private void OnSubstanceUsed(Item item, SubstanceDef substance)
        {
            var jenna = Character.Get(ModConstants.CharacterJenna);

            // Prevent mixing: if a different substance is still active, block and keep the item
            if (_satisfactionTimer > 0f && _activeSubstance != null && _activeSubstance.ItemKey != substance.ItemKey)
            {
                Item.GenerateErrorDialogue(jenna, BlockedLines.Get(_activeSubstance.ItemName), "Troubled");
                return;
            }

            _activeSubstance = substance;

            var stat = jenna.GetStat(ModConstants.StatAddiction);
            stat.BaseValue += substance.AddictionGain;
            int level = (int)stat.BaseValue;

            foreach (var stage in substance.Stages)
            {
                if (level <= stage.MaxLevel)
                {
                    Item.GenerateErrorDialogue(jenna, stage.Text, stage.Emotion);
                    break;
                }
            }

            float addictionPercent = stat.BaseValue / 200f;
            _satisfactionTimer = Mathf.Lerp(substance.SatisfactionMax, substance.SatisfactionMin, addictionPercent);
            _cravingTimer = 0f;
            ApplyRelaxedEffect(_satisfactionTimer);

            if (addictionPercent > 0.5f)
            {
                string line = addictionPercent > 0.8f ? substance.NegativeSevere : substance.NegativeMild;
                Item.GenerateErrorDialogue(jenna, line, "Sad");
            }

            jenna.Inventory.Remove(item);
        }
    }
}
