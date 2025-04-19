using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;
using System.Reflection.Emit;
using BBPlusAnimations.Components;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_ITEMS, ConfigEntryStorage.NAME_BOOTS_WALK, ConfigEntryStorage.DESC_BOOTS_WALK)]
	[HarmonyPatch(typeof(ITM_Boots), "Timer", MethodType.Enumerator)]
	internal static class ITMBootsPatch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(true, 
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldfld, name:"time"),
				new(OpCodes.Call, AccessTools.PropertyGetter(typeof(Time), "deltaTime")),
				new(OpCodes.Ldloc_1),
				new(CodeInstruction.LoadField(typeof(Item), "pm")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerManager), "PlayerTimeScale")),
				new(OpCodes.Mul),
				new(OpCodes.Sub)
				//new(OpCodes.Stfld, name:"time")
				)
			.Advance(2)
			.InsertAndAdvance(
				new(OpCodes.Ldloc_1),
				Transpilers.EmitDelegate<System.Action<ITM_Boots>>(x => x.GetComponent<BootsDistanceReach>()?.IncrementDistance(float.IsNaN(x.pm.plm.RealVelocity) ? 0f : x.pm.plm.RealVelocity / 25f))
				)
			.InstructionEnumeration();
	}
}
