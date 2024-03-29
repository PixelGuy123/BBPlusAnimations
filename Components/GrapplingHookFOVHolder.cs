using BBPlusAnimations.Patches;
using HarmonyLib;
using PixelInternalAPI.Classes;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Components
{
	internal class GrapplingHookFOVHolder : MonoBehaviour
	{
		internal BaseModifier fovModifier = new();

		internal bool deadLocked = false;

		internal IEnumerator FadeAnimation(ITM_GrapplingHook g, EnvironmentController ec)
		{
			float cooldown = Random.Range(2f, 5f);
			while (cooldown > 0f)
			{
				cooldown -= ec.EnvironmentTimeScale * Time.deltaTime;
				yield return null;
			}

			SpriteRenderer[] renderers = g.GetComponentsInChildren<SpriteRenderer>();
			bool flag = false;
			while (true)
			{
				renderers.Do((x) =>
				{
					Color alpha = x.color;
					alpha.a -= ec.EnvironmentTimeScale * Time.deltaTime;

					if (alpha.a > 0f)
						x.color = alpha;
					else flag = true;
				});


				if (flag)
					break;

				yield return null;
			}
			GrapplingHookAnimation.ActuallyEnd(g);

			yield break;
		}
	}
}
