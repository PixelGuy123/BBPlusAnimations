using HarmonyLib;
using PixelInternalAPI.Components;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_FIRSTPRIZE_SMOKE, ConfigEntryStorage.DESC_FIRSTPRIZE_SMOKE)]
	[HarmonyPatch]
	internal static class FirstPrizePatch_Smoking
	{
		[HarmonyPatch(typeof(FirstPrize_Stunned), "Enter")]
		[HarmonyPrefix]
		static void AddParticles(FirstPrize ___firstPrize)
		{
			var smoke = Object.Instantiate(smokes);
			smoke.transform.SetParent(___firstPrize.transform);
			smoke.transform.localPosition = Vector3.down;

			//smoke.gameObject.SetActive(true);

		}

		[HarmonyPatch(typeof(FirstPrize_Stunned), "Exit")]
		[HarmonyPrefix]
		static void RemoveParticles(FirstPrize ___firstPrize) =>
			___firstPrize.GetComponentsInChildren<ParticleSystem>()?.Do(x => ___firstPrize.StartCoroutine(WaitForRemoval(x)));


		static IEnumerator WaitForRemoval(ParticleSystem particle)
		{
			var emit = particle.emission;
			emit.rateOverTimeMultiplier = 0f;

			while (particle.particleCount > 0)
				yield return null;

			Object.Destroy(particle.gameObject);

			yield break;
		}


		internal static ParticleSystem smokes;
	}

	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_FIRSTPRIZE_WALLHIT, ConfigEntryStorage.DESC_FIRSTPRIZE_WALLHIT)]
	[HarmonyPatch]
	internal static class FirstPrizePatch_WallCrack
	{
		[HarmonyPatch(typeof(FirstPrize_Active), "Update")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> WallHit(IEnumerable<CodeInstruction> i, ILGenerator ge) =>
			new CodeMatcher(i, ge)

			.End()
			// *********** Wall Slam with cracks *************
			.MatchBack(false,
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(FirstPrize_StateBase), "firstPrize")),
				new(CodeInstruction.LoadField(typeof(FirstPrize), "audMan")),
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(FirstPrize_StateBase), "firstPrize")),
				new(CodeInstruction.LoadField(typeof(FirstPrize), "audBang")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(AudioManager), "PlaySingle", [typeof(SoundObject)]))
				)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(FirstPrize_StateBase), "firstPrize"),
				Transpilers.EmitDelegate<System.Action<FirstPrize>>(f =>
				{
					if (Physics.Raycast(f.transform.position, f.transform.forward, out var hit, 8f, 1, QueryTriggerInteraction.Collide))
					{
						var crack = Object.Instantiate(cracks);

						Vector3 pos = hit.transform.position + hit.normal * 0.05f; // Small normal offset to avoid z-fighting the wall
						pos.y = f.transform.position.y;
						crack.transform.position = pos;

						crack.transform.forward = hit.normal;
						crack.gameObject.SetActive(true);
						crack.GetComponent<EmptyMonoBehaviour>()?.StartCoroutine(Timer(crack, 10f, f.ec));
					}
				})
				)
			.InstructionEnumeration();


		static IEnumerator Timer(GameObject crack, float c, EnvironmentController ec)
		{
			float cooldown = c;
			while (cooldown > 0f)
			{
				cooldown -= ec.EnvironmentTimeScale * Time.deltaTime;
				yield return null;
			}

			var renderer = crack.GetComponentInChildren<SpriteRenderer>();
			renderer.material = new(renderer.material); // Don't affect other cracks with material
			Color co = renderer.color;
			while (co.a > 0f)
			{
				co.a -= ec.EnvironmentTimeScale * Time.deltaTime * 2f;
				renderer.color = co;
				yield return null;
			}

			Object.Destroy(crack);
			yield break;
		}

		internal static GameObject cracks;
	}

	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_FIRSTPRIZE_WINDOWHIT, ConfigEntryStorage.DESC_FIRSTPRIZE_WINDOWHIT)]
	[HarmonyPatch]
	internal static class FirstPrize_SorryWindowHit
	{
		static IEnumerable<CodeInstruction> WallHit(IEnumerable<CodeInstruction> i, ILGenerator ge) =>
			new CodeMatcher(i, ge)
			.End()
			// ************* Make Firstprize say Sorry **************
			.MatchBack(false,
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldflda, AccessTools.Field(typeof(FirstPrize_Active), "raycastHit")),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(RaycastHit), "transform")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "GetComponent", [], [typeof(Window)])),
				new(OpCodes.Ldc_I4_1),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(Window), "Break", [typeof(bool)]))
				)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldflda, AccessTools.Field(typeof(FirstPrize_Active), "raycastHit")),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(RaycastHit), "transform")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "GetComponent", [], [typeof(Window)])),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(FirstPrize_StateBase), "firstPrize"),
				Transpilers.EmitDelegate(BreaksWindow)
			)
			.InstructionEnumeration();

		internal static void BreaksWindow(Window w, FirstPrize prize)
		{
			if (!w.broken)
				prize.audMan.QueueAudio(audSorry, true);
		}

		internal static SoundObject audSorry;
	}
}
