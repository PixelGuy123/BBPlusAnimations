using HarmonyLib;
using System;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Entity unsquish animation", "If True, entities (npcs and player) will have an animation to indicate when they are unsquishing.")]
	[HarmonyPatch(typeof(Entity))]
	internal class EntityPatch
	{
		[HarmonyPatch("Unsquish")]
		[HarmonyPrefix]
		private static bool UnsquishAnimation(Entity __instance)
		{
			if (__instance.BaseHeight < __instance.InternalHeight)
			{
				__instance.StartCoroutine(Animation(__instance));
				return false;
			}

			return true;
		}

		[HarmonyPatch("Squish")]
		[HarmonyPrefix]
		static void AntecipateTimer(ref float time) =>
			time -= unsquishingAnimationLimit;
		

		static IEnumerator Animation(Entity entity)
		{
			float timer = unsquishingAnimationLimit;
			float factor = 0f;
			float baseFactor = entity.BaseHeight / entity.InternalHeight;
			float time = 0f;

			while (timer > 0f)
			{
				if (entity.Squished) // Check whether it got squished again
					yield break;
				timer -= Singleton<BaseGameManager>.Instance.Ec.NpcTimeScale * Time.deltaTime;
				time += Time.deltaTime;
				factor = baseFactor + Mathf.Abs(Mathf.Sin(time * Singleton<BaseGameManager>.Instance.Ec.NpcTimeScale * 7f) / 6f);
				entity.SetVerticalScale(factor);
				yield return null;
			}
			yield return null;

			while (true)
			{
				if (entity.Squished)
					yield break;
				factor += (1f - factor) / 3f * 25f * Time.deltaTime * Singleton<BaseGameManager>.Instance.Ec.NpcTimeScale;
				if (factor >= 0.98f)
					break;

				entity.SetVerticalScale(factor);
				yield return null;
			}

			yield return null;

			if (entity.Squished)
				yield break;

			entity.SetVerticalScale(1f);
			entity.squished = false;
			entity.squishTime = 0f;
			UpdateTrigger(entity);
			yield break;
		}

		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		[HarmonyPatch("UpdateTriggerState")]
		private static void UpdateTrigger(object instance) =>
			throw new NotImplementedException("This is a stub");

		const float unsquishingAnimationLimit = 4f;
	}
}
