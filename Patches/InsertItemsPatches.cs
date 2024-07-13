using BBPlusAnimations.Components;
using HarmonyLib;
using PixelInternalAPI.Components;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch]
	internal class InsertItemPatches
	{
		[HarmonyPatch(typeof(ITM_Acceptable), "Use")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> PatchHand(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false, 
				new(OpCodes.Ldloc_2),
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldarg_1),
				new(CodeInstruction.LoadField(typeof(PlayerManager), "ec")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(IItemAcceptor), "InsertItem", [typeof(PlayerManager), typeof(EnvironmentController)]))
				)
			.InsertAndAdvance(new(OpCodes.Ldarg_1), Transpilers.EmitDelegate((PlayerManager x) => Singleton<CoreGameManager>.Instance.GetCamera(x.playerNumber).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Insert)))
			.InstructionEnumeration();

		[HarmonyPatch(typeof(ITM_AcceptableNoUse), "Use")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> PatchHand2(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false,
				new(OpCodes.Ldloc_3),
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldarg_1),
				new(CodeInstruction.LoadField(typeof(PlayerManager), "ec")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(IItemAcceptor), "InsertItem", [typeof(PlayerManager), typeof(EnvironmentController)]))
				)
			.InsertAndAdvance(new(OpCodes.Ldarg_1), Transpilers.EmitDelegate((PlayerManager x) => Singleton<CoreGameManager>.Instance.GetCamera(x.playerNumber).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Insert)))
			.InstructionEnumeration();

		[HarmonyPatch(typeof(ITM_Quarter), "Use")]
		[HarmonyPatch(typeof(ITM_SwingDoorLock), "Use")]
		[HarmonyPatch(typeof(ITM_Tape), "Use")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> PatchHand3(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false,
				new(OpCodes.Ldloc_0),
				new(OpCodes.Ldarg_1),
				new(OpCodes.Ldarg_1),
				new(CodeInstruction.LoadField(typeof(PlayerManager), "ec")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(IItemAcceptor), "InsertItem", [typeof(PlayerManager), typeof(EnvironmentController)]))
				)
			.InsertAndAdvance(new(OpCodes.Ldarg_1), Transpilers.EmitDelegate((PlayerManager x) => Singleton<CoreGameManager>.Instance.GetCamera(x.playerNumber).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Insert)))
			.InstructionEnumeration();

		[HarmonyPatch(typeof(ITM_PortalPoster), "Use")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> PatchHand5(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false,
				new(OpCodes.Ldarg_1),
				new(CodeInstruction.LoadField(typeof(PlayerManager), "ec")),
				new(OpCodes.Ldloc_2),
				new(OpCodes.Ldloc_0),
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(ITM_PortalPoster), "windowObject")),
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(EnvironmentController), "BuildWindow", [typeof(Cell), typeof(Direction), typeof(WindowObject), typeof(bool)]))
				)
			.InsertAndAdvance(new(OpCodes.Ldarg_1), Transpilers.EmitDelegate((PlayerManager x) => Singleton<CoreGameManager>.Instance.GetCamera(x.playerNumber).GetComponent<CameraHandUI>().PlayAnimation(CameraHandUI.AnimType.Insert)))
			.InstructionEnumeration();
	}
}
