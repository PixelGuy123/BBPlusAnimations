using BBPlusAnimations.Components;
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
		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		[HarmonyPatch("UpdateTriggerState")]
		private static void UpdateTrigger(object instance) =>
			throw new NotImplementedException("This is a stub");

		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		[HarmonyPatch("Unsquish")]
		private static void Unsquish(object instance) =>
			throw new NotImplementedException("This is a stub");

		[HarmonyPatch("Awake")]
		[HarmonyPrefix]
		static void AniamtionComponentAddition(Entity __instance) =>
			__instance.gameObject.AddComponent<GenericAnimationExtraComponent>();

		[HarmonyPatch("Unsquish")]
		[HarmonyPrefix]
		private static bool UnsquishAnimation(Entity __instance, ref float ___squishTime)
		{
			if (__instance.BaseHeight >= __instance.InternalHeight)
				return true;
			___squishTime = unsquishingAnimationLimit + 1;
			var co = __instance.GetComponent<GenericAnimationExtraComponent>();
			if (co.runningAnimation != null)
				__instance.StopCoroutine(co.runningAnimation);
			co.runningAnimation = __instance.StartCoroutine(Animation(__instance));
			return false;
		}

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

			while (true)
			{
				timer -= Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale * Time.deltaTime;
				if (timer <= 0f)
					break;
				factor = baseFactor + Mathf.Abs(Mathf.Sin((unsquishingAnimationLimit - timer) * 7f) * 0.12f);
				entity.SetVerticalScale(factor);
				yield return null;
			}
			yield return null;

			while (true)
			{
				factor += (1f - factor) * 8.333f * Time.deltaTime * Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale;
				if (factor >= 0.98f)
					break;

				entity.SetVerticalScale(factor);
				yield return null;
			}

			yield return null;

			Unsquish(entity);
			yield break;
		}



		const float unsquishingAnimationLimit = 4f;
	}
}
