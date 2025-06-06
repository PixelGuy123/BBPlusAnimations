﻿using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_CHALKLES_LAUGHTER, ConfigEntryStorage.DESC_CHALKLES_LAUGHTER)]
	[HarmonyPatch(typeof(ChalkFace))]
	internal static class ChalklesPatches
	{
		[HarmonyPatch("Initialize")]
		[HarmonyPrefix]
		private static void GetSprite(ChalkFace __instance, SpriteRenderer ___flyingRenderer)
		{
			var co = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (co)
				co.sprites[0] = ___flyingRenderer.sprite;
		}
		

		[HarmonyPatch("AdvanceLaughter")]
		[HarmonyPostfix]
		private static void RotateChalkles(ChalkFace __instance, ref SpriteRenderer ___flyingRenderer)
		{
			___flyingRenderer.SetSpriteRotation(35f * Mathf.Sin(speed * Time.fixedTime)); // make chalkles rotate
			var co = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (co)
				___flyingRenderer.sprite = co.sprites[Mathf.FloorToInt(spriteSpeed * Time.fixedTime) % 2];
		}

		const float speed = 4f, spriteSpeed = 3f;
	}
}
