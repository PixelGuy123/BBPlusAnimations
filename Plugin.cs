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
using System.Reflection;

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
			baseSprite.material = GenericExtensions.FindResourceObjectByName<Material>("SpriteStandard_Billboard");
			baseSprite.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			baseSprite.receiveShadows = false;

			baseSprite.gameObject.layer = LayerStorage.billboardLayer;
			DontDestroyOnLoad(baseSprite.gameObject);
			man.Add("SpriteBillboardTemplate", baseSprite.gameObject);

			// Sprite NO Billboard object
			baseSprite = new GameObject("SpriteNoBillBoard").AddComponent<SpriteRenderer>();
			baseSprite.material = GenericExtensions.FindResourceObjectByName<Material>("SpriteStandard_NoBillboard");
			baseSprite.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
			baseSprite.receiveShadows = false;

			baseSprite.gameObject.layer = LayerStorage.billboardLayer;
			DontDestroyOnLoad(baseSprite.gameObject);
			baseSprite.gameObject.SetActive(false);
			man.Add("SpriteNoBillboardTemplate", baseSprite.gameObject);

			// Overlay
			man.Add("gumOverlay", GenericExtensions.FindResourceObjectByName<Canvas>("GumOverlay"));

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
			GenericExtensions.FindResourceObjects<ITM_BSODA>().Do((x) =>
			{
				var r = x.transform.Find("RendereBase").Find("Particles"); // Yes, Rendere. I didn't mistyped
				r.gameObject.SetActive(true);
				r.transform.localPosition = Vector3.zero; // maybe this is the issue?
			});
			Texture2D[] texs = [AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock.png")), AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock_left.png"))];
			// Door
			GenericExtensions.FindResourceObjects<StandardDoor>().Do(d => d.gameObject.AddComponent<StandardDoorExtraMaterials>().defaultTex = texs);// Sets a lock

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
			baldi.sprites = (Sprite[])GenericExtensions.FindResourceObject<BaldiDance>().ReflectionGetVariable("danceSprites"); // First time I'm using the api reflection lol
			DontDestroyOnLoad(baldi.gameObject);
			baldi.gameObject.SetActive(false);
			HappyBaldiPatch.baldi = baldi;

			// Chalkles component
			GenericExtensions.FindResourceObjects<ChalkFace>().Do((x) =>
			{
				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites[1] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "ChalkFace_1.png")), new Vector2(0.5f, 0.25f), 25f);
			});
			// Gotta Sweep Animation Sprites
			GenericExtensions.FindResourceObjects<GottaSweep>().Do((x) =>
			{
				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites = new Sprite[7];
				comp.sprites[0] = x.spriteRenderer[0].sprite;
				for (int i = 1; i < comp.sprites.Length; i++)
					comp.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"sweep_{i}.png")), 26f);

			});

			// JumpRope Quick Fix
			GenericExtensions.FindResourceObjects<Jumprope>().Do((x) =>
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
			GenericExtensions.FindResourceObjects<Cumulo>().DoIf(x => !x.GetComponent<PropagatedAudioManager>(), x => x.gameObject.CreateAudioManager(45, 75));
			CumuloPatch.blowBeforeBlowing = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_Blow.wav")), "Vfx_CC_Breath", SoundType.Voice, Color.white);
			CumuloPatch.pah = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_PAH.wav")), "Vfx_CC_PAH", SoundType.Voice, Color.white);

			// Portal Posters
			var portalPoster = GenericExtensions.FindResourceObjectByName<WindowObject>("PortalPosterWindow");
			var portal = portalPoster.windowPre.gameObject.AddComponent<TextureAnimator>(); // Texture animator setup
			portal.texs = new Texture[10];
			portal.texs[0] = portalPoster.windowPre.windows[0].materials[1].mainTexture;
			portal.renderers = portalPoster.windowPre.windows;
			portal.defaultIndex = 1;
			portal.speed = 20f;
			for (int i = 1; i < portal.texs.Length; i++)
				portal.texs[i] = AssetLoader.TextureFromFile(Path.Combine(ModPath, $"portalPoster_{i}.png"));

			// Plant Particles
			Sprite[] plantSprites = new Sprite[6];
			for (int i = 1; i < plantSprites.Length; i++)
				plantSprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"plant{i}.png")), new Vector2(0.5f, 0f),15f);
			var tex = AssetLoader.TextureFromFile(Path.Combine(ModPath, "leaves.png"));

			GenericExtensions.FindResourceObjects<RendererContainer>().DoIf(x => x.name.StartsWith("Plant"), (x) =>
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

			// Flipper Particle
			var flipperParticle = new GameObject("flipperParticle", typeof(ParticleSystem));
			flipperParticle.SetActive(false);
			DontDestroyOnLoad(flipperParticle);

			flipperParticle.GetComponent<ParticleSystemRenderer>().material = new(man.Get<Material>("particleMaterial")) { mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "flipperParticle.png")) };

			var particleSystem = flipperParticle.GetComponent<ParticleSystem>();
			var anim = particleSystem.textureSheetAnimation;
			anim.enabled = true;
			anim.frameOverTimeMultiplier = 0.0006f;
			anim.numTilesX = 2;
			anim.numTilesY = 1;
			anim.animation = ParticleSystemAnimationType.SingleRow;
			anim.mode = ParticleSystemAnimationMode.Grid;
			anim.cycleCount = int.MaxValue; // Bruh

			var main = particleSystem.main;
			main.gravityModifierMultiplier = 0.01f;
			main.startLifetimeMultiplier = 10f;
			main.startColor = new(Color.black, Color.white);
			main.startSpeedMultiplier = 0.75f;

			// I don't think this is actually doing anything, but whatever
			var vel = particleSystem.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-7f, 7f);
			vel.y = new(-7f, 7f);
			vel.z = new(-7f, 7f);
			

			var emission = particleSystem.emission;
			emission.rateOverTime = 0f;

			var emitter = flipperParticle.AddComponent<TemporaryParticles>();
			emitter.particles = particleSystem;
			emitter.audMan = flipperParticle.CreateAudioManager(85, 105, true);
			emitter.audExplode = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "flipperExplode.wav")), "Vfx_flipperExplode", SoundType.Voice, Color.white);
			emitter.minParticles = 75;
			emitter.maxParticles = 105;

			flipperParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			GravityEventPatch.particle = emitter;

			// Tape Player revert state

			FieldInfo tapePlayer_spriteToChange = AccessTools.Field(typeof(TapePlayer), "spriteToChange");
			FieldInfo tapePlayer_changeOnUse = AccessTools.Field(typeof(TapePlayer), "changeOnUse");
			FieldInfo tapePlayer_usedSprite = AccessTools.Field(typeof(TapePlayer), "usedSprite");

			// The editor somehow breaks Resources
			GenericExtensions.FindResourceObjects<TapePlayer>().Do(x => {
				if (x.name == "PayPhone") // everything was null, wow
				{
					tapePlayer_spriteToChange.SetValue(x, x.GetComponentInChildren<SpriteRenderer>());
					tapePlayer_changeOnUse.SetValue(x, true);
					tapePlayer_usedSprite.SetValue(x, AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "phoneActive.png")), 25f)); // No need to store in a variable, there's just one payphone
				}
				x.gameObject.AddComponent<TapePlayerReverser>().spriteToReverse = ((SpriteRenderer)tapePlayer_spriteToChange.GetValue(x)).sprite;
			});

			// Baldi Ruler Break Particles
			var rulerParticle = new GameObject("rulerParticle", typeof(ParticleSystem));
			rulerParticle.SetActive(false);
			DontDestroyOnLoad(rulerParticle);

			rulerParticle.GetComponent<ParticleSystemRenderer>().material = new(man.Get<Material>("particleMaterial")) { mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "rulerPiece.png")) };

			particleSystem = rulerParticle.GetComponent<ParticleSystem>();

			main = particleSystem.main;
			main.gravityModifierMultiplier = 0.7f;
			main.startLifetimeMultiplier = 10f;
			main.startSize = new(0.1f, 0.3f);

			// I don't think this is actually doing anything, but whatever
			vel = particleSystem.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-3f, 3f);
			vel.yMultiplier = 2f;
			vel.z = new(-3f, 3f);
			

			main.startRotation = new(0f, 360f);

			emission = particleSystem.emission;
			emission.rateOverTime = 0f;

			emitter = rulerParticle.AddComponent<TemporaryParticles>();
			emitter.particles = particleSystem;
			emitter.minParticles = 15;
			emitter.maxParticles = 20;
			emitter.cooldown = 4f;

			rulerParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			BaldiPatch.particle = emitter;

			// Arts and Crafters speed increase
			FieldInfo attackSpeed = AccessTools.Field(typeof(ArtsAndCrafters), "attackSpinAccel");
			GenericExtensions.FindResourceObjects<ArtsAndCrafters>().Do(x => attackSpeed.SetValue(x, 80f));

			// Water fountain water
			GenericAnimation.spoutWater = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "spoutSpit.png")), new Vector2(0.75f, 0.5f), 75f);
			GenericAnimation.normalSpout = GenericExtensions.FindResourceObject<WaterFountain>().transform.Find("FountainSpout").GetComponent<SpriteRenderer>().sprite; // yes

			// Smoke for 1st prize
			var smokeParticle = new GameObject("SmokeParticles").AddComponent<ParticleSystem>();
			var renderer = smokeParticle.GetComponent<ParticleSystemRenderer>();
			renderer.material = new(man.Get<Material>("particleMaterial")) { mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "smoke.png")) };


			smokeParticle.gameObject.SetActive(false);
			DontDestroyOnLoad(smokeParticle.gameObject);

			var rotLife = smokeParticle.rotationOverLifetime;
			rotLife.enabled = true;
			rotLife.x = new(1f, new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(10f, 90f))); // I guess this should work??

			main = smokeParticle.main;
			main.startSpeedMultiplier = 0f;
			main.startRotationX = new(-30f, 0f) { mode = ParticleSystemCurveMode.TwoConstants};
			main.gravityModifierMultiplier = 0f;
			main.startSize =new(1f, new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(10f, 3f)), new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(13f, 5f)));
			main.simulationSpace = ParticleSystemSimulationSpace.World;

			emission = smokeParticle.emission;
			emission.rateOverTimeMultiplier = 23f;

			vel = smokeParticle.velocityOverLifetime;
			vel.enabled = true;
			vel.speedModifierMultiplier = 5f;
			vel.x = new(1f,
				new(new Keyframe(0f, -8f), new Keyframe(0f, 0.001f)),
				new(new Keyframe(0f, 8f), new Keyframe(0f, 0.001f)));
			vel.y = new(1f,
				new(new Keyframe(0f, -8f), new Keyframe(0f, 0.001f)),
				new(new Keyframe(0f, 8f), new Keyframe(0f, 0.001f)));
			vel.z = new(1f,
				new(new Keyframe(0f, -8f), new Keyframe(0f, 0.001f)),
				new(new Keyframe(0f, 8f), new Keyframe(0f, 0.001f))); // Should give a good offset
			vel.space = ParticleSystemSimulationSpace.World;

			smokeParticle.transform.localScale = Vector3.one * 6f;

			FirstPrizeStunnedPatch.smokes = smokeParticle;

			// Bully blinking
			BullyBlinkComponent.bullyBlink = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "bully_blink.png")), 26f);
			GenericExtensions.FindResourceObjects<Bully>().Do(x => x.gameObject.AddComponent<BullyBlinkComponent>());

		}


		readonly AssetManager man = new();
		

		internal static string ModPath = string.Empty;
	}

	static class ModInfo
	{
		public const string PLUGIN_GUID = "pixelguy.pixelmodding.baldiplus.newanimations";

		public const string PLUGIN_NAME = "BB+ New Animations";

		public const string PLUGIN_VERSION = "1.1.1";
	}
}
