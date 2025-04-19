using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_PLAYTIME_HAPPY, ConfigEntryStorage.DESC_PLAYTIME_HAPPY)]
	internal static class PlaytimePatches
	{

		[HarmonyPatch(typeof(Playtime_Cooldown), "Update")]
		[HarmonyPrefix]
		static void PlaytimeHappyAnimation(Playtime ___playtime)
		{
			var comp = ___playtime.GetComponent<GenericAnimationExtraComponent>();
			if (comp && comp.isActive)
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
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp)
				comp.isActive = won;
			__instance.animator.enabled = !won;
		}
	}
}
