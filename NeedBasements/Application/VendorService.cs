using ANToolkit.Controllers;
using Asuna.CharManagement;
using Asuna.Dialogues;
using Asuna.Items;
using Asuna.Trading;
using Modding;
using NeedBasements.Domain.Addiction;
using NeedBasements.Domain.Substances;
using NeedBasements.Infrastructure.LimbEffects;
using UnityEngine;

namespace NeedBasements.Application
{
    // Spawns the vendor NPC on the right level and drives the buy/leave dialogue flow.
    internal class VendorService
    {
        private readonly AddictionState _state;
        private readonly SubstanceCatalog _catalog;
        private readonly ModManifest _manifest;
        private GameObject _vendorNpc;

        internal VendorService(AddictionState state, SubstanceCatalog catalog, ModManifest manifest)
        {
            _state = state;
            _catalog = catalog;
            _manifest = manifest;
        }

        internal void OnLevelChanged(string oldLevel, string newLevel)
        {
            if (newLevel == ModConstants.LevelToSell)
            {
                SpawnVendorNpc();
                return;
            }
        }

        internal void RemoveVendorNpc()
        {
            if (_vendorNpc != null)
                Object.Destroy(_vendorNpc);
        }

        private void SpawnVendorNpc()
        {
            RemoveVendorNpc();

            _vendorNpc = new GameObject();
            _vendorNpc.transform.position = new Vector3(16.73f, -9.30f);

            var collider = _vendorNpc.AddComponent<BoxCollider>();
            collider.size = new Vector3(0.5f, 1f);

            var spriteObj = new GameObject();
            spriteObj.transform.SetParent(_vendorNpc.transform, worldPositionStays: false);
            spriteObj.transform.localPosition = new Vector3(-0.7f, -0.7f);

            var spriteRenderer = spriteObj.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = _manifest.SpriteResolver.ResolveAsResource(ModConstants.SpritePath);
            spriteRenderer.transform.localScale = new Vector3(1f, 1f);

            var interactable = _vendorNpc.AddComponent<Interactable>();
            interactable.TypeOfInteraction = InteractionType.Talk;

            var seller = Character.Get(ModConstants.CharacterVendorMan);
            interactable.OnInteracted.AddListener(_ => StartVendorDialogue(seller));
        }

        private void StartVendorDialogue(Character seller)
        {
            var dialogue = ScriptableObject.CreateInstance<Dialogue>();
            var jenna = Character.Get(ModConstants.CharacterJenna);

            string greetingLineID = "vendor_greeting";

            if (!_state.HasMetVendor)
            {
                AppendIntroLines(dialogue, seller, jenna, nextID: greetingLineID);
                _state.MarkVendorMet();
            }
            else if (IsPriceHiked(jenna) && !_state.HasSeenPriceHike)
            {
                AppendPriceHikeLines(dialogue, seller, jenna, nextID: greetingLineID);
                _state.MarkPriceHikeSeen();
            }

            float multiplier = GetPriceMultiplier(jenna);

            var greeting = new DialogueLine
            {
                LineID    = greetingLineID,
                Text      = VendorLines.GetGreeting(_state.PurchaseCount),
                TextColor = Color.red,
                Character = seller,
                NextID    = ""
            };

            foreach (var substance in _catalog.All)
            {
                var captured = substance;
                int unitPrice = Mathf.RoundToInt(captured.ShopCost * multiplier);
                var choice = new DialogueChoice
                {
                    Text     = VendorLines.BuyOption(captured, unitPrice),
                    TargetID = "",
                    ChoiceID = $"vendor_buy_{captured.ItemKey}"
                };
                choice.OnChosen.AddListener(() => ShowQuantityDialogue(captured, multiplier, jenna, seller));
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

        private static void AppendIntroLines(Dialogue dialogue, Character seller, Character jenna, string nextID)
        {
            var script = new (string id, string text, Character speaker, Color color)[]
            {
                ("vendor_intro_v1", VendorLines.IntroVendor1, seller, Color.red),
                ("vendor_intro_j1", VendorLines.IntroJenna1,  jenna,  Color.white),
                ("vendor_intro_v2", VendorLines.IntroVendor2, seller, Color.red),
                ("vendor_intro_j2", VendorLines.IntroJenna2,  jenna,  Color.white),
                ("vendor_intro_v3", VendorLines.IntroVendor3, seller, Color.red),
                ("vendor_intro_j3", VendorLines.IntroJenna3,  jenna,  Color.white),
                ("vendor_intro_v4", VendorLines.IntroVendor4, seller, Color.red),
                ("vendor_intro_j4", VendorLines.IntroJenna4,  jenna,  Color.white),
                ("vendor_intro_v5", VendorLines.IntroVendor5, seller, Color.red),
            };

            AppendScript(dialogue, script, nextID);
        }

        private static void AppendPriceHikeLines(Dialogue dialogue, Character seller, Character jenna, string nextID)
        {
            var script = new (string id, string text, Character speaker, Color color)[]
            {
                ("vendor_hike_v1", VendorLines.PriceHikeVendor1, seller, Color.red),
                ("vendor_hike_j1", VendorLines.PriceHikeJenna1,  jenna,  Color.white),
                ("vendor_hike_v2", VendorLines.PriceHikeVendor2, seller, Color.red),
                ("vendor_hike_j2", VendorLines.PriceHikeJenna2,  jenna,  Color.white),
                ("vendor_hike_v3", VendorLines.PriceHikeVendor3, seller, Color.red),
                ("vendor_hike_j3", VendorLines.PriceHikeJenna3,  jenna,  Color.white),
                ("vendor_hike_v4", VendorLines.PriceHikeVendor4, seller, Color.red),
                ("vendor_hike_j4", VendorLines.PriceHikeJenna4,  jenna,  Color.white),
            };

            AppendScript(dialogue, script, nextID);
        }

        private static void AppendScript(Dialogue dialogue, (string id, string text, Character speaker, Color color)[] script, string nextID)
        {
            for (int i = 0; i < script.Length; i++)
            {
                var entry = script[i];
                dialogue.Lines.Add(new DialogueLine
                {
                    LineID    = entry.id,
                    Text      = entry.text,
                    TextColor = entry.color,
                    Character = entry.speaker,
                    NextID    = i + 1 < script.Length ? script[i + 1].id : nextID
                });
            }
        }

        private static bool IsPriceHiked(Character jenna)
        {
            var stat = jenna?.GetStat(ModConstants.StatAddiction);
            return stat != null && stat.BaseValue >= ModConstants.PriceHikeAddictionThreshold;
        }

        private static float GetPriceMultiplier(Character jenna) =>
            IsPriceHiked(jenna) ? ModConstants.PriceHikeMultiplier : 1f;

        private static readonly int[] _quantityOptions = { 1, 5, 10 };

        private void ShowQuantityDialogue(Substance substance, float multiplier, Character jenna, Character seller)
        {
            var dialogue = ScriptableObject.CreateInstance<Dialogue>();

            var prompt = new DialogueLine
            {
                LineID    = "vendor_quantity",
                Text      = VendorLines.QuantityPrompt(substance),
                TextColor = Color.red,
                Character = seller,
                NextID    = ""
            };

            foreach (var quantity in _quantityOptions)
            {
                var capturedQty = quantity;
                int totalPrice = Mathf.RoundToInt(substance.ShopCost * capturedQty * multiplier);
                var choice = new DialogueChoice
                {
                    Text     = VendorLines.QuantityOption(substance, capturedQty, totalPrice),
                    TargetID = "",
                    ChoiceID = $"vendor_buy_{substance.ItemKey}_{capturedQty}"
                };
                choice.OnChosen.AddListener(() => ExecutePurchase(substance, capturedQty, multiplier, jenna));
                prompt.AddChoice(choice);
            }

            prompt.AddChoice(new DialogueChoice
            {
                Text     = VendorLines.LeaveChoice,
                TargetID = "",
                ChoiceID = $"vendor_buy_{substance.ItemKey}_cancel"
            });

            dialogue.Lines.Add(prompt);
            DialogueManager.StartDialogue(dialogue);
        }

        private void ExecutePurchase(Substance substance, int quantity, float multiplier, Character jenna)
        {
            int total = Mathf.RoundToInt(substance.ShopCost * quantity * multiplier);
            CurrencyHelper.Pay(total, jenna,
                onSuccess: () => OnPurchaseSuccess(substance, quantity, jenna),
                onFail:    () => Item.GenerateErrorDialogue(jenna, VendorLines.PayFail, "Think"));
        }

        private void OnPurchaseSuccess(Substance substance, int quantity, Character jenna)
        {
            for (int i = 0; i < quantity; i++)
                _state.RegisterPurchase();
         
            var successDialogue = ScriptableObject.CreateInstance<Dialogue>();
            successDialogue.Lines.Add(new DialogueLine
            {
                LineID    = "vendor_success",
                Text      = VendorLines.GetSuccessLine(_state.PurchaseCount, substance),
                Character = jenna,
                NextID    = ""
            });
            DialogueManager.StartDialogue(successDialogue);

            var items = new Item[quantity];
            for (int i = 0; i < quantity; i++)
                items[i] = Item.Create<Consumable>(substance.ItemName);
            GiveItems.GiveToCharacter(jenna, false, false, items);

            var addedDialogue = ScriptableObject.CreateInstance<Dialogue>();
            addedDialogue.Lines.Add(new DialogueLine
            {
                LineID    = "substance_add_inventory",
                Text      = VendorLines.AddedToInventory(substance, quantity),
                TextColor = Color.yellow,
                NextID    = ""
            });
            DialogueManager.StartDialogue(addedDialogue);
        }
    }
}
