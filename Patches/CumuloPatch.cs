using BBPlusAnimations.Components;
using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Cumulo))]
	internal static class CumuloPatch
	{
		[HarmonyPatch("Blow")]
		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		private static void ActualBlow(object instance) =>
			throw new System.NotImplementedException("Stub");

		[HarmonyPatch("Blow")]
		[HarmonyPrefix]
		private static bool NotActualBlow(Cumulo __instance)
		{
			var audman = __instance.GetComponent<PropagatedAudioManager>();
			audman.PlaySingle(blowBeforeBlowing);

			if (ConfigEntryStorage.CFG_NPC_CUMULO_INHALE.Value)
				__instance.StartCoroutine(Animation(__instance, __instance.spriteBase.transform.Find("Sprite"), audman));
			else if (ConfigEntryStorage.CFG_NPC_CUMULO_WIND_PARTICLES.Value && __instance.TryGetComponent<CloudyCopterExtraComp>(out var cc))
				cc.shouldBlow = true;

			return false;
		}

		static IEnumerator Animation(Cumulo c, Transform trans, AudioManager audman)
		{
			Vector3 startPos = trans.position;
			float scale = 1f;
			while (true)
			{
				scale += (maxSize - scale) / 2.5f * 11f * c.ec.NpcTimeScale * Time.deltaTime;
				if (trans.position != startPos || scale >= maxSize - 0.01f)
					break;
				trans.localScale = Vector3.one * scale;
				yield return null;
			}
			if (trans.position == startPos)
			{
				scale = maxSize;
				trans.localScale = Vector3.one * scale;

				while (audman.AnyAudioIsPlaying)
					yield return null;

				yield return null;
				ActualBlow(c);
				if (ConfigEntryStorage.CFG_NPC_CUMULO_WIND_PARTICLES.Value && c.TryGetComponent<CloudyCopterExtraComp>(out var cc))
					cc.shouldBlow = true;
			}
			else audman.FlushQueue(true);

			

			while (true)
			{
				scale += (1f - scale) / 2.5f * 7f * c.ec.NpcTimeScale * Time.deltaTime;
				if (scale <= 1.01f)
					break;
				trans.localScale = Vector3.one * scale;
				yield return null;
			}
			trans.localScale = Vector3.one;
			

			yield break;
		}

		[HarmonyPatch("StopBlowing")]
		[HarmonyPostfix]
		private static void Pah(Cumulo __instance) 
		{
			var comp = __instance.GetComponent<CloudyCopterExtraComp>();
			if (comp)
			{
				if (ConfigEntryStorage.CFG_NPC_CUMULO_EXHALE.Value)
				__instance.GetComponent<PropagatedAudioManager>().PlaySingle(pah);
				if (comp.compToHold){
				comp.compToHold.transform.localPosition = Vector3.down * 10f;
				var e = comp.compToHold.emission;
				e.rateOverTimeMultiplier = 0f;
				}
				comp.shouldBlow = false;
			}
		}

		internal static SoundObject blowBeforeBlowing, pah;

		const float maxSize = 1.45f;

		[HarmonyPatch(typeof(Cumulo_Blowing), "Update")]
		[HarmonyPrefix]
		static void ChangeWindPosition(Cumulo ___cumulo)
		{
			var comp = ___cumulo.GetComponent<CloudyCopterExtraComp>();
			if (comp && comp.shouldBlow)
			{
				comp.compToHold.transform.position = ___cumulo.currentHall[Random.Range(0, ___cumulo.currentHall.Count)].CenterWorldPosition + new Vector3(Random.Range(-offset, offset), Random.Range(-offset, offset), Random.Range(-offset, offset));
				var e = comp.compToHold.emission;
				e.rateOverTimeMultiplier = 65f * (___cumulo.currentHall.Count * 0.3f);
			}
			
		}

		const float offset = 5f;
	}
}
