using BBPlusAnimations.Components;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(HappyBaldi), "SpawnWait", MethodType.Enumerator)]
	internal class HappyBaldiPatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(true,
				new(OpCodes.Ldloc_1),
				new(CodeInstruction.LoadField(typeof(HappyBaldi), "sprite")),
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(Renderer), "enabled"))
				)
			.InsertAndAdvance(
				new CodeInstruction(OpCodes.Ldloc_1),
				Transpilers.EmitDelegate<System.Action<HappyBaldi>>((x) =>
				{
					if (baldi == null) return;

					if (Singleton<CoreGameManager>.Instance.currentMode == Mode.Free)
					{
						var b = Object.Instantiate(baldi);
						b.transform.position = x.transform.position;
						b.ec = x.Ec;
						b.gameObject.SetActive(true);
					}
				})
				)
			.InstructionEnumeration();

		internal static BaldiFloatsAway baldi;
	}
}
