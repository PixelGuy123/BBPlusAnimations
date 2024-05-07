using HarmonyLib;
using MTM101BaldAPI.Components;
using PixelInternalAPI.Extensions;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(MainGameManager))]
	internal class MainGameManagerPatch
	{
		[HarmonyPatch("AllNotebooks")]
		static void Postfix()
		{
			if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Main)
			{
				for (int i = 0; i < Singleton<CoreGameManager>.Instance.setPlayers; i++)
					Singleton<CoreGameManager>.Instance.GetCamera(i).GetCustomCam().ReverseSlideFOVAnimation(new ValueModifier(), 35f, 5f);
			}
		}
	}
}
