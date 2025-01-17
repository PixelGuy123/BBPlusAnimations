using HarmonyLib;
using PixelInternalAPI.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Enable hand animation", "If True, hands will be displayed on screen when performing certain interactions.")]
	[HarmonyPatch(typeof(ElevatorScreen))]
	internal static class ElevatorScreenPatch
	{
		[HarmonyPatch("ButtonPressed")]
		[HarmonyReversePatch(HarmonyReversePatchType.Original)]
		static void StartGame(object instance, int val) =>
			throw new System.NotImplementedException("stub");

		[HarmonyPatch("Initialize")]
		[HarmonyPrefix]
		static void PlayAnimation(ElevatorScreen __instance)
		{
			if (prohibitedTypes.Contains(__instance.GetType()))
				return;

			var playButTransform = __instance.Canvas.transform.Find("Button");

			var img = ObjectCreationExtensions.CreateImage(__instance.Canvas, handAnim[0]);
			img.transform.localScale = Vector3.one;
			img.transform.SetSiblingIndex(playButTransform.GetSiblingIndex() + 1);
			img.enabled = false;
			img.name = "HandAnimation";

			var playBut = playButTransform.GetComponent<StandardMenuButton>();
			playBut.swapOnHold = false;

			playBut.OnPress = new();
			playBut.OnPress.AddListener(() =>
			{
				CursorController.Instance.Hide(true);
				__instance.StartCoroutine(Animation(img, __instance, playBut));
			});

			
		}

		static IEnumerator Animation(Image renderer, ElevatorScreen elv, StandardMenuButton but)
		{
			yield return null;
			Singleton<MusicManager>.Instance.soundSource.FlushQueue(true); // No no press sound!
			renderer.enabled = true;

			float frame = 0f;
			int max = handAnim.Length - 1;
			while (frame < max)
			{
				frame += speed * Time.unscaledDeltaTime;
				if (frame > max)
					frame = max;
				renderer.sprite = handAnim[Mathf.FloorToInt(frame)];

				yield return null;
			}

			Singleton<MusicManager>.Instance.PlaySoundEffect(but.audConfirmOverride);
			but.image.sprite = but.heldSprite;
			StartGame(elv, 0);

			frame = 0f;
			max = handAfterPressAnim.Length - 1;
			while (frame < max)
			{
				frame += speed * Time.unscaledDeltaTime;
				if (frame > max)
					frame = max;
				renderer.sprite = handAfterPressAnim[Mathf.FloorToInt(frame)];

				yield return null;
			}

			renderer.enabled = false;

			
		}


		internal static Sprite[] handAnim, handAfterPressAnim;

		public static void AddProhibitedElevatorType(System.Type t) => // If a mods wants to add some custom elevator screen, they can also call this
			prohibitedTypes.Add(t);

		static HashSet<System.Type> prohibitedTypes = [];

		const float speed = 24.5f;
	}
}
