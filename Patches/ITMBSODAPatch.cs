using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_BSODA), "Update")]
	internal class ITMBSODAPatch
	{
		private static void Prefix(ref SpriteRenderer ___spriteRenderer, EnvironmentController ___ec)
		{
			var mat = new MaterialPropertyBlock();
			___spriteRenderer.GetPropertyBlock(mat);
			mat.SetFloat("_SpriteRotation", (mat.GetFloat("_SpriteRotation") + ___ec.EnvironmentTimeScale) % 360f);
			___spriteRenderer.SetPropertyBlock(mat); // I hope this isn't expensive action
		}
	}
}
