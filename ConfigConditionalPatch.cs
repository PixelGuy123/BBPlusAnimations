using BepInEx.Configuration;
using MTM101BaldAPI;

namespace BBPlusAnimations
{
	internal class AnimationConditionalPatch(string name, string desc) : ConditionalPatch
	{
		public override bool ShouldPatch() =>
			config.Value;
		readonly ConfigEntry<bool> config = BasePlugin.file.Bind("Animation Management", name, true, desc);
	}
}
