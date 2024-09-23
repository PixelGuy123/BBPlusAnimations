using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using PixelInternalAPI.Extensions;

namespace BBPlusAnimations.Patches
{
	[AnimationConditionalPatch("Bus go away", "If True, once you leave the field trip winning, the bus goes away by itself.")]
	[HarmonyPatch(typeof(FieldTripEntranceRoomFunction))]
	internal class TripEntrancePatch
	{
		[HarmonyPatch("OnTripPlayed")]
		[HarmonyTranspiler]
		static IEnumerable<CodeInstruction> RemoveIf(IEnumerable<CodeInstruction> i) =>
			new CodeMatcher(i)
			.MatchForward(true, 
				new(OpCodes.Ldarg_0),
				new(CodeInstruction.LoadField(typeof(FieldTripEntranceRoomFunction), "busObjects")),
				new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(Component), "gameObject")),
				new(OpCodes.Ldc_I4_0),
				new(OpCodes.Callvirt, AccessTools.Method(typeof(GameObject), "SetActive", [typeof(bool)]))
				)
			.Advance(-1)
			.Set(OpCodes.Ldc_I4_1, null) // Basically this.busObjects.gameObject.SetActive(true)
			.InstructionEnumeration();



		[HarmonyPatch("OnTripPlayed")]
		[HarmonyPostfix]
		static void AddMyOwnBusLeave(FieldTripEntranceRoomFunction __instance, Transform ___busObjects)
		{
			Transform bus = null;
			foreach (var child in ___busObjects.AllChilds())
			{
				if (child.name != "Bus")
					child.gameObject.SetActive(false);
				else if (!bus)
					bus = child;
			}

			if (bus)
			{
				bus.GetComponentsInChildren<MeshRenderer>().Do(x => x.material = baldiInBus);
				__instance.StartCoroutine(BusGoAway(bus, bus.GetComponentInChildren<PropagatedAudioManager>(), __instance.Room.ec));
			}
		}

		internal static Material baldiInBus;

		static IEnumerator BusGoAway(Transform bus, PropagatedAudioManager audMan, EnvironmentController ec)
		{
			yield return null;

			while (Time.timeScale <= 0f) yield return null;

			float speed = 0f;
			float timeSpeed = 0f;
			while (timeSpeed < 6f)
			{
				audMan.volumeMultiplier += timeSpeed * 0.0005f;
				audMan.pitchModifier += timeSpeed * 0.02f;
				if (ec.CellFromPosition(bus.transform.position).Null)
					audMan.enabled = false; // To avoid exceptions

				speed += timeSpeed;
				timeSpeed += speedIncrease * ec.EnvironmentTimeScale * Time.deltaTime;
				bus.transform.position += bus.right * speed;
				yield return null;
			}

			bus.gameObject.SetActive(false);

			yield break;
		}

		const float speedIncrease = 0.00005f;
	}
}
