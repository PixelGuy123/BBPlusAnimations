using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class PlantAnimator : MonoBehaviour
	{

		private void OnTriggerEnter(Collider other)
		{
			if ((other.CompareTag("NPC") || other.CompareTag("Player")) && other.isTrigger) // IsTrigger because I forgor there are 2 triggers
			{
				var e = other.GetComponent<Entity>();
				if (e)
				{
					second = 0.5f;
					for (int i = 0; i < particles.Length; i++) 
					{
						var em = particles[i].emission;
						em.rateOverTimeMultiplier = Random.Range(65f, 85f);
					}
					e.ExternalActivity.moveMods.Add(moveMod);
					audMan.PlaySingle(aud_bushes);
				}
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("NPC") || other.CompareTag("Player"))
				other.GetComponent<Entity>()?.ExternalActivity.moveMods.Remove(moveMod);
		}


		private void Update()
		{
			if (Singleton<BaseGameManager>.Instance)
			{
				second += Singleton<BaseGameManager>.Instance.Ec.EnvironmentTimeScale * Time.deltaTime;
				if (second >= 1f)
				{
					second = 0f;
					for (int i = 0; i < particles.Length; i++)
					{
						var particle = particles[i];
						var e = particle.emission;
						e.rateOverTimeMultiplier = Random.Range(minOverTime, maxOverTime);
					}
				}
			}
		}

		private float second = 1f; // Start by updating already

		readonly MovementModifier moveMod = new(Vector3.zero, 0.2f, 0);

		[SerializeField]
		public ParticleSystem[] particles = [];

		[SerializeField]
		public float minOverTime = 0.1f, maxOverTime = 1.5f;

		[SerializeField]
		public SoundObject aud_bushes;

		[SerializeField]
		public PropagatedAudioManager audMan;
	}
}
