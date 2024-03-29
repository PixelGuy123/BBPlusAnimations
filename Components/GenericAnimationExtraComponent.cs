

using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class GenericAnimationExtraComponent : MonoBehaviour
	{
		[SerializeField]
		public Sprite[] sprites = new Sprite[2];

		public bool isActive = false;
	}
}
