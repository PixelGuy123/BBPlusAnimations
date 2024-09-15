﻿using HarmonyLib;
using UnityEngine;
using System.Collections.Generic;
using BBPlusAnimations.Components;
using System.Collections;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Bully))]
	[AnimationConditionalPatch("Bully gradual disappearance", "If True, Bully will fade out each time he wants to.")]
	internal class BullyPatch
	{
		internal static Vector3 GetPos(float degrees)
		{
#pragma warning disable Harmony003 // Harmony non-ref patch parameters modified
			degrees *= Mathf.Deg2Rad; // darn radians
#pragma warning restore Harmony003 // Harmony non-ref patch parameters modified
			return new((Mathf.Cos(degrees) * 3.27f) + (-Mathf.Sin(degrees) * 1.79f), (Mathf.Cos(degrees) * 1.79f) + (Mathf.Sin(degrees) * 3.27f)); // x = 3.27f y = 1.79f
			// This should make it work with any given rotation
		}

		[HarmonyPatch("StealItem")]
		[HarmonyPostfix]
		static void MakeBullyHappy(Bully __instance, SpriteRenderer ___spriteToHide, List<int> ___slotsToSteal)
		{
			if (___slotsToSteal.Count > 0)
			{
				var co = __instance.GetComponent<BullyBlinkComponent>();
				co.blink = false;
				co.itemRenderer.sprite = ItemManagerPatch.lastRemovedItem.itemSpriteLarge;

				var block = new MaterialPropertyBlock();
				___spriteToHide.GetPropertyBlock(block);
				co.itemRenderer.SetSpriteRotation(block.GetFloat("_SpriteRotation"));
				co.itemRenderer.transform.localPosition = GetPos(block.GetFloat("_SpriteRotation"));

				co.itemRenderer.enabled = true;
			}
		}

		[HarmonyPatch("Hide")]
		[HarmonyPostfix]
		static void BullyDisappearRapid(Bully __instance, SpriteRenderer ___spriteToHide)
		{
			___spriteToHide.enabled = true;
			__instance.StartCoroutine(HideAnimation(__instance, ___spriteToHide, __instance.GetComponent<BullyBlinkComponent>().itemRenderer));
		}

		static IEnumerator HideAnimation(Bully bu, SpriteRenderer ren, SpriteRenderer ren2)
		{
			var color = ren.color;
			var color2 = ren2.color;
			while (true)
			{
				color.a -= 0.4f * bu.TimeScale * Time.deltaTime;
				color2.a = color.a;
				if (color.a <= 0f) break;

				ren.color = color;
				ren2.color = color2;
				yield return null;
			}

			ren.enabled = false;
			ren2.enabled = false;

			color.a = 1f;
			color2.a = 1f;

			ren.color = color;
			ren2.color = color2;

			bu.GetComponent<BullyBlinkComponent>().blink = true;

			yield break;
		}
			
	}
}
