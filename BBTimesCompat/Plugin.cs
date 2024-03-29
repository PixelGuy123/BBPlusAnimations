using BBPlusAnimations.Components;
using BBTimes.CustomComponents.CustomDatas;
using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using UnityEngine;
using MTM101BaldAPI.AssetTools;
using System.IO;


namespace BBAnimationsForTimes
{
    [BepInPlugin("pixelguy.pixelmodding.baldiplus.newanimationstimes", PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
	[BepInDependency("mtm101.rulerp.bbplus.baldidevapi", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.bbextracontent", BepInDependency.DependencyFlags.HardDependency)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.newanimations")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
			LoadingEvents.RegisterOnAssetsLoaded(() =>
			{
				NPCMetaStorage.Instance.Get(Character.Sweep).prefabs.DoIf(x => x.Value.name == "ClassicGottaSweep" && x.Value.GetComponent<ClassicGottaSweepCustomData>(), (x) => {
					var comp = x.Value.gameObject.GetComponent<GenericAnimationExtraComponent>() ?? x.Value.gameObject.AddComponent<GenericAnimationExtraComponent>();
					comp.sprites = new Sprite[8];
					comp.sprites[0] = x.Value.spriteRenderer[0].sprite;
					string path = AssetLoader.GetModPath(BepInEx.Bootstrap.Chainloader.PluginInfos["pixelguy.pixelmodding.baldiplus.bbextracontent"].Instance); // Gets bb times instance
					for (int i = 1; i < comp.sprites.Length; i++)
						comp.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(path, "npcs", "ClassicGottaSweep", "Textures", "anims", $"oldsweep_{i - 1}.png")), 26f);
					});
			}, true);
        }
    }
}
