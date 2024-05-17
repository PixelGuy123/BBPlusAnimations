using HarmonyLib;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ItemManager), "RemoveItem")]
	internal class ItemManagerPatch
	{
		static void Prefix(ItemManager __instance, int val) =>
			lastRemovedItem = __instance.items[val];

		internal static ItemObject lastRemovedItem;
	}
}
