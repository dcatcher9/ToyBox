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
using Epic.OnlineServices;
using Kingmaker.Blueprints;
using Kingmaker.Designers.EventConditionActionSystem.Evaluators;
using Kingmaker.AreaLogic.SummonPool;
#if Wrath
using Kingmaker.UI.MVVM._PCView.Loot;
using Kingmaker.UI.MVVM._VM.Loot;
#endif

namespace ToyBox {
    internal static class SpawnHelper {
        public static RandomSpawnLogic RandomSpawnLogic = new RandomSpawnLogic();

        // Token: 0x04000707 RID: 1799
        public static BlueprintFaction Mobs = ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("0f539babafb47fe4586b719d02aff7c4");

        // Token: 0x04000708 RID: 1800
        public static BlueprintFaction WildAnimals = ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("b1525b4b33efe0241b4cbf28486cd2cc");

        // Token: 0x04000709 RID: 1801
        public static BlueprintFaction AngelTeam = ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("ff7b854fc400a64419f4f45273eed7ee");

        // Token: 0x0400070A RID: 1802
        public static BlueprintFaction Attack_everyone = ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("f69acd336dd8e2b4ca9e9618c7142658");

        // Token: 0x0400070B RID: 1803
        public static BlueprintFaction PerpetuallyAnnoyedFaction = ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("572cce024818db7409655b91e971907b");

        // Token: 0x0400070C RID: 1804
        public static BlueprintFaction Summoned = ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("1b08d9ed04518ec46a9b3e4e23cb5105");

        // Token: 0x0400070D RID: 1805
        public static BlueprintFaction Player = ResourcesLibrary.TryGetBlueprint<BlueprintFaction>("72f240260881111468db610b6c37c099");

        public static BlueprintSummonPool RandomUnitsPool = new BlueprintSummonPool() {
            Limit = 10000,
            DoNotRemoveDeadUnits = false
        };

        public static UnitEntityData SpawnUnit(UnitEntityData unit) {
            ISummonPool pool = Game.Instance.SummonPools.GetPool(SpawnHelper.RandomUnitsPool);
            if (pool == null)
                return null;

            if (pool.Units.Contains(unit))
                return null;

            Vector3 vector = FindRandomPositionNearbyWithSelector(unit.Position, UnityEngine.Random.Range(3, 8), unit);

            UnitEntityData spawnedUnit = Game.Instance.EntityCreator.SpawnUnit(unit.Blueprint, vector, Quaternion.LookRotation(unit.OrientationDirection), Game.Instance.State.LoadedAreaState.MainState, null);

            pool.Register(spawnedUnit);

            Mod.Log($"Spawn Unit {spawnedUnit} from unit={unit}, Pool Count ={pool.Count}");

            return spawnedUnit;
        }

        public static Vector3 FindRandomPositionNearbyWithSelector(Vector3 startPosition, int distance, UnitEntityData entityData) {
            Vector3 vector = startPosition + new Vector3((float)UnityEngine.Random.Range(-distance, distance), 0f, -1f);
            UnitEntityView unitEntityView = entityData.Blueprint.Prefab.Load(false, false);
            float num = 0.5f;
            bool flag = unitEntityView != null;
            if (flag) {
                num = unitEntityView.Corpulence;
            }
            FreePlaceSelector.PlaceSpawnPlaces(3, num, vector);
            return ObstacleAnalyzer.GetNearestNode(vector, null).position;
        }

    }

    internal class RandomSpawnLogic : IUnitSpawnHandler {
        public void Init() {
            EventBus.Subscribe(this);
            Mod.Log("Subscribe to RandomSpawnLogic");
        }

        public void HandleUnitSpawned(UnitEntityData unit) {
            Mod.Log($"Current Spawn Unit: {unit} ");

            if (unit.IsPartyOrPet() || unit.Faction.AssetGuid != SpawnHelper.Mobs.AssetGuid)
                return;

            if (Main.Settings.toggleRandomLootForEnemy) {
                unit.GenerateRandomLoot();

                Mod.Log($"Add loot for newly spawned unit {unit}");
            }

            if (Main.Settings.toogleMoreRandomEnemies) {
                SpawnHelper.SpawnUnit(unit)?.GenerateRandomLoot();
            }
        }
    }
}