using PixelInternalAPI.Components;
using HarmonyLib;
using static UnityEngine.Object;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(GravityEvent), "DestroyFlipper")]
	[AnimationConditionalPatch("Flipper explosion", "If True, gravity flippers will explode.")]
	internal class GravityEventPatch
	{
		private static void Prefix(GravityFlipper flipper, ref EnvironmentController ___ec)
		{
			var p = Instantiate(particle);
			p.transform.position = flipper.transform.position;
			p.ec = ___ec;
			//p.gameObject.SetActive(true);
		}

		internal static TemporaryParticles particle;
	}
}
