using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class DrReflexHammerComponent : MonoBehaviour
	{
		public void Init(Vector3 pos)
		{
			worldHammer = Instantiate(hammerPre);
			worldHammer.transform.position = pos;
		}

		public void HideHammer(bool hide) =>
			worldHammer.gameObject.SetActive(!hide);

		[SerializeField]
		internal Transform hammerPre;

		Transform worldHammer;
	}
}
