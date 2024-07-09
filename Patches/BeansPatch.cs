using UnityEngine;
using HarmonyLib;
using BBPlusAnimations.Components;
using System.Collections;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Beans))]
	internal static class BeansPatch
	{
		[HarmonyPatch("Stop")]
		[HarmonyPrefix]
		static bool AnimateWorry(Beans __instance, Animator ___animator)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp)
			{
				comp.StartCoroutine(Delay(comp, ___animator));
				return false;
			}
			return true;
		}

		static IEnumerator Delay(GenericAnimationExtraComponent comp, Animator ___animator)
		{
			for (int i = 0; i < 21; i++)
				yield return null; // delays

			___animator.enabled = false;
			comp.isActive = true;
			yield break;
		}

		[HarmonyPatch("GumHit")]
		[HarmonyPrefix]
		static void DisableWorry(Beans __instance, Animator ___animator)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp)
			{
				comp.StopAllCoroutines();
				comp.isActive = false;
			}

			___animator.enabled = true;
		}

		[HarmonyPatch("VirtualUpdate")]
		[HarmonyPostfix]
		static void Animate(Beans __instance)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp && comp.isActive)
				__instance.spriteRenderer[0].sprite = comp.sprites[Mathf.FloorToInt(Time.fixedTime * 36f) % comp.sprites.Length];
		}
	}
}
