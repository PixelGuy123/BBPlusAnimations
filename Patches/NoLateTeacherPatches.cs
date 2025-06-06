﻿using BBPlusAnimations.Components;
using HarmonyLib;
using MTM101BaldAPI.Components;
using PixelInternalAPI.Extensions;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.DESC_POMP_SIGHT, ConfigEntryStorage.DESC_POMP_SIGHT)]
	internal static class MrsPompPatch_DeadSight
	{
		[HarmonyPatch(typeof(NoLateTeacher_Wander), "PlayerInSight")]
		[HarmonyPostfix]
		private static void SpotPlayer(NoLateTeacher ___pomp, PlayerManager player)
		{
			if (player.Tagged) return;

			var comp = ___pomp.GetComponent<GenericAnimationExtraComponent>();
			if (comp && !comp.isActive)
			{
				comp.isActive = true;
				if (comp.runningAnimation != null)
					___pomp.StopCoroutine(comp.runningAnimation);
				comp.runningAnimation = ___pomp.StartCoroutine(SpotPlayerAnimation(___pomp, comp, ___pomp.sprite));
			}
		}

		[HarmonyPatch(typeof(NoLateTeacher), "ReleasePlayer")]
		[HarmonyPostfix]
		static void RevertEyeAnimation(NoLateTeacher __instance)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp && comp.isActive) 
			{
				comp.isActive = false;
				if (comp.runningAnimation != null)
					__instance.StopCoroutine(comp.runningAnimation);
				comp.runningAnimation = __instance.StartCoroutine(RevertEyesAnimation(__instance, comp, __instance.sprite));
			}
		}

		[HarmonyPatch(typeof(NoLateTeacher_Wander), "DestinationEmpty")]
		[HarmonyPostfix]
		static void RevertEyeAnimationWhenLost(NoLateTeacher ___pomp)
		{
			var comp = ___pomp.GetComponent<GenericAnimationExtraComponent>();
			if (comp && comp.isActive)
			{
				comp.isActive = false;
				if (comp.runningAnimation != null)
					___pomp.StopCoroutine(comp.runningAnimation);
				comp.runningAnimation = ___pomp.StartCoroutine(RevertEyesAnimation(___pomp, comp, ___pomp.sprite));
			}
		}

		
		static IEnumerator SpotPlayerAnimation(NoLateTeacher t, GenericAnimationExtraComponent comp, SpriteRenderer teacher)
		{
			float frame = 0f;
			int idx;
			while (true)
			{
				frame += 15f * t.TimeScale * Time.deltaTime;
				idx = Mathf.FloorToInt(frame);
				if (idx < comp.sprites.Length)
					teacher.sprite = comp.sprites[idx];
				else break;
				yield return null;
			}
			teacher.sprite = comp.sprites[comp.sprites.Length - 1];

			yield break;
		}

		static IEnumerator RevertEyesAnimation(NoLateTeacher t, GenericAnimationExtraComponent comp, SpriteRenderer teacher)
		{
			float frame = comp.sprites.Length - 1;
			int idx;
			while (true)
			{
				frame -= 15f * t.TimeScale * Time.deltaTime;
				idx = Mathf.FloorToInt(frame);
				if (idx >= 0)
					teacher.sprite = comp.sprites[idx];
				else break;
				yield return null;
			}
			teacher.sprite = comp.sprites[0];

			yield break;
		}
	}
	[HarmonyPatch]
	internal static class MrsPompPatch_Scream
	{
		[HarmonyPatch(typeof(NoLateTeacher), "Attack")]
		[HarmonyPrefix]
		private static void SetFovToPlayer(PlayerManager player)
		{
			if (ConfigEntryStorage.CFG_POMP_FOVINTENSITY.Value > 15f)
				player.GetCustomCam().ShakeFOVAnimation(new ValueModifier(), intensity: ConfigEntryStorage.CFG_POMP_FOVINTENSITY.Value, shakeCooldown: 2f);
		}
	}
}
