using BBPlusAnimations.Components;
using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Notebook pickup noise", "If True, notebooks will play a secondary noise when being picked up.")]
	[HarmonyPatch(typeof(Notebook), "Clicked", [typeof(int)])]
	internal class NotebookPatch
	{

		[HarmonyPrefix]
		static void NotebookNoise() =>
			Singleton<CoreGameManager>.Instance.audMan.PlaySingle(audPickNotebook);

		internal static SoundObject audPickNotebook;
	}

	[AnimationConditionalPatch("Enable hand animation", "If True, hands will be displayed on screen when performing certain interactions.")]
	[HarmonyPatch(typeof(Notebook), "Clicked", [typeof(int)])]
	internal class NotebookPatchForHands
	{
		[HarmonyPrefix]
		static void PickupHand(int player) =>
			Singleton<CoreGameManager>.Instance.GetCamera(player).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Pickup);
	}
}
