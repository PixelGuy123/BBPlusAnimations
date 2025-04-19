using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_ELEVATOR_GATEHIT, ConfigEntryStorage.DESC_ELEVATOR_GATEHIT)]
	[HarmonyPatch(typeof(Elevator), "Close")]
	internal static class ElevatorPatch
	{
		private static void Postfix(Elevator __instance, Door ___door, Animator ___animator) =>
			__instance.StartCoroutine(Animation(__instance.transform.Find("Gate"), ___door.ec, ___animator));

		static IEnumerator Animation(Transform i, EnvironmentController ec, Animator a)
		{
			while (i.transform.localPosition.y > 0f)
				yield return null;
			a.enabled = false;

			float speed = 0.065f;
			Vector3 pos = i.transform.localPosition;

			while (true)
			{
				pos.y += speed;
				if (pos.y < 0f)
					break;
				i.transform.localPosition = pos;
				speed -= 0.75f * ec.EnvironmentTimeScale * Time.deltaTime;
				yield return null;
			}
			pos.y = 0f;
			i.transform.localPosition = pos;

			yield break;
		}
	}
}
