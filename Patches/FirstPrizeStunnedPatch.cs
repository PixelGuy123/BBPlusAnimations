using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(FirstPrize_Stunned))]
	internal class FirstPrizeStunnedPatch
	{
		[HarmonyPatch("Enter")]
		[HarmonyPrefix]
		static void AddParticles(FirstPrize ___firstPrize)
		{
			var smoke = Object.Instantiate(smokes);
			smoke.transform.SetParent(___firstPrize.transform);
			smoke.transform.localPosition = Vector3.down;

			smoke.gameObject.SetActive(true);

		}

		[HarmonyPatch("Exit")]
		[HarmonyPrefix]
		static void RemoveParticles(FirstPrize ___firstPrize) =>
			___firstPrize.StartCoroutine(WaitForRemoval(___firstPrize.GetComponentInChildren<ParticleSystem>()));
		

		static IEnumerator WaitForRemoval(ParticleSystem particle)
		{
			var emit = particle.emission;
			emit.rateOverTimeMultiplier = 0f;

			while (particle.particleCount > 0)
				yield return null;
			
			Object.Destroy(particle.gameObject);

			yield break;
		}
		

		internal static ParticleSystem smokes;
	}
}
