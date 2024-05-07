using HarmonyLib;
using PixelInternalAPI.Extensions;
using MTM101BaldAPI.Components;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_Teleporter), "Teleport")]
	internal class ITMTeleporterPatch
	{
		private static void Prefix(PlayerManager ___pm) =>
			___pm.GetCustomCam().ReverseSlideFOVAnimation(new ValueModifier(), 65f, 4f);
	}
}
