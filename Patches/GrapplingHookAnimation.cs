using BBPlusAnimations.Components;
using HarmonyLib;
using PixelInternalAPI.Components;
using PixelInternalAPI.Extensions;

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
		[HarmonyPostfix]
		private static void Animation(ITM_GrapplingHook __instance, bool ___locked, float ___force, bool ___snapped)
		{
			if (___locked && !___snapped)
				__instance.GetComponent<GrapplingHookFOVHolder>().fovModifier.Mod = initialFov + ___force * 1.2f;
		}

		[HarmonyPatch("End")]
		[HarmonyPrefix]
		private static void EndIt(ITM_GrapplingHook __instance, PlayerManager ___pm)
		{
			var cam = Singleton<CoreGameManager>.Instance.GetCamera(___pm.playerNumber);
			cam.StartCoroutine(cam.GetComponent<CustomPlayerCameraComponent>()?.fovModifiers.ResetSlideFOVAnimation(__instance.GetComponent<GrapplingHookFOVHolder>().fovModifier, 3f));
		}


		const float initialFov = -65f;

	}
}
