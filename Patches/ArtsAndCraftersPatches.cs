using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Crafters visible jealously", "If True, Crafters will shake if he feels jealous for your notebooks.")]
	[HarmonyPatch]
	internal class ArtsAndCraftersPatches
	{
		[HarmonyPatch(typeof(ArtsAndCrafters_Stalking), "Update")]
		[HarmonyPrefix]
		private static void Shake(ArtsAndCrafters ___crafters, float ___timeInPlayerSight)
		{
			if (___crafters.Jealous)
				___crafters.spriteRenderer[0].transform.localPosition = new Vector3(Random.Range(-offset, offset) * ___timeInPlayerSight, Random.Range(-offset, offset) * ___timeInPlayerSight, Random.Range(-offset, offset) * ___timeInPlayerSight);
			
		}

		[HarmonyPostfix]
		[HarmonyPatch(typeof(ArtsAndCrafters_Chasing), "Enter")]
		[HarmonyPatch(typeof(ArtsAndCrafters_Waiting), "Enter")]
		[HarmonyPatch(typeof(ArtsAndCrafters_Fleeing), "Enter")]
		private static void CancelShakeSection(ArtsAndCrafters ___crafters) =>
			___crafters.spriteRenderer[0].transform.localPosition = Vector3.zero; // reset it

		const float offset = 1.05f;

	}
}
