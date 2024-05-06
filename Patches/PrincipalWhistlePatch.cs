using HarmonyLib;
using System.Collections.Generic;
using System.Collections;
using System.Reflection.Emit;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_PrincipalWhistle), "Use")]
	internal class PrincipalWhistlePatch // Completely overwrites 
	{
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> NoDestroyObjects(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)

			.MatchForward(false, 
				new(OpCodes.Ldarg_0),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Component), "gameObject")),
				new(CodeInstruction.Call(typeof(Object), "Destroy", [typeof(Object)]))
				)
			.RemoveInstructions(3) // Remove the destroy call
			.InstructionEnumeration();

		[HarmonyPrefix]
		static void WhistleAnimation(PlayerManager pm, ITM_PrincipalWhistle __instance)
		{
			var canvas = __instance.transform.Find("PrincipalCanvas");
			canvas.gameObject.SetActive(true);
			canvas.GetComponent<Canvas>().worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(pm.playerNumber).canvasCam;
			__instance.StartCoroutine(WhistleAnimation(pm, canvas.transform.Find("Image"), __instance));
		}
		

		static IEnumerator WhistleAnimation(PlayerManager pm, Transform canvas, ITM_PrincipalWhistle whistle)
		{
			
			Vector3 ogPos = canvas.localPosition + Vector3.down * 10f;
			Vector3 pos = ogPos + Vector3.down * downProfundity;

			while (pos.y < ogPos.y - 0.01f)
			{
				pos.y += (ogPos.y - pos.y) * pm.ec.EnvironmentTimeScale * 16f * Time.deltaTime;
				canvas.localPosition = pos + (new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f)) * Mathf.Clamp01(Time.timeScale));

				yield return null;
			}

			float cooldown = 1.4f;
			while (cooldown > 0f)
			{
				canvas.localPosition = pos + new Vector3(Random.Range(-3f, 3f), Random.Range(-3f, 3f), Random.Range(-3f, 3f)) * Mathf.Clamp01(Time.timeScale);
				cooldown -= pm.ec.EnvironmentTimeScale * Time.deltaTime;
				yield return null;
			}
			float time = 0f;
			float speed = 0f;
			ogPos += Vector3.down * downProfundity;
			float y = pos.y;

			while (true)
			{
				speed += 0.8f * pm.ec.EnvironmentTimeScale * Time.deltaTime;
				time += speed;
				if (time >= 1f)
					break;

				pos.y = Mathf.Lerp(y, ogPos.y, time);
				canvas.localPosition = pos;
				yield return null;
			}

			Object.Destroy(whistle.gameObject);

			yield break;
		}

		const float downProfundity = 100f;
	}
}
