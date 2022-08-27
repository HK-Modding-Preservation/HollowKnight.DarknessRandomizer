using DarknessRandomizer.Lib;
using ItemChanger;
using Modding;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace DarknessRandomizer.Rando
{
    public class DarknessRandomizerModule : ItemChanger.Modules.Module
    {
        public Dictionary<string, Darkness> DarknessOverrides = new();

        [JsonIgnore]
        private readonly List<Action> UnloadHooks = new();

        public override void Initialize()
        {
            InstallHook(new LambdaHook(
                () => ModHooks.SceneChanged += MaybeDeleteHazardRespawns,
                () => ModHooks.SceneChanged -= MaybeDeleteHazardRespawns));
        }

        public override void Unload() => UnloadHooks.ForEach(a => a.Invoke());

        private void InstallHook(IHook h)
        {
            h.Load();
            UnloadHooks.Add(() => h.Unload());
        }

        private bool PlayerHasLantern() => PlayerData.instance.GetBool(nameof(PlayerData.hasLantern));

        private void MaybeDeleteHazardRespawns(string scene)
        {
            DarknessRandomizer.Log($"Maybe delete? {scene}");
            if (!PlayerHasLantern() && DarknessOverrides.TryGetValue(scene, out Darkness d) && d == Darkness.Dark)
            {
                DarknessRandomizer.Log("Attempting to delete things!");
                foreach (var obj in GameObject.FindObjectsOfType<HazardRespawnTrigger>())
                {
                    DarknessRandomizer.Log("Deleting thing!");
                    GameObject.Destroy(obj);
                }
            }
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
