﻿using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(GottaSweep))]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_SWEEP_ANIMATION, ConfigEntryStorage.DESC_SWEEP_ANIMATION)]
	internal static class GottaSweepPatch_Animation
	{
		[HarmonyPatch("StartSweeping")]
		[HarmonyPrefix]
		private static void EnableIt(GottaSweep __instance)
		{
			var anim = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (anim)
				anim.isActive = true;
		}

		[HarmonyPatch("StopSweeping")]
		[HarmonyPrefix]
		private static void DisableIt(GottaSweep __instance)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp)
			{
				comp.isActive = false;
				__instance.spriteRenderer[0].sprite = comp.sprites[0];
			}
		}

		
	}

	[HarmonyPatch(typeof(GottaSweep))]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_SWEEP_SWEEPSOUND, ConfigEntryStorage.DESC_SWEEP_SWEEPSOUND)]
	internal static class GottaSweepPatch_SweepSound
	{
		[HarmonyPatch("StartSweeping")]
		[HarmonyPrefix]
		private static void EnableIt(GottaSweep __instance)
		{
			var sweep = __instance.GetComponent<GottaSweepComponent>();
			if (sweep)
				sweep.cooldown = cooldown;
		}

		[HarmonyPatch("VirtualUpdate")]
		[HarmonyPostfix]
		private static void Animate(GottaSweep __instance, AudioManager ___audMan)
		{
			// Animation
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp && comp.isActive)
			{
				__instance.spriteRenderer[0].sprite = comp.sprites[1 + (Mathf.FloorToInt(Time.fixedTime * __instance.TimeScale * 18f) % (comp.sprites.Length - 1))]; // minor () mistake for math

				var comp2 = __instance.GetComponent<GottaSweepComponent>(); // Sweeping audio
				if (comp2)
				{
					comp2.cooldown -= __instance.TimeScale * Time.deltaTime;
					if (comp2.cooldown < 0f)
					{
						comp2.cooldown += cooldown;
						if (Random.value <= chance)
						{
							___audMan.PlaySingle(comp2.aud_sweep);
							comp2.cooldown /= Random.Range(2f, 5f);
						}

					}
				}
			}

		}

		const float cooldown = 10f;
		const float chance = 0.675f;
	}
}
