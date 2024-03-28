using HarmonyLib;
using PixelInternalAPI.Components;
using PixelInternalAPI.Extensions;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_Teleporter), "Teleport")]
	internal class ITMTeleporterPatch
	{
		private static void Prefix(PlayerManager ___pm) =>
			Singleton<CoreGameManager>.Instance.GetCamera(___pm.playerNumber).StartCoroutine(Singleton<CoreGameManager>.Instance.GetCamera(___pm.playerNumber).GetComponent<CustomPlayerCameraComponent>()?.fovModifiers.ReverseSlideFOVAnimation(new(), 65f, 4f));
		
	}
}
