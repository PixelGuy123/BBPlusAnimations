using BBPlusAnimations.Components;
using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Enable hand animation", "If True, hands will be displayed on screen when performing certain interactions.")]
	[HarmonyPatch(typeof(GameButtonBase))]
	internal class GameButtonBasePatch
	{
		[HarmonyPatch("Clicked")]
		static void Prefix(int playerNumber) =>
			Singleton<CoreGameManager>.Instance.GetCamera(playerNumber).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Insert);
		
	}
}
