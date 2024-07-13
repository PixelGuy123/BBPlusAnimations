using BBPlusAnimations.Components;
using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(GameButtonBase))]
	internal class GameButtonBasePatch
	{
		[HarmonyPatch("Clicked")]
		static void Prefix(int playerNumber) =>
			Singleton<CoreGameManager>.Instance.GetCamera(playerNumber).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Insert);
		
	}
}
