using HarmonyLib;
using PixelInternalAPI.Extensions;
using MTM101BaldAPI.Components;
using PixelInternalAPI.Components;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ITEMS, ConfigEntryStorage.NAME_TELEPORTER_PARTICLES, ConfigEntryStorage.DESC_TELEPORTER_PARTICLES, false)]
	[HarmonyPatch(typeof(ITM_Teleporter), "Teleport")]
	internal static class ITMTeleporterPatch
	{
		private static void Prefix(PlayerManager ___pm) =>
			___pm.GetCustomCam().ReverseSlideFOVAnimation(new ValueModifier(), 65f, 4f);
		

		static void Postfix(PlayerManager ___pm)
		{
			var part = Object.Instantiate(parts);
			part.ec = ___pm.ec;
			part.transform.position = ___pm.transform.position;
		}

		internal static TemporaryParticles parts;
	}
}
