using UnityEngine;

namespace BBPlusAnimations.Components
{
	internal class CloudyCopterExtraComp : MonoBehaviour
	{
		[SerializeField]
		public ParticleSystem compToHold;

		public bool shouldBlow = false;
	}
}
