using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(FieldTripManager), "End")]
	internal class FieldTripManagerPatch
	{
		private static void Postfix(ref AudioManager ___baldiMan, int rank)
		{
			if (fieldTripYay != null && rank >= 1) // 3 stars
				___baldiMan.PlaySingle(fieldTripYay);
		}
		internal static SoundObject fieldTripYay;
	}
}
