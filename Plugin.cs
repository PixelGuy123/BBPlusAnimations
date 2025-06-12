using System.Collections;
using System.IO;
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
using UnityEngine;
using UnityEngine.UI;

namespace BBPlusAnimations
{
	[BepInPlugin(ModInfo.PLUGIN_GUID, ModInfo.PLUGIN_NAME, ModInfo.PLUGIN_VERSION)]
	[BepInDependency("pixelguy.pixelmodding.baldiplus.pixelinternalapi")]
	[BepInDependency("mtm101.rulerp.bbplus.baldidevapi")]
	[BepInDependency("mtm101.rulerp.baldiplus.texturepacks", BepInDependency.DependencyFlags.SoftDependency)] // To make sure it loads after texture pack. Should fix most of the issues



	public class BasePlugin : BaseUnityPlugin
	{
		internal static ConfigFile file;

		internal static int enabledSettings = 36;

#pragma warning disable IDE0051 // Remover membros privados não utilizados
		private void Awake()
#pragma warning restore IDE0051 // Remover membros privados não utilizados
		{
			file = Config;
			logger = Logger;

			Harmony h = new(ModInfo.PLUGIN_GUID);
			h.PatchAllConditionals();


			ModPath = AssetLoader.GetModPath(this);
			AssetLoader.LoadLocalizationFolder(Path.Combine(ModPath, "Language", "English"), Language.English);

			LoadingEvents.RegisterOnAssetsLoaded(Info, OnAssetLoad(), false);
			LoadingEvents.RegisterOnAssetsLoaded(Info, OnAssetLoadPost(), true);

			ConfigEntryStorage.InitializeConfigs(Config);
		}

		IEnumerator OnAssetLoad()
		{
			ParticleSystem GetParticleObj(string name, out ParticleSystemRenderer renderer)
			{
				var particle = Instantiate(man.Get<ParticleSystem>("particlePrefab"));
				var obj = particle.gameObject;
				particle.name = name;

				DestroyImmediate(particle); // Destroys original Particle instance

				particle = obj.AddComponent<ParticleSystem>(); // Addds a new fresh ParticleSystem to have everything set to default values
				renderer = obj.GetComponent<ParticleSystemRenderer>();
				renderer.material = new(renderer.material)
				{
					name = $"{name}_Mat"
				};
				return particle;
			}

			yield return enabledSettings;

			yield return "Grabbing material...";
			// Particle Material
			man.Add("particlePrefab", ((ChalkEraser)ItemMetaStorage.Instance.FindByEnum(Items.ChalkEraser).value.item).cloud.particles);
			TripEntrancePatch.baldiInBus = GenericExtensions.FindResourceObjectByName<Material>("Bus_Occupied");


			yield return "Creating gum splash asset...";


			// Gum
			Sprite[] gumSprs = TextureExtensions.LoadSpriteSheet(2, 1, 25f, ModPath, GetAssetName("gumSplashSheet.png"));

			var gumHolder = new GameObject("gumSplash");
			var gum = ObjectCreationExtensions.CreateSpriteBillboard(gumSprs[0], false).gameObject;
			gum.transform.SetParent(gumHolder.transform);
			gum.transform.localPosition = Vector3.zero;

			gum = ObjectCreationExtensions.CreateSpriteBillboard(gumSprs[1], false).gameObject;
			gum.transform.SetParent(gumHolder.transform);
			gum.transform.localPosition = Vector3.zero + (gum.transform.forward * -0.01f);

			gumHolder.ConvertToPrefab(true);

			gumHolder.AddComponent<EmptyMonoBehaviour>(); // For coroutines
			GumSplash_WallSplash.gumSplash = gumHolder.transform;

			GumSplash_WallSplash.splash = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("gumSplash.wav"))), "Vfx_GumSplash", SoundType.Voice, new(1, 0.2039f, 0.8863f));

			// Bsoda particles
			if (ConfigEntryStorage.CFG_BSODA_PARTICLES.Value)
			{
				yield return "Enabling bsoda particles...";
				GenericExtensions.FindResourceObjects<ITM_BSODA>().Do((x) =>
				{
					var r = x.transform.Find("RendereBase").Find("Particles"); // Yes, Rendere. I didn't mistyped
					r.gameObject.SetActive(true);
					r.transform.localPosition = Vector3.zero; // maybe this is the issue?
				});
			}
			yield return "Adding door locks for doors...";
			Texture2D[] texs = TextureExtensions.LoadTextureSheet(2, 1, ModPath, GetAssetName("doorLocksSheet.png"));
			// Door
			GenericExtensions.FindResourceObjects<StandardDoor>().Do(d => d.gameObject.AddComponent<StandardDoorExtraMaterials>().defaultTex = texs);// Sets a lock

			yield return "Loading balloon pop animations...";
			// Balloon Pop Animation
			NumberBalloonPatch_Explosion.explodeVisuals = TextureExtensions.LoadSpriteSheet(6, 1, 30f, ModPath, GetAssetName("ballExplodeAnim.png"));
			NumberBalloonPatch_Spawn.sound = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("BalloonRespawn.wav"))), "something", SoundType.Voice, Color.white);
			NumberBalloonPatch_Spawn.sound.subtitle = false; // No sub

			yield return "Loading playtime\'s jumprope cut animation...";
			// Jumprope Sprites
			PlaytimeJumpropePatch.sprites = TextureExtensions.LoadSpriteSheet(7, 1, 1f, ModPath, GetAssetName("jumpropeCut.png"));
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
				comp.sprites[1] = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("ChalkFace_1.png"))), new Vector2(0.5f, 0.25f), 25f);
			});



			yield return "Loading gum\'s overlay animation...";
			// Gum overlay animation
			GumSplash_GumCoverScreen.sprites = [.. TextureExtensions.LoadSpriteSheet(5, 1, 1f, ModPath, GetAssetName("gumOverlay.png")), Resources.FindObjectsOfTypeAll<Gum>()[0].canvas.GetComponentInChildren<Image>().sprite];

			yield return "Loading cloudy\'s copter audios...";
			// Cloudy Copter assets and components
			GenericExtensions.FindResourceObjects<Cumulo>().DoIf(x => !x.GetComponent<PropagatedAudioManager>(), x => x.gameObject.CreatePropagatedAudioManager(45, 75));
			CumuloPatch.blowBeforeBlowing = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("CC_Blow.wav"))), "Vfx_CC_Breath", SoundType.Voice, Color.white);
			CumuloPatch.pah = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("CC_PAH.wav"))), "Vfx_CC_PAH", SoundType.Voice, Color.white);
			if (ConfigEntryStorage.CFG_PORTAL_POSTER_ANIMATION.Value)
			{
				yield return "Loading portal poster\'s animation...";
				// Portal Posters
				var portalPoster = GenericExtensions.FindResourceObjectByName<WindowObject>("PortalPosterWindow");
				var portal = portalPoster.windowPre.gameObject.AddComponent<TextureAnimator>(); // Texture animator setup
				portal.texs = [portalPoster.windowPre.windows[0].materials[1].mainTexture, .. TextureExtensions.LoadTextureSheet(3, 3, ModPath, GetAssetName("portalPosterSpriteSheet.png"))];
				portal.renderers = portalPoster.windowPre.windows;
				portal.defaultIndex = 1;
				portal.speed = 20f;
			}

			if (ConfigEntryStorage.CFG_PLANT_ANIMATION.Value)
			{
				yield return "Loading plant particles...";
				// Plant Particles
				Sprite[] plantSprites = [null, .. TextureExtensions.LoadSpriteSheet(5, 1, 15f, new Vector2(0.5f, 0f), ModPath, GetAssetName("plantSheet.png"))];
				var plantTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("leaves.png")));

				GenericExtensions.FindResourceObjects<RendererContainer>().DoIf(x => x.name.StartsWith("Plant"), (x) =>
				{
					ParticleSystem[] pars = new ParticleSystem[3];

					for (int i = 0; i < pars.Length; i++)
					{
						var particle = GetParticleObj("LeavesParticle", out var leafRenderer);
						particle.transform.SetParent(x.renderers[0].transform);
						particle.transform.localPosition = Vector3.up * (i + 2);
						particle.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
						var main = particle.main;
						main.gravityModifierMultiplier = 0.03f;
						main.startLifetimeMultiplier = 3f + i;

						var vel = particle.velocityOverLifetime;
						vel.enabled = true;
						vel.zMultiplier = -4f;

						leafRenderer.material.mainTexture = plantTexture;
						x.renderers = x.renderers.AddToArray(leafRenderer);


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

						var emission = particle.emission;
						emission.enabled = false;

						pars[i] = particle;
					}

					var col = new GameObject("PlantCollider")
					{
						layer = LayerStorage.ignoreRaycast
					}.AddComponent<CapsuleCollider>(); // I hate the plant having 2 prefabs >:(
					col.transform.SetParent(x.renderers[0].transform);
					col.transform.localPosition = Vector3.up * 5f;
					col.isTrigger = true;
					col.radius = 2f;

					var animator = col.gameObject.AddComponent<PlantAnimator>();
					animator.particles = pars;
					animator.audMan = animator.gameObject.CreatePropagatedAudioManager(30f, 45f);
					animator.aud_bushes = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("bushes.wav"))), "Vfx_plantNoise", SoundType.Voice, Color.white);
					animator.renderer = (SpriteRenderer)x.renderers[0];
					animator.sprites = plantSprites;
					animator.sprites[0] = animator.renderer.sprite;

				});
			}


			// Tape Player revert state

			// The editor somehow breaks Resources
			if (ConfigEntryStorage.CFG_PHONE_VISUAL_CHANGE.Value)
			{
				yield return "Loading the pay phone\'s sprite...";
				GenericExtensions.FindResourceObjects<TapePlayer>().Do(x =>
				{
					if (x.name == "PayPhone") // everything was null, wow
					{
						x.spriteToChange = x.GetComponentInChildren<SpriteRenderer>(); //tapePlayer_spriteToChange.SetValue(x, x.GetComponentInChildren<SpriteRenderer>());
						x.changeOnUse = true; //tapePlayer_changeOnUse.SetValue(x, true);
						x.usedSprite = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("phoneActive.png"))), 25f); //tapePlayer_usedSprite.SetValue(x, AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, "phoneActive.png")), 25f)); // No need to store in a variable, there's just one payphone
					}
					x.gameObject.AddComponent<TapePlayerReverser>().spriteToReverse = x.spriteToChange.sprite; //((SpriteRenderer)tapePlayer_spriteToChange.GetValue(x)).sprite;
				});
			}

			// Arts and Crafters speed increase

			if (ConfigEntryStorage.CFG_CRAFTERS_ACCELERATION.Value)
			{
				yield return "Setting Arts and Crafters spin acceleration...";
				GenericExtensions.FindResourceObjects<ArtsAndCrafters>().Do(x => x.attackSpinAccel = 200f); //attackSpeed.SetValue(x, 80f));
			}

			yield return "Loading water fountain\'s water...";
			// Water fountain water
			VendingMachineAnimation_WaterFountain.spoutWater = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("spoutSpit.png"))), new Vector2(0.75f, 0.5f), 75f);
			VendingMachineAnimation_WaterFountain.normalSpout = GenericExtensions.FindResourceObject<WaterFountain>().transform.Find("FountainSpout").GetComponent<SpriteRenderer>().sprite; // yes

			yield return "Loading first prize\'s new assets...";
			// Smoke for 1st prize/new audio
			var smokeParticle = GetParticleObj("SmokeParticle", out var smokeRenderer);
			smokeRenderer.material.mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("smoke.png")));
			FirstPrize_SorryWindowHit.audSorry = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("firstprizebreak.wav"))), "Vfx_FirstPrize_Sorry", SoundType.Voice, Color.cyan);

			// First prize cracks
			var crackHolder = new GameObject("Cracks");
			var crack = ObjectCreationExtensions.CreateSpriteBillboard(AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("firstPrizeCracks.png"))), 25f), false).gameObject;
			crack.transform.SetParent(crackHolder.transform);
			crack.transform.localPosition = Vector3.zero;

			crackHolder.ConvertToPrefab(true);
			crackHolder.AddComponent<EmptyMonoBehaviour>();

			FirstPrizePatch_WallCrack.cracks = crackHolder;


			smokeParticle.gameObject.ConvertToPrefab(true);

			var rotLife = smokeParticle.rotationOverLifetime;
			rotLife.enabled = true;
			rotLife.x = new(1f, new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(10f, 90f))); // I guess this should work??

			var main = smokeParticle.main;
			main.startSpeedMultiplier = 0f;
			main.startRotationX = new(-30f, 0f) { mode = ParticleSystemCurveMode.TwoConstants };
			main.gravityModifierMultiplier = 0f;
			main.startSize = new(1f, new AnimationCurve(new Keyframe(0f, 0.2f), new Keyframe(10f, 3f)), new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(13f, 5f)));
			main.simulationSpace = ParticleSystemSimulationSpace.World;

			var emission = smokeParticle.emission;
			emission.rateOverTimeMultiplier = 23f;

			var color = smokeParticle.colorOverLifetime;
			color.enabled = true;
			color.color = new Gradient()
			{
				alphaKeys = [
					new GradientAlphaKey(0f, 0f),
					new GradientAlphaKey(1f, 0.35f),
					new GradientAlphaKey(1f, 0.85f),
					new GradientAlphaKey(0f, 1f)
					],
			};

			var vel = smokeParticle.velocityOverLifetime;
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

			FirstPrizePatch_Smoking.smokes = smokeParticle;

			yield return "Loading bully\'s animations...";
			// Bully blinking and catching items
			var sprs = TextureExtensions.LoadSpriteSheet(2, 1, 26f, ModPath, GetAssetName("bullyAnim.png"));
			BullyBlinkComponent.bullyBlink = sprs[0];
			BullyBlinkComponent.bullyCatch = sprs[1];
			GenericExtensions.FindResourceObjects<Bully>().Do(x =>
			{
				var comp = x.gameObject.AddComponent<BullyBlinkComponent>();
				comp.blink = ConfigEntryStorage.CFG_NPC_BULLY_BLINK.Value;

				comp.itemRenderer = ObjectCreationExtensions.CreateSpriteBillboard(null);

				var block = new MaterialPropertyBlock();
				x.spriteToHide.GetPropertyBlock(block);

				var obj = new GameObject("SpriteRenderHolder", typeof(BillboardRotator));
				comp.itemRenderer.transform.SetParent(obj.transform);
				comp.itemRenderer.transform.localPosition = BullyPatch_StealItem.GetPos(block.GetFloat("_SpriteRotation")); // TO-DO: Some function that relates the sprite rotation to the current offset (maybe use sin or cos)
				comp.itemRenderer.enabled = false;

				obj.transform.SetParent(x.transform);
				obj.transform.localPosition = Vector3.zero;
			});

			yield return "Loading Gotta Sweep\'s sweping animation...";
			// Gotta sweep audio and tex
			var aud = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("GS_Sweeping.wav"))), "Vfx_GottaSweep", SoundType.Voice, new(0, 0.6226f, 0.0614f));
			GenericExtensions.FindResourceObjects<GottaSweep>().Do((x) =>
			{
				var c = x.gameObject.AddComponent<GottaSweepComponent>();
				c.aud_sweep = aud;

				x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = [x.spriteRenderer[0].sprite, .. TextureExtensions.LoadSpriteSheet(6, 1, x.spriteRenderer[0].sprite.pixelsPerUnit, ModPath, GetAssetName("sweepAnim.png"))];
			});

			yield return "Loading Mrs Pomp looking animation...";
			// Mrs Pomp looking at ya
			GenericExtensions.FindResourceObjects<NoLateTeacher>().Do(x =>
			x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = [x.normalSprite, .. TextureExtensions.LoadSpriteSheet(3, 1, x.normalSprite.pixelsPerUnit, ModPath, GetAssetName("pompLook.png"))]);

			yield return "Loading playtime\'s happy face animation...";
			// playtime happy visual
			GenericExtensions.FindResourceObjects<Playtime>().Do(x =>
				x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = TextureExtensions.LoadSpriteSheet(7, 1, x.spriteRenderer[0].sprite.pixelsPerUnit, ModPath, GetAssetName("playtimeHappy.png")));

			yield return "Loading Principal\'s whistle animation...";
			// Principal's Whistle actually visible
			var whistletex = AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("whistleScreen.png"))), 1f);
			GenericExtensions.FindResourceObjects<ITM_PrincipalWhistle>().Do(x =>
			{
				var principalCanvas = ObjectCreationExtensions.CreateCanvas(); // Only way to make a proper overlay on this
				principalCanvas.name = "PrincipalWhistleCanvas";
				ObjectCreationExtensions.CreateImage(principalCanvas, whistletex);
				principalCanvas.transform.SetParent(x.transform);
				principalCanvas.transform.localPosition = Vector3.zero;
			});

			yield return "Loading Techno Boots noises...";
			// Big ol' Boots footsteps
			var step1 = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("step0.wav"))), string.Empty, SoundType.Effect, Color.white);
			step1.subtitle = false;
			var step2 = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("step1.wav"))), string.Empty, SoundType.Effect, Color.white);
			step2.subtitle = false;

			GenericExtensions.FindResourceObjects<ITM_Boots>().Do(x =>
			{
				var comp = x.gameObject.AddComponent<BootsDistanceReach>();
				comp.audFootstep = step1;
				comp.audFootstep2 = step2;
				comp.audMan = x.gameObject.CreateAudioManager(45f, 65f).MakeAudioManagerNonPositional();
			});

			yield return "Loading Principal\'s animation...";
			// principal detention animation
			var spr = GenericExtensions.FindResourceObject<Principal>().spriteRenderer[0].sprite;
			PrincipalDetentionPatch.sprites = [spr, .. TextureExtensions.LoadSpriteSheet(7, 1, spr.pixelsPerUnit, ModPath, GetAssetName("principalSwing.png"))];

			// Zesty eating animation
			yield return "Loading zesty bar particles...";

			var particle = GetParticleObj("ZestyParticles", out var zestyRenderer);
			particle.gameObject.ConvertToPrefab(true);

			zestyRenderer.material.mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("zestyPiecesSheet.png")));

			var anim = particle.textureSheetAnimation;
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

			ArtsAndCraftersPatch_StaticAttack.craftSprites = TextureExtensions.LoadSpriteSheet(6, 1, craf.visibleRenderer.sprite.pixelsPerUnit, ModPath, GetAssetName("crafterAngry.png"));

			// Notebook pickup audio
			yield return "Loading notebook audio...";

			NotebookPatch.audPickNotebook = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("notebookCollect.wav"))), string.Empty, SoundType.Effect, Color.white);
			NotebookPatch.audPickNotebook.subtitle = false; // nuh uh

			// ******* Points animation *******
			yield return "Loading points animation...";

			var flipperParticle = GetParticleObj("PointsParticle", out _);
			flipperParticle.gameObject.ConvertToPrefab(true);

			main = flipperParticle.main;

			main.gravityModifier = 0.01f;
			main.loop = false;

			main.startLifetimeMultiplier = 25f;
			main.startSpeedMultiplier = 1f;
			main.startSize = new(0.75f, 0.85f);

			// I don't think this is actually doing anything, but whatever
			vel = flipperParticle.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-10f, 10f);
			vel.y = new(-10f, 10f);
			vel.z = new(-10f, 10f);


			emission = flipperParticle.emission;
			emission.rateOverTime = 0f;

			var emitter = flipperParticle.gameObject.AddComponent<TemporaryParticles>();
			emitter.particles = flipperParticle;
			emitter.minParticles = 35;
			emitter.maxParticles = 45;

			flipperParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			PickupPatches.particles = emitter;
			yield return "Loading WOOOOOOW...";
			MathMachinePatch_WOOW.aud_BalWow = ObjectCreators.CreateSoundObject(AssetLoader.AudioClipFromFile(Path.Combine(ModPath, GetAssetName("Animation_BAL_Wow.wav"))), "Vfx_Bal_WOW", SoundType.Voice, Color.green);

			yield return "Creating Dr Reflex\'s Hammer...";

			var hammer = ObjectCreationExtensions.CreateSpriteBillboard(AssetLoader.SpriteFromTexture2D(AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("reflexHammer.png"))), 65f)).AddSpriteHolder(out var hammerRenderer, 0f, 0);
			hammer.gameObject.ConvertToPrefab(true);
			hammer.name = "ReflexHammer";
			hammerRenderer.name = "ReflexHammer_Renderer";
			hammer.gameObject.AddComponent<PickupBob>();

			GenericExtensions.FindResourceObjects<DrReflex>().Do(x => x.gameObject.AddComponent<DrReflexHammerComponent>().hammerPre = hammer.transform);

			yield return "Creating Beans' worried animation...";

			var anims = TextureExtensions.LoadSpriteSheet(4, 1, NPCMetaStorage.Instance.Get(Character.Beans).value.spriteRenderer[0].sprite.pixelsPerUnit, ModPath, GetAssetName("beansWorried.png"));

			GenericExtensions.FindResourceObjects<Beans>().Do(x => x.gameObject.AddComponent<GenericAnimationExtraComponent>().sprites = anims);

			yield return "Creating Chalkeraser visual...";

			if (ConfigEntryStorage.CFG_CHALKERASER_VISUAL.Value)
			{
				GenericExtensions.FindResourceObjects<ChalkEraser>().Do(x =>
				{
					var chalkVisual = ObjectCreationExtensions.CreateSpriteBillboard(ItemMetaStorage.Instance.FindByEnum(Items.ChalkEraser).value.itemSpriteLarge);
					var chalkGround = chalkVisual.gameObject.AddComponent<ChalkBeatingUpInGround>();
					chalkGround.renderer = chalkVisual;
					chalkGround.particles = x.cloud.particles;
					chalkVisual.transform.SetParent(x.transform);
				});
			}

			yield return "Creating glass piece for window...";

			var rulerParticle = GetParticleObj("WindowPiecesParticle", out var rulerRenderer);
			rulerParticle.gameObject.ConvertToPrefab(true);

			rulerRenderer.material.mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("glassPiece.png")));

			main = rulerParticle.main;
			main.gravityModifierMultiplier = 0.7f;
			main.startLifetimeMultiplier = 10f;
			main.startSize = new(0.1f, 0.3f);

			// I don't think this is actually doing anything, but whatever
			vel = rulerParticle.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-7f, 7f);
			vel.y = new(1f, 2.5f);
			vel.yMultiplier = 2f;
			vel.z = new(-7f, 7f);


			main.startRotation = new(0f, 360f);

			emission = rulerParticle.emission;
			emission.rateOverTime = 0f;

			emitter = rulerParticle.gameObject.AddComponent<TemporaryParticles>();
			emitter.particles = rulerParticle;
			emitter.minParticles = 25;
			emitter.maxParticles = 45;
			emitter.cooldown = 4f;

			rulerParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			WindowPatch.glassPieces = emitter;

			yield return "Creating hand animations...";

			Sprite[] handSprs = TextureExtensions.LoadSpriteSheet(4, 7, 1f, ModPath, GetAssetName("pickupHand.png"));


			GenericExtensions.FindResourceObjects<GameCamera>().Do(x =>
			{
				var handCanvas = ObjectCreationExtensions.CreateCanvas();
				handCanvas.name = "AnimatedHandCanvas";
				var handImg = ObjectCreationExtensions.CreateImage(handCanvas);
				handImg.enabled = false;

				handCanvas.transform.SetParent(x.transform);
				handImg.transform.localScale = Vector3.one;
				handImg.transform.localPosition = Vector3.zero;

				var comp = x.gameObject.AddComponent<CameraHandUI>();

				comp.canvas = handCanvas;
				comp.img = handImg;
				comp.sprsPickup = handSprs;

			});

			yield return "Creating cloudy copter\'s wind particles...";
			var cumuloWindTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("airAnim.png")));

			GenericExtensions.FindResourceObjects<Cumulo>().Do(x =>
			{

				var particleSystem = GetParticleObj("WindParticle", out var windRenderer);
				windRenderer.material.mainTexture = cumuloWindTexture;

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
				particleSystem.transform.SetParent(x.transform);
				particleSystem.transform.localPosition = Vector3.down * 10f;
			});

			yield return "Creating hand animation for elevator screen...";

			Sprite[] handAnim = TextureExtensions.LoadSpriteSheet(5, 4, 1f, ModPath, GetAssetName("handPressBut.png"));

			ElevatorScreenPatch.handAnim = handAnim.Take(0, 12);
			ElevatorScreenPatch.handAfterPressAnim = handAnim.Take(12, 8);


			yield return "Creating Hideable Locker animation...";

			var tex = AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("blueLockerAlmostOpen.png")));
			GenericExtensions.FindResourceObjects<HideableLocker>().Do(x =>
			{
				if (x.GetComponent<HideableLockerExtraComponent>()) return;

				var comp = x.gameObject.AddComponent<HideableLockerExtraComponent>();
				comp.renderer = x.GetComponentInChildren<MeshRenderer>();

				comp.closed = (Texture2D)comp.renderer.materials[0].GetTexture("_MainTex");
				comp.open = tex;
			});

			yield return "Loading teleporter particles...";
			// Teleport Particle
			flipperParticle = GetParticleObj("TeleporterParticles", out var teleporterRenderer);
			flipperParticle.gameObject.ConvertToPrefab(true);

			teleporterRenderer.material.mainTexture = AssetLoader.TextureFromFile(Path.Combine(ModPath, GetAssetName("teleportParticles.png")));

			anim = flipperParticle.textureSheetAnimation;
			anim.enabled = true;
			anim.fps = 16f;
			anim.timeMode = ParticleSystemAnimationTimeMode.FPS;
			anim.startFrame = new(0, 3);
			anim.numTilesX = 2;
			anim.numTilesY = 2;
			anim.animation = ParticleSystemAnimationType.WholeSheet;
			anim.mode = ParticleSystemAnimationMode.Grid;
			anim.cycleCount = int.MaxValue; // Bruh

			main = flipperParticle.main;
			main.gravityModifierMultiplier = 0.01f;
			main.startLifetimeMultiplier = 10f;
			main.startSpeedMultiplier = 0.75f;

			// I don't think this is actually doing anything, but whatever
			vel = flipperParticle.velocityOverLifetime;
			vel.enabled = true;
			vel.x = new(-7f, 7f);
			vel.y = new(-7f, 7f);
			vel.z = new(-7f, 7f);


			emission = flipperParticle.emission;
			emission.rateOverTime = 0f;

			emitter = flipperParticle.gameObject.AddComponent<TemporaryParticles>();
			emitter.particles = flipperParticle;
			emitter.minParticles = 75;
			emitter.maxParticles = 105;

			flipperParticle.transform.rotation = Quaternion.Euler(270f, 0f, 0f);

			ITMTeleporterPatch.parts = emitter;

			// Baldi Peeking Animation Load
			yield return "Loading Baldi Peeking Animation...";
			BaldiPatch_PeekInside.bal_peek_sprites = TextureExtensions.LoadSpriteSheet(5, 8, 30f, ModPath, GetAssetName("IknowYoureThere.png"));

			yield break;
		}

		IEnumerator OnAssetLoadPost()
		{
			yield return 1;
			yield return "Loading light switches...";
			if (ConfigEntryStorage.CFG_LIGHT_SWITCHES.Value)
			{
				var lightSwitchSprites = TextureExtensions.LoadSpriteSheet(2, 1, 76f, ModPath, GetAssetName("lightswitch.png"));

				var lightSwitchPre = ObjectCreationExtensions.CreateSpriteBillboard(lightSwitchSprites[0], false);
				lightSwitchPre.name = "LightSwitch";
				lightSwitchPre.gameObject.ConvertToPrefab(true);

				foreach (var room in Resources.FindObjectsOfTypeAll<RoomAsset>())
				{
					if (room.roomFunctionContainer && room.type == RoomType.Room &&
						room.category != RoomCategory.Special && room.category != RoomCategory.Mystery && room.category != RoomCategory.Store && room.category != RoomCategory.FieldTrip && room.category != RoomCategory.Null)
					{
						var lRoomFunc = room.AddRoomFunctionToContainer<LightSwitchRoomFunction>();
						lRoomFunc.on = lightSwitchSprites[0];
						lRoomFunc.off = lightSwitchSprites[1];
						lRoomFunc.lightSwitchPre = lightSwitchPre;
					}
				}
			}

			yield break;
		}


		readonly AssetManager man = new();

		public AssetManager AssetMan => man;

		internal static ManualLogSource logger;


		internal static string ModPath = string.Empty;

		public static string GetAssetName(string name) => "BBPlusAnimations_" + name;
	}

	static class ModInfo
	{
		public const string PLUGIN_GUID = "pixelguy.pixelmodding.baldiplus.newanimations";

		public const string PLUGIN_NAME = "BB+ New Animations";

		public const string PLUGIN_VERSION = "1.3.2";
	}

	internal static class ConfigExtensions
	{
		public static T[] Take<T>(this T[] ar, int index, int count)
		{
			var newAr = new T[count];
			for (int z = 0; z < count; z++)
				newAr[z] = ar[index++];
			return newAr;
		}
	}


}
