using BepInEx.Configuration;
using MTM101BaldAPI;

namespace BBPlusAnimations
{
	internal class AnimationConditionalPatch(string category, string name, string desc, bool defaultValue = true) : ConditionalPatch
	{
		public override bool ShouldPatch()
		{
			//Debug.Log($"Checking val of {name}: " + config.Value);
			return config.Value;
		}
		readonly ConfigEntry<bool> config = BasePlugin.file.Bind(category, name, defaultValue, desc);
	}
}
