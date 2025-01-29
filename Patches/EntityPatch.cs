using BBPlusAnimations.Components;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Entity unsquish animation", "If True, entities (npcs and player) will have an animation to indicate when they are unsquishing.")]
	[HarmonyPatch(typeof(Entity))]
	internal class EntityPatch
	{
		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		[HarmonyPatch("UpdateTriggerState")]
		private static void UpdateTrigger(object instance) =>
			throw new NotImplementedException("This is a stub");

		[HarmonyPatch("Awake")]
		[HarmonyPrefix]
		static void AniamtionComponentAddition(Entity __instance) =>
			__instance.gameObject.AddComponent<GenericAnimationExtraComponent>();

		[HarmonyPatch("Update")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> GetUnsquishWorking(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.End()
			.MatchBack(false,
				new CodeMatch(CodeInstruction.Call(typeof(Entity), "Unsquish"))
				)
			.SetInstruction(Transpilers.EmitDelegate((Entity instance) =>
			{
				if (instance.BaseHeight >= instance.InternalHeight)
				{
					instance.Unsquish();
					return;
				}
				instance.squishTime = unsquishingAnimationLimit + 15f;
				var co = instance.GetComponent<GenericAnimationExtraComponent>();
				if (co.runningAnimation != null)
					instance.StopCoroutine(co.runningAnimation);
				co.runningAnimation = instance.StartCoroutine(Animation(instance));
			}))
			.InstructionEnumeration();

		[HarmonyPatch("Squish")]
		[HarmonyPrefix]
		static void AntecipateTimer(Entity __instance, ref float time)
		{
			time -= unsquishingAnimationLimit;
			var co = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (co.runningAnimation != null)
				__instance.StopCoroutine(co.runningAnimation);
		}


		static IEnumerator Animation(Entity entity)
		{
			float timer = unsquishingAnimationLimit;
			float factor = 0f;
			float baseFactor = entity.BaseHeight / entity.InternalHeight;
			var ec = entity.environmentController;

			while (true)
			{
				timer -= ec.EnvironmentTimeScale * Time.deltaTime;
				if (timer <= 0f)
					break;
				factor = baseFactor + Mathf.Abs(Mathf.Sin((unsquishingAnimationLimit - timer) * 7f) * 0.12f);
				entity.SetVerticalScale(factor);
				yield return null;
			}
			yield return null;

			while (true)
			{
				factor += (1f - factor) * 8.333f * Time.deltaTime * ec.EnvironmentTimeScale;
				if (factor >= 0.98f)
					break;

				entity.SetVerticalScale(factor);
				yield return null;
			}

			yield return null;

			entity.Unsquish();
			yield break;
		}



		const float unsquishingAnimationLimit = 4f;
	}
}
