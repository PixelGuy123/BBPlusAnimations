using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class NanaPeelSlipper : MonoBehaviour
	{
		[SerializeField]
		ITM_NanaPeel myPeel;

		[SerializeField]
		internal SpriteRenderer renderer;
		public void SetMyPeel(ITM_NanaPeel peel) =>
			myPeel = peel;

		void Update()
		{
			if (myPeel)
			{
				if (myPeel.slipping)
				{
					var rot = Quaternion.LookRotation(myPeel.direction).eulerAngles;
					rot.y -= 90f;
					transform.rotation = Quaternion.Euler(rot);
					transform.position = myPeel.transform.position - transform.right * 2f;
					renderer.enabled = true;
					return;
				}
				renderer.enabled = false;
			}
		}
	}
}
