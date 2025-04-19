using BBPlusAnimations.Components;
using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_MISC, ConfigEntryStorage.NAME_NOTEBOOK_SOUND, ConfigEntryStorage.DESC_NOTEBOOK_SOUND)]
	[HarmonyPatch(typeof(Notebook), "Clicked", [typeof(int)])]
	internal static class NotebookPatch
	{
		[HarmonyPrefix]
		static void NotebookNoise() =>
			Singleton<CoreGameManager>.Instance.audMan.PlaySingle(audPickNotebook);

		internal static SoundObject audPickNotebook;
	}

	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_MISC, ConfigEntryStorage.NAME_HANDANIM_NOTEBOOK, ConfigEntryStorage.DESC_HANDANIM_NOTEBOOK)]
	[HarmonyPatch(typeof(Notebook), "Clicked", [typeof(int)])]
	internal static class NotebookPatchForHands
	{
		[HarmonyPrefix]
		static void PickupHand(int player) =>
			Singleton<CoreGameManager>.Instance.GetCamera(player).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Pickup);
	}
}
