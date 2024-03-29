using HarmonyLib;
using PixelInternalAPI.Components;
using UnityEngine;
using static UnityEngine.Object;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine.UI;
using PixelInternalAPI.Extensions;

namespace BBPlusAnimations.Patches
{
	[HarmonyPatch(typeof(Gum))]
	internal class GumSplash
	{
		[HarmonyPatch("OnEntityMoveCollision")]
		[HarmonyPrefix]
		private static void AnimationPre(out bool __state, bool ___flying) => // Basically trigger .Hide() after the gum is properly disabled
			__state = ___flying;

		[HarmonyPatch("OnEntityMoveCollision")]
		[HarmonyPostfix]
		private static void TriggerAnimation(bool __state, Gum __instance, ref RaycastHit hit)
		{
			if (__state && hit.transform.gameObject.layer != 2)
			{
				__instance.Hide();
				var gum = Instantiate(gumSplash);
				gum.transform.position = hit.transform.position - __instance.transform.forward * 0.03f;
				gum.transform.rotation = Quaternion.Euler(0f, (__instance.transform.rotation.eulerAngles.y + 180f) % 360f, 0f); // Quaternion.Inverse doesn't reverse y with 180 and 0 angles. Wth
				gum.transform.localScale = Vector3.zero;
				gum.gameObject.SetActive(true);
				gum.GetComponent<EmptyMonoBehaviour>().StartCoroutine(Timer(gum, 10f, __instance.ec));
			}
		}


		[HarmonyPatch("Initialize")]
		[HarmonyPostfix]
		private static void SetAudio(ref SoundObject ___audSplat) => ___audSplat = splash;

		static IEnumerator Timer(Transform target, float cooldown, EnvironmentController ec)
		{
			float sizeSpeed = 0f;
			float size = 0;
			while (true)
			{
				sizeSpeed += 0.6f * Time.deltaTime * ec.EnvironmentTimeScale;
				size += sizeSpeed;
				if (size >= 1.01f)
					break;
				target.localScale = Vector3.one * size;
				yield return null;
			}
			target.localScale = Vector3.one;
			size = 1;

			float c = cooldown;
			while (c > 0f)
			{
				c -= Time.deltaTime * ec.EnvironmentTimeScale;
				yield return null;
			}

			sizeSpeed = 0f;
			while (true)
			{
				sizeSpeed += 0.5f * Time.deltaTime * ec.EnvironmentTimeScale;
				size -= sizeSpeed;
				if (size <= 0f)
					break;
				target.localScale = Vector3.one * size;
				yield return null;
			}
			Destroy(target.gameObject);

			yield break;
		}


		internal static Transform gumSplash;
		internal static SoundObject splash;

		[HarmonyPatch("EntityTriggerEnter")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> AnimationUponPlayerHit(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false, 
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(Gum), "beans")),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(Beans), "HitPlayer"))
				)
			.InsertAndAdvance(
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Gum), "canvas"),
				Transpilers.EmitDelegate<System.Action<Gum, Canvas>>((x, y) => x.StartCoroutine(OverlayAnimation(y, x.ec)))
				)
			.InstructionEnumeration();

		[HarmonyPatch("Timer", MethodType.Enumerator)]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> DeleteThatCutVariable(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(false, 
				new(OpCodes.Ldloc_1),
				new(OpCodes.Ldc_I4_0), 
				new(CodeInstruction.StoreField(typeof(Gum), "cut"))
				)
			.SetAndAdvance(OpCodes.Nop, null)
			.SetAndAdvance(OpCodes.Nop, null) // AGAIN THIS NOP, WHY CAN'T I REMOVE INSTRUCTIONS
			.SetAndAdvance(OpCodes.Nop, null)
			.InstructionEnumeration();

		[HarmonyPatch("Hide")]
		[HarmonyTranspiler]
		private static IEnumerable<CodeInstruction> HideGumCover(IEnumerable<CodeInstruction> i, ILGenerator g) =>
		
			new CodeMatcher(i, g)
			.Start()
			.InsertAndAdvance()

			.MatchForward(false, 
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(Gum), "entity")),
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(Entity), "SetActive", [typeof(bool)]))
				)
			.RemoveInstructions(4) // Remove this to not disable the entity before the 

				.MatchForward(false,
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(Gum), "canvas")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), "gameObject")),
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), "SetActive", [typeof(bool)]))
				)
			.RemoveInstructions(5) // Remove them
			.InsertAndAdvance( // Animation for canvas
				new(OpCodes.Ldarg_0),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Gum), "canvas"),
				new(OpCodes.Ldarg_0),
				CodeInstruction.LoadField(typeof(Gum), "entity"),
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(Gum), "cut")),
				Transpilers.EmitDelegate<System.Action<Gum, Canvas, Entity, bool>>((x, y, z, u) => x.StartCoroutine(OverlayDisappearAnimation(y, x.ec, z, u)))
				) // Now add new one
			.InstructionEnumeration();
		

		[HarmonyPatch("Hide")]
		[HarmonyPostfix]
		private static void SetCutFalse(ref bool ___cut) =>
			___cut = false;
		

		static IEnumerator OverlayAnimation(Canvas c, EnvironmentController ec)
		{
			var img = c.GetComponentInChildren<Image>();
			img.sprite = sprites[0];

			float cooldown = 2f;
			while (cooldown > 0f)
			{
				cooldown -= ec.EnvironmentTimeScale * Time.deltaTime;
				yield return null;
			}
			
			float frame = 0f;
			int idx;
			while (true)
			{
				frame += 9f * ec.EnvironmentTimeScale * Time.deltaTime;
				idx = Mathf.FloorToInt(frame);
				if (idx < sprites.Length)
					img.sprite = sprites[idx];
				else
					break;

				yield return null;
			}

			yield break;

		}

		static IEnumerator OverlayDisappearAnimation(Canvas ca, EnvironmentController ec, Entity e, bool beenCut)
		{
			if (beenCut)
			{
				ca.gameObject.SetActive(false);
				e.SetActive(false);
				yield break;
			}
			var c = ca.GetComponentInChildren<Image>().GetComponent<CanvasRenderer>(); // Get the renderer from it
			while (true)
			{
				var co = c.GetColor();
				co.a -= ec.EnvironmentTimeScale * Time.deltaTime;
				if (co.a <= 0f)
					break;

				c.SetColor(co);
				
				yield return null;
			}

			ca.gameObject.SetActive(false);

			var co2 = c.GetColor();
			co2.a = 1f;
			c.SetColor(co2);

			e.SetActive(false);

			yield break;
		}

		internal static Sprite[] sprites;
	}
}
