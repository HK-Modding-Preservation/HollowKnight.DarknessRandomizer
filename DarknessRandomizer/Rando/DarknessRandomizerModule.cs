using DarknessRandomizer.Data;
using DarknessRandomizer.Lib;
using ItemChanger;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DarknessRandomizer.Rando
{
    public class DarknessRandomizerModule : ItemChanger.Modules.Module
    {
        public DarknessDictionary DarknessOverrides = new();

        [JsonIgnore]
        private readonly List<Action> UnloadHooks = new();

        public override void Initialize()
        {
            InstallHook(new LambdaHook(
                () => Events.OnSceneChange += AdjustDarknessRelatedObjects,
                () => Events.OnSceneChange -= AdjustDarknessRelatedObjects));
        }

        public override void Unload() => UnloadHooks.ForEach(a => a.Invoke());

        private void InstallHook(IHook h)
        {
            h.Load();
            UnloadHooks.Add(() => h.Unload());
        }

        private bool PlayerHasLantern() => PlayerData.instance.GetBool(nameof(PlayerData.hasLantern));

        private void DeleteHazardRespawnTriggers()
        {
            foreach (var obj in GameObject.FindObjectsOfType<HazardRespawnTrigger>())
            {
                GameObject.Destroy(obj);
            }
        }

        private void AdjustDarknessRelatedObjects(UnityEngine.SceneManagement.Scene scene)
        {
            if (!SceneName.TryGetSceneName(scene.name, out SceneName sceneName)
                || !DarknessOverrides.TryGetValue(sceneName, out Darkness darkness))
            {
                darkness = Darkness.Bright;
            }

            if (darkness == Darkness.Dark && !PlayerHasLantern())
            {
                DeleteHazardRespawnTriggers();
            }

            // FIXME: Scenes
            if (darkness != Darkness.Dark) { }

            // FIXME: Custom respawners for bosses, arenas
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
        public FsmEditHook(string scene, FsmID id, Action<PlayMakerFSM> action) : base(
            () => Events.AddFsmEdit(scene, id, action),
            () => Events.RemoveFsmEdit(scene, id, action)) { }

        public FsmEditHook(FsmID id, Action<PlayMakerFSM> action) : base(
            () => Events.AddFsmEdit(id, action),
            () => Events.RemoveFsmEdit(id, action)) { }
    }
}
