using BBPlusAnimations.Components;
using HarmonyLib;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_MATHMACHINE_NUMBERPOPUP, ConfigEntryStorage.DESC_MATHMACHINE_NUMBERPOPUP)]
	[HarmonyPatch(typeof(MathMachine))]

	internal static class MathMachinePatch_TextExpand
	{
		[HarmonyPatch("Completed", [typeof(int), typeof(bool), typeof(Activity)])]
		private static void Postfix(TMP_Text ___answerText, MathMachine __instance, bool correct)
		{
			if (correct)
			{
				if (!__instance.TryGetComponent<GenericAnimationExtraComponent>(out var comp))
					comp = __instance.gameObject.AddComponent<GenericAnimationExtraComponent>();


				if (comp.runningAnimation != null)
					__instance.StopCoroutine(comp.runningAnimation);
				comp.runningAnimation = __instance.StartCoroutine(Animation(__instance.room.ec, ___answerText.transform));
			}
		}


	

		private static IEnumerator Animation(EnvironmentController ec, Transform transform)
		{
			float ogScale = 1.2f;
			while (true)
			{
				ogScale += (1f - ogScale) * 5f * ec.EnvironmentTimeScale * Time.deltaTime;
				if (ogScale <= 1f)
					break;
				transform.localScale = Vector3.one * ogScale;
				yield return null;
			}
			transform.localScale = Vector3.one;

			yield break;
		}


	}

	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_MATHMACHINE_WOW, ConfigEntryStorage.DESC_MATHMACHINE_WOW)]
	[HarmonyPatch(typeof(MathMachine))]
	internal static class MathMachinePatch_WOOW
	{
		[HarmonyPatch("Completed", [typeof(int), typeof(bool), typeof(Activity)])]
		private static void Postfix(ref AudioManager ___audMan, bool correct)
		{
			if (correct && aud_BalWow)
				___audMan.PlaySingle(aud_BalWow);

		}
		internal static SoundObject aud_BalWow;
	}
}
