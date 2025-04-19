using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_BSODA))]
	internal static class ITMBSODAPatch
	{
		[HarmonyPatch("Update")]
		private static void Prefix(ref SpriteRenderer ___spriteRenderer, EnvironmentController ___ec, float ___time)
		{
			if (ConfigEntryStorage.CFG_BSODA_ROTATION_SPEED.Value > 0f)
			{
				var mat = new MaterialPropertyBlock();
				___spriteRenderer.GetPropertyBlock(mat);
				mat.SetFloat("_SpriteRotation", (mat.GetFloat("_SpriteRotation") + ___ec.EnvironmentTimeScale * ConfigEntryStorage.CFG_BSODA_ROTATION_SPEED.Value) % 360f);
				___spriteRenderer.SetPropertyBlock(mat); // I hope this isn't expensive action
			}

			if (___time <= ConfigEntryStorage.CFG_BSODA_THRESHOLD.Value)
			{
				Color co = ___spriteRenderer.color;
				co.a = ___time / ConfigEntryStorage.CFG_BSODA_THRESHOLD.Value;
				___spriteRenderer.color = co;
			}
		}

		[HarmonyPatch("Use")]
		private static void Postfix(ITM_BSODA __instance, SpriteRenderer ___spriteRenderer)
		{
			if (ConfigEntryStorage.CFG_BSODA_GROWN_SPEED.Value > 0f)
				__instance.StartCoroutine(SpawnAnimation(___spriteRenderer.transform, __instance.ec));
		}

		static IEnumerator SpawnAnimation(Transform bsoda, EnvironmentController ec)
		{
			float scale = 0.1f;
			while (true)
			{
				scale += (1f - scale) * ConfigEntryStorage.CFG_BSODA_GROWN_SPEED.Value * Time.deltaTime * ec.EnvironmentTimeScale;
				if (scale >= 1f)
					break;
				
				bsoda.localScale = Vector3.one * scale;
				yield return null;
			}
			bsoda.localScale = Vector3.one * 1f;
			yield break;
		}
	}
}
