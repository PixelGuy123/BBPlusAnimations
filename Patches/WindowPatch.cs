using UnityEngine;
using HarmonyLib;
using PixelInternalAPI.Components;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_WINDOW_SHATTER, ConfigEntryStorage.DESC_WINDOW_SHATTER)]
	[HarmonyPatch(typeof(Window), "Break", [typeof(bool)])]
	internal class WindowPatch
	{
		static void Prefix(bool ___broken, out bool __state) =>
			__state = ___broken;
		
		static void Postfix(Window __instance, bool ___broken, bool __state)
		{
			if (___broken != __state) // If it's different, it means it has actually broke
			{
				var p = Object.Instantiate(glassPieces);
				p.transform.position = __instance.windows[0].transform.position;
				p.ec = __instance.ec;
				p.gameObject.SetActive(true);
			}
		}

		internal static TemporaryParticles glassPieces;
	}
}
