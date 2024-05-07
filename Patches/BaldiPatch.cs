using BBPlusAnimations.Components;
using HarmonyLib;
using PixelInternalAPI.Components;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	internal class BaldiPatch
	{
		[HarmonyPatch(typeof(Baldi), "SlapBreak")]
		[HarmonyPostfix]
		private static void BreakWithBeauty(Baldi __instance)
		{
			var p = Object.Instantiate(particle);
			p.transform.position = __instance.transform.position;
			p.ec = __instance.ec;
			p.gameObject.SetActive(true);
		}

		[HarmonyPatch(typeof(Baldi), "TakeApple")]
		[HarmonyPrefix]
		static void GetAppleParticles(Baldi __instance)
		{
			var part = Object.Instantiate(appleParticles);
			part.transform.SetParent(__instance.transform);
			part.transform.localPosition = Vector3.up;
		//	part.gameObject.SetActive(true);
			__instance.GetComponent<BaldiEatAppleComponent>().particles = part;
		}

		[HarmonyPatch(typeof(Baldi), "EatSound")]
		[HarmonyPostfix]
		static void EatWithBeauty(Baldi __instance) =>
			__instance.GetComponent<BaldiEatAppleComponent>().particles?.Emit(Random.Range(3, 25));

		internal static TemporaryParticles particle;
		internal static ParticleSystem appleParticles;

		[HarmonyPatch(typeof(Baldi_Apple), "Update")]
		[HarmonyPostfix]
		static void CanDestroyParticles(Baldi ___baldi, float ___time)
		{
			if (___time <= 0f)
			{
				var comp = ___baldi.GetComponent<BaldiEatAppleComponent>();
				comp.particles.transform.SetParent(null, true);
				comp.SetCooldownToDestroyParticles();
			}
			
		}
	}
}
