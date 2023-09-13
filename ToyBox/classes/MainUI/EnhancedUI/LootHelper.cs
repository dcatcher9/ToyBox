using Kingmaker;
using Kingmaker.Blueprints.Loot;
using Kingmaker.Designers.EventConditionActionSystem.Actions;
using Kingmaker.ElementsSystem;
using Kingmaker.EntitySystem;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.Items;
using Kingmaker.PubSubSystem;
using Kingmaker.UI.MVVM;
using Kingmaker.UnitLogic;
using Kingmaker.Utility;
using Kingmaker.View;
using Kingmaker.View.MapObjects;
using ModKit;
using Newtonsoft.Json;
using Owlcat.Runtime.UI.Controls.Button;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Kingmaker.Blueprints.Items.Armors;
using Kingmaker.Blueprints.Items.Equipment;
using Kingmaker.Blueprints.Items.Weapons;
using Kingmaker.Blueprints.Items;
using System.ComponentModel;
using Kingmaker.Craft;
using Kingmaker.Blueprints.Items.Shields;
using ToyBox.Inventory;
using static RootMotion.FinalIK.GrounderQuadruped;
using Kingmaker.Blueprints;
using Kingmaker.Blueprints.Items.Ecnchantments;
using Kingmaker.UnitLogic.Mechanics;
#if Wrath
using Kingmaker.UI.MVVM._PCView.Loot;
using Kingmaker.UI.MVVM._VM.Loot;
#endif

namespace ToyBox {
    public static class LootHelper {

        private static Dictionary<Type, Dictionary<RarityType, List<BlueprintItem>>> _cachedLoots = new();

        private static Dictionary<RarityType, List<BlueprintItemEnchantment>> _cachedEnchantment = new();

        public static void BuildCachedLoots() {
            _cachedLoots.Clear();

            _cachedLoots[typeof(BlueprintItemWeapon)] = BlueprintExtensions.GetBlueprints<BlueprintItemWeapon>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemArmor)] = BlueprintExtensions.GetBlueprints<BlueprintItemArmor>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemShield)] = BlueprintExtensions.GetBlueprints<BlueprintItemShield>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentBelt)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentBelt>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentRing)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentRing>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentGloves)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentGloves>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentFeet)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentFeet>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentHead)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentHead>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentNeck)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentNeck>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentShirt)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentShirt>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentShoulders)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentShoulders>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentUsable)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentUsable>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentGlasses)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentGlasses>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentHand)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentHand>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentHandSimple)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentHandSimple>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentSimple)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentSimple>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItemEquipmentWrist)] = BlueprintExtensions.GetBlueprints<BlueprintItemEquipmentWrist>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());
            _cachedLoots[typeof(BlueprintItem)] = BlueprintExtensions.GetBlueprints<BlueprintItem>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList());
            _cachedLoots[typeof(BlueprintIngredient)] = BlueprintExtensions.GetBlueprints<BlueprintIngredient>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList<BlueprintItem>());

            _cachedEnchantment = BlueprintExtensions.GetBlueprints<BlueprintItemEnchantment>().GroupBy(item => item.Rarity()).ToDictionary(group => group.Key, group => group.ToList());

            foreach (var kv in _cachedLoots) {
                foreach (var kv2 in kv.Value) {
                    Mod.Log($"{kv.Key} = [{kv2.Key}]{kv2.Value.Count}");
                }
            }

            foreach (var kv in _cachedEnchantment) {
                Mod.Log($"Enchantment = [{kv.Key}]{kv.Value.Count}");
            }

            UnityEngine.Random.InitState(System.DateTime.UtcNow.Ticks.GetHashCode());
        }

        public static void AddRandomEnchantment(this ItemEntity item) {
            var rarity = item.Rarity();

            if (rarity == RarityType.Notable)
                rarity = RarityType.Uncommon;

            Mod.Log($"[Enchant] Item {item} Rarity = {rarity}");

            var fake_context = new MechanicsContext(default); // if context is null, items may stack which could cause bugs

            for (var i = 0; i < 3; i++) {
                if (_cachedEnchantment.TryGetValue(rarity, out var list) && list?.Any() == true) {
                    var enchantment = list.Random();
                    if ((item is ItemEntityWeapon && enchantment is BlueprintArmorEnchantment)
                        || (item is ItemEntityArmor && enchantment is BlueprintWeaponEnchantment)
                        || (item is ItemEntitySimple && (enchantment is BlueprintWeaponEnchantment || enchantment is BlueprintArmorEnchantment))
                        || (item is ItemEntityUsable)
                        )
                        continue;

                    Mod.Log($"Add Enchantment {enchantment} to item {item}");

                    var itemEntityShield = item as ItemEntityShield;
                    switch (enchantment) {
                        case BlueprintWeaponEnchantment _ when itemEntityShield != null:
                            var weaponComponent = itemEntityShield.WeaponComponent;
                            if (weaponComponent == null)
                                break;
                            weaponComponent.AddEnchantment(enchantment, fake_context);
                            break;
                        case BlueprintArmorEnchantment _ when itemEntityShield != null:
                            itemEntityShield.ArmorComponent.AddEnchantment(enchantment, fake_context);
                            break;
                        default:
                            item.AddEnchantment(enchantment, fake_context);
                            break;
                    }

                    return;
                }
            }
        }

        public static BlueprintItem GenerateRandomLoot() {
            var bps = BlueprintExtensions.GetBlueprints<BlueprintItem>();
            if (bps?.Any() == true) {
                var bp = bps.Random();
                var rarity = bp.Rarity();
                if ( rarity >= Main.Settings.minLootRarity && rarity != RarityType.Notable )
                    return bp;
            }

            return null;
        }

        public static BlueprintItem MoreLikeThis(this ItemEntity item, bool upgrade = false) {
            if (item.Blueprint == null)
                return null;

            Type type = item.Blueprint.GetType();

            Mod.Log($"type = {type}, {_cachedLoots.ContainsKey(type)}");

            if (!_cachedLoots.TryGetValue(type, out var bps) || bps == null)
                return null;

            RarityType rarity = item.Rarity();

            if (upgrade) {
                Mod.Log($"Upgrade from {rarity} to {rarity + 1}");
                rarity++;
            }

            if (rarity >= RarityType.Notable)
                rarity = RarityType.Notable - 1;

            while (rarity >= Main.Settings.minLootRarity) {
                Mod.Log($"Rarity = {rarity}");

                if (bps.TryGetValue(rarity, out var list) && list?.Any() == true) {
                    return list.Random();
                }

                rarity--;
            }

            return null;
        }

        public static void GenerateRandomLoot(this LootWrapper loot, bool fUpgrade = false) {
            const string c_prefix = "[Rnd]";

            ItemsCollection collection = loot.GetInteraction();
            if (collection == null)
                return;

            var existingRandomItems = collection.Where(item => item.UniqueId.StartsWith(c_prefix) && item.Blueprint != null).Select(item => item.Blueprint).ToList();

            if (existingRandomItems.Any()) {
                if (!Main.Settings.toggleResetRandomLootEveryLoading)
                    return;

                // remove existing item with unique name starting with [Rnd]
                foreach (var bp in existingRandomItems) {
                    collection.Remove(bp);
                }
            }

            List<BlueprintItem> newLoots = new();

            foreach (var lootItem in collection) {
                var newLoot = lootItem.MoreLikeThis(upgrade: UnityEngine.Random.Range(0, 100) > 80);

                Mod.Log($"Bp = {newLoot?.Name} : {newLoot?.AssetGuid} from {lootItem.Blueprint}");

                if (newLoot != null)
                    newLoots.Add(newLoot);
            }

            if (!newLoots.Any()) {
                var count = UnityEngine.Random.Range(1, 4); // 1,2,3
                while (count-- > 0) {
                    var pureRandomLoot = GenerateRandomLoot();
                    if (pureRandomLoot != null)
                        newLoots.Add(pureRandomLoot);
                }
            }

            foreach (var bpItem in newLoots) {
                var entity = bpItem.CreateEntity();
                entity.UniqueId = c_prefix + entity.UniqueId;

                entity.AddRandomEnchantment();

                collection.Add(entity);
            }
        }

        public static string NameAndOwner(this ItemEntity u, bool showRating, bool darkmode = false) =>
            (showRating ? $"{u.Rating()} ".orange().bold() : "")
#if Wrath
            + (u.Owner != null ? $"({u.Owner.CharacterName}) ".orange() : "")
#elif RT
            + (u.Owner != null ? $"({u.Owner.Name}) ".orange() : "")
#endif                
            + (darkmode ? u.Name.StripHTML().DarkModeRarity(u.Rarity()) : u.Name);
        public static string NameAndOwner(this ItemEntity u, bool darkmode = false) => u.NameAndOwner(Main.Settings.showRatingForEnchantmentInventoryItems, darkmode);
        public static bool IsLootable(this ItemEntity item, RarityType filter = RarityType.None) {
            var rarity = item.Rarity();
            if ((int)rarity < (int)filter) return false;
            return item.IsLootable;
        }
        public static List<ItemEntity> Lootable(this List<ItemEntity> loots, RarityType filter = RarityType.None) => loots.Where(l => l.IsLootable(filter)).ToList();
        public static string GetName(this LootWrapper present) {
            if (present.InteractionLoot != null) {
                //                var name = present.InteractionLoot.Owner.View.name;
#if Wrath
                var name = present.InteractionLoot.Source.name;
#elif RT
                var name = present.InteractionLoot.Source.ToString();
#endif
                if (name == null || name.Length == 0) name = "Ground";
                return name;
            }
            if (present.Unit != null) return present.Unit.CharacterName;
            return null;
        }

        public static ItemsCollection GetInteraction(this LootWrapper present) {
            if (present.InteractionLoot != null) return present.InteractionLoot.Loot;

            if (present.Unit != null) return present.Unit.Inventory;

            return null;
        }
        public static IEnumerable<ItemEntity> Search(this IEnumerable<ItemEntity> items, string searchText) => items.Where(i => searchText.Length > 0 ? i.Name.ToLower().Contains(searchText.ToLower()) : true);
        public static List<ItemEntity> GetLewtz(this LootWrapper present, string searchText = "") {
            if (present.InteractionLoot != null) return present.InteractionLoot.Loot.Items.Search(searchText).ToList();
            if (present.Unit != null) return present.Unit.Inventory.Items.Search(searchText).ToList();
            return null;
        }
#if Wrath
        public static List<LootWrapper> GetMassLootFromCurrentArea() {
            List<LootWrapper> lootWrapperList = new();
            var units = Shodan.AllUnits
                .Where<UnitEntityData>((Func<UnitEntityData, bool>)(u => u.IsInGame && !u.Descriptor.IsPartyOrPet()));
            //.Where<UnitEntityData>((Func<UnitEntityData, bool>)(u => u.IsRevealed && u.IsDeadAndHasLoot));
            foreach (var unitEntityData in units)
                lootWrapperList.Add(new LootWrapper() {
                    Unit = unitEntityData
                });
            var interactionLootParts = Game.Instance.State.MapObjects.All
                .Where<EntityDataBase>(e => e.IsInGame)
                .Select<EntityDataBase, InteractionLootPart>(i => i.Get<InteractionLootPart>())
                .Where<InteractionLootPart>(i => i?.Loot != Game.Instance.Player.SharedStash)
                .NotNull<InteractionLootPart>();
            var source = TempList.Get<InteractionLootPart>();
            foreach (var interactionLootPart in interactionLootParts) {
                if (// interactionLootPart.Owner.IsRevealed && 
                    interactionLootPart.Loot?.HasLoot ?? true
                    //&& (
                    //    interactionLootPart.LootViewed || interactionLootPart.View is DroppedLoot && !(bool)(EntityPart)interactionLootPart.Owner.Get<DroppedLoot.EntityPartBreathOfMoney>() || (bool)(UnityEngine.Object)interactionLootPart.View.GetComponent<SkinnedMeshRenderer>()
                    //    )
                    )
                    source.Add(interactionLootPart);
            }
            var collection = source.Distinct<InteractionLootPart>((IEqualityComparer<InteractionLootPart>)new MassLootHelper.LootDuplicateCheck()).Select<InteractionLootPart, LootWrapper>((Func<InteractionLootPart, LootWrapper>)(i => new LootWrapper() {
                InteractionLoot = i
            }));
            lootWrapperList.AddRange(collection);
            return lootWrapperList;
        }
#elif RT
        // TODO: implement ToyBox improvements
        public static IEnumerable<LootWrapper> GetMassLootFromCurrentArea()
        {
            var lootFromCurrentArea = new List<LootWrapper>();
            foreach (var baseUnitEntity in Shodan.AllUnits.Where(u => u.IsRevealed && u.IsDeadAndHasLoot))
                lootFromCurrentArea.Add(new LootWrapper
                {
                    Unit = baseUnitEntity
                });
            var interactionLootParts = Game.Instance.State.MapObjects.Select(i => i.GetOptional<InteractionLootPart>())
                                           .Concat(Game.Instance.State.AllUnits.Select(i => i.GetOptional<InteractionLootPart>())).NotNull();
            var source = TempList.Get<InteractionLootPart>();
            foreach (var interactionLootPart in interactionLootParts)
                if (interactionLootPart.Owner.IsRevealed && interactionLootPart.Loot.HasLoot &&
                    (interactionLootPart.LootViewed ||
                     (interactionLootPart.View is DroppedLoot && !(bool)(EntityPart)interactionLootPart.Owner
                                                                                                       .GetOptional<DroppedLoot.EntityPartBreathOfMoney>()) ||
                     (bool)(UnityEngine.Object)interactionLootPart.View.GetComponent<SkinnedMeshRenderer>()))
                    source.Add(interactionLootPart);
            var collection = source.Distinct(new LootDuplicateCheck()).Select(i => new LootWrapper
            {
                InteractionLoot = i
            });
            lootFromCurrentArea.AddRange(collection);
            return lootFromCurrentArea;
        }
#endif
#if Wrath
        public static void ShowAllChestsOnMap(bool hidden = false) {
            var interactionLootParts = Game.Instance.State.MapObjects.All
                .Where<EntityDataBase>(e => e.IsInGame)
                .Select<EntityDataBase, InteractionLootPart>(i => i.Get<InteractionLootPart>())
                .Where<InteractionLootPart>(i => i?.Loot != Game.Instance.Player.SharedStash)
                .NotNull<InteractionLootPart>();
            foreach (var interactionLootPart in interactionLootParts) {
                if (hidden) interactionLootPart.Owner.IsPerceptionCheckPassed = true;
                interactionLootPart.Owner.SetIsRevealedSilent(true);
            }
        }

        public static void ShowAllInevitablePortalLoot() {
            var interactionLootRevealers = Game.Instance.State.MapObjects.All.OfType<MapObjectEntityData>()
                .Where(e => e.IsInGame)
                .SelectMany(e => e.Interactions).OfType<InteractionSkillCheckPart>().NotNull()
                .Where(i => i.Settings?.DC == 0 && i.Settings.Skill == StatType.Unknown)
                .SelectMany(i => i.Settings.CheckPassedActions?.Get()?.Actions?.Actions ?? new GameAction[0]).OfType<HideMapObject>()
                .Where(a => a.Unhide)
                .Where(a => a.MapObject.GetValue()?.Get<InteractionLootPart>() is not null);
            foreach (var revealer in interactionLootRevealers) {
                revealer.RunAction();
            }
        }
#endif
        public static void OpenMassLoot() {
            var loot = MassLootHelper.GetMassLootFromCurrentArea();
            if (loot == null) return;
            var count = loot.Count();
            var count2 = loot.Count(present => present.InteractionLoot != null);
            Mod.Debug($"MassLoot: Count = {loot.Count()}");
            Mod.Debug($"MassLoot: Count2 = {count}");
            if (count == 0) return;
            // Access to LootContextVM
            var contextVM = RootUIContext.Instance
#if Wrath
                                         .InGameVM?
#elif RT
                                         .SurfaceVM?
#endif
                                         .StaticPartVM?.LootContextVM;
            if (contextVM == null) return;
            // Add new loot...
            var lootVM = new LootVM(LootContextVM.LootWindowMode.ZoneExit, loot, null, () => contextVM.DisposeAndRemove(contextVM.LootVM));

            // Open window add lootVM int contextVM
            contextVM.LootVM.Value = lootVM;

            //EventBus.RaiseEvent((Action<ILootInterractionHandler>)(e => e.HandleZoneLootInterraction(null)));
        }
        public static void OpenPlayerChest() {
            // Access to LootContextVM
            var contextVM = RootUIContext.Instance
#if Wrath
                                         .InGameVM?
#elif RT
                                         .SurfaceVM?
#endif                                         
                                         .StaticPartVM?.LootContextVM;
            if (contextVM == null) return;
            // Add new loot...
            var objects = new EntityViewBase[] { };
            var lootVM = new LootVM(LootContextVM.LootWindowMode.PlayerChest, objects, () => contextVM.DisposeAndRemove(contextVM.LootVM));
            var sharedStash = Game.Instance.Player.SharedStash;
#if Wrath
            var lootObjectVM = new LootObjectVM("Player Chest".localize(),
                                                "",
                                                sharedStash,
                                                LootContextVM.LootWindowMode.PlayerChest,
                                                1);
#elif RT
            var lootObjectVM = new LootObjectVM(LootObjectType.Normal,
                                                "Player Chest".localize(), 
                                                "",
                                                null,
                                                null,
                                                sharedStash,
                                                null,
                                                LootContextVM.LootWindowMode.PlayerChest
                                                );
#endif
            lootVM.ContextLoot.Add(lootObjectVM);
            lootVM.AddDisposable(lootObjectVM);
            // Open window add lootVM int contextVM
            contextVM.LootVM.Value = lootVM;
        }
    }
}