using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessRandomizer.Rando
{
    public class DarknessRandomizerModule : ItemChanger.Modules.Module
    {
        public SceneDarknessDict DarknessOverrides = new();

        [JsonIgnore]
        private readonly List<Action> UnloadHooks = new();

        public override void Initialize()
        {
            InstallHook(new LambdaHook(
                () => Modding.ModHooks.GetPlayerBoolHook += OverrideGetBool,
                () => Modding.ModHooks.GetPlayerBoolHook -= OverrideGetBool));
            InstallHook(new LambdaHook(
                () => Events.OnSceneChange += AdjustDarknessRelatedObjects,
                () => Events.OnSceneChange -= AdjustDarknessRelatedObjects));

            // Allow dark objects to be used if the room is bright.
            InstallMaybeDisableLanternCheck(SceneName.CrossroadsPeakDarkToll, new("Toll Gate Machine", "Disable if No Lantern"));
            InstallMaybeDisableLanternCheck(SceneName.CrossroadsPeakDarkToll, new("Toll Gate Machine (1)", "Disable if No Lantern"));
            InstallMaybeDisableLanternCheck(SceneName.GreenpathStoneSanctuary, new("Ghost Warrior NPC", "FSM"));

            // Delete ghost warriors in dark rooms.
            InstallDeleteGhostWarriorIfDark(SceneName.CliffsGorb);
            InstallDeleteGhostWarriorIfDark(SceneName.DeepnestGalienArena);
            InstallDeleteGhostWarriorIfDark(SceneName.EdgeMarkothArena);
            InstallDeleteGhostWarriorIfDark(SceneName.FungalElderHu);
            InstallDeleteGhostWarriorIfDark(SceneName.GardensGardensStag);
            InstallDeleteGhostWarriorIfDark(SceneName.GreenpathStoneSanctuary);
            InstallDeleteGhostWarriorIfDark(SceneName.GroundsXero);

            // Make tollgates unusable in dark rooms.
            InstallDarkTollgateCheck(SceneName.GreenpathToll, new("Toll Gate Machine", "Toll Machine"));
            InstallDarkTollgateCheck(SceneName.GreenpathToll, new("Toll Gate Machine (1)", "Toll Machine"));
            InstallDarkTollgateCheck(SceneName.CityTollBench, new("Toll Machine Bench", "Toll Machine Bench"));
            InstallDarkTollgateCheck(SceneName.BasinCorridortoBrokenVessel, new("Toll Machine Bench", "Toll Machine Bench"));

            // The Shade Soul door is inoperable in the dark.
            InstallElegantKeyDarkCheck();

            // Preserve hazard respawns in combat arenas.
            PreservedHazardRespawns.GetOrAddNew(SceneName.CrossroadsGlowingWombArena).Add("Hazard Respawn Trigger v2");
            PreservedHazardRespawns.GetOrAddNew(SceneName.FogUumuuArena).Add("Hazard Respawn Trigger v2 (6)");
            PreservedHazardRespawns.GetOrAddNew(SceneName.FogOvergrownMound).Add("Hazard Respawn Trigger v2");
            PreservedHazardRespawns.GetOrAddNew(SceneName.FungalMantisLords).Add("Hazard Respawn Trigger (5)");
            PreservedHazardRespawns.GetOrAddNew(SceneName.CrystalMound).Add("Hazard Respawn Trigger v2 (3)");
        }

        public override void Unload() => UnloadHooks.ForEach(a => a.Invoke());

        private void InstallHook(IHook h)
        {
            h.Load();
            UnloadHooks.Add(() => h.Unload());
        }

        private bool PlayerHasLantern() => PlayerData.instance.GetBool(nameof(PlayerData.hasLantern));

        private static readonly Dictionary<SceneName, HashSet<string>> PreservedHazardRespawns = new();

        private void DisableDarkRoomObjects(SceneName sceneName)
        {
            foreach (var obj in GameObject.FindObjectsOfType<HazardRespawnTrigger>())
            {
                if (sceneName == null || !PreservedHazardRespawns.TryGetValue(sceneName, out HashSet<string> names)
                    || !names.Contains(obj.name))
                {
                    obj.gameObject.GetOrAddComponent<DeactivateInDarknessWithoutLantern>();
                }
            }
        }

        private void EnableDisabledLanternObjects()
        {
            foreach (var obj in GameObject.FindObjectsOfType<DeactivateInDarknessWithoutLantern>(true))
            {
                obj.enabled = false;
                obj.gameObject.SetActive(true);
            }
        }

        private void AdjustDarknessRelatedObjects(UnityEngine.SceneManagement.Scene scene)
        {
            if (!SceneName.TryGetSceneName(scene.name, out SceneName sceneName)
                || !DarknessOverrides.TryGetValue(sceneName, out Darkness darkness))
            {
                darkness = Darkness.Bright;
            }

            bool dark = darkness == Darkness.Dark;
            bool lantern = PlayerHasLantern();
            if (!lantern)
            {
                if (dark)
                {
                    DisableDarkRoomObjects(sceneName);
                }
                else
                {
                    EnableDisabledLanternObjects();
                }
            }
        }

        private const string TrueBool = "DarknessRandomizerTrue";
        private const string FalseBool = "DarknessRandomizerFalse";
        private readonly Dictionary<string, bool> customBool = new()
        {
            { TrueBool, true },
            { FalseBool, false }
        };

        private bool IsDark(SceneName sceneName)
        {
            if (PlayerHasLantern()) return false;

            if (DarknessOverrides.TryGetValue(sceneName, out Darkness d))
            {
                return d == Darkness.Dark;
            }
            return false;
        }

        private bool OverrideGetBool(string name, bool orig) => customBool.TryGetValue(name, out bool b) ? b : orig;

        private void InstallMaybeDisableLanternCheck(SceneName sceneName, FsmID id)
        {
            InstallHook(new FsmEditHook(sceneName, id, fsm =>
            {
                if (!IsDark(sceneName))
                {
                    fsm.GetState("Check").GetFirstActionOfType<PlayerDataBoolTest>().boolName = TrueBool;
                }
            }));
        }

        private void InstallDarkTollgateCheck(SceneName sceneName, FsmID id)
        {
            InstallHook(new FsmEditHook(sceneName, id, fsm =>
            {
                if (IsDark(sceneName))
                {
                    fsm.GetState("Can Inspect?").GetFirstActionOfType<BoolTest>().boolVariable = new FsmBool() { Value = false };
                }
            }));
            InstallHook(new FsmEditHook(sceneName, new("Arrow Prompt(Clone)", "Prompt Control"), fsm =>
            {
                if (IsDark(sceneName))
                {
                    GameObject.Destroy(fsm.gameObject);
                }
            }));
        }

        private void InstallDeleteGhostWarriorIfDark(SceneName sceneName)
        {
            InstallHook(new FsmEditHook(sceneName, new("Ghost Warrior NPC", "Conversation Control"), fsm =>
            {
                if (IsDark(sceneName))
                {
                    fsm.gameObject.GetOrAddComponent<DeactivateInDarknessWithoutLantern>();
                }
            }));
        }

        private void InstallElegantKeyDarkCheck()
        {
            InstallHook(new FsmEditHook(SceneName.CityTollBench, new("Mage Door", "npc_control"), fsm =>
            {
                if (IsDark(SceneName.CityTollBench))
                {
                    fsm.GetState("Can Talk?").GetFirstActionOfType<BoolTest>().boolVariable = new FsmBool() { Value = false };
                    GameObject.Find("/Mage Door/Prompt Marker")?.GetOrAddComponent<DeactivateInDarknessWithoutLantern>();
                }
            }));
        }
    }

    interface IHook
    {
        public void Load();
        public void Unload();
    }

    class LambdaHook : IHook
    {
        private readonly Action load;
        private readonly Action unload;

        public LambdaHook(Action load, Action unload)
        {
            this.load = load;
            this.unload = unload;
        }

        public void Load() => load.Invoke();

        public void Unload() => unload.Invoke();
    }

    class FsmEditHook : LambdaHook
    {
        public FsmEditHook(SceneName scene, FsmID id, Action<PlayMakerFSM> action) : base(
            () => Events.AddFsmEdit(scene.Name(), id, action),
            () => Events.RemoveFsmEdit(scene.Name(), id, action)) { }

        public FsmEditHook(FsmID id, Action<PlayMakerFSM> action) : base(
            () => Events.AddFsmEdit(id, action),
            () => Events.RemoveFsmEdit(id, action)) { }
    }
}
