﻿using System.Collections;
using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Jumprope))]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_PLAYTIME_JUMPROPECUT, ConfigEntryStorage.DESC_PLAYTIME_JUMPROPECUT)]
	internal static class PlaytimeJumpropePatch
	{
		[HarmonyPatch("End")]
		private static void Prefix(Jumprope __instance, bool success)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (comp)
				comp.isActive = success;
		}

		[HarmonyPatch("Destroy")]
		private static bool Prefix(Jumprope __instance, Animator ___animator, Canvas ___ropeCanvas, ref MovementModifier ___moveMod, Canvas ___textCanvas)
		{
			var comp = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (!comp || comp.isActive) // Quick fix to not show the animation on the wrong moment
				return true;

			__instance.player.plm.am.moveMods.Remove(___moveMod);
			__instance.player.jumpropes.Remove(__instance); // I HATE YOU, SCISSORS FOR CRASHING MY GAME
			___animator.enabled = false;
			__instance.StopAllCoroutines(); // No jumprope checks
			__instance.StartCoroutine(Animation(__instance, ___ropeCanvas.transform.Find("Jumprope_0").GetComponent<SpriteRenderer>(), ___ropeCanvas, ___textCanvas));
			return false; // Nuh uh
		}

		static IEnumerator Animation(Jumprope j, SpriteRenderer sprite, Canvas ropeCanvas, Canvas textCanvas)
		{
			int idx;
			float frames = 0f;
			while (true)
			{
				frames += 31f * Time.deltaTime;
				idx = Mathf.FloorToInt(frames);
				if (idx < sprites.Length)
					sprite.sprite = sprites[idx];
				else
					break;
				yield return null;
			}

			Singleton<CoreGameManager>.Instance.GetCamera(j.player.playerNumber).UpdateTargets(null, 24);
			j.player.jumpropes.Remove(j);
			Object.Destroy(textCanvas.gameObject);
			Object.Destroy(ropeCanvas.gameObject);
			Object.Destroy(j.gameObject);


			yield break;
		}

		internal static Sprite[] sprites;
	}
}
