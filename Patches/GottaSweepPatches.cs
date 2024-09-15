using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(GottaSweep))]
	[AnimationConditionalPatch("Gotta sweep sweeping", "If True, Gotta Sweep will display an unique *sweeping* animation.")]
	internal class GottaSweepPatches
	{
		[HarmonyPatch("StartSweeping")]
		[HarmonyPrefix]
		private static void EnableIt(GottaSweep __instance)
		{
			__instance.GetComponent<GenericAnimationExtraComponent>().isActive = true;
			__instance.GetComponent<GottaSweepComponent>().cooldown = cooldown;
		}

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
		private static void Animate(GottaSweep __instance, AudioManager ___audMan)
		{
			// Animation
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp.isActive)
			{
				__instance.spriteRenderer[0].sprite = comp.sprites[1 + (Mathf.FloorToInt(Time.fixedTime * __instance.TimeScale * 18f) % (comp.sprites.Length - 1))]; // minor () mistake for math

				var comp2 = __instance.GetComponent<GottaSweepComponent>(); // Sweeping audio
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

		const float cooldown = 10f;
		const float chance = 0.55f;
	}
}
