using System.Collections;
using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class PlantAnimator : MonoBehaviour
	{

		private void OnTriggerEnter(Collider other)
		{
			if (other.isTrigger && other.GetComponent<Entity>()) // IsTrigger because I forgor there are 2 triggers
			{
				if (animation != null)
					StopCoroutine(animation);
				animation = StartCoroutine(Animation());

				audMan.PlaySingle(aud_bushes);
				for (int i = 0; i < particles.Length; i++)
					particles[i].Emit(Random.Range(25, 35));
				
			}
		}


		private void Update()
		{
			if (Singleton<BaseGameManager>.Instance)
			{
				second += Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale * Time.deltaTime;
				if (second >= 1f)
				{
					do
					{
						second -= 1f;
						for (int i = 0; i < particles.Length; i++)
						{
							var particle = particles[i];
							var e = particle.emission;
							e.rateOverTimeMultiplier = Random.Range(minOverTime, maxOverTime);
						}
					} while (second >= 1f);
				}
			}
		}

		IEnumerator Animation()
		{
			float frame = 0f;
			int idx;
			while (true)
			{
				frame += 15 * Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale * Time.deltaTime;
				idx = Mathf.FloorToInt(frame);
				if (idx < sprites.Length)
					renderer.sprite = sprites[idx];
				else break;
				yield return null;
			}

			renderer.sprite = sprites[0];

			yield break;
		}

		private float second = 1f; // Start by updating already

		private Coroutine animation = null;

		[SerializeField]
		public ParticleSystem[] particles = [];

		[SerializeField]
		public float minOverTime = 0.1f, maxOverTime = 2f;

		[SerializeField]
		public SoundObject aud_bushes;

		[SerializeField]
		public PropagatedAudioManager audMan;

		[SerializeField]
		public SpriteRenderer renderer;

		[SerializeField]
		public Sprite[] sprites;
	}
}
