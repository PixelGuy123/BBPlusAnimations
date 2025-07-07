using System.Collections.Generic;
using System.Reflection.Emit;
using BBPlusAnimations.Components;
using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_REFLEX_WORLDHAMMER, ConfigEntryStorage.DESC_REFLEX_WORLDHAMMER)]
	[HarmonyPatch(typeof(DrReflex))]
	internal static class DrReflexPatch
	{
		[HarmonyPatch("Initialize")]
		[HarmonyPostfix]
		static void InitHammer(DrReflex __instance) =>
			__instance.GetComponent<DrReflexHammerComponent>()?.Init(__instance.transform.position);

		[HarmonyPatch("GetHammer")]
		[HarmonyPrefix]
		static void GetHammer(DrReflex __instance) =>
			__instance.GetComponent<DrReflexHammerComponent>()?.HideHammer(true);

		[HarmonyPatch("RapidHammer", MethodType.Enumerator)]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> ResetHammer(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false,
				new(OpCodes.Ldloc_1),
				new(CodeInstruction.LoadField(typeof(DrReflex), "audioManager")),
				new(OpCodes.Ldloc_1),
				new(CodeInstruction.LoadField(typeof(DrReflex), "audNotBad")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(AudioManager), "QueueAudio", [typeof(SoundObject)]))
				)
			.InsertAndAdvance(
				new(OpCodes.Ldloc_1), // Get DrReflex instance
				Transpilers.EmitDelegate<System.Action<DrReflex>>(x => x.GetComponent<DrReflexHammerComponent>()?.HideHammer(false))
				)
			.InstructionEnumeration();
	}
}
