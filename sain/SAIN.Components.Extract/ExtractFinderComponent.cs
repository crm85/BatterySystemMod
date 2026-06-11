using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Comfort.Common;
using EFT;
using EFT.Interactive;
using HarmonyLib;
using SAIN.Helpers;
using UnityEngine;

namespace SAIN.Components.Extract;

public class ExtractFinderComponent : MonoBehaviour
{
	private ExfiltrationPoint[] AllExfils;

	private ExfiltrationPoint[] AllScavExfils;

	private Dictionary<ExfiltrationPoint, Vector3> ValidExfils = new Dictionary<ExfiltrationPoint, Vector3>();

	private Dictionary<ExfiltrationPoint, Vector3> ValidScavExfils = new Dictionary<ExfiltrationPoint, Vector3>();

	private Dictionary<ExfiltrationPoint, ExtractPositionFinder> extractPositionFinders = new Dictionary<ExfiltrationPoint, ExtractPositionFinder>();

	private float CheckExtractDelay = 10f;

	private float NextCheckExtractTime = 0f;

	private bool hasExfilControl = false;

	public bool IsFindingExtracts { get; private set; } = false;

	public static bool DebugMode => SAINPlugin.DebugSettings.Logs.DebugExtract;

	public void ManualUpdate(float currentTime, float deltaTime)
	{
		if (!hasExfilControl && !GetExfilControl())
		{
			NextCheckExtractTime = Time.time + 0.1f;
			return;
		}
		hasExfilControl = true;
		if (!(NextCheckExtractTime > Time.time))
		{
			NextCheckExtractTime = Time.time + CheckExtractDelay;
			if (!IsFindingExtracts)
			{
				((MonoBehaviour)this).StartCoroutine(FindAllExfils());
			}
		}
	}

	public void OnDisable()
	{
		((MonoBehaviour)this).StopAllCoroutines();
	}

	public void OnGUI()
	{
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Expected O, but got Unknown
		//IL_0059: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Expected O, but got Unknown
		//IL_00a6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		//IL_015a: Unknown result type (might be due to invalid IL or missing references)
		//IL_015f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0164: Unknown result type (might be due to invalid IL or missing references)
		//IL_0166: Unknown result type (might be due to invalid IL or missing references)
		DebugGizmos.OnGUIGame();
		DebugGizmos.OnGUIDebug();
		if (!DebugMode || !SAINPlugin.DebugSettings.Logs.DrawDebugLabels)
		{
			return;
		}
		GUIStyle val = new GUIStyle(GUI.skin.label);
		val.alignment = (TextAnchor)3;
		val.fontSize = 14;
		val.margin = new RectOffset(3, 3, 3, 3);
		foreach (ExfiltrationPoint key in extractPositionFinders.Keys)
		{
			Vector3[] array = extractPositionFinders[key].PathEndpoints.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 worldPos = array[i] + new Vector3(0f, 1f, 0f);
				DebugGizmos.OnGUIDrawLabel(worldPos, "Path Endpoint " + (i + 1) + ": " + key.Settings.Name, val);
			}
			if (extractPositionFinders[key].ExtractPosition.HasValue)
			{
				Vector3 worldPos2 = extractPositionFinders[key].ExtractPosition.Value + new Vector3(0f, 1f, 0f);
				DebugGizmos.OnGUIDrawLabel(worldPos2, "Extract point: " + key.Settings.Name, val);
			}
		}
	}

	private void DrawGizmoSpheres(ExtractPositionFinder finder)
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Unknown result type (might be due to invalid IL or missing references)
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0084: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0089: Unknown result type (might be due to invalid IL or missing references)
		//IL_0095: Unknown result type (might be due to invalid IL or missing references)
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		if (!DebugGizmos.DrawGizmos || !DebugMode)
		{
			return;
		}
		foreach (Vector3 pathEndpoint in finder.PathEndpoints)
		{
			DebugGizmos.Sphere(pathEndpoint, 1f, Color.blue, CheckExtractDelay);
		}
		if (finder.ExtractPosition.HasValue)
		{
			Color color = (finder.ValidPathFound ? Color.green : Color.red);
			DebugGizmos.Sphere(finder.ExtractPosition.Value, 1f, color, CheckExtractDelay);
		}
	}

	public int CountValidExfilsForBot(BotComponent bot)
	{
		return GetValidExfilsForBot(bot).Count;
	}

	public IDictionary<ExfiltrationPoint, Vector3> GetValidExfilsForBot(BotComponent bot)
	{
		return bot.Info.Profile.IsScav ? ValidScavExfils : ValidExfils;
	}

	private bool GetExfilControl()
	{
		AbstractGame instance = Singleton<AbstractGame>.Instance;
		if (((instance != null) ? instance.GameTimer : null) == null)
		{
			return false;
		}
		ExfiltrationControllerClass exfiltrationController = Singleton<GameWorld>.Instance.ExfiltrationController;
		if (exfiltrationController == null)
		{
			return false;
		}
		if (exfiltrationController.ExfiltrationPoints == null)
		{
			return false;
		}
		ExfiltrationPoint[] exfiltrationPoints = exfiltrationController.ExfiltrationPoints;
		ExfiltrationPoint[] secretExfiltrationPoints = (ExfiltrationPoint[])(object)exfiltrationController.SecretExfiltrationPoints;
		AllExfils = CollectionExtensions.AddRangeToArray<ExfiltrationPoint>(exfiltrationPoints, secretExfiltrationPoints);
		if (DebugMode)
		{
			Logger.LogInfo($"Found {AllExfils?.Length} possible Exfil Points in this map.");
		}
		secretExfiltrationPoints = (ExfiltrationPoint[])(object)exfiltrationController.GetScavSecretExits();
		ExfiltrationPoint[] array = secretExfiltrationPoints;
		secretExfiltrationPoints = (ExfiltrationPoint[])(object)exfiltrationController.ScavExfiltrationPoints;
		AllScavExfils = CollectionExtensions.AddRangeToArray<ExfiltrationPoint>(secretExfiltrationPoints, array);
		if (DebugMode)
		{
			Logger.LogInfo($"Found {AllScavExfils?.Length} possible Scav Exfil Points in this map.");
		}
		return true;
	}

	private IEnumerator FindAllExfils()
	{
		bool completedCoroutine = false;
		try
		{
			IsFindingExtracts = true;
			yield return UpdateValidExfils(ValidExfils, AllExfils);
			yield return UpdateValidExfils(ValidScavExfils, AllScavExfils);
			completedCoroutine = true;
		}
		finally
		{
			IsFindingExtracts = false;
			if (!completedCoroutine)
			{
				Logger.LogError("An error occurred when searching for extracts.");
			}
		}
	}

	private IEnumerator UpdateValidExfils(IDictionary<ExfiltrationPoint, Vector3> validExfils, ExfiltrationPoint[] allExfils)
	{
		if (allExfils == null)
		{
			yield break;
		}
		foreach (ExfiltrationPoint ex in allExfils)
		{
			ExtractPositionFinder finder = GetExtractPositionSearchJob(ex);
			if (validExfils.ContainsKey(ex))
			{
				DrawGizmoSpheres(finder);
				continue;
			}
			yield return finder.SearchForExfilPosition();
			DrawGizmoSpheres(finder);
			if (finder.ValidPathFound)
			{
				validExfils.Add(ex, finder.ExtractPosition.Value);
			}
		}
	}

	private ExtractPositionFinder GetExtractPositionSearchJob(ExfiltrationPoint ex)
	{
		if (extractPositionFinders.ContainsKey(ex))
		{
			return extractPositionFinders[ex];
		}
		ExtractPositionFinder extractPositionFinder = new ExtractPositionFinder(ex);
		extractPositionFinders.Add(ex, extractPositionFinder);
		return extractPositionFinder;
	}
}
