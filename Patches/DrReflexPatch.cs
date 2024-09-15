using BBPlusAnimations.Components;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Dr Reflex hammer", "If True, Dr Reflex will spawn with a physic hammer in his position.")]
	[HarmonyPatch(typeof(DrReflex))]
	internal class DrReflexPatch
	{
		[HarmonyPatch("Initialize")]
		[HarmonyPrefix]
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
			.Insert(new(OpCodes.Ldloc_1), 
				Transpilers.EmitDelegate<System.Action<DrReflex>>(x => x.GetComponent<DrReflexHammerComponent>()?.HideHammer(false)))
			.InstructionEnumeration();
	}
}
