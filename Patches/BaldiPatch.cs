using System.Collections;
using HarmonyLib;
using UnityEngine;

namespace BBPlusAnimations.Patches; 
// FIX BALDI APPLE INTERACTION BREAKING BALDI
	
[AnimationConditionalPatch(ConfigEntryStorage.CATEGORY_NPCs, ConfigEntryStorage.NAME_BALDI_PEEK_LOCKER, ConfigEntryStorage.DESC_BALDI_PEEK_LOCKER)]
[HarmonyPatch]
internal static class BaldiPatch_PeekInside{
	[HarmonyPatch(typeof(HideableLockerBaldiInteraction), "Trigger")]
	[HarmonyPostfix]
	static void ForceBaldiAlignment(Baldi baldi, Transform ___targetTransform, HideableLocker ___locker){
		baldi.StartCoroutine(
			BaldiPeek(
				baldi, 
				(___targetTransform.position - ___targetTransform.forward * 2.15f).ZeroOutY(),
				___locker
				)
			);
	}

	[HarmonyPatch(typeof(Baldi_OpenLocker), "Enter")]
	[HarmonyPrefix]
	static bool AvoidBaldiPraise(Baldi ___baldi){
		___baldi.Navigator.SetSpeed(0f);
		return false;
	}


	static IEnumerator BaldiPeek(Baldi bal, Vector3 expectedPosition, HideableLocker locker){
		bal.volumeAnimator.enabled = false;
		bal.animator.enabled = false;
		float frame = 0f;

		yield return null;
		bal.Navigator.Entity.Teleport(expectedPosition);
		Vector3 ogRendererPosition = bal.spriteRenderer[0].transform.localPosition;
		bal.spriteRenderer[0].transform.localPosition = Vector3.up;

		while (bal.transform.position.ZeroOutY() == expectedPosition && locker.playerInside && bal.behaviorStateMachine.currentState is Baldi_OpenLocker){
			frame += Time.deltaTime * fixedSpeed * bal.TimeScale;
			if (frame >= bal_peek_sprites.Length){
				break;
			}
			bal.spriteRenderer[0].sprite = bal_peek_sprites[Mathf.FloorToInt(frame)];
			yield return null;
		}
		bal.spriteRenderer[0].transform.localPosition = ogRendererPosition;
		bal.animator.enabled = true;
	}

	internal static Sprite[] bal_peek_sprites;

	const float fixedSpeed = 13.25f;

}

