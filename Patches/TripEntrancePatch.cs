using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	//[HarmonyPatch(typeof(TripEntrance))]
	//internal class TripEntrancePatch
	//{
	//	[HarmonyPatch("OnTriggerEnter")]
	//	[HarmonyPrefix]
	//	private static void BeforeTriggerForCheck(out bool __state, bool ___entered) =>
	//		__state = ___entered;


	//	[HarmonyPatch("OnTriggerEnter")]
	//	[HarmonyPostfix]
	//	private static void TriggerEnterEffect(TripEntrance __instance, EnvironmentController ___ec, bool __state, bool ___entered)
	//	{
	//		if (__state == ___entered) return;

	//		__instance.StartCoroutine(BusGoAway(__instance.transform.Find("Door_Swinging").Find("Bus"), ___ec));
	//		__instance.StartCoroutine(BusGoAway(__instance.transform.Find("Door_Swinging").Find("Bus_1"), ___ec));
	//	}

	//	static IEnumerator BusGoAway(Transform bus, EnvironmentController ec)
	//	{
	//		yield return null;

	//		while (Time.timeScale <= 0f) yield return null;

	//		float speed = 0f;
	//		float timeSpeed = 0f;
	//		while (timeSpeed < 8f)
	//		{
	//			speed += timeSpeed;
	//			timeSpeed += speedIncrease * ec.EnvironmentTimeScale * Time.deltaTime;
	//			bus.transform.position += bus.right * speed;
	//			yield return null;
	//		}

	//		bus.gameObject.SetActive(false);

	//		yield break;
	//	}

	//	[HarmonyPatch("Start")]
	//	[HarmonyPostfix]
	//	static void RemoveBusIfNeeded(TripEntrance __instance, bool ___entered)
	//	{
	//		__instance.transform.Find("Door_Swinging").Find("Bus").gameObject.SetActive(!___entered);
	//		__instance.transform.Find("Door_Swinging").Find("Bus_1").gameObject.SetActive(!___entered);
	//	}

	//	const float speedIncrease = 0.002f;
	//}
}
