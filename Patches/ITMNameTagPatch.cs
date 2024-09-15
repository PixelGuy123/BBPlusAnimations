using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(ITM_Nametag))]
	[AnimationConditionalPatch("Nametag slide", "If True, nametags will slide from below to the screen and vice-versa.")]
	internal class ITMNameTagPatch
	{
		[HarmonyPatch("Use")]
		private static void Prefix(ITM_Nametag __instance, Canvas ___canvas, PlayerManager pm) =>
			__instance.StartCoroutine(FadeInAnimation(___canvas.GetComponentInChildren<Image>().transform, pm.ec));
		

		static IEnumerator FadeInAnimation(Transform t, EnvironmentController ec)
		{
			var ogPos = t.localPosition;
			var modifiedPos = ogPos;
			modifiedPos.y -= 112;
			t.localPosition = modifiedPos;
			while (true)
			{
				modifiedPos.y += speed * ec.EnvironmentTimeScale * Time.deltaTime;
				if (modifiedPos.y > ogPos.y)
					break;
				t.localPosition = modifiedPos;
				yield return null;
			}
			t.localPosition = ogPos;
			yield break;
		}

		static IEnumerator FadeOutAnimation(ITM_Nametag ta, Transform t, EnvironmentController ec)
		{
			var ogPos = t.localPosition + Vector3.down * 112;
			var modifiedPos = t.localPosition;
			while (true)
			{
				modifiedPos.y -= speed * ec.EnvironmentTimeScale * Time.deltaTime;
				if (modifiedPos.y < ogPos.y)
					break;
				t.localPosition = modifiedPos;
				yield return null;
			}

			Object.Destroy(ta.gameObject);

			yield break;
		}

		[HarmonyPatch("Timer", MethodType.Enumerator)]
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.End()
			.MatchBack(false,
				new(OpCodes.Ldloc_1),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Component), "gameObject")),
				new(CodeInstruction.Call(typeof(Object), "Destroy", [typeof(Object)])),
				new(OpCodes.Ldc_I4_0)
				)
			.RemoveInstructions(3)
			.InsertAndAdvance(
				new(OpCodes.Ldloc_1),
				new(OpCodes.Ldloc_1),
				CodeInstruction.LoadField(typeof(ITM_Nametag), "canvas"), // Basically a coroutine call with 3 fields lol
				new(OpCodes.Ldloc_1),
				CodeInstruction.LoadField(typeof(Item), "pm"),
				CodeInstruction.LoadField(typeof(PlayerManager), "ec"),
				Transpilers.EmitDelegate<System.Action<ITM_Nametag, Canvas, EnvironmentController>>((x, y, z) => x.StartCoroutine(FadeOutAnimation(x, y.GetComponentInChildren<Image>().transform, z)))
				)
			.InstructionEnumeration();

		const float speed = 65f;
	}
}
