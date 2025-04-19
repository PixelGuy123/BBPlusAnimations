using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	// SodaMachine animation
	[HarmonyPatch(typeof(SodaMachine), "InsertItem")]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_VENDINGMACHINE_ANIMATION, ConfigEntryStorage.DESC_VENDINGMACHINE_ANIMATION)]
	internal static class ExpansionAnimation_VendingMachine
	{
		private static void Postfix(SodaMachine __instance, PlayerManager pm) =>
			__instance.StartCoroutine(GenericOffsetAnimation(__instance.transform, pm, 1.4f, 1.1f, 4f, 2f));

		internal static IEnumerator GenericOffsetAnimation(Transform m, PlayerManager pm, float offsetx, float offsety, float speedx, float speedy, Sprite replacement = null, Sprite ogSprite = null)
		{
			SpriteRenderer spriter = null;
			if (replacement)
			{
				spriter = m.GetComponent<SpriteRenderer>();
				spriter.sprite = replacement;
			}
			float scalex = 1f;
			float scaley = 1f;
			while (true)
			{
				scalex += speedx * pm.ec.EnvironmentTimeScale * Time.deltaTime;
				scaley += speedy * pm.ec.EnvironmentTimeScale * Time.deltaTime;
				if (scaley > offsety)
					scaley = offsety;
				if (scalex >= offsetx)
					break;
				m.localScale = new Vector3(scalex, scaley, 1f);
				yield return null;
			}
			yield return null;
			m.localScale = new Vector3(offsetx, offsety, 1f);
			scalex = offsetx;
			scaley = offsety;
			while (true)
			{
				scalex -= speedx * pm.ec.EnvironmentTimeScale * Time.deltaTime;
				scaley -= speedy * pm.ec.EnvironmentTimeScale * Time.deltaTime;
				if (scaley < 1f)
					scaley = 1f;
				if (scalex < 1f)
					break;
				m.localScale = new Vector3(scalex, scaley, 1f);
				yield return null;
			}
			m.localScale = Vector3.one;
			if (replacement && spriter != null)
				spriter.sprite = ogSprite;
		}
	}

	// TapePlayer animation
	[HarmonyPatch(typeof(TapePlayer), "InsertItem")]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_TAPEPLAYER_INSERT_ANIMATION, ConfigEntryStorage.DESC_TAPEPLAYER_INSERT_ANIMATION)]
	internal static class ExpansionAnimation_TapePlayer
	{
		private static void Postfix(TapePlayer __instance, PlayerManager player) =>
			__instance.StartCoroutine(ExpansionAnimation_VendingMachine.GenericOffsetAnimation(__instance.transform.Find("Sprite"), player, 1.2f, 1.2f, 3.5f, 3.5f));
	}

	// WaterFountain animation
	[HarmonyPatch(typeof(WaterFountain), "Clicked")]
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_WATERFOUNTAIN_ANIMATION, ConfigEntryStorage.DESC_WATERFOUNTAIN_ANIMATION)]
	internal static class VendingMachineAnimation_WaterFountain
	{
		internal static Sprite spoutWater, normalSpout;
		private static void Postfix(WaterFountain __instance, int playerNumber)
		{
			var pm = Singleton<CoreGameManager>.Instance.GetPlayer(playerNumber);
			__instance.StartCoroutine(ExpansionAnimation_VendingMachine.GenericOffsetAnimation(__instance.transform.Find("FountainSpout"), pm, 1.05f, 1.3f, 1f, 2f, spoutWater, normalSpout));
		}
	}
}
