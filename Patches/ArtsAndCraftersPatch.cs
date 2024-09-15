using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Crafters visual anger", "If True, Crafters will have an unique animation when angry.")]
	[HarmonyPatch(typeof(ArtsAndCrafters))]
	internal class ArtsAndCraftersPatch
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
}
