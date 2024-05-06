using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_BSODA))]
	internal class ITMBSODAPatch
	{
		[HarmonyPatch("Update")]
		private static void Prefix(ref SpriteRenderer ___spriteRenderer, EnvironmentController ___ec)
		{
			var mat = new MaterialPropertyBlock();
			___spriteRenderer.GetPropertyBlock(mat);
			mat.SetFloat("_SpriteRotation", (mat.GetFloat("_SpriteRotation") + ___ec.EnvironmentTimeScale) % 360f);
			___spriteRenderer.SetPropertyBlock(mat); // I hope this isn't expensive action
		}

		[HarmonyPatch("Use")]
		private static void Postfix(ITM_BSODA __instance) =>
			__instance.StartCoroutine(SpawnAnimation(__instance.transform, __instance.ec));

		static IEnumerator SpawnAnimation(Transform bsoda, EnvironmentController ec)
		{
			float scale = 0.1f;
			while (true)
			{
				scale += (1f - scale) * 0.9f * Time.deltaTime * ec.EnvironmentTimeScale;
				if (scale >= 1f)
					break;
				
				bsoda.localScale = Vector3.one * scale;
				yield return null;
			}
			bsoda.localScale = Vector3.one;
			yield break;
		}
	}
}
