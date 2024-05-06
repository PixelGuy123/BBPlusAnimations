using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class BaldiEatAppleComponent : MonoBehaviour
	{
		public void SetCooldownToDestroyParticles()
		{
			var done = particles.gameObject.AddComponent<DestroyWhenDone>();
			done.particles = particles;
		}

		[SerializeField]
		internal ParticleSystem particles;
	}
}
