using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(SwingDoor), "Lock", [typeof(bool)])]
	internal class SwingingDoorPatch
	{
		static void Prefix(AudioManager ___audMan) =>
			___audMan.PlaySingle(audLock);

		internal static SoundObject audLock;

	}
}
