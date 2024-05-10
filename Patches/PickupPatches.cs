using HarmonyLib;
using PixelInternalAPI.Components;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Pickup), "Clicked", [typeof(int)])]
	internal class PickupPatches
	{
		static void Prefix(Pickup __instance, int player)
		{
			if (__instance.item.itemType != Items.Points) return; // ONLY POINTS ANIMATION!!
			Singleton<CoreGameManager>.Instance.audMan.PlaySingle(audCollect); 
			var par = Object.Instantiate(particles);
			par.transform.position = __instance.transform.position;
			par.ec = Singleton<CoreGameManager>.Instance.GetPlayer(player).ec;
			par.GetComponent<ParticleSystemRenderer>().material.mainTexture = __instance.item.itemSpriteLarge.texture;
		}

		internal static SoundObject audCollect;

		internal static TemporaryParticles particles;
	}
}
