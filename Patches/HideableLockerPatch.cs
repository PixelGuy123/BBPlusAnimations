using BBPlusAnimations.Components;
using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(HideableLocker))]
	[AnimationConditionalPatch("Hideable locker opening animation", "If True, all hideable lockers (blue lockers) will slightly open if you aim at them.")]
	internal static class HideableLockerPatch
	{
		[HarmonyPatch("ClickableSighted")]
		[HarmonyPrefix]
		static void ChangeSightMaterial(HideableLocker __instance)
		{
			var comp = __instance.GetComponent<HideableLockerExtraComponent>();
			if (!comp) return;

			comp.renderer.materials[0].SetTexture("_MainTex", comp.open);
		}

		[HarmonyPatch("ClickableUnsighted")]
		[HarmonyPrefix]
		static void ChangeUnsightMaterial(HideableLocker __instance)
		{
			var comp = __instance.GetComponent<HideableLockerExtraComponent>();
			if (!comp) return;

			comp.renderer.materials[0].SetTexture("_MainTex", comp.closed);
		}
	}
}
