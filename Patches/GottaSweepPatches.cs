using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(GottaSweep))]
	internal class GottaSweepPatches
	{
		[HarmonyPatch("StartSweeping")]
		[HarmonyPrefix]
		private static void EnableIt(GottaSweep __instance) =>
			__instance.GetComponent<GenericAnimationExtraComponent>().isActive = true;

		[HarmonyPatch("StopSweeping")]
		[HarmonyPrefix]
		private static void DisableIt(GottaSweep __instance)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			comp.isActive = false;
			__instance.spriteRenderer[0].sprite = comp.sprites[0];
		}

		[HarmonyPatch("VirtualUpdate")]
		[HarmonyPostfix]
		private static void Animate(GottaSweep __instance)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp.isActive)
				__instance.spriteRenderer[0].sprite = comp.sprites[1 + (Mathf.FloorToInt(Time.fixedTime * __instance.TimeScale * 18f) % (comp.sprites.Length - 1))]; // minor () mistake for math
			
		}
	}
}
