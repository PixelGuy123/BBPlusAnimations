using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Apple tree drop", "If True, each time a tree is forced to drop an item, it will drop in a linear slide.")]
	[HarmonyPatch(typeof(AppleTree), "OnTriggerEnter")]
	internal class AppleTreePatch
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false, 
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(AppleTree), "apple")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), "transform")),
				new(OpCodes.Dup),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Transform), "position")),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Vector3), "up")),
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(AppleTree), "apple")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), "transform")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Transform), "position")),
				new(CodeInstruction.LoadField(typeof(Vector3), "y")),
				new(OpCodes.Ldc_R4, name:"5"),
				new(OpCodes.Sub),
				new(CodeInstruction.Call(typeof(Vector3), "op_Multiply", [typeof(Vector3), typeof(float)])),
				new(CodeInstruction.Call(typeof(Vector3), "op_Subtraction", [typeof(Vector3), typeof(Vector3)])),
				new(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(Transform), "position"))
				)
			.RemoveInstructions(16)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(AppleTree), "apple"),
				Transpilers.EmitDelegate<System.Action<AppleTree, Transform>>((x, y) => x.StartCoroutine(Animation(y)))
				)

			.InstructionEnumeration();

		static IEnumerator Animation(Transform apple)
		{
			float speed = 0f;
			float posOffset = 0f;
			Vector3 ogPos = apple.transform.position;
			while (true)
			{
				speed += Time.deltaTime * Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale;

				posOffset += speed;
				if (posOffset >= down - 0.01f)
					break;

				apple.transform.position = ogPos + Vector3.down * posOffset;
				yield return null;
			}
			posOffset = down;
			apple.transform.position = ogPos + Vector3.down * posOffset;
			yield break;
		}

		const float down = 5f;
	}
}
