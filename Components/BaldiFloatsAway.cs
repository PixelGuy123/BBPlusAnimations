using UnityEngine;

namespace BBPlusAnimations.Components
{
	public class BaldiFloatsAway : MonoBehaviour
	{
		private void Update()
		{
			int idx = Mathf.FloorToInt(frame);
			frame += frameSpeed * ec.EnvironmentTimeScale * Time.deltaTime;

			if (idx >= sprites.Length)
			{
				frame = 0f;
				idx = 0;
			}
			renderer.sprite = sprites[idx];

			cooldown -= ec.EnvironmentTimeScale * Time.deltaTime;
			if (cooldown <= 0f) // Despawn
				Destroy(gameObject);

			speed += speedMultiplier * ec.EnvironmentTimeScale * Time.deltaTime;
			transform.position += Vector3.up * speed * Time.timeScale;
		}
		float frame = 0f, speed = 0f, cooldown = maxCooldown;

		public EnvironmentController ec;

		[SerializeField]
		public SpriteRenderer renderer;

		[SerializeField]
		public Sprite[] sprites;

		const float speedMultiplier = 0.5f, maxCooldown = 15f, frameSpeed = 10f;
	}
}
