using HarmonyLib;
using PixelInternalAPI.Components;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Baldi))]
	internal class BaldiPatch
	{
		[HarmonyPatch("SlapBreak")]
		[HarmonyPostfix]
		private static void BreakWithBeauty(Baldi __instance)
		{
			var p = Object.Instantiate(particle);
			p.transform.position = __instance.transform.position;
			p.ec = __instance.ec;
			p.gameObject.SetActive(true);
		}

		internal static TemporaryParticles particle;
	}
}
