using BBPlusAnimations.Components;
using BBPlusAnimations.Patches;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using MTM101BaldAPI;
using MTM101BaldAPI.AssetTools;
using MTM101BaldAPI.Registers;
using PixelInternalAPI.Classes;
using PixelInternalAPI.Components;
using PixelInternalAPI.Extensions;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
//using MTM101BaldAPI.Reflection;
//using System.Reflection;

namespace BBPlusAnimations
{
	[BepInPlugin(ModInfo.PLUGIN_GUID, ModInfo.PLUGIN_NAME, ModInfo.PLUGIN_VERSION)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.pixelinternalapi")]
	[BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]

	internal class BasePlugin : BaseUnityPlugin
	{
		private void Awake()
		{
			logger = Logger;
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

			LoadingEvents.RegisterOnAssetsLoaded(Info, OnAssetLoad(), false);


		}

		IEnumerator OnAssetLoad()
		{
			yield return enumeratorReturnSize;

			yield return "Grabbing material...";
			// Particle Material
			man.Add("particleMaterial", Items.ChalkEraser.GetFirstInstance().item.GetComponent<ParticleSystemRenderer>().material);

			yield return "Creating gum splash asset...";
			// Gum
			var gumHolder = new GameObject("gumSplash");
			var gum = ObjectCreationExtensions.CreateSpriteBillboard(AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "gumSplash.png")), 25f), false).gameObject;
			gum.transform.SetParent(gumHolder.transform);
			gum.transform.localPosition = Vector3.zero;

			gum = ObjectCreationExtensions.CreateSpriteBillboard(AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "gumSplash_back.png")), 25f), false).gameObject;
			gum.transform.SetParent(gumHolder.transform);
			gum.transform.localPosition = Vector3.zero + (gum.transform.forward * -0.01f);

			gumHolder.ConvertToPrefab(true);

			gumHolder.AddComponent<EmptyMonoBehaviour>(); // For coroutines
			GumSplash.gumSplash = gumHolder.transform;

			GumSplash.splash = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "gumSplash.wav")), "Vfx_GumSplash", SoundType.Voice, new(1, 0.2039f, 0.8863f));

			yield return "Enabling bsoda particles...";
			// Bsoda particles
			GenericExtensions.FindResourceObjects<ITM_BSODA>().Do((x) =>
			{
				var r = x.transform.Find("RendereBase").Find("Particles"); // Yes, Rendere. I didn't mistyped
				r.gameObject.SetActive(true);
				r.transform.localPosition = Vector3.zero; // maybe this is the issue?
			});
			yield return "Adding door locks for doors...";
			Texture2D[] texs = [AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock.png")), AssetLoader.TextureFromFile(Path.Combine(ModPath, "doorLock_left.png"))];
			// Door
			GenericExtensions.FindResourceObjects<StandardDoor>().Do(d => d.gameObject.AddComponent<StandardDoorExtraMaterials>().defaultTex = texs);// Sets a lock

			yield return "Loading balloon pop animations...";
			// Balloon Pop Animation
			NumberBalloonPatch.explodeVisuals = new Sprite[6];
			for (int i = 0; i < NumberBalloonPatch.explodeVisuals.Length; i++)
				NumberBalloonPatch.explodeVisuals[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"balExplode_{i}.png")), 30f);
			NumberBalloonPatch.sound = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "BalloonRespawn.wav")), "something", SoundType.Voice, Color.white);
			NumberBalloonPatch.sound.subtitle = false; // No sub

			yield return "Loading the test\'s animation...";
			// TheTestAnimation

			var canvas = ObjectCreationExtensions.CreateCanvas();
			ObjectCreationExtensions.CreateImage(canvas);

			canvas.gameObject.ConvertToPrefab(true);


			TheTestPatch.sprites = new Sprite[7];

			TheTestPatch.sprites[0] = AssetLoader.SpriteFromTexture2D(TextureExtensions.CreateSolidTexture(480, 360, Color.black), 1f);
			for (int i = 1; i < TheTestPatch.sprites.Length; i++)
				TheTestPatch.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"theTestblind_{i - 3}.png")), 1f);

			canvas.name = "TheTestOverlayCanvas";
			TheTestPatch.canvas = canvas;

			yield return "Loading playtime\'s jumprope cut animation...";
			// Jumprope Sprites
			PlaytimeJumpropePatch.sprites = new Sprite[6];
			for (int i = 0; i < PlaytimeJumpropePatch.sprites.Length; i++)
				PlaytimeJumpropePatch.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"jmpropecut_{i}.png")), 1f);
			GenericExtensions.FindResourceObjects<Jumprope>().Do((x) =>
			{
				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites = null; // No reference in this
			});

			yield return "Loading Baldi\'s Explorer mode easter egg...";
			// Baldi Haha
			var baldiRender = ObjectCreationExtensions.CreateSpriteBillboard(null);
			var baldi = baldiRender.gameObject.AddComponent<BaldiFloatsAway>();
			baldi.name = "BaldiTheFloater";
			baldi.renderer = baldiRender;
			baldi.sprites = GenericExtensions.FindResourceObject<BaldiDance>().danceSprites; //(Sprite[])GenericExtensions.FindResourceObject<BaldiDance>().ReflectionGetVariable("danceSprites"); // First time I'm using the api reflection lol
			baldi.gameObject.ConvertToPrefab(true);
			HappyBaldiPatch.baldi = baldi;

			yield return "Loading chalkle\'s laughing animation...";
			// Chalkles component
			GenericExtensions.FindResourceObjects<ChalkFace>().Do((x) =>
			{
				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites[1] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "ChalkFace_1.png")), new Vector2(0.5f, 0.25f), 25f);
			});



			yield return "Loading gum\'s overlay animation...";
			// Gum overlay animation
			GumSplash.sprites = new Sprite[6];
			GumSplash.sprites[5] = Resources.FindObjectsOfTypeAll<Gum>()[0].canvas.GetComponentInChildren<Image>().sprite;  // There must be at least one
			for (int i = 0; i < GumSplash.sprites.Length - 1; i++)
				GumSplash.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"gumOverlay_{i + 1}.png")), 1f);

			yield return "Loading cloudy\'s copter audios...";
			// Cloudy Copter assets and components
			GenericExtensions.FindResourceObjects<Cumulo>().DoIf(x => !x.GetComponent<PropagatedAudioManager>(), x => x.gameObject.CreatePropagatedAudioManager(45, 75));
			CumuloPatch.blowBeforeBlowing = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_Blow.wav")), "Vfx_CC_Breath", SoundType.Voice, Color.white);
			CumuloPatch.pah = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_PAH.wav")), "Vfx_CC_PAH", SoundType.Voice, Color.white);

			yield return "Loading portal poster\'s animation...";
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

			yield return "Loading plant particles...";
			// Plant Particles
			Sprite[] plantSprites = new Sprite[6];
			for (int i = 1; i < plantSprites.Length; i++)
				plantSprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"plant{i}.png")), new Vector2(0.5f, 0f), 15f);
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
				animator.audMan = animator.gameObject.CreatePropagatedAudioManager(30f, 45f);
				animator.aud_bushes = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "bushes.wav")), "Vfx_plantNoise", SoundType.Voice, Color.white);
				animator.renderer = (SpriteRenderer)x.renderers[0];
				animator.sprites = plantSprites;
				animator.sprites[0] = animator.renderer.sprite;

			});

			yield return "Loading flipper\'s particles...";
			// Flipper Particle
			var flipperParticle = new GameObject("flipperParticle", typeof(ParticleSystem));
			flipperParticle.ConvertToPrefab(true);

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
			emitter.audMan = flipperParticle.CreatePropagatedAudioManager(85, 105);
			emitter.audExplode = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "flipperExplode.wav")), "Vfx_flipperExplode", SoundType.Voice, Color.white);
			emitter.minParticles = 75;
			emitter.maxParticles = 105;

			flipperParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			GravityEventPatch.particle = emitter;

			// Tape Player revert state

			//FieldInfo tapePlayer_spriteToChange = AccessTools.Field(typeof(TapePlayer), "spriteToChange");
			//FieldInfo tapePlayer_changeOnUse = AccessTools.Field(typeof(TapePlayer), "changeOnUse");
			//FieldInfo tapePlayer_usedSprite = AccessTools.Field(typeof(TapePlayer), "usedSprite");

			// The editor somehow breaks Resources
			yield return "Loading the pay phone\'s sprite...";
			GenericExtensions.FindResourceObjects<TapePlayer>().Do(x =>
			{
				if (x.name == "PayPhone") // everything was null, wow
				{
					x.spriteToChange = x.GetComponentInChildren<SpriteRenderer>(); //tapePlayer_spriteToChange.SetValue(x, x.GetComponentInChildren<SpriteRenderer>());
					x.changeOnUse = true; //tapePlayer_changeOnUse.SetValue(x, true);
					x.usedSprite = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "phoneActive.png")), 25f); //tapePlayer_usedSprite.SetValue(x, AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "phoneActive.png")), 25f)); // No need to store in a variable, there's just one payphone
				}
				x.gameObject.AddComponent<TapePlayerReverser>().spriteToReverse = x.spriteToChange.sprite; //((SpriteRenderer)tapePlayer_spriteToChange.GetValue(x)).sprite;
			});

			// Baldi Ruler Break Particles
			yield return "Loading Baldi\'s ruler particles...";
			var rulerParticle = new GameObject("rulerParticle", typeof(ParticleSystem));
			rulerParticle.ConvertToPrefab(true);

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
			vel.y = new(1f, 2.5f);
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
			//FieldInfo attackSpeed = AccessTools.Field(typeof(ArtsAndCrafters), "attackSpinAccel");
			yield return "Setting Arts and Crafters spin acceleration...";
			GenericExtensions.FindResourceObjects<ArtsAndCrafters>().Do(x => x.attackSpinAccel = 80f); //attackSpeed.SetValue(x, 80f));

			yield return "Loading water fountain\'s water...";
			// Water fountain water
			GenericAnimation.spoutWater = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "spoutSpit.png")), new Vector2(0.75f, 0.5f), 75f);
			GenericAnimation.normalSpout = GenericExtensions.FindResourceObject<WaterFountain>().transform.Find("FountainSpout").GetComponent<SpriteRenderer>().sprite; // yes

			yield return "Loading first prize\'s new assets...";
			// Smoke for 1st prize/new audio
			var smokeParticle = new GameObject("SmokeParticles").AddComponent<ParticleSystem>();
			var renderer = smokeParticle.GetComponent<ParticleSystemRenderer>();
			renderer.material = new(man.Get<Material>("particleMaterial")) { mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "smoke.png")) };
			FirstPrizePatches.audSorry = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "firstprizebreak.wav")), "Vfx_FirstPrize_Sorry", SoundType.Voice, Color.cyan);

			// First prize cracks
			var crackHolder = new GameObject("Cracks");
			var crack = ObjectCreationExtensions.CreateSpriteBillboard(AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "firstPrizeCracks.png")), 25f), false).gameObject;
			crack.transform.SetParent(crackHolder.transform);
			crack.transform.localPosition = Vector3.zero;

			crackHolder.ConvertToPrefab(true);
			crackHolder.AddComponent<EmptyMonoBehaviour>();

			FirstPrizePatches.cracks = crackHolder;


			smokeParticle.gameObject.ConvertToPrefab(true);

			var rotLife = smokeParticle.rotationOverLifetime;
			rotLife.enabled = true;
			rotLife.x = new(1f, new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(10f, 90f))); // I guess this should work??

			main = smokeParticle.main;
			main.startSpeedMultiplier = 0f;
			main.startRotationX = new(-30f, 0f) { mode = ParticleSystemCurveMode.TwoConstants };
			main.gravityModifierMultiplier = 0f;
			main.startSize = new(1f, new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(10f, 3f)), new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(13f, 5f)));
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

			FirstPrizePatches.smokes = smokeParticle;

			yield return "Loading bully\'s animations...";
			// Bully blinking and catching items
			BullyBlinkComponent.bullyBlink = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "bully_blink.png")), 26f);
			BullyBlinkComponent.bullyCatch = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "bullyHappy.png")), 26f);
			GenericExtensions.FindResourceObjects<Bully>().Do(x => 
			{
				var comp = x.gameObject.AddComponent<BullyBlinkComponent>();
				comp.itemRenderer = ObjectCreationExtensions.CreateSpriteBillboard(null);

				var block = new MaterialPropertyBlock();
				x.spriteToHide.GetPropertyBlock(block);

				var obj = new GameObject("SpriteRenderHolder", typeof(BillboardRotator));
				comp.itemRenderer.transform.SetParent(obj.transform);
				comp.itemRenderer.transform.localPosition = BullyPatch.GetPos(block.GetFloat("_SpriteRotation")); // TO-DO: Some function that relates the sprite rotation to the current offset (maybe use sin or cos)
				comp.itemRenderer.enabled = false;

				obj.transform.SetParent(x.transform);
				obj.transform.localPosition = Vector3.zero;
			});

			yield return "Loading Gotta Sweep\'s sweping animation...";
			// Gotta sweep audio and tex
			var aud = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "GS_Sweeping.wav")), "Vfx_GottaSweep", SoundType.Voice, new(0, 0.6226f, 0.0614f));
			GenericExtensions.FindResourceObjects<GottaSweep>().Do((x) =>
			{
				var c = x.gameObject.AddComponent<GottaSweepComponent>();
				c.aud_sweep = aud;

				var comp = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				comp.sprites = new Sprite[7];
				comp.sprites[0] = x.spriteRenderer[0].sprite;
				for (int i = 1; i < comp.sprites.Length; i++)
					comp.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"sweep_{i}.png")), x.spriteRenderer[0].sprite.pixelsPerUnit);
			});

			yield return "Loading Mrs Pomp looking animation...";
			// Mrs Pomp looking at ya
			GenericExtensions.FindResourceObjects<NoLateTeacher>().Do(x =>
			{
				var a = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				a.sprites = new Sprite[4];
				a.sprites[0] = x.normalSprite;
				for (int i = 1; i < a.sprites.Length; i++)
					a.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"pompLookAt{i - 1}.png")), x.normalSprite.pixelsPerUnit);
			});

			yield return "Loading playtime\'s happy face animation...";
			// playtime happy visual
			GenericExtensions.FindResourceObjects<Playtime>().Do(x =>
			{
				var a = x.gameObject.AddComponent<GenericAnimationExtraComponent>();
				a.sprites = new Sprite[7];
				for (int i = 0; i < a.sprites.Length; i++)
					a.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"playtimeHappy{i}.png")), x.spriteRenderer[0].sprite.pixelsPerUnit);
			});

			yield return "Loading Baldi eating apple particles...";
			// Baldi eat apple particles
			var appleParticle = new GameObject("appleParticle", typeof(ParticleSystem));

			appleParticle.GetComponent<ParticleSystemRenderer>().material = new(man.Get<Material>("particleMaterial")) { mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "applePiece.png")) };
			appleParticle.ConvertToPrefab(true);

			particleSystem = appleParticle.GetComponent<ParticleSystem>();

			main = particleSystem.main;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			main.gravityModifierMultiplier = 0.7f;
			main.startLifetimeMultiplier = 3f;
			main.startSize = new(0.1f, 0.25f);

			// I don't think this is actually doing anything, but whatever
			vel = particleSystem.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-1.5f, 1f);
			vel.y = new(1f, 2f); // "All velocities must be in the same mode" error fks up the console
			vel.yMultiplier = 1.5f;
			vel.z = new(-1.5f, 1f);


			main.startRotation = new(0f, 360f);

			emission = particleSystem.emission;
			emission.rateOverTime = 0f;

			appleParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);
			BaldiPatch.appleParticles = particleSystem;

			GenericExtensions.FindResourceObjects<Baldi>().Do(x => x.gameObject.AddComponent<BaldiEatAppleComponent>());

			yield return "Replacing zesty bar\'s eating noise...";
			// Zesty Bar audio change
			//FieldInfo zestyBarAud = AccessTools.Field(typeof(ITM_ZestyBar), "audEat");
			var zestyEatAudio = AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "zesty_eating.wav"));
			foreach (var zesty in GenericExtensions.FindResourceObjects<ITM_ZestyBar>())
			{
				var audio = zesty.audEat; //(SoundObject)zestyBarAud.GetValue(zesty);
				audio.MarkAsNeverUnload();
				audio.soundClip = zestyEatAudio;
				//zestyBarAud.SetValue(zesty, audio); // << idk if I need this, but whatever
			}

			yield return "Loading Principal\'s whistle animation...";
			// Principal's Whistle actually visible
			var whistletex = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "whistleScreen.png")), 1f);
			GenericExtensions.FindResourceObjects<ITM_PrincipalWhistle>().Do(x =>
			{

				var principalCanvas = ObjectCreationExtensions.CreateCanvas(); // Only way to make a proper overlay on this
				principalCanvas.name = "PrincipalWhistleCanvas";
				ObjectCreationExtensions.CreateImage(principalCanvas, whistletex);
				principalCanvas.transform.SetParent(x.transform);
				principalCanvas.transform.localPosition = Vector3.zero;
			});

			yield return "Loading Big Ol\' Boots noises...";
			// Big ol' Boots footsteps
			var step1 = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "step0.wav")), string.Empty, SoundType.Effect, Color.white);
			step1.subtitle = false;
			var step2 = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "step1.wav")), string.Empty, SoundType.Effect, Color.white);
			step2.subtitle = false;

			GenericExtensions.FindResourceObjects<ITM_Boots>().Do(x =>
			{
				var comp = x.gameObject.AddComponent<BootsDistanceReach>();
				comp.audFootstep = step1;
				comp.audFootstep2 = step2;
				comp.audMan = x.gameObject.CreateAudioManager(45f, 65f).MakeAudioManagerNonPositional();
			});
			// ITM_Bsodas already have a 0 scale as default lol
			GenericExtensions.FindResourceObjects<ITM_BSODA>().Do(x => x.transform.localScale = Vector3.one * 0.1f);
			yield return "Loading Principal\'s animation...";
			// principal detention animation
			PrincipalPatch.sprites = new Sprite[8];
			PrincipalPatch.sprites[0] = GenericExtensions.FindResourceObject<Principal>().spriteRenderer[0].sprite;			
			for (int i = 1; i < PrincipalPatch.sprites.Length; i++)
				PrincipalPatch.sprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"principalDetention{i}.png")), PrincipalPatch.sprites[0].pixelsPerUnit);
			// Swinging door lock animation
			yield return "Loading Swinging door locking noise...";
			SwingingDoorPatch.audLock = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "swingingdoorlock.wav")), "Sfx_Doors_StandardLock", SoundType.Voice, Color.white);

			// Zesty eating animation
			yield return "Loading zesty bar particles...";

			var particle = new GameObject("zestyEatPiecesAnimation").AddComponent<ParticleSystem>();
			particle.gameObject.ConvertToPrefab(true);

			particle.GetComponent<ParticleSystemRenderer>().material = new(man.Get<Material>("particleMaterial"))
			{
				mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "zestyPiecesSheet.png"))
			};

			anim = particle.textureSheetAnimation;
			anim.enabled = true;
			anim.frameOverTimeMultiplier = 0.0003f;
			anim.numTilesX = 2;
			anim.numTilesY = 1;
			anim.animation = ParticleSystemAnimationType.SingleRow;
			anim.mode = ParticleSystemAnimationMode.Grid;
			anim.cycleCount = int.MaxValue; // Bruh

			main = particle.main;
			main.gravityModifierMultiplier = 0.75f;
			main.startLifetimeMultiplier = 3f;
			main.startSize = new(0.5f, 0.8f);
			main.startSpeed = new(0.5f, 1.2f);

			// I don't think this is actually doing anything, but whatever
			vel = particle.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-0.2f, 0.2f);
			vel.y = new(0.2f, 0.7f); // "All velocities must be in the same mode" error fks up the console
			vel.yMultiplier = 1.5f;
			vel.z = new(2f, 4f);

			var emi = particle.emission;
			emi.rateOverTimeMultiplier = 0f;

			ITMZestyBar.prefab = particle;

			// Crafters angry animation
			yield return "Loading crafters angry sprites...";

			var craf = (ArtsAndCrafters)Character.Crafters.GetFirstInstance();

			ArtsAndCraftersPatch.craftSprites = new Sprite[6];
			for (int i = 0; i < ArtsAndCraftersPatch.craftSprites.Length; i++)
				ArtsAndCraftersPatch.craftSprites[i] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, $"craftersAngry{i}.png")), craf.visibleRenderer.sprite.pixelsPerUnit);

			// Notebook pickup audio
			yield return "Loading notebook audio...";

			NotebookPatch.audPickNotebook = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "notebookCollect.wav")), string.Empty, SoundType.Effect, Color.white);
			NotebookPatch.audPickNotebook.subtitle = false; // nuh uh

			// ******* Points animation *******
			yield return "Loading points animation...";
			
			flipperParticle = new GameObject("pointsParticle", typeof(ParticleSystem));
			flipperParticle.ConvertToPrefab(true);

			flipperParticle.GetComponent<ParticleSystemRenderer>().material = new(man.Get<Material>("particleMaterial"));

			particleSystem = flipperParticle.GetComponent<ParticleSystem>();

			main = particleSystem.main;

			main.gravityModifier = 0.01f;
			main.loop = false;

			main.startLifetimeMultiplier = 25f;
			main.startSpeedMultiplier = 1f;
			main.startSize = new(0.75f, 0.85f);

			// I don't think this is actually doing anything, but whatever
			vel = particleSystem.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-10f, 10f);
			vel.y = new(-10f, 10f);
			vel.z = new(-10f, 10f);


			emission = particleSystem.emission;
			emission.rateOverTime = 0f;

			emitter = flipperParticle.AddComponent<TemporaryParticles>();
			emitter.particles = particleSystem;
			emitter.minParticles = 35;
			emitter.maxParticles = 45;

			flipperParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			PickupPatches.particles = emitter;
			PickupPatches.audCollect = GenericExtensions.FindResourceObjectByName<SoundObject>("Xylophone"); // Xylophone
			// WOOOOW Math machine noises
			yield return "Loading WOOOOOOW...";
			MathMachinePatch.aud_BalWow = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "Animation_BAL_Wow.wav")), "Vfx_Bal_WOW", SoundType.Voice, Color.green);

			// Fog music change
			yield return "Changing the fog song...";
			var fogSound = AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "newFog_CreepyOldComputer.wav"));

			GenericExtensions.FindResourceObjects<FogEvent>().Do(x => x.music.soundClip = fogSound); // Replace fog music

			// Nana Peels slipping animation
			yield return "Creating nana peel\'s slipping animation...";

			var visual = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "slippingWind.png")), 25f);

			GenericExtensions.FindResourceObjects<ITM_NanaPeel>().Do(x =>
			{
				var slip = ObjectCreationExtensions.CreateSpriteBillboard(visual, false);
				var slipper = slip.gameObject.AddComponent<NanaPeelSlipper>();
				slipper.renderer = slip;

				slipper.transform.SetParent(x.transform);
				slipper.transform.localPosition = Vector3.zero;

				slipper.SetMyPeel(x);			
			});
			

			yield break;
		}

		const int enumeratorReturnSize = 36;


		readonly AssetManager man = new();

		internal static BepInEx.Logging.ManualLogSource logger;


		internal static string ModPath = string.Empty;
	}

	static class ModInfo
	{
		public const string PLUGIN_GUID = "pixelguy.pixelmodding.baldiplus.newanimations";

		public const string PLUGIN_NAME = "BB+ New Animations";

		public const string PLUGIN_VERSION = "1.2.2";
	}


}
