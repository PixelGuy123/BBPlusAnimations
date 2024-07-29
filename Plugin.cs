using BBPlusAnimations.Components;
using BBPlusAnimations.Patches;
using BepInEx;
using BepInEx.Configuration;
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

namespace BBPlusAnimations
{
	[BepInPlugin(ModInfo.PLUGIN_GUID, ModInfo.PLUGIN_NAME, ModInfo.PLUGIN_VERSION)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.pixelinternalapi")]
	[BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]



	public class BasePlugin : BaseUnityPlugin
	{
		ConfigEntry<bool> enableHands;

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

			enableHands = Config.Bind("Animation Management", "Enable hand animations", true);
		}

		IEnumerator OnAssetLoad()
		{
			yield return enumeratorReturnSize;

			yield return "Grabbing material...";
			// Particle Material
			man.Add("particleMaterial", ItemMetaStorage.Instance.FindByEnum(Items.ChalkEraser).value.item.GetComponent<ParticleSystemRenderer>().material);

			yield return "Creating gum splash asset...";


			// Gum
			Sprite[] gumSprs = TextureExtensions.LoadSpriteSheet(2, 1, 25f, ModPath, "gumSplashSheet.png");

			var gumHolder = new GameObject("gumSplash");
			var gum = ObjectCreationExtensions.CreateSpriteBillboard(gumSprs[0], false).gameObject;
			gum.transform.SetParent(gumHolder.transform);
			gum.transform.localPosition = Vector3.zero;

			gum = ObjectCreationExtensions.CreateSpriteBillboard(gumSprs[1], false).gameObject;
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
			Texture2D[] texs = TextureExtensions.LoadTextureSheet(2, 1, ModPath, "doorLocksSheet.png");
			// Door
			GenericExtensions.FindResourceObjects<StandardDoor>().Do(d => d.gameObject.AddComponent<StandardDoorExtraMaterials>().defaultTex = texs);// Sets a lock

			yield return "Loading balloon pop animations...";
			// Balloon Pop Animation
			NumberBalloonPatch.explodeVisuals = TextureExtensions.LoadSpriteSheet(6, 1, 30f, ModPath, "ballExplodeAnim.png");
			NumberBalloonPatch.sound = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "BalloonRespawn.wav")), "something", SoundType.Voice, Color.white);
			NumberBalloonPatch.sound.subtitle = false; // No sub

			yield return "Loading the test\'s animation...";
			// TheTestAnimation

			var canvas = ObjectCreationExtensions.CreateCanvas();
			ObjectCreationExtensions.CreateImage(canvas);

			canvas.gameObject.ConvertToPrefab(true);


			TheTestPatch.sprites = [AssetLoader.SpriteFromTexture2D(TextureExtensions.CreateSolidTexture(480, 360, Color.black), 1f), .. TextureExtensions.LoadSpriteSheet(6, 1, 1f, ModPath, "theTestAnimation.png")];

			canvas.name = "TheTestOverlayCanvas";
			TheTestPatch.canvas = canvas;

			yield return "Loading playtime\'s jumprope cut animation...";
			// Jumprope Sprites
			PlaytimeJumpropePatch.sprites = TextureExtensions.LoadSpriteSheet(7, 1, 1f, ModPath, "jumpropeCut.png");
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
			GumSplash.sprites = [.. TextureExtensions.LoadSpriteSheet(5, 1, 1f, ModPath, "gumOverlay.png"), Resources.FindObjectsOfTypeAll<Gum>()[0].canvas.GetComponentInChildren<Image>().sprite];

			yield return "Loading cloudy\'s copter audios...";
			// Cloudy Copter assets and components
			GenericExtensions.FindResourceObjects<Cumulo>().DoIf(x => !x.GetComponent<PropagatedAudioManager>(), x => x.gameObject.CreatePropagatedAudioManager(45, 75));
			CumuloPatch.blowBeforeBlowing = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_Blow.wav")), "Vfx_CC_Breath", SoundType.Voice, Color.white);
			CumuloPatch.pah = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "CC_PAH.wav")), "Vfx_CC_PAH", SoundType.Voice, Color.white);

			yield return "Loading portal poster\'s animation...";
			// Portal Posters
			var portalPoster = GenericExtensions.FindResourceObjectByName<WindowObject>("PortalPosterWindow");
			var portal = portalPoster.windowPre.gameObject.AddComponent<TextureAnimator>(); // Texture animator setup
			portal.texs = [portalPoster.windowPre.windows[0].materials[1].mainTexture, .. TextureExtensions.LoadTextureSheet(3, 3, ModPath, "portalPosterSpriteSheet.png")];
			portal.renderers = portalPoster.windowPre.windows;
			portal.defaultIndex = 1;
			portal.speed = 20f;

			yield return "Loading plant particles...";
			// Plant Particles
			Sprite[] plantSprites = [null, .. TextureExtensions.LoadSpriteSheet(5, 1, 15f, new Vector2(0.5f, 0f), ModPath, "plantSheet.png")];
			var mat = new Material(man.Get<Material>("particleMaterial"))
			{
				mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "leaves.png"))
			};

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
					renderer.material = mat;


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
			var sprs = TextureExtensions.LoadSpriteSheet(2, 1, 26f, ModPath, "bullyAnim.png");
			BullyBlinkComponent.bullyBlink = sprs[0];
			BullyBlinkComponent.bullyCatch = sprs[1];
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

				x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = [x.spriteRenderer[0].sprite, .. TextureExtensions.LoadSpriteSheet(6, 1, x.spriteRenderer[0].sprite.pixelsPerUnit, ModPath, "sweepAnim.png")];
			});

			yield return "Loading Mrs Pomp looking animation...";
			// Mrs Pomp looking at ya
			GenericExtensions.FindResourceObjects<NoLateTeacher>().Do(x => 
			x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = [x.normalSprite, .. TextureExtensions.LoadSpriteSheet(3, 1, x.normalSprite.pixelsPerUnit, ModPath, "pompLook.png")]);

			yield return "Loading playtime\'s happy face animation...";
			// playtime happy visual
			GenericExtensions.FindResourceObjects<Playtime>().Do(x =>
				x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = TextureExtensions.LoadSpriteSheet(7, 1, x.spriteRenderer[0].sprite.pixelsPerUnit, ModPath, "playtimeHappy.png"));

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
			GenericExtensions.FindResourceObjects<ITM_BSODA>().Do(x => x.spriteRenderer.transform.localScale = Vector3.one * 0.1f);
			yield return "Loading Principal\'s animation...";
			// principal detention animation
			var spr = GenericExtensions.FindResourceObject<Principal>().spriteRenderer[0].sprite;
			PrincipalPatch.sprites = [spr, .. TextureExtensions.LoadSpriteSheet(7, 1, spr.pixelsPerUnit, ModPath, "principalSwing.png")];

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

			var craf = (ArtsAndCrafters)NPCMetaStorage.Instance.Get(Character.Crafters).value;

			ArtsAndCraftersPatch.craftSprites = TextureExtensions.LoadSpriteSheet(6, 1, craf.visibleRenderer.sprite.pixelsPerUnit, ModPath, "crafterAngry.png");

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
			yield return "Loading WOOOOOOW...";
			MathMachinePatch.aud_BalWow = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "Animation_BAL_Wow.wav")), "Vfx_Bal_WOW", SoundType.Voice, Color.green);

			// Fog music change
			yield return "Changing the fog song...";
			var fogSound = AssetLoader.AudioClipFromFile(Path.Combine(ModPath, "newFog_CreepyOldComputer.wav"));

			GenericExtensions.FindResourceObjects<FogEvent>().Do(x => x.music.soundClip = fogSound); // Replace fog music

			// Nana Peels slipping animation
			yield return "Creating nana peel\'s slipping animation...";

			var visual = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "slippingWind.png")), 25f);
			man.Add("nanaPeelSlide", visual);

			GenericExtensions.FindResourceObjects<ITM_NanaPeel>().Do(x =>
			{
				var slip = ObjectCreationExtensions.CreateSpriteBillboard(visual, false);
				var slipper = slip.gameObject.AddComponent<NanaPeelSlipper>();
				slipper.renderer = slip;

				slipper.transform.SetParent(x.transform);
				slipper.transform.localPosition = Vector3.zero;

				slipper.SetMyPeel(x);
			});

			yield return "Creating Dr Reflex\'s Hammer...";

			var hammer = ObjectCreationExtensions.CreateSpriteBillboard(AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "reflexHammer.png")), 65f)).AddSpriteHolder(0f, 0);
			hammer.transform.parent.gameObject.ConvertToPrefab(true);

			hammer.gameObject.AddComponent<PickupBob>();

			GenericExtensions.FindResourceObjects<DrReflex>().Do(x => x.gameObject.AddComponent<DrReflexHammerComponent>().hammerPre = hammer.transform.parent);

			yield return "Creating Beans' worried animation...";

			var anims = TextureExtensions.LoadSpriteSheet(4, 1, NPCMetaStorage.Instance.Get(Character.Beans).value.spriteRenderer[0].sprite.pixelsPerUnit, ModPath, "beansWorried.png");

			GenericExtensions.FindResourceObjects<Beans>().Do(x => x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = anims);

			yield return "Creating Chalkeraser visual...";

			var chalkVisual = ObjectCreationExtensions.CreateSpriteBillboard(ItemMetaStorage.Instance.FindByEnum(Items.ChalkEraser).value.itemSpriteLarge);
			chalkVisual.gameObject.AddComponent<ChalkBeatingUpInGround>();

			GenericExtensions.FindResourceObjects<ChalkEraser>().Do(x => chalkVisual.transform.SetParent(x.transform));

			yield return "Creating glass piece for window...";

			rulerParticle = new GameObject("windowParticles", typeof(ParticleSystem));
			rulerParticle.ConvertToPrefab(true);

			rulerParticle.GetComponent<ParticleSystemRenderer>().material = new(man.Get<Material>("particleMaterial")) { mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "glassPiece.png")) };

			particleSystem = rulerParticle.GetComponent<ParticleSystem>();

			main = particleSystem.main;
			main.gravityModifierMultiplier = 0.7f;
			main.startLifetimeMultiplier = 10f;
			main.startSize = new(0.1f, 0.3f);

			// I don't think this is actually doing anything, but whatever
			vel = particleSystem.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-7f, 7f);
			vel.y = new(1f, 2.5f);
			vel.yMultiplier = 2f;
			vel.z = new(-7f, 7f);


			main.startRotation = new(0f, 360f);

			emission = particleSystem.emission;
			emission.rateOverTime = 0f;

			emitter = rulerParticle.AddComponent<TemporaryParticles>();
			emitter.particles = particleSystem;
			emitter.minParticles = 25;
			emitter.maxParticles = 45;
			emitter.cooldown = 4f;

			rulerParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			WindowPatch.glassPieces = emitter;

			yield return "Creating hand animations...";

			Sprite[][] handSprs = null;
			if (enableHands.Value)
				handSprs = [[.. TextureExtensions.LoadSpriteSheet(13, 1, 1f, ModPath, "handInsert.png")], [.. TextureExtensions.LoadSpriteSheet(11, 1, 1f, ModPath, "pickupHand.png")]];
			

			GenericExtensions.FindResourceObjects<GameCamera>().Do(x =>
			{
				var handCanvas = ObjectCreationExtensions.CreateCanvas();
				handCanvas.name = "AnimatedHandCanvas";
				var handImg = ObjectCreationExtensions.CreateImage(handCanvas);
				handImg.enabled = false;

				handCanvas.transform.SetParent(x.transform);
				handImg.transform.localScale = new(2f, 2f, 2f);
				handImg.transform.localPosition = new(-319.76f, 178.83f);

				var comp = x.gameObject.AddComponent<CameraHandUI>();
				if (enableHands.Value)
				{
					comp.canvas = handCanvas;
					comp.img = handImg;
					comp.sprsInsert = handSprs[0];
					comp.sprsPickup = handSprs[1];
				}
			});

			yield return "Creating cloudy copter\'s wind particles...";

			mat = new(man.Get<Material>("particleMaterial")) { mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, "airAnim.png")) };

			GenericExtensions.FindResourceObjects<Cumulo>().Do(x =>
			{
				flipperParticle = new GameObject("WindParticle", typeof(ParticleSystem));

				flipperParticle.GetComponent<ParticleSystemRenderer>().material = mat;

				particleSystem = flipperParticle.GetComponent<ParticleSystem>();
				anim = particleSystem.textureSheetAnimation;
				anim.enabled = true;
				anim.numTilesX = 3;
				anim.numTilesY = 3;
				anim.animation = ParticleSystemAnimationType.WholeSheet;
				anim.mode = ParticleSystemAnimationMode.Grid;
				anim.cycleCount = 1;
				anim.timeMode = ParticleSystemAnimationTimeMode.FPS;
				anim.fps = 14f;

				main = particleSystem.main;
				main.gravityModifierMultiplier = 0f;
				main.startLifetimeMultiplier = 0.6f;
				main.startSpeedMultiplier = 0f;
				main.simulationSpace = ParticleSystemSimulationSpace.World;
				main.startSize = new(2f, 4f);

				emission = particleSystem.emission;

				x.gameObject.AddComponent<CloudyCopterExtraComp>().compToHold = particleSystem;
				flipperParticle.transform.SetParent(x.transform);
				flipperParticle.transform.localPosition = Vector3.down * 10f;
			});

			

			yield break;
		}

		const int enumeratorReturnSize = 42;


		readonly AssetManager man = new();

		public AssetManager AssetMan => man;

		internal static ManualLogSource logger;


		internal static string ModPath = string.Empty;
	}

	static class ModInfo
	{
		public const string PLUGIN_GUID = "pixelguy.pixelmodding.baldiplus.newanimations";

		public const string PLUGIN_NAME = "BB+ New Animations";

		public const string PLUGIN_VERSION = "1.2.3.1";
	}


}
