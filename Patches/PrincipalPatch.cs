using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using System.Collections;

namespace BBPlusAnimations.Patches
{
    [AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_PRINCIPAL_WHISTLE_ANIMATION, ConfigEntryStorage.DESC_PRINCIPAL_WHISTLE_ANIMATION)]
	[HarmonyPatch(typeof(Principal))]
	internal static class PrincipalWhistlePatch
	{
		[HarmonyPatch("WhistleChance")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> Animation(IEnumerable<CodeInstruction> instructions) =>
			new CodeMatcher(instructions)
			.MatchForward(false,
				new CodeMatch(OpCodes.Ldarg_0),
				new CodeMatch(CodeInstruction.LoadField(typeof(Principal), "audMan")),
				new CodeMatch(OpCodes.Ldarg_0),
				new CodeMatch(CodeInstruction.LoadField(typeof(Principal), "audWhistle")),
				new CodeMatch(OpCodes.Callvirt, AccessTools.Method("AudioManager:PlaySingle", [typeof(SoundObject)]))
				)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Principal), "audMan"),
				Transpilers.EmitDelegate<System.Action<Principal, AudioManager>>((pri, audMan) => pri.Navigator.Entity.StartCoroutine(Animation(pri, audMan))))
			.InstructionEnumeration();

		static IEnumerator Animation(Principal p, AudioManager man)
		{
			bool turn = false;
			float scale = 1f;
			var target = p.spriteRenderer[0].transform;
			yield return null;

			while (man.QueuedAudioIsPlaying)
			{
				if (!turn)
				{
					scale += p.TimeScale * Time.deltaTime;
					if (scale >= 1.2f)
					{
						scale = 1.2f;
						turn = true;
					}
				}
				else
				{
					scale -= p.TimeScale * Time.deltaTime;
					if (scale <= 1f)
					{
						turn = false;
						scale = 1f;
					}
				}

				target.localScale = Vector3.one * scale;
				yield return null;
			}

			while (scale >= 1f)
			{
				scale -= p.TimeScale * Time.deltaTime;
				if (scale <= 1f)
					break;
				target.localScale = Vector3.one * scale;
				yield return null;
			}

			target.localScale = Vector3.one;
			yield break;
		}
	}

	[AnimationConditionalPatch(
		ConfigEntryStorage.CATEGORY_NPCs,
		ConfigEntryStorage.NAME_PRINCIPAL_DETENTION_ANIMATION,
		ConfigEntryStorage.DESC_PRINCIPAL_DETENTION_ANIMATION)]
	[HarmonyPatch(typeof(Principal))]
	internal static class PrincipalDetentionPatch
	{
		[HarmonyPatch("SendToDetention")]
		[HarmonyPrefix]
		static void CoolDetentionAnimation(Principal __instance)
		{
			if (__instance.ec.offices.Count > 0)
				__instance.StartCoroutine(DetentionAnimation(__instance.spriteRenderer[0], __instance));
		}

		static IEnumerator DetentionAnimation(SpriteRenderer renderer, Principal p)
		{
			float time = 3f;
			while (time > 0f)
			{
				time -= p.TimeScale * Time.deltaTime;
				renderer.sprite = sprites[1 + (Mathf.FloorToInt(Time.fixedTime * 8f * p.TimeScale) % (sprites.Length - 1))];
				yield return null;
			}
			renderer.sprite = sprites[0];
			yield break;
		}

		internal static Sprite[] sprites;
	}
}
