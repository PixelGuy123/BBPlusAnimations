﻿using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class ChalkBeatingUpInGround : MonoBehaviour
	{
		[SerializeField]
		internal ParticleSystem particles;

		[SerializeField]
		internal SpriteRenderer renderer;

		void Start()
		{
			if (renderer == null || GetComponent<ChalkEraser>() == null)
			{
				Destroy(gameObject);
				return;
			}
			UpdatePos();
		}

		void Update()
		{
			var emission = particles.emission;
			if (!emission.enabled)
			{
				renderer.enabled = false;
				return;
			}
			if (Time.timeScale == 0f) return;

			renderer.enabled = true;

			beginDelay -= Time.deltaTime;
			if (beginDelay >= 0f)
			{
				transform.localPosition = new(Random.Range(-0.7f, 0.7f), Random.Range(-0.7f, 0.7f), Random.Range(-0.7f, 0.7f));
				return;
			}

			if (reset)
			{
				transform.localPosition = Vector3.SmoothDamp(transform.localPosition, ogPos, ref velocity, time);
				if ((transform.localPosition - ogPos).magnitude <= 0.1f)
				{
					UpdatePos();
					reset = false;
				}
			}
			else
			{
				transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, time);
				if ((transform.localPosition - targetPos).magnitude <= 0.1f)
					reset = true;

			}
		}
		void UpdatePos() =>
			targetPos = new Vector3(Random.Range(-5f, 5f), -4.5f, Random.Range(-0.5f, 5f));

		const float time = 0.1f;
		float beginDelay = 1.35f;

		Vector3 ogPos = Vector3.zero, targetPos, velocity;

		bool reset = false;
	}
}
