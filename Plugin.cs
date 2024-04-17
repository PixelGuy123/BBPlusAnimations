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
using UnityEngine.Animations;

namespace BBPlusAnimations
{
	[BepInPlugin(ModInfo.PLUGIN_GUID, ModInfo.PLUGIN_NAME, ModInfo.PLUGIN_VERSION)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.pixelinternalapi")]
	[BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

	internal class BasePlugin : BaseUnityPlugin
	{
		private void Awake()
		{
			try
			{
				Harmony h = new(ModInfo.PLUGIN_GUID);
				h.PatchAll();
			}
			catch (System.Exception e)
			{
				Debug.LogException(e);
				MTM101BaldiDevAPI.CauseCrash(Info, e);
				return;
			}
			

			ModPath = AssetLoader.GetModPath(this);

			LoadingEvents.RegisterOnAssetsLoaded(() =>
			{
				try
				{
					OnAssetLoad();
				}
				catch (System.Exception e)
				{
					Debug.LogException(e);
					MTM101BaldiDevAPI.CauseCrash(Info, e);
				}
			}, false);

			
		}

		void OnAssetLoad()
		{

			// Particle Material
			man.Add("particleMaterial", Items.ChalkEraser.GetFirstInstance().item.GetComponent<ParticleSystemRenderer>().material);

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

			// Overlay
			man.Add("gumOverlay", Resources.FindObjectsOfTypeAll<Canvas>().First(x => x.name == "GumOverlay"));

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

			GumSplash.splash = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "gumSplash.wav")), "Vfx_GumSplash", SoundType.Voice, new(1 ,0.2039f ,0.8863f));

			// Bsoda particles
			Resources.FindObjectsOfTypeAll<ITM_BSODA>().Do((x) =>
			{
				var r = x.transform.Find("RendereBase").Find("Particles"); // Yes, Rendere. I didn't mistyped
				r.gameObject.SetActive(true);
				r.transform.localPosition = Vector3.zero; // maybe this is the issue?
			});

			// Door
			Resources.FindObjectsOfTypeAll<StandardDoor>().Do(d => d.gameObject.AddComponent<StandardDoorExtraMaterials>().defaultTex = [AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock.png")), AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock_left.png"))]);// Sets a lock

			// Balloon Pop Animation
			NumberBalloonPatch.explodeVisuals = new Sprite[6];
			for (int i = 0; i < NumberBalloonPatch.explodeVisuals.Length; i++)
				NumberBalloonPatch.explodeVisuals[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"balExplode_{i}.png")), 30f);
			NumberBalloonPatch.sound = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "BalloonRespawn.wav")), "something", SoundType.Voice, Color.white);
			NumberBalloonPatch.sound.subtitle = false; // No sub

			// TheTestAnimation

			var canvas = Instantiate(man.Get<Canvas>("gumOverlay")); // Only way to make a proper overlay on this

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

			// Chalkles component
			Resources.FindObjectsOfTypeAll<ChalkFace>().Do((x) =>
			{
				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites[1] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "ChalkFace_1.png")), new Vector2(0.5f, 0.25f), 25f);
			});
			// Gotta Sweep Animation Sprites
			Resources.FindObjectsOfTypeAll<GottaSweep>().Do((x) =>
			{
				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites = new Sprite[7];
				comp.sprites[0] = x.spriteRenderer[0].sprite;
				for (int i = 1; i < comp.sprites.Length; i++)
					comp.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"sweep_{i}.png")), 26f);

			});

			// JumpRope Quick Fix
			Resources.FindObjectsOfTypeAll<Jumprope>().Do((x) =>
			{
				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites = null; // No reference in this
			});

			// Gum overlay animation
			GumSplash.sprites = new Sprite[6];
			GumSplash.sprites[5] = man.Get<Canvas>("gumOverlay").GetComponentInChildren<Image>().sprite;  // There must be at least one
			for (int i = 0; i < GumSplash.sprites.Length - 1; i++)
				GumSplash.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"gumOverlay_{i + 1}.png")), 1f);

			// Cloudy Copter assets and components
			Resources.FindObjectsOfTypeAll<Cumulo>().DoIf(x => !x.GetComponent<PropagatedAudioManager>(), x => x.gameObject.CreateAudioManager(45, 75));
			CumuloPatch.blowBeforeBlowing = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_Blow.wav")), "Vfx_CC_Breath", SoundType.Voice, Color.white);
			CumuloPatch.pah = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_PAH.wav")), "Vfx_CC_PAH", SoundType.Voice, Color.white);

			// Portal Posters
			var portalPoster = Resources.FindObjectsOfTypeAll<WindowObject>().First(x => x.name == "PortalPosterWindow");
			var portal = portalPoster.windowPre.gameObject.AddComponent<TextureAnimator>(); // Texture animator setup
			portal.texs = new Texture[10];
			portal.texs[0] = portalPoster.windowPre.windows[0].materials[1].mainTexture;
			portal.renderers = portalPoster.windowPre.windows;
			portal.defaultIndex = 1;
			portal.speed = 20f;
			for (int i = 1; i < portal.texs.Length; i++)
				portal.texs[i] = AssetLoader.TextureFromFile(Path.Combine(ModPath, $"portal_{i}.png"));

			// Plant Particles
			Sprite[] plantSprites = new Sprite[6];
			for (int i = 1; i < plantSprites.Length; i++)
				plantSprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"plant{i}.png")), new Vector2(0.5f, 0f),15f);
			var tex = AssetLoader.TextureFromFile(Path.Combine(ModPath, "leaves.png"));

			Resources.FindObjectsOfTypeAll<RendererContainer>().DoIf(x => x.name.StartsWith("Plant"), (x) =>
			{
				ParticleSystem[] pars = new ParticleSystem[3];

				for (int i = 0; i < pars.Length; i++)
				{
					var particle = new GameObject("leaves").AddComponent<ParticleSystem>();
					particle.transform.SetParent(x.renderers[0].transform);
					particle.transform.localPosition = Vector3.up * (i + 2);
					particle.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
					var main = particle.main;
					main.gravityModifierMultiplier = 0.03f;
					main.startLifetimeMultiplier = 10f;

					var vel = particle.velocityOverLifetime;
					vel.enabled = true;
					vel.zMultiplier = -4f;


					var renderer = particle.GetComponent<ParticleSystemRenderer>();

					renderer.material = new(man.Get<Material>("particleMaterial"))
					{
						mainTexture = tex
					};

					var anim = particle.textureSheetAnimation;
					anim.enabled = true;
					anim.frameOverTimeMultiplier = 0.0001f;
					anim.numTilesX = 4;
					anim.numTilesY = 1;
					anim.animation = ParticleSystemAnimationType.SingleRow;
					anim.mode = ParticleSystemAnimationMode.Grid;
					anim.cycleCount = int.MaxValue; // Bruh

					var co = particle.collision;
					co.enabled = true;
					co.type = ParticleSystemCollisionType.World;
					co.collidesWith = LayerStorage.windowLayer | 1;
					co.bounceMultiplier = 0f;
					co.lifetimeLossMultiplier = 0f;

					pars[i] = particle;
				}

				var col = new GameObject("PlantCollider").AddComponent<CapsuleCollider>(); // I hate the plant having 2 prefabs >:(
				col.transform.SetParent(x.renderers[0].transform);
				col.transform.localPosition = Vector3.up * 5f;
				col.isTrigger = true;
				col.radius = 2f;

				var animator = col.gameObject.AddComponent<PlantAnimator>();
				animator.particles = pars;
				animator.audMan = animator.gameObject.CreateAudioManager(30f, 45f);
				animator.aud_bushes = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "bushes.wav")), "Vfx_plantNoise", SoundType.Voice, Color.white);
				animator.renderer = (SpriteRenderer)x.renderers[0];
				animator.sprites = plantSprites;
				animator.sprites[0] = animator.renderer.sprite;

			});

			
		}


		readonly AssetManager man = new();
		

		internal static string ModPath = string.Empty;
	}

	static class ModInfo
	{
		public const string PLUGIN_GUID = "pixelguy.pixelmodding.baldiplus.newanimations";

		public const string PLUGIN_NAME = "BB+ New Animations";

		public const string PLUGIN_VERSION = "1.0.2.1";
	}
}
