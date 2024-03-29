using BBPlusAnimations.Components;
using HarmonyLib;
using PixelInternalAPI.Components;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_GrapplingHook))]
	internal class GrapplingHookAnimation
	{

		[HarmonyPatch("Use")]
		[HarmonyPrefix]
		private static void Setup(ITM_GrapplingHook __instance, PlayerManager pm)
		{
			var h = __instance.gameObject.AddComponent<GrapplingHookFOVHolder>();
			Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).GetComponent<CustomPlayerCameraComponent>()?.fovModifiers.Add(h.fovModifier);
		}

		[HarmonyPatch("Update")]
		[HarmonyPrefix]
		private static bool IsAbleOfRunning(ITM_GrapplingHook __instance, out bool __state)
		{
			__state = !__instance.GetComponent<GrapplingHookFOVHolder>().deadLocked;
			return __state;
		}

		[HarmonyPatch("Update")]
		[HarmonyPostfix]
		private static void Animation(ITM_GrapplingHook __instance, bool ___locked, float ___force, bool ___snapped, bool __state)
		{
			if (___locked && !___snapped && __state)
				__instance.GetComponent<GrapplingHookFOVHolder>().fovModifier.Mod = initialFov + ___force * 1.2f;
		}

		[HarmonyPatch("End")]
		[HarmonyPrefix]
		private static bool EndIt(ITM_GrapplingHook __instance, PlayerManager ___pm, LineRenderer ___lineRenderer, ref MovementModifier ___moveMod, AudioSource ___motorAudio)
		{
			var cam = Singleton<CoreGameManager>.Instance.GetCamera(___pm.playerNumber);
			var comp = __instance.GetComponent<GrapplingHookFOVHolder>();
			cam.StartCoroutine(comp.fovModifier.ResetSlideFOVAnimation(cam.GetComponent<CustomPlayerCameraComponent>().fovModifiers, 3f));
			comp.deadLocked = true; // to not be active in Update()

			___lineRenderer.enabled = false;
			___pm.Am.moveMods.Remove(___moveMod);
			___motorAudio.Stop();
			__instance.StartCoroutine(comp.FadeAnimation(__instance, ___pm.ec));
			return false;
		}



		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		[HarmonyPatch("End")]
		internal static void ActuallyEnd(object instance) => // The og method without patches
			throw new System.NotImplementedException("stub");


		const float initialFov = -65f;

	}
}
