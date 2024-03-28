using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using UnityEngine.UI;


namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(LookAtGuy_Active), "OnStateTriggerEnter")]
	internal class TheTestPatch // Literally an animation for the test blinding player
	{
		private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> insts) =>
			new CodeMatcher(insts)
			.MatchForward(true, 
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(LookAtGuy_BaseState), "theTest")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(LookAtGuy), "Blind", []))
			)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_1),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(Component), "GetComponent", [], [typeof(PlayerManager)])),
				Transpilers.EmitDelegate<System.Action<PlayerManager>>((x) => x.StartCoroutine(AnimatePlayerHud(x)))
			)
			.InstructionEnumeration();

		static IEnumerator AnimatePlayerHud(PlayerManager man)
		{

			canvas.worldCamera = Singleton<CoreGameManager>.Instance.GetCamera(man.playerNumber).canvasCam;
			canvas.gameObject.SetActive(true);
			img.sprite = sprites[0];

			float cooldown = 1f;
			while (cooldown > 0f)
			{
				cooldown -= man.ec.NpcTimeScale * Time.deltaTime;
				yield return null;
			}
			
			float frame = 0f;
			int idx;

			while (true)
			{
				frame += 20f * man.ec.NpcTimeScale * Time.deltaTime;
				idx = Mathf.FloorToInt(frame);
				if (idx < sprites.Length) img.sprite = sprites[idx];
				else break;

				yield return null;
			}

			canvas.gameObject.SetActive(false);

			yield break;
		}

		internal static Canvas canvas;
		internal static Image img;
		internal static Sprite[] sprites;
	}
}
