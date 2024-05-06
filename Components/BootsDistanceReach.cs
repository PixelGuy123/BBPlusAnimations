using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class BootsDistanceReach : MonoBehaviour
	{
		public void IncrementDistance(float distance)
		{
			reachDistance = Mathf.Min(limit * 3, reachDistance + distance); // Max of three steps at the same time to not bug out the audio
			while (reachDistance > limit)
			{
				step = !step;
				audMan.pitchModifier = 1f + (distance / 1.5f);
				audMan.PlaySingle(step ? audFootstep : audFootstep2);
				reachDistance -= limit;
			}
		}

		float reachDistance = 0f;

		bool step = false;

		const float limit = 25f;

		[SerializeField]
		internal SoundObject audFootstep, audFootstep2;

		[SerializeField]
		internal AudioManager audMan;
	}
}
