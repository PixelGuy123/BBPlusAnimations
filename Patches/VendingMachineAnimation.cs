using HarmonyLib;
using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	internal class GenericAnimation
	{
		[HarmonyPatch(typeof(SodaMachine), "InsertItem")]
		private static void Postfix(SodaMachine __instance, PlayerManager pm) =>
			__instance.StartCoroutine(Animation(__instance.transform, pm, 1.4f, 1.1f, 4f, 2f));
		

		[HarmonyPatch(typeof(TapePlayer), "InsertItem")]
		private static void Postfix(TapePlayer __instance, PlayerManager player) =>
			__instance.StartCoroutine(Animation(__instance.transform.Find("Sprite"), player, 1.2f, 1.2f, 3.5f, 3.5f));
		

		[HarmonyPatch(typeof(WaterFountain), "Clicked")]
		private static void Postfix(WaterFountain __instance, int playerNumber) =>
			__instance.StartCoroutine(Animation(__instance.transform.Find("FountainSpout"), Singleton<CoreGameManager>.Instance.GetPlayer(playerNumber), 1.05f, 1.3f, 1f, 2f));
		




		static IEnumerator Animation(Transform m, PlayerManager pm, float offsetx, float offsety, float speedx, float speedy)
		{
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
			yield break;
		}
	}
}
