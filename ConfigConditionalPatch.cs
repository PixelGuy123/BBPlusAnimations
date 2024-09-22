using BepInEx.Configuration;
using MTM101BaldAPI;
using UnityEngine;

namespace BBPlusAnimations
{
	internal class AnimationConditionalPatch(string name, string desc) : ConditionalPatch
	{
		public override bool ShouldPatch()
		{
			//Debug.Log($"Checking val of {name}: " + config.Value);
			return config.Value;
		}
		readonly ConfigEntry<bool> config = BasePlugin.file.Bind("Animation Management", name, true, desc);
	}
}
