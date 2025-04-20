using BBPlusAnimations.Components;
using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
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
				new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Entity), "Unsquish"))
				)
			.SetInstruction(Transpilers.EmitDelegate((Entity instance) =>
			{
				var co = instance.GetComponent<GenericAnimationExtraComponent>();
				if (co && co.isActive)
					return;
				
				instance.squishTime = ConfigEntryStorage.CFG_ENTITY_UNSQUISH_TIME_LIMIT.Value;
				
				if (co)
				{
					if (co.runningAnimation != null)
						instance.StopCoroutine(co.runningAnimation);
					
					co.runningAnimation = instance.StartCoroutine(Animation(instance, co));
					co.isActive = true;
				}
				
			}))
			.InstructionEnumeration();

		[HarmonyPatch("Squish")]
		[HarmonyPrefix]
		static void AntecipateTimer(Entity __instance, ref float time)
		{
			time -= ConfigEntryStorage.CFG_ENTITY_UNSQUISH_TIME_LIMIT.Value;
			var co = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (co)
			{
				if (co.runningAnimation != null)
					__instance.StopCoroutine(co.runningAnimation);
				co.isActive = false;
			}
		}


		static IEnumerator Animation(Entity entity, GenericAnimationExtraComponent comp)
		{
			float timer = ConfigEntryStorage.CFG_ENTITY_UNSQUISH_TIME_LIMIT.Value;
			float factor = 0f;
			float baseFactor = entity.BaseHeight / entity.InternalHeight;
			var ec = entity.environmentController;

			while (true)
			{
				timer -= ec.EnvironmentTimeScale * Time.deltaTime;
				if (timer <= 0f)
					break;
				factor = baseFactor + Mathf.Abs(Mathf.Sin((ConfigEntryStorage.CFG_ENTITY_UNSQUISH_TIME_LIMIT.Value - timer) * 7f) * 0.12f);
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
			if (comp)
				comp.isActive = false;
			yield break;
		}
	}
}
