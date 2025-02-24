﻿using Kingmaker;
using Kingmaker.PubSubSystem;
using ModKit;
using TMPro;
using ToyBox.classes.MainUI.Inventory;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Area;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Utility;
using System.ComponentModel;
using System.Threading;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.AreaLogic.SummonPool;
using Kingmaker.EntitySystem.Entities;
#if Wrath
using Kingmaker.UI.MVVM._VM.ServiceWindows.Inventory;
using Kingmaker.UI.ServiceWindow;
#endif

namespace ToyBox {
    public class OnAreaLoadComplete : IAreaLoadingStagesHandler {
        public Settings Settings => Main.Settings;
        public void OnAreaLoadingComplete() {
            Mod.Log("OnAreaLoadingComplete");
            ISummonPool pool = Game.Instance.SummonPools.GetPool(SpawnHelper.RandomUnitsPool);
            if (pool != null) {
                foreach (UnitEntityData unitEntityData in pool.Units) {
                    if( unitEntityData.IsInGame == false )
                        continue;

                    Mod.Log("Destroying a pre-existing random unit on the random pool. " + unitEntityData.CharacterName);
                    unitEntityData.IsInGame = false;
                    unitEntityData.MarkForDestroy();
                }

                Mod.Log($"Pool size after loading = {pool.Count}");
            }

            foreach (var lootWrapper in LootHelper.GetMassLootFromCurrentArea().Where(l => l.InteractionLoot != null || l.Unit?.Faction.AssetGuid == SpawnHelper.Mobs.AssetGuid)) {
                if ((lootWrapper.InteractionLoot != null && Settings.toggleRandomLootForContainer) || (lootWrapper.Unit != null && Settings.toggleRandomLootForEnemy)) {
                    lootWrapper.GenerateRandomLoot();
                }

                if (Settings.toogleMoreRandomEnemies && lootWrapper.Unit != null) {
                    SpawnHelper.SpawnUnit(lootWrapper.Unit)?.GenerateRandomLoot();
                }
            }
        }
        public void OnAreaScenesLoaded() {

        }
    }

    public class OnAreaLoad : IAreaHandler {
        public Settings Settings => Main.Settings;
        public void SelectedCharacterDidChange() {

        }
        public void OnAreaDidLoad() {
            SelectedCharacterObserver.Shared.Notifiers -= SelectedCharacterDidChange;
            SelectedCharacterObserver.Shared.Notifiers += SelectedCharacterDidChange;
            Mod.Log("OnAreaDidLoad");
            EnhancedInventory.RefreshRemappers();
            BagOfPatches.Tweaks.NoWeight_Patch1.Refresh(Settings.toggleEquipmentNoWeight);
            if (Settings.toggleEnhancedSpellbook) {
                LoadSpellbookSearchBar();
            }
#if Wrath
            BagOfPatches.CameraPatches.CameraRigPatch.OnAreaLoad();
#endif
#if false
            if (Settings.EnableInventorySearchBar)
            {
                LoadInventorySearchBar();
            }


            if (Main.Settings.EnableHighlightableLoot)
            {
                LoadHighlightLoot();
            }

            if (Main.Settings.EnableVisualOverhaulSorting)
            {
                SetupSortingStyle();
            }
#endif
        }

        public void OnAreaBeginUnloading() { }

        private readonly (string, InventoryType)[] m_inventory_paths = new (string, InventoryType)[] {
            // Regular, in-game inventory.
            ("ServiceWindowsPCView/InventoryPCView/Inventory/Stash/StashContainer", InventoryType.InventoryStash),

            // World map inventory.
            ("ServiceWindowsConfig/InventoryPCView/Inventory/Stash/StashContainer", InventoryType.InventoryStash),

            // Vendor screen: PC inventory view.
            ("VendorPCView/MainContent/PlayerStash", InventoryType.InventoryStash),

            // Vendor screen: Vendor goods view.
            ("VendorPCView/MainContent/VendorBlock", InventoryType.Vendor),

            // Shared stash: PC inventory view.
            ("LootPCView/Window/Inventory", InventoryType.LootInventoryStash),

            // Shared stash: Stash items view.
            ("LootPCView/Window/Collector", InventoryType.LootCollector),
        };

        private void LoadInventorySearchBar() {
            foreach ((string path, InventoryType type) in m_inventory_paths) {
                Transform filters_block_transform = Game.Instance.UI.MainCanvas.transform.Find(path);
                if (filters_block_transform != null) {
                    filters_block_transform.gameObject.AddComponent<EnhancedInventoryController>().Type = type;
                }
            }
        }

        // InGamePCView(Clone)/InGameStaticPartPCView/StaticCanvas/ServiceWindowsPCView/Background/Windows/SpellbookPCView/SpellbookScreen/MainContainer/Information/MainTitle/
        // GlobalMapPCView(Clone)/StaticCanvas/ServiceWindowsConfig/Background/Windows/SpellbookPCView/SpellbookScreen/MainContainer/Information/MainTitle/
        private void LoadSpellbookSearchBar() {
            string[] paths = new string[] {
                "ServiceWindowsPCView/Background/Windows/SpellbookPCView/SpellbookScreen", // game
                "ServiceWindowsConfig/Background/Windows/SpellbookPCView/SpellbookScreen" // world map
            };

            foreach (string path in paths) {
                Transform spellbook = Game.Instance.UI.MainCanvas.transform.Find(path);
                if (spellbook != null) {
                    var controller = spellbook.gameObject.AddComponent<EnhancedSpellbookController>();
                    //                    controller.Awake(); // FIXME - why do I have to call this? What is the proper way to get this controller installed and get awake called by the framework and not by Marria
                }
            }
        }

        private void SetupSortingStyle() {
            foreach ((string path, InventoryType type) in m_inventory_paths) {
                string viewport_path = $"{path}/{EnhancedInventoryController.PathToSorter(type)}/Sorting/Dropdown/Template/Viewport";
                Transform viewport = Game.Instance.UI.MainCanvas.transform.Find(viewport_path);

                // This happens if we're on a screen that we don't have access to or screens that have different formatting.
                if (viewport == null) continue;

                Transform content = viewport.Find("Content");
                Transform item = content.Find("Item");

                VerticalLayoutGroup group = content.GetComponent<VerticalLayoutGroup>();
                TextMeshProUGUI item_label = item.Find("Item Label").GetComponent<TextMeshProUGUI>();
                RectTransform item_background = item.Find("Item Background").GetComponent<RectTransform>();
                RectTransform item_checkmark = item.Find("Item Checkmark").GetComponent<RectTransform>();
                RectTransform item_bottom_border = item.Find("BottomBorderImage").GetComponent<RectTransform>();

                group.spacing = 4;
                group.padding.top = 0;
                group.padding.bottom = 0;

                item_label.fontSize = 16.0f;
                item_label.horizontalAlignment = HorizontalAlignmentOptions.Center;

                item_background.anchorMin = new Vector2(0.0f, 0.0f);
                item_background.anchorMax = new Vector2(1.0f, 1.0f);
                item_background.offsetMin = new Vector2(0.0f, 0.0f);
                item_background.offsetMax = new Vector2(0.0f, 0.0f);

                item_checkmark.anchorMin = new Vector2(0.0f, 0.0f);
                item_checkmark.anchorMax = new Vector2(1.0f, 1.0f);
                item_checkmark.offsetMin = new Vector2(0.0f, 0.0f);
                item_checkmark.offsetMax = new Vector2(0.0f, 0.0f);

                item_bottom_border.anchorMin = new Vector2(0.0f, 0.0f);
                item_bottom_border.anchorMax = new Vector2(1.0f, 0.0f);
                item_bottom_border.offsetMin = new Vector2(0.0f, -2.0f);
                item_bottom_border.offsetMax = new Vector2(0.0f, 0.0f);
            }
        }
    }
}