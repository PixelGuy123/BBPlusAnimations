using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	[AnimationConditionalPatch("Playtime hapiness", "If True, Playtime will display an unique animation when completing her minigame.")]
	internal class PlaytimePatches
	{

		[HarmonyPatch(typeof(Playtime_Cooldown), "Update")]
		[HarmonyPrefix]
		static void PlaytimeHappyAnimation(Playtime ___playtime)
		{
			var comp = ___playtime.GetComponent<GenericAnimationExtraComponent>();
			if (comp.isActive)
				___playtime.spriteRenderer[0].sprite = comp.sprites[Mathf.FloorToInt(Time.fixedTime * 17f * ___playtime.TimeScale) % comp.sprites.Length];
		}

		[HarmonyPatch(typeof(Playtime), "EndCooldown")]
		[HarmonyPrefix]
		static void PlayTimeResetAnimator(Animator ___animator) =>
			___animator.enabled = true;

		[HarmonyPatch(typeof(Playtime), "EndJumprope")]
		[HarmonyPrefix]
		static void PlaytimeIsSadOrNot(Playtime __instance, bool won)
		{
			__instance.GetComponent<GenericAnimationExtraComponent>().isActive = won;
			__instance.animator.enabled = !won;
		}
	}
}
