using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BBPlusAnimations.Components
{
	public class CameraHandUI : MonoBehaviour
	{
		void Start()
		{
			if (canvas)
			{
				canvas.worldCamera = GetComponent<GameCamera>().canvasCam;
				initialized = true;
			}
		}

		public void PlayAnimation(AnimType type)
		{
			if (!initialized) // If it is null, it's disabled and should be ignored
				return;

			if (currentAnim != null)
				StopCoroutine(currentAnim);
			Sprite[] sprs = type switch
			{
				AnimType.Pickup => sprsPickup,
				AnimType.Insert => sprsInsert,
				_ => throw new System.ArgumentException("The AnimType enum is invalid."),
			};
			currentAnim = StartCoroutine(Animator(sprs));
		}


		IEnumerator Animator(Sprite[] sprs)
		{
			img.enabled = true;			
			float frame = 0f;
			while (true)
			{
				frame += Time.deltaTime * 26f;
				if (frame >= sprs.Length)
					break;

				img.sprite = sprs[Mathf.FloorToInt(frame)];
				yield return null;
			}
			img.enabled = false;
			yield break;
		}

		Coroutine currentAnim = null;
		bool initialized = false;

		[SerializeField]
		internal Sprite[] sprsPickup, sprsInsert;

		[SerializeField]
		internal Image img = null;

		[SerializeField]
		internal Canvas canvas = null;

		public enum AnimType
		{
			Pickup,
			Insert
		}
	}
}
