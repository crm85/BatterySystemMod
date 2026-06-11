using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using SAIN.Preset;
using SAIN.Preset.GearStealthValues;

namespace SAIN.Helpers;

public static class JsonUtility
{
	public static class Load
	{
		public static void LoadAllJsonFiles<T>(List<T> list, params string[] folders)
		{
			LoadAllFiles(list, folders);
		}

		public static void LoadCustomPresetOptions(List<SAINPresetDefinition> list)
		{
			list.Clear();
			if (!GetFoldersPath(out var path, "Presets"))
			{
				Directory.CreateDirectory(path);
			}
			string[] directories = Directory.GetDirectories(path);
			string[] array = directories;
			foreach (string path2 in array)
			{
				string text = Path.Combine(path2, "Info") + ".json";
				if (File.Exists(text))
				{
					string file = File.ReadAllText(text);
					SAINPresetDefinition sAINPresetDefinition = DeserializeObject<SAINPresetDefinition>(file);
					if (sAINPresetDefinition.IsCustom)
					{
						list.Add(sAINPresetDefinition);
					}
				}
				else
				{
					Logger.LogError("Could not Import Info.json at path [" + text + "]. Is the file missing?");
				}
			}
		}

		public static void LoadAllFiles<T>(List<T> list, params string[] folders)
		{
			if (GetFoldersPath(out var path, folders))
			{
				string[] files = Directory.GetFiles(path, "*.json");
				foreach (string path2 in files)
				{
					string text = File.ReadAllText(path2);
					list.Add(JsonConvert.DeserializeObject<T>(text));
				}
			}
		}

		public static void LoadStealthValues(List<ItemStealthValue> list, params string[] folders)
		{
			if (GetFoldersPath(out var path, folders))
			{
				string[] files = Directory.GetFiles(path, "*.json");
				foreach (string path2 in files)
				{
					string text = File.ReadAllText(path2);
					list.Add(JsonConvert.DeserializeObject<ItemStealthValue>(text));
				}
			}
		}

		public static T DeserializeObject<T>(string file)
		{
			return JsonConvert.DeserializeObject<T>(file);
		}

		public static string LoadTextFile(string fileExtension, string fileName, params string[] folders)
		{
			if (GetFoldersPath(out var path, folders))
			{
				string text = Path.Combine(path, fileName);
				text += fileExtension;
				if (File.Exists(text))
				{
					return File.ReadAllText(text);
				}
			}
			return null;
		}

		public static bool LoadJsonFile(out string json, string fileName, params string[] folders)
		{
			json = LoadTextFile(".json", fileName, folders);
			return json != null;
		}

		public static bool LoadObject<T>(out T obj, string fileName, params string[] folders)
		{
			string text = LoadTextFile(".json", fileName, folders);
			if (text != null)
			{
				obj = DeserializeObject<T>(text);
				return true;
			}
			obj = default(T);
			return false;
		}
	}

	public static readonly Dictionary<JsonEnum, string> FileAndFolderNames = new Dictionary<JsonEnum, string>
	{
		{
			JsonEnum.Presets,
			"Presets"
		},
		{
			JsonEnum.GlobalSettings,
			"GlobalSettings"
		}
	};

	public const string PresetsFolder = "Presets";

	public const string JSON = ".json";

	public const string JSONSearch = "*.json";

	public const string Info = "Info";

	public static void SaveObjectToJson(object objectToSave, string fileName, params string[] folders)
	{
		if (objectToSave == null)
		{
			return;
		}
		try
		{
			if (!GetFoldersPath(out var path, folders))
			{
				Directory.CreateDirectory(path);
			}
			string text = Path.Combine(path, fileName);
			text += ".json";
			string value = JsonConvert.SerializeObject(objectToSave, (Formatting)1);
			File.Create(text).Dispose();
			StreamWriter streamWriter = new StreamWriter(text);
			streamWriter.Write(value);
			streamWriter.Flush();
			streamWriter.Close();
		}
		catch (Exception data)
		{
			Logger.LogError(data);
		}
	}

	public static bool DoesFileExist(string fileName, params string[] folders)
	{
		if (!GetFoldersPath(out var path, folders))
		{
			return false;
		}
		string text = Path.Combine(path, fileName);
		text += ".json";
		return File.Exists(text);
	}

	public static void DeletePreset(SAINPresetDefinition preset)
	{
		string path = GetPath("Presets", preset.Name);
		if (Directory.Exists(path))
		{
			Directory.Delete(path, recursive: true);
		}
	}

	private static void CheckCreateFolder(string path)
	{
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
	}

	public static void CreateFolder(params string[] subFolders)
	{
		string path = GetPath(subFolders);
		CheckCreateFolder(path);
	}

	public static bool DoesFolderExist(params string[] subFolders)
	{
		string path = GetPath(subFolders);
		return Directory.Exists(path);
	}

	public static bool GetFoldersPath(out string path, params string[] folders)
	{
		path = GetPath(folders);
		return Directory.Exists(path);
	}

	private static string GetPath(params string[] folders)
	{
		string text = GetSAINPluginPath();
		for (int i = 0; i < folders.Length; i++)
		{
			text = Path.Combine(text, folders[i]);
		}
		return text;
	}

	public static string GetSAINPluginPath()
	{
		string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
		CheckCreateFolder(directoryName);
		return directoryName;
	}
}
