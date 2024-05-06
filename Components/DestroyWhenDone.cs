using UnityEngine;

namespace BBPlusAnimations.Components
{
	internal class DestroyWhenDone : MonoBehaviour
	{
		void Update()
		{
			if (particles.particleCount <= 0)
				Destroy(particles.gameObject);
		}

		internal ParticleSystem particles;
	}
}
