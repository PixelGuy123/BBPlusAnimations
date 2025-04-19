using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_CRAFTERS_ANGRY_ANIMATION, ConfigEntryStorage.DESC_CRAFTERS_ANGRY_ANIMATION)]
	[HarmonyPatch(typeof(ArtsAndCrafters))]
	internal static class ArtsAndCraftersPatch_StaticAttack
	{
		[HarmonyPatch("GetAngry")]
		[HarmonyPostfix]
		static void DoAnimation(ArtsAndCrafters __instance, SpriteRenderer ___visibleRenderer) =>
			__instance.StartCoroutine(Animation(___visibleRenderer, __instance));

		static IEnumerator Animation(SpriteRenderer renderer, ArtsAndCrafters arts)
		{
			float frame = 0f;
			while (true)
			{
				frame += 16f * arts.TimeScale * Time.deltaTime;
				if (frame >= craftSprites.Length)
					frame = 3;

				renderer.sprite = craftSprites[Mathf.FloorToInt(frame)];
				yield return null;
			}
		}


		internal static Sprite[] craftSprites;
	}

	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_CRAFTERS_VISIBLY_ANGRY, ConfigEntryStorage.DESC_CRAFTERS_VISIBLY_ANGRY)]
	[HarmonyPatch]
	internal static class ArtsAndCraftersPatch_VisiblyAngry
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
