using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Elevator gate hit", "If True, the elevator gate will bounce a bit up after hitting.")]
	[HarmonyPatch(typeof(Elevator), "Close")]
	internal class ElevatorPatch
	{
		private static void Postfix(Elevator __instance, Door ___door, Animator ___animator) =>
			__instance.StartCoroutine(Animation(__instance.transform.Find("Gate"), ___door.ec, ___animator));

		static IEnumerator Animation(Transform i, EnvironmentController ec, Animator a)
		{
			while (i.transform.localPosition.y > 0f)
				yield return null;
			a.enabled = false;

			float speed = 0.05f;
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
