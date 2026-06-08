using System.Linq;
using System.Reflection;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;

namespace BatterySystem
{
	public class BatteryResourceAttributePatch : ModulePatch
	{
		protected override MethodBase GetTargetMethod()
		{
			return typeof(ResourceComponent)
				.GetConstructors(BindingFlags.Instance | BindingFlags.Public)
				.First(constructor =>
				{
					ParameterInfo[] parameters = constructor.GetParameters();
					return parameters.Length == 2 && parameters[0].ParameterType == typeof(Item);
				});
		}

		[HarmonyPostfix]
		private static void Postfix(ResourceComponent __instance)
		{
			Item item = __instance?.Item;
			if (!BatterySystem.IsBatteryItem(item)) return;

			item.Attributes?.RemoveAll(attribute =>
				attribute?.Id is EItemAttributeId id && id == EItemAttributeId.Resource);
		}
	}
}
