using HarmonyLib;
using UnityEngine;
using System.Collections;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(
		ConfigEntryStorage.CATEGORY_NPCs,
		ConfigEntryStorage.NAME_PRINCIPAL_DETENTION_ANIMATION,
		ConfigEntryStorage.DESC_PRINCIPAL_DETENTION_ANIMATION)]
	[HarmonyPatch(typeof(Principal))]
	internal class PrincipalDetentionPatch
	{
		[HarmonyPatch("SendToDetention")]
		[HarmonyPrefix]
		static void CoolDetentionAnimation(Principal __instance)
		{
			if (__instance.ec.offices.Count > 0)
				__instance.StartCoroutine(DetentionAnimation(__instance.spriteRenderer[0], __instance));
		}

		static IEnumerator DetentionAnimation(SpriteRenderer renderer, Principal p)
		{
			float time = 3f;
			while (time > 0f)
			{
				time -= p.TimeScale * Time.deltaTime;
				renderer.sprite = sprites[1 + (Mathf.FloorToInt(Time.fixedTime * 8f * p.TimeScale) % (sprites.Length - 1))];
				yield return null;
			}
			renderer.sprite = sprites[0];
			yield break;
		}

		internal static Sprite[] sprites;
	}
}