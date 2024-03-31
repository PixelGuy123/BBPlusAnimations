using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using System.Collections;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(MathMachineNumber))]
	internal class NumberBalloonPatch
	{
		[HarmonyPatch("Start")]
		[HarmonyPostfix]
		private static void SpawnAnimation(Transform ___sprite, AudioManager ___audMan)
		{
			if (!PropagatedAudioManager.paused)
			{
				___audMan.FlushQueue(true);
				___audMan.PlaySingle(sound);
				___sprite.localScale = Vector3.zero;
			}
		}

		[HarmonyPatch("Pop")]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) => // Removes the gameObject.SetActive(false) instruction
			new CodeMatcher(instructions)
			.MatchForward(false,
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(MathMachineNumber), "sprite")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), "gameObject")), // Component has the gameObject thingy, bruh
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), "SetActive", [typeof(bool)]))
				)

			.RemoveInstructions(5)
			.InstructionEnumeration();

		[HarmonyPatch("Pop")]
		private static void Prefix(MathMachineNumber __instance, ref Transform ___sprite, bool ___popping)
		{
			if (!___popping)
				__instance.StartCoroutine(Animation(___sprite.GetComponent<SpriteRenderer>()));
			
		}

		static IEnumerator Animation(SpriteRenderer renderer)
		{
			float frame = 0f;
			int idx = 0;
			while (idx < explodeVisuals.Length)
			{
				frame += 30f * Time.deltaTime * Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale;
				idx = Mathf.FloorToInt(frame);
				if (idx < explodeVisuals.Length)
					renderer.sprite = explodeVisuals[idx];
				yield return null;
			}

			renderer.gameObject.SetActive(false);
			yield break;
		}

		[HarmonyPatch("Update")]
		private static void Prefix(Transform ___sprite)
		{
			float scale = ___sprite.localScale.y; // Just base in y
			if (scale < 1f)
			{
				scale += (1f - scale) / 11f * 25f * Time.deltaTime * Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale;
				___sprite.localScale = Vector3.one * scale;
			}
		}

		internal static Sprite[] explodeVisuals;

		internal static SoundObject sound;
	}
}
