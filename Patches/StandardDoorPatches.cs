using BBPlusAnimations.Components;
using HarmonyLib;
using PixelInternalAPI.Extensions;
using System.Collections.Generic;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(StandardDoor))]
	public class StandardDoorPatches
	{
		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		private static void Setup(StandardDoor __instance)
		{
			var comp = __instance.GetComponent<StandardDoorExtraMaterials>();
			if (comp == null) return;

			
			Material mat = new(__instance.overlayShut[0]);
			var tex = (Texture2D)mat.mainTexture;
			int idx = invertedTextures.Contains(tex.name) ? 1 : 0;
			if (storedTextures.ContainsKey(tex.name)) // Basically cache the textures and re-use them when replaying the game
			{
				var t = storedTextures[tex.name];
				t.OverlayTexture(comp.defaultTex[idx]);
				mat.mainTexture = t;
			}
			else
			{
				var readable = tex.MakeReadableTexture();
				readable.OverlayTexture(comp.defaultTex[idx]);
				storedTextures.Add(tex.name, readable);
				mat.mainTexture = readable;
			}

			Material mat2 = new(__instance.overlayShut[1]);
			tex = (Texture2D)mat2.mainTexture;
			idx = invertedTextures.Contains(tex.name) ? 1 : 0;

			if (storedTextures.ContainsKey(tex.name))
			{
				var t = storedTextures[tex.name];
				t.OverlayTexture(comp.defaultTex[idx]);
				mat2.mainTexture = t;
			}
			else
			{
				var readable = tex.MakeReadableTexture();
				readable.OverlayTexture(comp.defaultTex[idx]);
				storedTextures.Add(tex.name, readable);
				mat2.mainTexture = readable;
			}

			comp.doorLockedMat = [mat, mat2];
		}

		[HarmonyPatch("Lock")]
		[HarmonyPrefix]
		private static void LockOverride(StandardDoor __instance)
		{
			var comp = __instance.GetComponent<StandardDoorExtraMaterials>();
			if (comp == null) return;

			var texs = __instance.overlayShut;
			__instance.overlayShut = comp.doorLockedMat;
			__instance.UpdateTextures();
			__instance.overlayShut = texs;
		}

		[HarmonyPatch("Unlock")]
		[HarmonyPrefix]
		private static void UnlockOverride(StandardDoor __instance) => __instance.UpdateTextures(); // Just update it back anyways

		static readonly Dictionary<string, Texture2D> storedTextures = [];

		static readonly HashSet<string> invertedTextures = ["DoorTexture_Closed", "Principal_Closed", "Supplies_Closed"]; // Useful for mods that have the door using the left orietantion

		public static void AddDoorTextureName(string name) => invertedTextures.Add(name);
	}
}
