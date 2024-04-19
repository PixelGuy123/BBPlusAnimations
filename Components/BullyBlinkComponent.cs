using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class BullyBlinkComponent : MonoBehaviour
	{
		private void Awake()
		{
			target = GetComponent<Bully>();
			bullyNormal = target.spriteRenderer[0].sprite;
		}

		void Update()
		{
			if (blinkTime > 0f)
			{
				blinkTime -= target.TimeScale * Time.deltaTime;
				if (blinkTime <= 0f)
				{
					target.spriteRenderer[0].sprite = bullyNormal;
					nonBlinkTime = Random.Range(0.5f, 1.5f);
				}
				return;
			}

			if (nonBlinkTime > 0f)
			{
				nonBlinkTime -= target.TimeScale * Time.deltaTime;
				return;
			}

			if (Random.value > 0.95d)
			{
				blinkTime = Random.Range(0.1f, 0.15f);
				target.spriteRenderer[0].sprite = bullyBlink;
			}
			
		}

		float blinkTime = 0f;
		float nonBlinkTime = 0f;

		[SerializeField]
		Bully target;

		[SerializeField]
		Sprite bullyNormal;

		internal static Sprite bullyBlink;
	}
}
