using HarmonyLib;
using PixelInternalAPI.Components;
using PixelInternalAPI.Classes;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_Teleporter), "Teleport")]
	internal class ITMTeleporterPatch
	{
		private static void Prefix(PlayerManager ___pm) =>
			Singleton<CoreGameManager>.Instance.GetCamera(___pm.playerNumber).GetComponent<CustomPlayerCameraComponent>().ReverseSlideFOVAnimation(new BaseModifier(), 65f, 4f);
	}
}
