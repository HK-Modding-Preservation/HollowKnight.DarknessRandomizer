using DarknessRandomizer.Data;
using DarknessRandomizer.IC;
using DarknessRandomizer.Lib;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessRandomizer.Rando
{
    public class DarknessRandomizerModule : ItemChanger.Modules.Module
    {
        public SceneDarknessDict DarknessOverrides = new();
        public int NumLanternShardsCollected = 0;

        [JsonIgnore]
        private readonly List<Action> UnloadHooks = new();

        public override void Initialize()
        {
            InstallHook(new LambdaHook(
                () => Modding.ModHooks.GetPlayerBoolHook += OverrideGetBool,
                () => Modding.ModHooks.GetPlayerBoolHook -= OverrideGetBool));
            InstallHook(new LambdaHook(
                () => Modding.ModHooks.SetPlayerBoolHook += OverrideSetBool,
                () => Modding.ModHooks.SetPlayerBoolHook -= OverrideSetBool));
            InstallHook(new LambdaHook(
                () => Modding.ModHooks.GetPlayerIntHook += OverrideGetInt,
                () => Modding.ModHooks.GetPlayerIntHook -= OverrideGetInt));
            InstallHook(new LambdaHook(
                () => Modding.ModHooks.SetPlayerIntHook += OverrideSetInt,
                () => Modding.ModHooks.SetPlayerIntHook -= OverrideSetInt));
            InstallHook(new LambdaHook(
                () => On.SceneManager.Start += OnSceneManagerStart,
                () => On.SceneManager.Start -= OnSceneManagerStart));
            InstallHook(new FsmEditHook(new("Darkness Region"), ModifyDarknessRegions));

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
            InstallDarkTollgateCheck(SceneName.BasinCorridortoBrokenVessel, new("Toll Machine Bench", "Toll Machine Bench"));
            InstallDarkTollgateCheck(SceneName.CityTollBench, new("Toll Machine Bench", "Toll Machine Bench"));
            InstallDarkTollgateCheck(SceneName.GreenpathToll, new("Toll Gate Machine", "Toll Machine"));
            InstallDarkTollgateCheck(SceneName.GreenpathToll, new("Toll Gate Machine (1)", "Toll Machine"));

            // The Shade Soul door is inoperable in the dark.
            InstallElegantKeyDarkCheck();

            // Preserve hazard respawns in combat arenas.
            PreservedHazardRespawns.GetOrAddNew(SceneName.CrossroadsGlowingWombArena).Add("Hazard Respawn Trigger v2");
            PreservedHazardRespawns.GetOrAddNew(SceneName.FogOvergrownMound).Add("Hazard Respawn Trigger v2");
            PreservedHazardRespawns.GetOrAddNew(SceneName.FogUumuuArena).Add("Hazard Respawn Trigger v2 (6)");
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

        private static void DeactiveGameObject(GameObject obj)
        {
            if (obj == null) return;

            obj.GetOrAddComponent<DeactivateInDarknessWithoutLantern>().enabled = true;
            obj.SetActive(false);
        }

        private void DisableDarkRoomObjects()
        {
            if (sceneData == null) return;

            foreach (var obj in GameObject.FindObjectsOfType<HazardRespawnTrigger>())
            {
                if (!PreservedHazardRespawns.TryGetValue(sceneData.CurrentScene, out HashSet<string> names) || !names.Contains(obj.name))
                {
                    DeactiveGameObject(obj.gameObject);
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

        private record DarknessData
        {
            public SceneName CurrentScene;
            public Darkness PrevDarkness;
            public Darkness NewDarkness;

            public bool Brighter => PrevDarkness >= Darkness.SemiDark && PrevDarkness > NewDarkness;
            public bool Darker => NewDarkness >= Darkness.SemiDark && NewDarkness > PrevDarkness;
            public bool Unchanged => PrevDarkness == NewDarkness;
        }
        private DarknessData? sceneData;

        private DarknessData? ComputeDarknessData(string sceneName, int prevDarkness)
        {
            if (!SceneName.TryGetValue(sceneName, out SceneName currentScene)
                || !DarknessOverrides.TryGetValue(currentScene, out Darkness newDarkness))
            {
                return null;
            }

            Darkness prevDarknessTyped = prevDarkness.ToDarkness() ?? Darkness.SuperBright;

            return new()
            {
                CurrentScene = currentScene,
                PrevDarkness = prevDarknessTyped,
                NewDarkness = newDarkness
            };
        }

        private void OnSceneManagerStart(On.SceneManager.orig_Start orig, SceneManager self)
        {
            sceneData = ComputeDarknessData(self.gameObject.scene.name, self.darknessLevel);
            if (sceneData?.Unchanged ?? true)
            {
                orig(self);
                return;
            }

            self.darknessLevel = (int)sceneData.NewDarkness;
            orig(self);

            // Make logical changes first.
            if (!PlayerHasLantern())
            {
                if (sceneData.NewDarkness == Darkness.Dark)
                {
                    DisableDarkRoomObjects();
                }
                else
                {
                    EnableDisabledLanternObjects();
                }
            }
        }

        private void ModifyDarknessRegions(PlayMakerFSM fsm)
        {
            if (sceneData == null) return;

            Darkness? d = fsm.FsmVariables.FindFsmInt("Darkness").Value.ToDarkness();
            if (d == null) return;

            // Disable this darkness region only if our change obsoletes it.
            if ((sceneData.Brighter && d > sceneData.NewDarkness) || (sceneData.Darker && d < sceneData.NewDarkness))
            {
                fsm.GetState("Init").ClearTransitions();
            }
        }

        private const string TrueBool = "DarknessRandomizerTrue";
        private const string FalseBool = "DarknessRandomizerFalse";

        private bool OverrideGetBool(string name, bool orig)
        {
            return name switch
            {
                TrueBool => true,
                FalseBool => false,
                _ => orig
            };
        }

        private bool OverrideSetBool(string name, bool orig)
        {
            if (orig && name == nameof(PlayerData.instance.hasLantern))
            {
                NumLanternShardsCollected = LanternShards.TotalNumShards;
            }

            return orig;
        }

        private int OverrideGetInt(string name, int orig) => name == LanternShards.PDName ? NumLanternShardsCollected : orig;

        private int OverrideSetInt(string name, int orig)
        {
            if (name == LanternShards.PDName)
            {
                NumLanternShardsCollected = Math.Min(LanternShards.TotalNumShards, orig);
                return NumLanternShardsCollected;
            }

            return orig;
        }

        private bool IsDark(SceneName sceneName)
        {
            if (PlayerHasLantern()) return false;

            if (DarknessOverrides.TryGetValue(sceneName, out Darkness d))
            {
                return d == Darkness.Dark;
            }
            return false;
        }

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

        private static readonly Color darkTollColor = new(0.2647f, 0.2647f, 0.2647f);

        private void InstallDarkTollgateCheck(SceneName sceneName, FsmID id)
        {
            InstallHook(new FsmEditHook(sceneName, id, fsm =>
            {
                if (IsDark(sceneName))
                {
                    fsm.GetState("Can Inspect?").GetFirstActionOfType<BoolTest>().boolVariable = new FsmBool() { Value = false };
                    fsm.gameObject.GetComponent<tk2dSprite>().color = darkTollColor;
                }
            }));
            InstallHook(new FsmEditHook(sceneName, new("Arrow Prompt(Clone)", "Prompt Control"), fsm =>
            {
                if (IsDark(sceneName))
                {
                    DeactiveGameObject(fsm.gameObject);
                }
            }));
        }

        private void InstallDeleteGhostWarriorIfDark(SceneName sceneName)
        {
            InstallHook(new FsmEditHook(sceneName, new("Ghost Warrior NPC", "Conversation Control"), fsm =>
            {
                if (IsDark(sceneName))
                {
                    DeactiveGameObject(fsm.gameObject);
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
                    fsm.gameObject.GetComponent<tk2dSprite>().color = darkTollColor;
                    DeactiveGameObject(GameObject.Find("/Mage Door/Prompt Marker"));
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
