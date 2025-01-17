using UnityEngine;

namespace BBPlusAnimations.Components
{
	internal class HideableLockerExtraComponent : MonoBehaviour
	{
		[SerializeField]
		internal Texture2D closed, open;

		[SerializeField]
		internal MeshRenderer renderer;
	}
}
