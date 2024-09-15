using HarmonyLib;
using PixelInternalAPI.Components;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Pickup), "Clicked", [typeof(int)])]
	[AnimationConditionalPatch("Points unique pickup", "If True, Points will have an unique animation when being picked up.")]
	internal class PickupPatches
	{
		static void Prefix(Pickup __instance, int player)
		{
			if (__instance.item.itemType != Items.Points) return; // ONLY POINTS ANIMATION!!
			var par = Object.Instantiate(particles);
			par.transform.position = __instance.transform.position;
			par.ec = Singleton<CoreGameManager>.Instance.GetPlayer(player).ec;
			par.GetComponent<ParticleSystemRenderer>().material.mainTexture = __instance.item.itemSpriteLarge.texture;
		}

		internal static TemporaryParticles particles;
	}
}
