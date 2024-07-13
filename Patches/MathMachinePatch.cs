using BBPlusAnimations.Components;
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
		private static void Postfix(TMP_Text ___answerText, MathMachine __instance, ref AudioManager ___audMan, bool ___givePoints)
		{
			__instance.StartCoroutine(Animation(__instance.room.ec, ___answerText.transform));
			if (___givePoints && aud_BalWow != null)
				___audMan.PlaySingle(aud_BalWow);
		}


		internal static SoundObject aud_BalWow;

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

		[HarmonyPatch("Clicked", [typeof(int)])]
		[HarmonyPrefix]
		static void Prefix(bool[] ___playerIsHolding, int player)
		{
			if (___playerIsHolding[player])
				Singleton<CoreGameManager>.Instance.GetCamera(player).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Insert);
		}
		

	}
}
