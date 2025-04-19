using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;
using PixelInternalAPI.Extensions;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_GrapplingHook))]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ITEMS, ConfigEntryStorage.NAME_GRAPPLINGHOOK_FOV, ConfigEntryStorage.DESC_GRAPPLINGHOOK_FOV)]
	internal class GrapplingHookAnimation_FOV
	{

		[HarmonyPatch("Use")]
		[HarmonyPrefix]
		private static void Setup(ITM_GrapplingHook __instance, PlayerManager pm)
		{
			var h = __instance.gameObject.AddComponent<GrapplingHookFOVHolder>();
			pm.GetCustomCam().AddModifier(h.modifier);
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
		private static void Animation(ITM_GrapplingHook __instance, bool ___locked, float ___force, bool ___snapped, bool __state, bool __runOriginal) // Should only run if runOriginal is true to avoid incompatibilities with other mods...
		{
			if (___locked && __runOriginal && !___snapped && __state)
				__instance.GetComponent<GrapplingHookFOVHolder>().modifier.addend = initialFov + ___force * 1.2f;
		}

		[HarmonyPatch("End")]
		[HarmonyPrefix]
		private static bool EndIt(ITM_GrapplingHook __instance, PlayerManager ___pm, LineRenderer ___lineRenderer, ref MovementModifier ___moveMod, AudioSource ___motorAudio)
		{
			var cam = Singleton<CoreGameManager>.Instance.GetCamera(___pm.playerNumber);
			var comp = __instance.GetComponent<GrapplingHookFOVHolder>();
			cam.GetCustomCam().ResetSlideFOVAnimation(comp.modifier, 3f);
			comp.deadLocked = true; // to not be active in Update()

			___lineRenderer.enabled = false;
			___pm.Am.moveMods.Remove(___moveMod);
			___motorAudio.Stop();
			return false;
		}



		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		[HarmonyPatch("End")]
		internal static void ActuallyEnd(object instance) => // The og method without patches
			throw new System.NotImplementedException("stub");


		const float initialFov = -65f;

	}
	[HarmonyPatch(typeof(ITM_GrapplingHook))]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ITEMS, ConfigEntryStorage.NAME_GRAPPLINGHOOK_FADEOUT, ConfigEntryStorage.DESC_GRAPPLINGHOOK_FADEOUT)]
	internal static class GrapplingHookAnimation_FadeOut
	{
		[HarmonyPatch("End")]
		[HarmonyPrefix]
		private static bool EndIt(ITM_GrapplingHook __instance, PlayerManager ___pm, LineRenderer ___lineRenderer, ref MovementModifier ___moveMod, AudioSource ___motorAudio)
		{
			var comp = __instance.GetComponent<GrapplingHookFOVHolder>();
			__instance.StartCoroutine(comp.FadeAnimation(__instance, ___pm.ec));
			return false;
		}
	}
}
