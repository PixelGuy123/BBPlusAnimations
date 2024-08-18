using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class CloudyCopterExtraComp : MonoBehaviour
	{
		[SerializeField]
		public ParticleSystem compToHold;

		public bool shouldBlow = false;
	}
}
