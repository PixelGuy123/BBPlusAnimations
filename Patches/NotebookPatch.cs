using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Notebook), "Clicked", [typeof(int)])]
	internal class NotebookPatch
	{
		static void Prefix() =>
			Singleton<CoreGameManager>.Instance.audMan.PlaySingle(audPickNotebook);

		internal static SoundObject audPickNotebook;
	}
}
