using BBPlusAnimations.Components;
using HarmonyLib;
using PixelInternalAPI.Extensions;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ENVIRONMENT, ConfigEntryStorage.NAME_TAPEPLAYER_ANIMATION, ConfigEntryStorage.DESC_TAPEPLAYER_ANIMATION)]
	[HarmonyPatch(typeof(TapePlayer), "Cooldown", MethodType.Enumerator)]
	internal class TapePlayerPatch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i, ILGenerator gen) =>
			new CodeMatcher(i, gen)
			.End()
			.MatchBack(true, 
				new(OpCodes.Ldloc_1),
				new(CodeInstruction.LoadField(typeof(TapePlayer), "dijkstraMap")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(DijkstraMap), "Deactivate"))
				)
			.Advance(1)
			.InsertAnIfBlockAndAdvance(
				[
				new(OpCodes.Ldloc_1),
				CodeInstruction.LoadField(typeof(TapePlayer), "changeOnUse"), // Only reverse if there's a changeOnUse
				],
				OpCodes.Brfalse_S, // If block
				new(OpCodes.Ldloc_1),
				CodeInstruction.LoadField(typeof(TapePlayer), "spriteToChange"),

				new(OpCodes.Ldloc_1),
				CodeInstruction.Call(typeof(Component), "GetComponent", [], [typeof(TapePlayerReverser)]),
				CodeInstruction.LoadField(typeof(TapePlayerReverser), "spriteToReverse"),

				new(OpCodes.Call, AccessTools.PropertySetter(typeof(SpriteRenderer), "sprite"))
				)

			.InstructionEnumeration();
	}
}
