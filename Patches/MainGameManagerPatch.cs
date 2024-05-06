using HarmonyLib;
using PixelInternalAPI.Classes;
using PixelInternalAPI.Components;

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
					Singleton<CoreGameManager>.Instance.GetCamera(i).GetComponent<CustomPlayerCameraComponent>().ReverseSlideFOVAnimation(new BaseModifier(), 35f, 5f);
			}
		}
	}
}
