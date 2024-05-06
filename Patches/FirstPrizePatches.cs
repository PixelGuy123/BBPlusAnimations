using HarmonyLib;
using PixelInternalAPI.Components;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	public class FirstPrizePatches
	{
		[HarmonyPatch(typeof(FirstPrize_Stunned), "Enter")]
		[HarmonyPrefix]
		static void AddParticles(FirstPrize ___firstPrize)
		{
			var smoke = Object.Instantiate(smokes);
			smoke.transform.SetParent(___firstPrize.transform);
			smoke.transform.localPosition = Vector3.down;

			smoke.gameObject.SetActive(true);

		}

		[HarmonyPatch(typeof(FirstPrize_Stunned), "Exit")]
		[HarmonyPrefix]
		static void RemoveParticles(FirstPrize ___firstPrize) =>
			___firstPrize.GetComponentsInChildren<ParticleSystem>().Do(x => ___firstPrize.StartCoroutine(WaitForRemoval(x)));
		

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

		[HarmonyPatch(typeof(FirstPrize_Active), "Update")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> SaySorry(IEnumerable<CodeInstruction> i, ILGenerator ge) =>
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
				Transpilers.EmitDelegate<System.Action<Window, FirstPrize>>((w, f) =>
				{
					if (!w.broken)
						f.audMan.QueueAudio(audSorry, true);
					
				})
				)

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

						Vector3 pos = hit.transform.position - f.transform.forward * 0.03f;
						pos.y = f.transform.position.y;
						crack.transform.position = pos;

						crack.transform.rotation = f.transform.rotation;
						crack.gameObject.SetActive(true);
						crack.GetComponent<EmptyMonoBehaviour>().StartCoroutine(Timer(crack, 10f, f.ec));
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

		internal static SoundObject audSorry;

		internal static GameObject cracks;
	}
}
