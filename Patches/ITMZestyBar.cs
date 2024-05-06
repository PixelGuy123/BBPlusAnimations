using BBPlusAnimations.Components;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_ZestyBar), "Use")]
	internal class ITMZestyBar
	{
		static void Prefix(PlayerManager pm)
		{
			var part = Object.Instantiate(prefab);
			part.transform.forward = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).transform.forward;
			part.transform.position = pm.transform.position + Vector3.down;
			part.gameObject.SetActive(true);

			part.Emit(Random.Range(25, 35));

			part.gameObject.AddComponent<DestroyWhenDone>().particles = part;
		}

		internal static ParticleSystem prefab;
	}
}
