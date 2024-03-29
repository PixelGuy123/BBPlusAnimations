using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ChalkFace))]
	internal class ChalklesPatches
	{
		[HarmonyPatch("Initialize")]
		[HarmonyPrefix]
		private static void GetSprite(ChalkFace __instance, SpriteRenderer ___flyingRenderer) =>
			__instance.GetComponent<GenericAnimationExtraComponent>().sprites[0] = ___flyingRenderer.sprite;
		

		[HarmonyPatch("AdvanceLaughter")]
		[HarmonyPostfix]
		private static void RotateChalkles(ChalkFace __instance, ref SpriteRenderer ___flyingRenderer)
		{
			___flyingRenderer.SetSpriteRotation(35f * Mathf.Sin(speed * Time.fixedTime)); // make chalkles rotate
			___flyingRenderer.sprite = __instance.GetComponent<GenericAnimationExtraComponent>().sprites[Mathf.FloorToInt(spriteSpeed * Time.fixedTime) % 2];
		}

		const float speed = 4f, spriteSpeed = 3f;
	}
}
