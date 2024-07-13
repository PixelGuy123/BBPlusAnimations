using UnityEngine;
using System.Collections.Generic;

namespace BBPlusAnimations.Components
{
	public class BalloonManager : MonoBehaviour
	{
		readonly public List<MathMachineNumber> ballsToDestroy = [];

		internal static BalloonManager i;

		void Awake() =>
			i = this;

		void Update()
		{
			for (int i = 0; i < ballsToDestroy.Count; i++)
			{
				if (!ballsToDestroy[i])
				{
					ballsToDestroy.RemoveAt(i--);
					continue;
				}

				if (!ballsToDestroy[i].gameObject.activeSelf)
				{
					Destroy(ballsToDestroy[i].gameObject);
					ballsToDestroy.RemoveAt(i--);
				}
			}
		}
	}
}
