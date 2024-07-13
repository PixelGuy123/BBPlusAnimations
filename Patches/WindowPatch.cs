using UnityEngine;
using HarmonyLib;
using PixelInternalAPI.Components;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Window), "Break", [typeof(bool)])]
	internal class WindowPatch
	{
		static void Postfix(Window __instance, bool ___broken)
		{
			if (___broken)
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
