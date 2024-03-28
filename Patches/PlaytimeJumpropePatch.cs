using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	internal class PlaytimeJumpropePatch
	{
		[HarmonyPatch(typeof(Jumprope), "Destroy")]
		private static bool Prefix(Jumprope __instance, Animator ___animator, Canvas ___ropeCanvas, ref MovementModifier ___moveMod, Canvas ___textCanvas)
		{
			__instance.player.plm.am.moveMods.Remove(___moveMod);
			__instance.player.jumpropes.Remove(__instance); // I HATE YOU, SCISSORS FOR CRASHING MY GAME
			___animator.enabled = false;
			__instance.StartCoroutine(Animation(__instance, ___ropeCanvas.transform.Find("Jumprope_0").GetComponent<SpriteRenderer>(), ___ropeCanvas, ___textCanvas));
			return false; // Nuh uh
		}

		static IEnumerator Animation(Jumprope j, SpriteRenderer sprite, Canvas ropeCanvas, Canvas textCanvas)
		{
			int idx = 0;
			float frames = 0f;
			while (idx < sprites.Length)
			{
				frames += 31f * j.player.ec.NpcTimeScale * Time.deltaTime;
				idx = Mathf.FloorToInt(frames);
				if (idx < sprites.Length)
					sprite.sprite = sprites[idx];
				yield return null;
			}

			Singleton<CoreGameManager>.Instance.GetCamera(j.player.playerNumber).UpdateTargets(null, 24);
			j.player.jumpropes.Remove(j);
			Object.Destroy(textCanvas.gameObject);
			Object.Destroy(ropeCanvas.gameObject);
			Object.Destroy(j.gameObject);


			yield break;
		}

		internal static Sprite[] sprites;
	}
}
