using System;
using System.Collections.Generic;
using System.Reflection;
using SAIN.Attributes;
using UnityEngine;

namespace SAIN.Editor;

public sealed class SettingsContainer
{
	public readonly string Name;

	public readonly List<Category> Categories = new List<Category>();

	public readonly List<Category> SelectedCategories = new List<Category>();

	public string SearchPattern = string.Empty;

	public bool Open = false;

	public bool SecondOpen = false;

	public Vector2 Scroll = Vector2.zero;

	public Vector2 SecondScroll = Vector2.zero;

	public SettingsContainer(Type settingsType, string name = null)
	{
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		Name = name ?? settingsType.Name;
		FieldInfo[] fields = settingsType.GetFields(BindingFlags.Instance | BindingFlags.Public);
		foreach (FieldInfo member in fields)
		{
			ConfigInfoClass configInfoClass = new ConfigInfoClass(member);
			if (!configInfoClass.Hidden)
			{
				Category category = new Category(configInfoClass);
				category.OptionCount(out var realCount);
				if (realCount > 0)
				{
					Categories.Add(category);
				}
			}
		}
	}
}
