using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(PlayerMovement))] // MUST BE REMOVED UPON RELEASE!!
	internal class Fast
	{

		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		private static void GottaGoFAST(PlayerMovement __instance)
		{
			__instance.walkSpeed *= 3;
			__instance.runSpeed *= 3;
		}
	}

	[HarmonyPatch(typeof(HappyBaldi), "SpawnWait", MethodType.Enumerator)]
	internal static class QuickCheatBox
	{
		[HarmonyTranspiler]
		internal static IEnumerable<CodeInstruction> Zero(IEnumerable<CodeInstruction> instructions) =>
			new CodeMatcher(instructions)
			.MatchForward(false, new CodeMatch(OpCodes.Ldc_I4_S, name: "9"))
			.Set(OpCodes.Ldc_I4_0, null)
			.InstructionEnumeration();
	}
	[HarmonyPatch(typeof(Baldi_Chase), "OnStateTriggerStay")]
	internal static class QuickBaldiNoDeath
	{
		[HarmonyPrefix]
		internal static bool NoDeath() => false;
	}
	[HarmonyPatch(typeof(BaseGameManager), "Initialize")]
	internal static class AlwaysFullMap
	{
		private static void Prefix(BaseGameManager __instance)
		{
			__instance.CompleteMapOnReady();
		}
	}
}
