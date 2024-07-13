using BBPlusAnimations.Components;
using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Notebook), "Clicked", [typeof(int)])]
	internal class NotebookPatch
	{
		static void Prefix(int player)
		{
			Singleton<CoreGameManager>.Instance.audMan.PlaySingle(audPickNotebook);
			Singleton<CoreGameManager>.Instance.GetCamera(player).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Pickup);
		}

		internal static SoundObject audPickNotebook;
	}
}
