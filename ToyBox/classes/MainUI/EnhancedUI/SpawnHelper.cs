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
#if Wrath
using Kingmaker.UI.MVVM._PCView.Loot;
using Kingmaker.UI.MVVM._VM.Loot;
#endif

namespace ToyBox {
    internal static class SpawnHelper {
        public static RandomSpawnLogic RandomSpawnLogic = new RandomSpawnLogic();

    }

    internal class RandomSpawnLogic : IUnitSpawnHandler {
        public void Init() {
            EventBus.Subscribe(this);
            Mod.Log( "Subscribe to RandomSpawnLogic" );
        }

        public void HandleUnitSpawned(UnitEntityData unit) {
            Mod.Log( $"Current Spawn Unit: {unit} " );

            if ( unit.IsPartyOrPet())
                return;

            if( Main.Settings.toggleRandomLootForEnemy ) {
                new LootWrapper() {
                    Unit = unit,
                }.GenerateRandomLoot();
            }
        }
    }
}