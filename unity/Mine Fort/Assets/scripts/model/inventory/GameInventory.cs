using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MoonSharp.Interpreter;
using Newtonsoft.Json.Linq;
using Rimworld.Entities;
using Rimworld.model.entities;

namespace Rimworld.model.Inventory
{
    [MoonSharpUserData]
    [System.Diagnostics.DebuggerDisplay("GameInventory {ObjectType} {StackSize}/{MaxStackSize}")]
    public class GameInventory : ISelectable, IContextActionProvider, IPrototypable
    {
        private const float ClaimDuration = 120; // in Seconds

        internal int stackSize = 1;
        private List<InventoryClaim> claims;

        public GameInventory()
        {
            claims = new List<InventoryClaim>();
        }

        public GameInventory(string type, int stackSize, int maxStackSize = 50)
        {
            Type = type;
            ImportPrototypeSettings(maxStackSize, 1f, "inv_cat_none");
            StackSize = stackSize;
            claims = new List<InventoryClaim>();
        }

        private GameInventory(GameInventory other)
        {
            Type = other.Type;
            MaxStackSize = other.MaxStackSize;
            BasePrice = other.BasePrice;
            Category = other.Category;
            StackSize = other.StackSize;
            Locked = other.Locked;
            LocalizationName = other.LocalizationName;
            LocalizationDescription = other.LocalizationDescription;
            claims = new List<InventoryClaim>();
        }

        private GameInventory(string type, int maxStackSize, float basePrice, string category, string localizationName, string localizationDesc)
        {
            Type = type;
            MaxStackSize = maxStackSize;
            BasePrice = basePrice;
            Category = category;
            LocalizationName = localizationName;
            LocalizationDescription = localizationDesc;
        }

        public event Action<GameInventory> StackSizeChanged;

        public string Type { get; private set; }

        public int MaxStackSize { get; set; }

        public float BasePrice { get; set; }

        public string Category { get; private set; }

        public string LocalizationName { get; private set; }

        public string LocalizationDescription { get; private set; }

        public Tile Tile { get; set; }

        // Should this GameInventory be allowed to be picked up for completing a job?
        public bool Locked { get; set; }

        public int StackSize
        {
            get
            {
                return stackSize;
            }

            set
            {
                if (stackSize == value)
                {
                    return;
                }

                stackSize = value;
                InvokeStackSizeChanged(this);
            }
        }

        public int AvailableGameInventory
        {
            get
            {
                float requestTime = TimeManager.Instance.GameTime;
                return this.stackSize - claims.Where(claim => (requestTime - claim.time) < ClaimDuration).Sum(claim => claim.amount);
            }
        }

        public bool IsSelected { get; set; }

        /// <summary>
        /// Creates an GameInventory to be used as a prototype. Still needs to be added to the PrototypeMap.
        /// </summary>
        /// <returns>The prototype.</returns>
        /// <param name="type">Prototype's Type.</param>
        /// <param name="maxStackSize">Prototype's Max stack size.</param>
        /// <param name="basePrice">Prototype's Base price.</param>
        /// <param name="category">Prototype's Category.</param>
        public static GameInventory CreatePrototype(string type, int maxStackSize, float basePrice, string category, string localizationName, string localizationDesc)
        {
            return new GameInventory(type, maxStackSize, basePrice, category, localizationName, localizationDesc);
        }

        public GameInventory Clone()
        {
            return new GameInventory(this);
        }

        public void Claim(GameCharacter character, int amount)
        {
            float requestTime = TimeManager.Instance.GameTime;
            List<InventoryClaim> validClaims = claims.Where(claim => (requestTime - claim.time) < ClaimDuration).ToList();
            int availableGameInventory = this.stackSize - validClaims.Sum(claim => claim.amount);
            if (availableGameInventory >= amount)
            {
                validClaims.Add(new InventoryClaim(requestTime, character, amount));
            }

            // Set claims to validClaims to keep claims from filling up with old claims
            claims = validClaims;
        }

        public void ReleaseClaim(GameCharacter character)
        {
            bool noneAvailable = AvailableGameInventory == 0;
            claims.RemoveAll(claim => claim.character == character);
            if (noneAvailable && AvailableGameInventory > 0)
            {
                World.Current.jobQueue.ReevaluateWaitingQueue(this);
            }
        }

        public bool CanClaim()
        {
            float requestTime = TimeManager.Instance.GameTime;
            List<InventoryClaim> validClaims = claims.Where(claim => (requestTime - claim.time) < ClaimDuration).ToList();
            int availableGameInventory = this.stackSize - validClaims.Sum(claim => claim.amount);

            // Set claims to validClaims to keep claims from filling up with old claims
            claims = validClaims;
            return availableGameInventory > 0;
        }

        public string GetName()
        {
            return Type;
        }

        public string GetDescription()
        {
            return string.Format("StackSize: {0}\nCategory: {1}\nBasePrice: {2:N2}", StackSize, Category, BasePrice);
        }

        public string GetJobDescription()
        {
            return string.Empty;
        }

        public bool CanAccept(GameInventory inv)
        {
            return inv.Type == Type && inv.StackSize + stackSize <= MaxStackSize;
        }

        public IEnumerable<string> GetAdditionalInfo()
        {
            // Does GameInventory have hitpoints? How does it get destroyed? Maybe it's just a percentage chance based on damage.
            yield return string.Format("StackSize: {0}", stackSize);
            yield return string.Format("Available Amount: {0}", AvailableGameInventory);
            yield return string.Format("Category: {0}", BasePrice);
            yield return string.Format("BasePrice: {0:N2}", BasePrice);
        }

        public object ToJSon()
        {
            JObject GameInventoryJson = new JObject();
            if (Tile != null)
            {
                GameInventoryJson.Add("X", Tile.X);
                GameInventoryJson.Add("Y", Tile.Y);
                GameInventoryJson.Add("Z", Tile.Z);
            }

            GameInventoryJson.Add("Type", Type);
            GameInventoryJson.Add("MaxStackSize", MaxStackSize);
            GameInventoryJson.Add("StackSize", StackSize);
            GameInventoryJson.Add("BasePrice", BasePrice);
            GameInventoryJson.Add("Category", Category);
            GameInventoryJson.Add("Locked", Locked);
            GameInventoryJson.Add("LocalizationName", LocalizationName);
            GameInventoryJson.Add("LocalizationDesc", LocalizationDescription);

            return GameInventoryJson;
        }

        public void FromJson(JToken GameInventoryToken)
        {
            Type = (string)GameInventoryToken["Type"];
            MaxStackSize = (int)GameInventoryToken["MaxStackSize"];
            StackSize = (int)GameInventoryToken["StackSize"];
            BasePrice = (float)GameInventoryToken["BasePrice"];
            Category = (string)GameInventoryToken["Category"];
            Locked = (bool)GameInventoryToken["Locked"];
            LocalizationName = (string)GameInventoryToken["LocalizationName"];
            LocalizationDescription = (string)GameInventoryToken["LocalizationDesc"];
        }

        public IEnumerable<ContextMenuAction> GetContextMenuActions(ContextMenu contextMenu)
        {
            yield return new ContextMenuAction
            {
                LocalizationKey = "Sample Item Context action",
                RequireCharacterSelected = true,
                Action = (cm, c) => UnityDebugger.Debugger.Log("GameInventory", "Sample menu action")
            };

            if (PrototypeManager.Furniture.Has(this.Type))
            {
                yield return new ContextMenuAction
                {
                    LocalizationKey = "install_order",
                    RequireCharacterSelected = false,
                    Action = (cm, c) => BuildModeController.Instance.SetMode_BuildFurniture(Type, true)
                };
            }
        }

        public bool CanBePickedUp(bool canTakeFromStockpile)
        {
            // You can't pick up stuff that isn't on a tile or if it's locked
            if (Tile == null || Locked || !CanClaim())
            {
                return false;
            }

            return Tile.Furniture == null || canTakeFromStockpile == true || Tile.Furniture.HasTypeTag("Storage") == false;
        }

        public override string ToString()
        {
            return string.Format("{0} [{1}/{2}]", Type, StackSize, MaxStackSize);
        }

        public void ReadXmlPrototype(XmlReader reader_parent)
        {
            Type = reader_parent.GetAttribute("type");
            MaxStackSize = int.Parse(reader_parent.GetAttribute("maxStackSize") ?? "50");
            BasePrice = float.Parse(reader_parent.GetAttribute("basePrice") ?? "1");
            Category = reader_parent.GetAttribute("category");
            LocalizationName = reader_parent.GetAttribute("localizationName");
            LocalizationDescription = reader_parent.GetAttribute("localizationDesc");
        }

        private void ImportPrototypeSettings(int defaulMaxStackSize, float defaultBasePrice, string defaultCategory)
        {
            if (PrototypeManager.Inventory.Has(Type))
            {
                GameInventory prototype = PrototypeManager.Inventory.Get(Type);
                MaxStackSize = prototype.MaxStackSize;
                BasePrice = prototype.BasePrice;
                Category = prototype.Category;
                LocalizationName = prototype.LocalizationName;
                LocalizationDescription = prototype.LocalizationDescription;
            }
            else
            {
                MaxStackSize = defaulMaxStackSize;
                BasePrice = defaultBasePrice;
                Category = defaultCategory;
            }
        }

        private void InvokeStackSizeChanged(GameInventory GameInventory)
        {
            Action<GameInventory> handler = StackSizeChanged;
            if (handler != null)
            {
                handler(GameInventory);
            }
        }

        public struct InventoryClaim
        {
            public float time;
            public GameCharacter character;
            public int amount;

            public InventoryClaim(float time, GameCharacter character, int amount)
            {
                this.time = time;
                this.character = character;
                this.amount = amount;
            }
        }
    }
}
