using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(BaseGameManager), "Initialize")]
	internal class BaseGameManagerPatches
	{
		static void Prefix()
		{
			new GameObject("BalloonManager").AddComponent<BalloonManager>(); // Literally creates a balloon manager (yes, I didn't add in the base game manager because of the level editor)
		}
	}
}
