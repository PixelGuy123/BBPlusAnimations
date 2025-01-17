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
			float speed;
			Sprite[] sprs;
			switch (type)
			{
				case AnimType.Pickup:
					sprs = sprsPickup;
					speed = speedForPickup;
					break;
				default: throw new System.ArgumentException("The AnimType enum is invalid.");
			};
			currentAnim = StartCoroutine(Animator(sprs, speed));
		}


		IEnumerator Animator(Sprite[] sprs, float speed)
		{
			img.enabled = true;			
			float frame = 0f;
			while (true)
			{
				frame += Time.deltaTime * speed;
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
		internal Sprite[] sprsPickup;

		[SerializeField]
		internal float speedForPickup = 22.5f;

		[SerializeField]
		internal Image img = null;

		[SerializeField]
		internal Canvas canvas = null;

		public enum AnimType
		{
			Pickup
		}
	}
}
