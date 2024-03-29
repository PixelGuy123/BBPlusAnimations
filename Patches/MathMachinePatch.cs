using HarmonyLib;
using System.Collections;
using TMPro;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(MathMachine))]
	internal class MathMachinePatch
	{
		[HarmonyPatch("Completed")]
		private static void Postfix(TMP_Text ___answerText, MathMachine __instance) =>
			__instance.StartCoroutine(Animation(__instance.room.ec, ___answerText.transform));

		private static IEnumerator Animation(EnvironmentController ec, Transform transform)
		{
			float ogScale = 1.2f;
			while (true)
			{
				ogScale += (1f - ogScale) / 3f * 15f * ec.EnvironmentTimeScale * Time.deltaTime;
				if (ogScale <= 1f)
					break;
				transform.localScale = Vector3.one * ogScale;
				yield return null;
			}
			transform.localScale = Vector3.one;

			yield break;
		}

	}
}
