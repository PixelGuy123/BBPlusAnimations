using PixelInternalAPI.Components;
using HarmonyLib;
using static UnityEngine.Object;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(GravityEvent), "DestroyFlipper")]
	[AnimationConditionalPatch("Flipper explosion", "If True, gravity flippers will explode.")]
	internal static class GravityEventPatch
	{
		private static void Prefix(GravityFlipper flipper, ref EnvironmentController ___ec)
		{
			var p = Instantiate(particle);
			p.transform.position = flipper.transform.position;
			p.ec = ___ec;
		}

		internal static TemporaryParticles particle;
	}
}
