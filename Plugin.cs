using BepInEx;
using HarmonyLib;
using MTM101BaldAPI.Registers;
using UnityEngine;
using PixelInternalAPI.Classes;
using System.Linq;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI;
using PixelInternalAPI.Components;
using System.IO;
using BBPlusAnimations.Patches;
using BBPlusAnimations.Components;
using PixelInternalAPI.Extensions;
using UnityEngine.UI;
using MTM101BaldAPI.Reflection;

namespace BBPlusAnimations
{
	[BepInPlugin(ModInfo.PLUGIN_GUID, ModInfo.PLUGIN_NAME, ModInfo.PLUGIN_VERSION)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.pixelinternalapi")]
	[BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

	internal class BasePlugin : BaseUnityPlugin
	{
		private void Awake()
		{
			Harmony h = new(ModInfo.PLUGIN_GUID);
			h.PatchAll();

			ModPath = AssetLoader.GetModPath(this);

			LoadingEvents.RegisterOnAssetsLoaded(OnAssetLoad, false);
		}


		
		void OnAssetLoad()
		{

			// Sprite Billboard object
			var baseSprite = new GameObject("SpriteBillBoard").AddComponent<SpriteRenderer>();
			baseSprite.material = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name == "SpriteStandard_Billboard" && x.GetInstanceID() > 0);
			baseSprite.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			baseSprite.receiveShadows = false;

			baseSprite.gameObject.layer = LayerStorage.billboardLayer;
			DontDestroyOnLoad(baseSprite.gameObject);
			man.Add("SpriteBillboardTemplate", baseSprite.gameObject);

			// Sprite NO Billboard object
			baseSprite = new GameObject("SpriteNoBillBoard").AddComponent<SpriteRenderer>();
			baseSprite.material = Resources.FindObjectsOfTypeAll<Material>().First(x => x.name == "SpriteStandard_NoBillboard" && x.GetInstanceID() > 0);
			baseSprite.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			baseSprite.receiveShadows = false;

			baseSprite.gameObject.layer = LayerStorage.billboardLayer;
			DontDestroyOnLoad(baseSprite.gameObject);
			baseSprite.gameObject.SetActive(false);
			man.Add("SpriteNoBillboardTemplate", baseSprite.gameObject);

			// Gum
			var gumHolder = new GameObject("gumSplash");
			var gum = Instantiate(man.Get<GameObject>("SpriteNoBillboardTemplate")); // front of the gum
			gum.GetComponent<SpriteRenderer>().sprite = AssetLoader.SpriteFromTexture2D(
				AssetLoader.TextureFromFile(Path.Combine(ModPath, "gumSplash.png"))
				, 25f);
			gum.layer = LayerStorage.billboardLayer;
			gum.transform.SetParent(gumHolder.transform);
			gum.transform.localPosition = Vector3.zero;
			gum.SetActive(true);

			gum = Instantiate(man.Get<GameObject>("SpriteNoBillboardTemplate")); // Back of the gum
			gum.GetComponent<SpriteRenderer>().sprite = AssetLoader.SpriteFromTexture2D(
				AssetLoader.TextureFromFile(Path.Combine(ModPath, "gumSplash_back.png"))
				, 25f);
			gum.transform.SetParent(gumHolder.transform);
			gum.transform.localPosition = Vector3.zero + gum.transform.forward * -0.01f;
			gum.SetActive(true);

			gumHolder.SetActive(false);
			DontDestroyOnLoad(gumHolder);

			gumHolder.AddComponent<EmptyMonoBehaviour>(); // For coroutines
			GumSplash.gumSplash = gumHolder.transform;

			GumSplash.splash = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "gumSplash.wav")), "Vfx_GumSplash", SoundType.Voice, new(0.99609f, 0, 0.99609f));

			// Bsoda particles
			Resources.FindObjectsOfTypeAll<ITM_BSODA>().Do(x => x.transform.Find("RendereBase").Find("Particles").gameObject.SetActive(true));

			// Door
			Resources.FindObjectsOfTypeAll<StandardDoor>().Do(d => d.gameObject.AddComponent<StandardDoorExtraMaterials>().defaultTex = [AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock.png")), AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock_left.png"))]);// Sets a lock

			// Balloon Pop Animation
			NumberBalloonPatch.explodeVisuals = new Sprite[6];
			for (int i = 0; i < NumberBalloonPatch.explodeVisuals.Length; i++)
				NumberBalloonPatch.explodeVisuals[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"balExplode_{i}.png")), 30f);
			NumberBalloonPatch.sound = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "BalloonRespawn.wav")), "something", SoundType.Voice, Color.white);
			NumberBalloonPatch.sound.subtitle = false; // No sub

			// TheTestAnimation

			var canvas = Instantiate(Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "GumOverlay")); // Only way to make a proper overlay on this

			var img = canvas.transform.Find("Image").GetComponent<Image>();

			DontDestroyOnLoad(canvas);
			canvas.gameObject.SetActive(false);


			TheTestPatch.sprites = new Sprite[7];

			TheTestPatch.sprites[0] = AssetLoader.SpriteFromTexture2D(TextureExtensions.CreateSolidTexture(480, 360, Color.black), 1f);
			for (int i = 1; i < TheTestPatch.sprites.Length; i++)
				TheTestPatch.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"theTestblind_{i - 3}.png")), 1f);

			img.name = "TheTestOverlay";
			TheTestPatch.img = img;
			canvas.name = "TheTestOverlayCanvas";
			TheTestPatch.canvas = canvas;

			// Jumprope Sprites
			PlaytimeJumpropePatch.sprites = new Sprite[6];
			for (int i = 0; i < PlaytimeJumpropePatch.sprites.Length; i++)
				PlaytimeJumpropePatch.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"jmpropecut_{i}.png")), 1f);

			// Baldi Haha
			var baldi = Instantiate(man.Get<GameObject>("SpriteBillboardTemplate")).AddComponent<BaldiFloatsAway>();
			baldi.name = "BaldiTheFloater";
			baldi.renderer = baldi.GetComponent<SpriteRenderer>();
			baldi.sprites = (Sprite[])Resources.FindObjectsOfTypeAll<BaldiDance>()[0].ReflectionGetVariable("danceSprites"); // First time I'm using the api reflection lol
			DontDestroyOnLoad(baldi.gameObject);
			baldi.gameObject.SetActive(false);
			HappyBaldiPatch.baldi = baldi;
			
		}

		readonly AssetManager man = new();
		

		internal static string ModPath = string.Empty;
	}

	static class ModInfo
	{
		public const string PLUGIN_GUID = "pixelguy.pixelmodding.baldiplus.newanimations";

		public const string PLUGIN_NAME = "BB+ New Animations";

		public const string PLUGIN_VERSION = "1.0.0";
	}
}
