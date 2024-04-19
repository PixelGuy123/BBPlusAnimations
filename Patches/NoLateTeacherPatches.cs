using BBPlusAnimations.Components;
using HarmonyLib;
using PixelInternalAPI.Classes;
using PixelInternalAPI.Components;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	internal class NoLateTeacherPatches
	{
		[HarmonyPatch(typeof(NoLateTeacher), "Attack")]
		[HarmonyPrefix]
		private static void SetFovToPlayer(PlayerManager player) =>
			Singleton<CoreGameManager>.Instance.GetCamera(player.playerNumber).GetComponent<CustomPlayerCameraComponent>().ShakeFOVAnimation(new BaseModifier(), intensity: 55f, shakeCooldown: 2f);
		
		
	}
}
