using HarmonyLib;
using PixelInternalAPI.Extensions;
using MTM101BaldAPI.Components;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Teleporter fov changer", "If True, the Dangerous Teleporter will also change the fov when teleporting.")]
	[HarmonyPatch(typeof(ITM_Teleporter), "Teleport")]
	internal class ITMTeleporterPatch
	{
		private static void Prefix(PlayerManager ___pm) =>
			___pm.GetCustomCam().ReverseSlideFOVAnimation(new ValueModifier(), 65f, 4f);
	}
}
