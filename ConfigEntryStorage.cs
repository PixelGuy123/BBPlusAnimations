using BepInEx.Configuration;

namespace BBPlusAnimations
{
    public static class ConfigEntryStorage
    {
		internal static void InitializeConfigs(ConfigFile config)
		{
			CFG_BSODA_GROWN_SPEED = config.Bind(CATEGORY_ITEMS, NAME_BSODA_GROWN_SPEED, 0.9f, DESC_BSODA_GROWN_SPEED);
			CFG_BSODA_ROTATION_SPEED = config.Bind(CATEGORY_ITEMS, NAME_BSODA_ROTATION_SPEED, 1f, DESC_BSODA_ROTATION_SPEED);
			CFG_BSODA_THRESHOLD = config.Bind(CATEGORY_ITEMS, NAME_BSODA_DESPAWN_THRESHOLD, 5f, DESC_BSODA_DESPAWN_THRESHOLD);
			CFG_BSODA_PARTICLES = config.Bind(CATEGORY_ITEMS, NAME_BSODA_PARTICLES, true, DESC_BSODA_PARTICLES);

			CFG_NPC_BULLY_BLINK = config.Bind(CATEGORY_NPCs, NAME_BULLY_BLINK, true, DESC_BULLY_BLINK);
			CFG_NPC_CUMULO_INHALE = config.Bind(CATEGORY_NPCs, NAME_CUMULO_INHALE, true, DESC_CUMULO_INHALE);
			CFG_NPC_CUMULO_EXHALE = config.Bind(CATEGORY_NPCs, NAME_CUMULO_EXHALE, true, DESC_CUMULO_EXHALE);
			CFG_NPC_CUMULO_WIND_PARTICLES = config.Bind(CATEGORY_NPCs, NAME_CUMULO_WIND_PARTICLES, false, DESC_CUMULO_WIND_PARTICLES);
			CFG_POMP_FOVINTENSITY = config.Bind(CATEGORY_NPCs, NAME_POMP_SCREAM, 55f, DESC_POMP_SCREAM);

			CFG_ENTITY_UNSQUISH_TIME_LIMIT = config.Bind(CATEGORY_ENVIRONMENT, NAME_ENTITY_UNSQUISH_TIME_LIMIT, 4f, DESC_ENTITY_UNSQUISH_TIME_LIMIT);
			CFG_APPLETREE_APPLE_DROP_SPEED = config.Bind(CATEGORY_ENVIRONMENT, NAME_APPLETREE_APPLE_DROP_SPEED, 1f, DESC_APPLETREE_APPLE_DROP_SPEED);

			CFG_GUM_FULLCOVERCOOL_COOL = config.Bind(CATEGORY_UI, NAME_GUM_FULLCOVERCOOL, 2f, DESC_GUM_FULLCOVERCOOL);
			CFG_GUM_FADEOUTSPEED_SPEED = config.Bind(CATEGORY_UI, NAME_GUM_FADEOUTSPEED, 1f, DESC_GUM_FADEOUTSPEED);

			CFG_PORTAL_POSTER_ANIMATION = config.Bind(CATEGORY_ENVIRONMENT, NAME_PORTAL_POSTER_ANIMATION, true, DESC_PORTAL_POSTER_ANIMATION);
			CFG_PLANT_ANIMATION = config.Bind(CATEGORY_ENVIRONMENT, NAME_PLANT_ANIMATION, true, DESC_PLANT_ANIMATION);
			CFG_CRAFTERS_ACCELERATION = config.Bind(CATEGORY_NPCs, NAME_CRAFTERS_ACCELERATION, true, DESC_CRAFTERS_ACCELERATION);
			CFG_PHONE_VISUAL_CHANGE = config.Bind(CATEGORY_ENVIRONMENT, NAME_PHONE_VISUAL_CHANGE, true, DESC_PHONE_VISUAL_CHANGE);
			CFG_CHALKERASER_VISUAL = config.Bind(CATEGORY_ENVIRONMENT, NAME_CHALKERASER_VISUAL, true, DESC_CHALKERASER_VISUAL);
			CFG_LIGHT_SWITCHES = config.Bind(CATEGORY_ENVIRONMENT, NAME_LIGHT_SWITCHES, true, DESC_LIGHT_SWITCHES);
		}

		public const string
			TRUE_CDT = "If set to True", // CDT = ConDiTional
			FALSE_CDT = "If set to False", // CDT = ConDiTional
			SET_ZERO = "Set this to 0 to disable this animation.",
		
			// ===============================================
			CATEGORY_NPCs = "ANIMATIONS FOR NPCs",
			NAME_CRAFTERS_ANGRY_ANIMATION = "ARTS AND CRAFTERS ANIMATED ATTACK", DESC_CRAFTERS_ANGRY_ANIMATION = $"{TRUE_CDT}, Arts and Crafters will have an unique attack animation.",

			NAME_CRAFTERS_VISIBLY_ANGRY = "ARTS AND CRAFTERS VISIBLE ANGRINESS", DESC_CRAFTERS_VISIBLY_ANGRY = $"{TRUE_CDT}, Arts and Crafters will gradually shake agressively when being stared at.",
			NAME_CRAFTERS_ACCELERATION = "CRAFTERS ACCELERATION", DESC_CRAFTERS_ACCELERATION = $"{TRUE_CDT}, Arts and Crafters will gradually spin faster when he catches you.",

			NAME_BALDI_PEEK_LOCKER = "BALDI LOCKER PEEKING", DESC_BALDI_PEEK_LOCKER = $"{TRUE_CDT} and Baldi *knows* you\'re in there, he\'ll have a peeking animation.",

			NAME_BEANS_WORRY = "BEANS VISIBLY WORRIED", DESC_BEANS_WORRY = $"{TRUE_CDT}, Beans will appear worried after spitting a gum.",

			NAME_BULLY_HIDE = "BULLY FADE OUT", DESC_BULLY_HIDE = $"{TRUE_CDT}, Bully will fade out whenever he wants to go away.",
			NAME_BULLY_STEALITEM = "BULLY VISIBLE STEAL", DESC_BULLY_STEALITEM = $"{TRUE_CDT}, Bully will show the item he stole in his hand (only works properly if Bully\'s fading out animation is enabled).",
			NAME_BULLY_BLINK = "BULLY BLINKING", DESC_BULLY_BLINK = $"{TRUE_CDT}, Bully will occasionally blink while active.",

			NAME_CHALKLES_LAUGHTER = "CHALKLES LAUGHING", DESC_CHALKLES_LAUGHTER = $"{TRUE_CDT}, Chalkles will be actually laughing outside his chalkboard.",

			NAME_CUMULO_INHALE = "CLOUDY COPTER INHALE", DESC_CUMULO_INHALE = $"{TRUE_CDT}, Cloudy Copter will inhale before blowing.",
			NAME_CUMULO_EXHALE = "CLOUDY COPTER EXHALE", DESC_CUMULO_EXHALE = $"{TRUE_CDT}, Cloudy Copter will *PAH* after blowing.",
			NAME_CUMULO_WIND_PARTICLES = "CLOUDY COPTER WIND FLOW", DESC_CUMULO_WIND_PARTICLES = $"{TRUE_CDT}, Cloudy Copter\'s wind will incldue some *wind* particles.",
			NAME_REFLEX_WORLDHAMMER = "DR REFLEX REAL HAMMER", DESC_REFLEX_WORLDHAMMER = $"{TRUE_CDT}, Dr. Reflex\'s hammer will physically exist in his clinic.",

			NAME_FIRSTPRIZE_SMOKE = "1st PRIZE SMOKE", DESC_FIRSTPRIZE_SMOKE = $"{TRUE_CDT}, First Prize will start emitting smoke when cutted by scissors.",
			NAME_FIRSTPRIZE_WALLHIT = "1st PRIZE MAKING CRACK", DESC_FIRSTPRIZE_WALLHIT = $"{TRUE_CDT}, First Prize will make cracks into anything it slams.",
			NAME_FIRSTPRIZE_WINDOWHIT = "1st PRIZE BEING SORRY", DESC_FIRSTPRIZE_WINDOWHIT = $"{TRUE_CDT}, First Prize says sorry for breaking a window.",

			NAME_SWEEP_ANIMATION = "GOTTA SWEEP SWEEPING", DESC_SWEEP_ANIMATION = $"{TRUE_CDT}, Gotta Sweep will have an animation while sweeping.",
			NAME_SWEEP_SWEEPSOUND = "GOTTA SWEEP SWEEP NOISES", DESC_SWEEP_SWEEPSOUND = $"{TRUE_CDT}, Gotta Sweep will randomly make sweeping noises while active.",

			NAME_POMP_SIGHT = "MRS. POMP DEAD LOOK", DESC_POMP_SIGHT = $"{TRUE_CDT}, Mrs. Pomp will look directly at your soul when talking to you.",
			NAME_POMP_SCREAM = "MRS. POMP SCREAMING FOV", DESC_POMP_SCREAM = $"When Mrs. Pomp catches you while angry, she\'ll shake your camera (change your fov) crazily.\n" +
																			 $"The shakiness can be controlled by this factor (minimum limit of 15).\n" +
																			 SET_ZERO,
			NAME_PLAYTIME_JUMPROPECUT = "PLAYTIME JUMPROPE CUT", DESC_PLAYTIME_JUMPROPECUT = $"{TRUE_CDT} and Playtime\'s jumprope is cutted, the jumprope will be displayed being cut on screen.",
			NAME_PLAYTIME_HAPPY = "PLAYTIME EXCITED", DESC_PLAYTIME_HAPPY = $"{TRUE_CDT}, Playtime appears happy when you complete the minigame.",

			NAME_PRINCIPAL_WHISTLE_ANIMATION = "PRINCIPAL WHISTLE ANIMATION", DESC_PRINCIPAL_WHISTLE_ANIMATION = $"{TRUE_CDT}, Principal will display an animation for whistling.",
			NAME_PRINCIPAL_DETENTION_ANIMATION = "PRINCIPAL DETENTION ANIMATION", DESC_PRINCIPAL_DETENTION_ANIMATION = $"{TRUE_CDT}, Principal will display a finger-swinging animation when sending you to detention.",

			// ===============================================
			CATEGORY_ITEMS = "ANIMATIONS FOR ITEMS",
			NAME_BSODA_GROWN_SPEED = "SCALING SPEED", DESC_BSODA_GROWN_SPEED = $"Each BSODA instance will spawn at a small size and gradually grown to their normal scale." +
																			   $"\nThis number (range of 0 to 1) tells how fast the BSODA gradually grows (values close to 1 makes it scale up quicker).\n" +
																			   SET_ZERO,

			NAME_BSODA_ROTATION_SPEED = "ROTATION SPEED", DESC_BSODA_ROTATION_SPEED = $"Each BSODA instance will rotate in a constant speed." +
																			   $"\nThis number (range of 0 to 100) multiplies the speed of how fast the BSODA rotates.\n" +
																			   SET_ZERO,

			NAME_BSODA_DESPAWN_THRESHOLD = "DESPAWN INDICATION", DESC_BSODA_DESPAWN_THRESHOLD = $"Each BSODA instance will start fading away when reaching a certain offset." +
																			   $"\nFor example, if a BSODA lasts for 10 seconds and the offset is set to 5, the BSODA will start fading away in the last 5 seconds of its lifetime.\n" +
																			   SET_ZERO,
			NAME_BSODA_PARTICLES = "BSODA PARTICLES", DESC_BSODA_PARTICLES = $"{TRUE_CDT}, the BSODA will display particles while active.",																   
			NAME_GRAPPLINGHOOK_FOV = "GRAPPLING HOOK FOV", DESC_GRAPPLINGHOOK_FOV = $"{TRUE_CDT}, the Grappling Hook will manipulate the player fov when active.",
			NAME_GRAPPLINGHOOK_FADEOUT = "GRAPPLING HOOK FADE OUT", DESC_GRAPPLINGHOOK_FADEOUT = $"{TRUE_CDT}, the Grappling Hook will fade out when despawning.",
			NAME_BOOTS_WALK = "LOUD BOOTS", DESC_BOOTS_WALK = $"{TRUE_CDT}, the Big Ol' Boots will be noisy when walking with them.",
			NAME_TELEPORTER_PARTICLES = "TELEPORTER EFFECT", DESC_TELEPORTER_PARTICLES = $"{TRUE_CDT}, the Dangerous Teleporter will spawn particles each iteration.",
			NAME_ZESTY_PARTICLES = "ZESTY PIECES", DESC_ZESTY_PARTICLES = $"{TRUE_CDT}, eating a Zesty Bar will spawn pieces of the chocolate bar.",
			NAME_POINTS_PARTICLES = "YTP EXPLOSION", DESC_POINTS_PARTICLES = $"{TRUE_CDT}, collecting a YTP will make it \"explode\".",
			NAME_PORTAL_POSTER_ANIMATION = "PORTAL POSTER ROTATION", DESC_PORTAL_POSTER_ANIMATION = $"{TRUE_CDT}, portal posters will always be seen rotating when placed in a wall.",
			NAME_CHALKERASER_VISUAL = "CHALKERASER VISIBLE", DESC_CHALKERASER_VISUAL = $"{TRUE_CDT}, the chalk eraser will have an animation when being used.",
			NAME_WHISTLE_ANIMATION = "WHISTLE ANIMATION", DESC_WHISTLE_ANIMATION = $"{TRUE_CDT}, the whistle will be displayed in-screen when used.",

			// ===============================================
			CATEGORY_ENVIRONMENT = "ANIMATIONS FOR ENVIRONMENT",
			NAME_APPLETREE_APPLE_DROP_SPEED = "APPLE TREE DROP SPEED", DESC_APPLETREE_APPLE_DROP_SPEED = $"When the Apple from an Apple Tree is dropped, it\'ll have a falling animation. This variable sets how fast it accelerates downwards.\n" +
																										 SET_ZERO,

			NAME_ENTITY_UNSQUISH_TIME_LIMIT = "UNSQUISH INDICATION", DESC_ENTITY_UNSQUISH_TIME_LIMIT = $"When the entity is about to be unsquished, this time offset will point out how early will the unsquishing animation plays." +
																									   $"\nFor example, if the entity is squished for 15 seconds and this offset is set to 5, the entity will \"start unsquishing\" in the last 5 seconds.\n" +
																									   SET_ZERO,
			NAME_DOOR_LOCK = "DOOR VISIBLE LOCK", DESC_DOOR_LOCK = $"{TRUE_CDT}, the doors will display a swinging door lock when locked.",
			NAME_ELEVATOR_GATEHIT = "ELEVATOR GATE BUMP", DESC_ELEVATOR_GATEHIT = $"{TRUE_CDT}, the elevator\'s gate will bounce a little before lying on the floor.",
			//NAME_FLIPPER_EXPLOSION = "GRAVITY FLIPPER EXPLOSION", DESC_FLIPPER_EXPLOSION = $"{TRUE_CDT}, Gravity Flippers will explode with particles.",
			NAME_GUM_SPLASH = "GUM SPLASH", DESC_GUM_SPLASH = $"{TRUE_CDT}, the gum will literally splat into a wall.",
			NAME_HIDEABLELOCKER_INTERACTION = "HIDEABLE LOCKER INTERACT", DESC_HIDEABLELOCKER_INTERACTION = $"{TRUE_CDT}, the blue lockers will slightly open when selecting them.",
			NAME_NUMBALL_SPAWN = "NUMBER BALLOON SPAWN", DESC_NUMBALL_SPAWN = $"{TRUE_CDT}, the Number Balloon literally inflates when spawning.",
			NAME_NUMBALL_DESPAWN = "NUMBER BALLOON EXPLOSION", DESC_NUMBALL_DESPAWN = $"{TRUE_CDT}, the Number Balloon explodes when popping up.",
			NAME_MATHMACHINE_NUMBERPOPUP = "MATH MACHINE NUMBER POPUP", DESC_MATHMACHINE_NUMBERPOPUP = $"{TRUE_CDT}, the math machine will slightly expand the answer number.",
			NAME_MATHMACHINE_WOW = "MATH MACHINE WOW", DESC_MATHMACHINE_WOW = $"{TRUE_CDT}, the math machine will scream WOOOW when answering correctly.",
			NAME_VENDINGMACHINE_ANIMATION = "VENDING MACHINE ANIMATION", DESC_VENDINGMACHINE_ANIMATION = $"{TRUE_CDT}, vending machines and other interactables will visually indicate when used.",
			NAME_TAPEPLAYER_ANIMATION = "TAPE PLAYER REVERT", DESC_TAPEPLAYER_ANIMATION = $"{TRUE_CDT}, the tape player will revert to its original form after making noise.",
			NAME_TAPEPLAYER_INSERT_ANIMATION = "TAPE PLAYER INSERT ANIMATION", DESC_TAPEPLAYER_INSERT_ANIMATION = $"{TRUE_CDT}, the tape player will visually animate when a tape is inserted.",
			NAME_TRIPENTRANCE_ANIMATION = "FIELD TRIP BUS LEAVE", DESC_TRIPENTRANCE_ANIMATION = $"{TRUE_CDT}, the field trip bus will leave after leaving the field trip area.",
			NAME_WINDOW_SHATTER = "WINDOW SHATTER", DESC_WINDOW_SHATTER = $"{TRUE_CDT}, windows will display broken pieces particles when broken.",
			NAME_WATERFOUNTAIN_ANIMATION = "WATER FOUNTAIN ANIMATION", DESC_WATERFOUNTAIN_ANIMATION = $"{TRUE_CDT}, water fountains will visually animate when used.",
			NAME_PLANT_ANIMATION = "PLANT ANIMATION", DESC_PLANT_ANIMATION = $"{TRUE_CDT}, plants will display a unique particle and be interactable.",
			NAME_PHONE_VISUAL_CHANGE = "PHONE VISUAL CHANGE", DESC_PHONE_VISUAL_CHANGE = $"{TRUE_CDT}, the phone will display as enabled when used.",
			NAME_LIGHT_SWITCHES = "LIGHT SWITCHES", DESC_LIGHT_SWITCHES = $"{TRUE_CDT}, light switches will spawn in (almost) every room to display whether they are powered or not.",

			// ===============================================
			CATEGORY_UI = "ANIMATIONS FOR CANVAS",
			NAME_HANDANIM_ELEVATOR = "HAND ANIMATION IN ELEVATOR", DESC_HANDANIM_ELEVATOR = $"{TRUE_CDT}, a hand will press the button in the elevator screen.",

			NAME_GUM_FULLCOVERCOOL = "GUM BLINDING COOLDOWN", DESC_GUM_FULLCOVERCOOL = $"When the gum hits you, it\'ll blind you for a few seconds (you can set a range from 0 to 10 seconds).\n" +
																			  SET_ZERO,
			NAME_GUM_FADEOUTSPEED = "GUM FADE OUT SPEED", DESC_GUM_FADEOUTSPEED = $"When the gum wears out from you, it\'ll fade out slowly (you can set a speed factor from range 0 to 15).\n" +
																			  SET_ZERO,
			NAME_HANDANIM_NOTEBOOK = "HAND ANIMATION PICKING UP NOTEBOOK", DESC_HANDANIM_NOTEBOOK = $"{TRUE_CDT}, a hand will pick up the notebook when the notebook is interacted with.",

			// ===============================================
			CATEGORY_MISC = "ANIMATIONS FOR MISC",
			NAME_HAPPYBALDI_LEAVE = "BALDI MOCKING", DESC_HAPPYBALDI_LEAVE = $"{TRUE_CDT}, in Explorer Mode, Baldi will literally float away from the school.",
			NAME_NOTEBOOK_SOUND = "NOTEBOOK PICKUP SOUND", DESC_NOTEBOOK_SOUND = $"{TRUE_CDT}, a *paper* sound will play when picking up the notebook."

			;

		internal static ConfigEntry<float>
			CFG_BSODA_GROWN_SPEED, // default(0.9f)
			CFG_BSODA_ROTATION_SPEED,  // default(1f)
			CFG_BSODA_THRESHOLD, // default(5f)

			CFG_ENTITY_UNSQUISH_TIME_LIMIT, // default(4f)

			CFG_APPLETREE_APPLE_DROP_SPEED, // default(1f)

			CFG_GUM_FULLCOVERCOOL_COOL, // default(2f)
			CFG_GUM_FADEOUTSPEED_SPEED, // default(1f)

			CFG_POMP_FOVINTENSITY // default(55f)

			;

		internal static ConfigEntry<bool>
			CFG_NPC_BULLY_BLINK,
			CFG_NPC_CUMULO_INHALE,
			CFG_NPC_CUMULO_EXHALE,
			CFG_NPC_CUMULO_WIND_PARTICLES,
			CFG_PORTAL_POSTER_ANIMATION,
			CFG_PLANT_ANIMATION,
			CFG_BSODA_PARTICLES,
			CFG_CRAFTERS_ACCELERATION,
			CFG_PHONE_VISUAL_CHANGE,
			CFG_CHALKERASER_VISUAL,
			CFG_LIGHT_SWITCHES
			;
	}	
}
