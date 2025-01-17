using UnityEngine;

namespace BBPlusAnimations.Components
{
	internal class LightSwitchRoomFunction : RoomFunction
	{
		public override void Initialize(RoomController room)
		{
			base.Initialize(room);
			hasPower = room.Powered;
		}

		public override void OnGenerationFinished()
		{
			base.OnGenerationFinished();
			lightSwitches = new SpriteRenderer[room.doors.Count];
			for (int i = 0; i < lightSwitches.Length; i++)
			{
				Cell spot = room.doors[i].aTile.TileMatches(room) ? 
					room.doors[i].aTile : room.doors[i].bTile;
				Direction dir = room.doors[i].aTile.TileMatches(room) ?
					room.doors[i].direction : room.doors[i].direction.GetOpposite();

				var lightSwitch = Instantiate(lightSwitchPre);
				lightSwitch.transform.SetParent(spot.ObjectBase);
				lightSwitch.transform.localPosition = Vector3.up * 5f + dir.ToVector3() * 4.97f + dir.PerpendicularList()[0].ToVector3() * 3.25f;
				lightSwitch.transform.rotation = dir.GetOpposite().ToRotation();

				spot.AddRenderer(lightSwitch);
				lightSwitches[i] = lightSwitch;
			}
		}

#pragma warning disable IDE0051
		void Update()
#pragma warning restore IDE0051
		{
			if (hasPower != room.Powered)
			{
				hasPower = room.Powered;
				for (int i = 0; i < lightSwitches.Length; i++)
					lightSwitches[i].sprite = hasPower ? on : off;
			}
		}

		[SerializeField]
		internal Sprite off, on;

		[SerializeField]
		internal SpriteRenderer lightSwitchPre;

		SpriteRenderer[] lightSwitches;

		bool hasPower = true;
	}
}
