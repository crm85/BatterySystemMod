using System.Collections;
using System.Collections.Generic;
using BepInEx.Logging;
using EFT;
using SAIN.Editor;
using SAIN.Editor.Util;
using SAIN.Plugin;
using UnityEngine;
using UnityEngine.AI;

namespace SAIN.Helpers;

public class DebugGizmos
{
	public class DrawLists
	{
		private static ManualLogSource Logger;

		private Color ColorA;

		private Color ColorB;

		private GameObject[] DebugObjects;

		public DrawLists(Color colorA, Color colorB, string LogName = "", bool randomColor = false)
		{
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0056: Unknown result type (might be due to invalid IL or missing references)
			//IL_005c: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_002c: Unknown result type (might be due to invalid IL or missing references)
			//IL_0031: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			//IL_004b: Unknown result type (might be due to invalid IL or missing references)
			LogName += "[Drawer]";
			if (randomColor)
			{
				ColorA = new Color(Random.value, Random.value, Random.value);
				ColorB = new Color(Random.value, Random.value, Random.value);
			}
			else
			{
				ColorA = colorA;
				ColorB = colorB;
			}
			Logger = Logger.CreateLogSource(LogName);
		}

		public void DrawTempPath(NavMeshPath Path, bool active, Color colorActive, Color colorInActive, float lineSize = 0.05f, float expireTime = 0.5f, bool useDrawerSetColors = false)
		{
			//IL_001f: Unknown result type (might be due to invalid IL or missing references)
			//IL_0024: Unknown result type (might be due to invalid IL or missing references)
			//IL_0029: Unknown result type (might be due to invalid IL or missing references)
			//IL_002e: Unknown result type (might be due to invalid IL or missing references)
			//IL_0038: Unknown result type (might be due to invalid IL or missing references)
			//IL_003d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0042: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_006f: Unknown result type (might be due to invalid IL or missing references)
			//IL_006b: Unknown result type (might be due to invalid IL or missing references)
			//IL_005d: Unknown result type (might be due to invalid IL or missing references)
			//IL_0055: Unknown result type (might be due to invalid IL or missing references)
			//IL_0070: Unknown result type (might be due to invalid IL or missing references)
			//IL_0062: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0075: Unknown result type (might be due to invalid IL or missing references)
			if (DrawGizmos)
			{
				for (int i = 0; i < Path.corners.Length - 1; i++)
				{
					Vector3 startPoint = Path.corners[i] + Vector3.up;
					Vector3 endPoint = Path.corners[i + 1] + Vector3.up;
					Color color = ((!useDrawerSetColors) ? (active ? colorActive : colorInActive) : (active ? ColorA : ColorB));
					Line(startPoint, endPoint, color, lineSize, expireTime);
				}
			}
		}

		public void Draw(List<Vector3> list, bool destroy, float size = 0.1f, bool rays = false, float rayLength = 0.35f)
		{
			if (!DrawGizmos)
			{
				DestroyDebug();
			}
			else if (destroy)
			{
				DestroyDebug();
			}
			else if (list.Count > 0 && DebugObjects == null)
			{
				Logger.LogWarning((object)$"Drawing {list.Count} Vector3s");
				DebugObjects = Create(list, size, rays, rayLength);
			}
		}

		public void Draw(Vector3[] array, bool destroy, float size = 0.1f, bool rays = false, float rayLength = 0.35f)
		{
			if (!DrawGizmos)
			{
				DestroyDebug();
			}
			else if (destroy)
			{
				DestroyDebug();
			}
			else if (array.Length != 0 && DebugObjects == null)
			{
				Logger.LogWarning((object)$"Drawing {array.Length} Vector3s");
				DebugObjects = Create(array, size, rays, rayLength);
			}
		}

		private GameObject[] Create(List<Vector3> list, float size = 0.1f, bool rays = false, float rayLength = 0.35f)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0071: Unknown result type (might be due to invalid IL or missing references)
			//IL_0074: Unknown result type (might be due to invalid IL or missing references)
			//IL_0047: Unknown result type (might be due to invalid IL or missing references)
			//IL_0048: Unknown result type (might be due to invalid IL or missing references)
			//IL_004e: Unknown result type (might be due to invalid IL or missing references)
			List<GameObject> list2 = new List<GameObject>();
			foreach (Vector3 item3 in list)
			{
				if (rays)
				{
					size *= Random.Range(0.5f, 1.5f);
					rayLength *= Random.Range(0.5f, 1.5f);
					GameObject item = Ray(item3, Vector3.up, ColorA, rayLength, size);
					list2.Add(item);
				}
				else
				{
					GameObject item2 = Sphere(item3, size, ColorA);
					list2.Add(item2);
				}
			}
			return list2.ToArray();
		}

		private GameObject[] Create(Vector3[] array, float size = 0.1f, bool rays = false, float rayLength = 0.35f)
		{
			//IL_0013: Unknown result type (might be due to invalid IL or missing references)
			//IL_0018: Unknown result type (might be due to invalid IL or missing references)
			//IL_0073: Unknown result type (might be due to invalid IL or missing references)
			//IL_0076: Unknown result type (might be due to invalid IL or missing references)
			//IL_0049: Unknown result type (might be due to invalid IL or missing references)
			//IL_004a: Unknown result type (might be due to invalid IL or missing references)
			//IL_0050: Unknown result type (might be due to invalid IL or missing references)
			List<GameObject> list = new List<GameObject>();
			foreach (Vector3 val in array)
			{
				if (rays)
				{
					size *= Random.Range(0.5f, 1.5f);
					rayLength *= Random.Range(0.5f, 1.5f);
					GameObject item = Ray(val, Vector3.up, ColorA, rayLength, size);
					list.Add(item);
				}
				else
				{
					GameObject item2 = Sphere(val, size, ColorA);
					list.Add(item2);
				}
			}
			return list.ToArray();
		}

		private void DestroyDebug()
		{
			if (DebugObjects != null)
			{
				GameObject[] debugObjects = DebugObjects;
				foreach (GameObject val in debugObjects)
				{
					Object.Destroy((Object)(object)val);
				}
				DebugObjects = null;
			}
		}
	}

	public class Components
	{
		public class FollowLineScript : MonoBehaviour
		{
			public GameObject startObject;

			public GameObject endObject;

			public LineRenderer lineRenderer;

			public float yOffset = 1f;

			public void Update()
			{
				//IL_0013: Unknown result type (might be due to invalid IL or missing references)
				//IL_0028: Unknown result type (might be due to invalid IL or missing references)
				//IL_002d: Unknown result type (might be due to invalid IL or missing references)
				//IL_004a: Unknown result type (might be due to invalid IL or missing references)
				//IL_005f: Unknown result type (might be due to invalid IL or missing references)
				//IL_0064: Unknown result type (might be due to invalid IL or missing references)
				lineRenderer.SetPosition(0, startObject.transform.position + new Vector3(0f, yOffset, 0f));
				lineRenderer.SetPosition(1, endObject.transform.position + new Vector3(0f, yOffset, 0f));
			}

			public void SetColor(Color color)
			{
				//IL_000c: Unknown result type (might be due to invalid IL or missing references)
				((Renderer)lineRenderer).material.color = color;
			}
		}

		public static GameObject FollowLine(GameObject startObject, GameObject endObject, float lineWidth, Color color)
		{
			//IL_0001: Unknown result type (might be due to invalid IL or missing references)
			//IL_0007: Expected O, but got Unknown
			//IL_0014: Unknown result type (might be due to invalid IL or missing references)
			//IL_0033: Unknown result type (might be due to invalid IL or missing references)
			//IL_0046: Unknown result type (might be due to invalid IL or missing references)
			GameObject val = new GameObject();
			LineRenderer val2 = val.AddComponent<LineRenderer>();
			((Renderer)val2).material.color = color;
			val2.startWidth = lineWidth;
			val2.endWidth = lineWidth;
			val2.SetPosition(0, startObject.transform.position);
			val2.SetPosition(1, endObject.transform.position);
			FollowLineScript followLineScript = val.AddComponent<FollowLineScript>();
			followLineScript.startObject = startObject;
			followLineScript.endObject = endObject;
			followLineScript.lineRenderer = val2;
			return val;
		}
	}

	internal class TempCoroutine : MonoBehaviour
	{
		internal class TempCoroutineRunner : MonoBehaviour
		{
		}

		public static void DestroyAfterDelay(GameObject obj, float delay)
		{
			//IL_0012: Unknown result type (might be due to invalid IL or missing references)
			if ((Object)(object)obj != (Object)null)
			{
				TempCoroutineRunner tempCoroutineRunner = new GameObject("TempCoroutineRunner").AddComponent<TempCoroutineRunner>();
				if (tempCoroutineRunner != null)
				{
					((MonoBehaviour)tempCoroutineRunner).StartCoroutine(RunDestroyAfterDelay(obj, delay));
				}
			}
		}

		private static IEnumerator RunDestroyAfterDelay(GameObject obj, float delay)
		{
			yield return (object)new WaitForSeconds(delay);
			if ((Object)(object)obj != (Object)null)
			{
				TempCoroutineRunner runner = obj.GetComponentInParent<TempCoroutineRunner>();
				if ((Object)(object)runner != (Object)null)
				{
					Object.Destroy((Object)(object)((Component)runner).gameObject);
				}
				Object.Destroy((Object)(object)obj);
			}
		}

		public void OnDestroy()
		{
			((MonoBehaviour)this).StopAllCoroutines();
		}
	}

	private static readonly List<GameObject> DrawnGizmos;

	private static GUIStyle DefaultStyle;

	private static readonly List<GUIObject> DebugLabels;

	private static readonly List<GUIObject> GameLabels;

	private static float _screenScale;

	private static float _nextCheckScreenTime;

	private const float sphereMulti = 1.5f;

	private const float maxSphere = 10f;

	private const float minSphere = 0.15f;

	private const float minMag = 0.01f;

	public static bool DrawGizmos => SAINPlugin.DrawDebugGizmos;

	private static float RandomFloat => Random.Range(0.2f, 1f);

	public static Color RandomColor => new Color(RandomFloat, RandomFloat, RandomFloat);

	static DebugGizmos()
	{
		DrawnGizmos = new List<GameObject>();
		DebugLabels = new List<GUIObject>();
		GameLabels = new List<GUIObject>();
		_screenScale = 1f;
		GameWorld.OnDispose += Dispose;
		PresetHandler.OnEditorSettingsChanged += Update;
	}

	public static void Update(PresetEditorDefaults defaults)
	{
		if (!DrawGizmos)
		{
			ClearGizmos();
		}
	}

	private static void Dispose()
	{
		ClearGizmos();
		DebugLabels.Clear();
	}

	private static void ClearGizmos()
	{
		if (DrawnGizmos.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < DrawnGizmos.Count; i++)
		{
			if ((Object)(object)DrawnGizmos[i] != (Object)null)
			{
				Object.Destroy((Object)(object)DrawnGizmos[i]);
			}
		}
		DrawnGizmos.Clear();
	}

	public static void OnGUIGame()
	{
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		foreach (GUIObject gameLabel in GameLabels)
		{
			if (gameLabel.Enabled)
			{
				string text = (GClass1437.IsNullOrEmpty(gameLabel.Text) ? gameLabel.StringBuilder.ToString() : gameLabel.Text);
				OnGUIDrawLabel(gameLabel.WorldPos, text, gameLabel.Style, gameLabel.Scale);
			}
		}
	}

	public static void OnGUIDebug()
	{
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINPlugin.DebugSettings.Logs.DrawDebugLabels)
		{
			DebugLabels.Clear();
			return;
		}
		foreach (GUIObject debugLabel in DebugLabels)
		{
			if (debugLabel.Enabled)
			{
				string text = (GClass1437.IsNullOrEmpty(debugLabel.Text) ? debugLabel.StringBuilder.ToString() : debugLabel.Text);
				OnGUIDrawLabel(debugLabel.WorldPos, text, debugLabel.Style, debugLabel.Scale);
			}
		}
	}

	public static GUIObject CreateLabel(Vector3 worldPos, string text, GUIStyle guiStyle = null, float scale = 1f, bool debug = true)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		GUIObject gUIObject = new GUIObject
		{
			WorldPos = worldPos,
			Text = text,
			Style = guiStyle,
			Scale = scale
		};
		AddGUIObject(gUIObject, debug);
		return gUIObject;
	}

	public static void AddGUIObject(GUIObject obj, bool debug)
	{
		if (debug)
		{
			if (!DebugLabels.Contains(obj))
			{
				DebugLabels.Add(obj);
			}
		}
		else if (!GameLabels.Contains(obj))
		{
			GameLabels.Add(obj);
		}
	}

	public static void DestroyLabel(GUIObject obj)
	{
		DebugLabels.Remove(obj);
		GameLabels.Remove(obj);
	}

	public static void OnGUIDrawLabel(Vector3 worldPos, string text, GUIStyle guiStyle = null, float scale = 1f)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_001e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0023: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Expected O, but got Unknown
		//IL_00f3: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_0102: Unknown result type (might be due to invalid IL or missing references)
		//IL_0118: Unknown result type (might be due to invalid IL or missing references)
		//IL_0120: Unknown result type (might be due to invalid IL or missing references)
		//IL_0131: Unknown result type (might be due to invalid IL or missing references)
		//IL_0136: Unknown result type (might be due to invalid IL or missing references)
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Expected O, but got Unknown
		//IL_0090: Unknown result type (might be due to invalid IL or missing references)
		//IL_009a: Expected O, but got Unknown
		if ((Object)(object)Camera.main == (Object)null)
		{
			return;
		}
		Vector3 val = Camera.main.WorldToScreenPoint(worldPos);
		if (val.z <= 0f)
		{
			return;
		}
		if (guiStyle == null)
		{
			if (DefaultStyle == null)
			{
				DefaultStyle = new GUIStyle(GUI.skin.box);
				DefaultStyle.alignment = (TextAnchor)3;
				DefaultStyle.fontSize = 20;
				DefaultStyle.margin = new RectOffset(3, 3, 3, 3);
				ApplyToStyle.BackgroundAllStates(null, DefaultStyle);
			}
			guiStyle = DefaultStyle;
		}
		int fontSize = guiStyle.fontSize;
		if (scale < 1f)
		{
			int fontSize2 = Mathf.RoundToInt((float)fontSize * scale);
			guiStyle.fontSize = fontSize2;
		}
		GUIContent val2 = new GUIContent(text);
		float screenScale = GetScreenScale();
		Vector2 val3 = guiStyle.CalcSize(val2);
		float num = val.x * screenScale - val3.x / 2f;
		float num2 = (float)Screen.height - (val.y * screenScale + val3.y);
		Rect val4 = default(Rect);
		((Rect)(ref val4))._002Ector(new Vector2(num, num2), val3);
		GUI.Label(val4, val2, guiStyle);
		guiStyle.fontSize = fontSize;
	}

	private static float GetScreenScale()
	{
		if (_nextCheckScreenTime < Time.time && ((Behaviour)CameraClass.Instance.SSAA).isActiveAndEnabled)
		{
			_nextCheckScreenTime = Time.time + 10f;
			_screenScale = (float)CameraClass.Instance.SSAA.GetOutputWidth() / (float)CameraClass.Instance.SSAA.GetInputWidth();
		}
		return _screenScale;
	}

	public static GameObject Sphere(Vector3 position, float size, Color color, float expiretime = -1f)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return null;
		}
		if (!SAINPlugin.DebugMode)
		{
			return null;
		}
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)0);
		val.GetComponent<Renderer>().material.color = color;
		val.GetComponent<Collider>().enabled = false;
		val.transform.position = new Vector3(position.x, position.y, position.z);
		val.transform.localScale = new Vector3(size, size, size);
		AddGizmo(val, expiretime);
		return val;
	}

	public static void UpdateSphere(GameObject Sphere, Vector3 position, float size, Color color)
	{
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		if (DrawGizmos && SAINPlugin.DebugMode && (Object)(object)Sphere != (Object)null)
		{
			Sphere.GetComponent<Renderer>().material.color = color;
			Sphere.transform.position = new Vector3(position.x, position.y, position.z);
			Sphere.transform.localScale = new Vector3(size, size, size);
		}
	}

	public static GameObject Box(Vector3 position, float length, float height, Color color, float expiretime = -1f)
	{
		//IL_0035: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return null;
		}
		if (!SAINPlugin.DebugMode)
		{
			return null;
		}
		GameObject val = GameObject.CreatePrimitive((PrimitiveType)3);
		val.GetComponent<Renderer>().material.color = color;
		val.GetComponent<Collider>().enabled = false;
		val.transform.position = position;
		val.transform.localScale = new Vector3(length * 2f, height * 2f, length * 2f);
		AddGizmo(val, expiretime);
		return val;
	}

	private static void AddGizmo(GameObject obj, float expireTime)
	{
		if (expireTime > 0f)
		{
			TempCoroutine.DestroyAfterDelay(obj, expireTime);
		}
		else
		{
			DrawnGizmos.Add(obj);
		}
	}

	public static GameObject Sphere(Vector3 position, float size, float expiretime = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return Sphere(position, size, RandomColor, expiretime);
	}

	public static GameObject Sphere(Vector3 position, float expiretime = 1f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		return Sphere(position, 0.25f, RandomColor, expiretime);
	}

	public static GameObject Line(Vector3 startPoint, Vector3 endPoint, Color color, float lineWidth = 0.1f, float expiretime = -1f, bool taperLine = false)
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Expected O, but got Unknown
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_005e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0067: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return null;
		}
		if (!SAINPlugin.DebugMode)
		{
			return null;
		}
		GameObject val = new GameObject();
		LineRenderer val2 = val.AddComponent<LineRenderer>();
		((Renderer)val2).material.color = color;
		val2.startWidth = lineWidth;
		val2.endWidth = (taperLine ? (lineWidth / 4f) : lineWidth);
		val2.SetPosition(0, startPoint);
		val2.SetPosition(1, endPoint);
		AddGizmo(val, expiretime);
		return val;
	}

	public static void UpdateLine(GameObject Line, Vector3 startPoint, Vector3 endPoint, float lineWidth, Color color)
	{
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		if (SAINPlugin.DebugMode && !((Object)(object)Line == (Object)null))
		{
			LineRenderer component = Line.GetComponent<LineRenderer>();
			if ((Object)(object)component != (Object)null)
			{
				((Renderer)component).material.color = color;
				component.startWidth = lineWidth;
				component.endWidth = lineWidth;
				component.SetPosition(0, startPoint);
				component.SetPosition(1, endPoint);
			}
		}
	}

	public static void UpdatePositionLine(Vector3 a, Vector3 b, GameObject gameObject)
	{
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		if (!((Object)(object)gameObject == (Object)null))
		{
			LineRenderer component = gameObject.GetComponent<LineRenderer>();
			if (component != null)
			{
				component.SetPosition(0, a);
			}
			if (component != null)
			{
				component.SetPosition(1, b);
			}
		}
	}

	public static GameObject Line(Vector3 startPoint, Vector3 endPoint, float lineWidth = 0.1f, float expiretime = 1f, bool taperLine = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		return Line(startPoint, endPoint, RandomColor, lineWidth, expiretime, taperLine);
	}

	public static GameObject Ray(Vector3 startPoint, Vector3 direction, Color color, float length = 0.35f, float lineWidth = 0.1f, bool temporary = false, float expiretime = 1f, bool taperLine = false)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0004: Unknown result type (might be due to invalid IL or missing references)
		//IL_000a: Unknown result type (might be due to invalid IL or missing references)
		//IL_000f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		Vector3 endPoint = startPoint + ((Vector3)(ref direction)).normalized * length;
		return Line(startPoint, endPoint, color, lineWidth, expiretime, taperLine);
	}

	public static List<GameObject> DrawLinesBetweenPoints(float lineSize, float raisePoints, params Vector3[] points)
	{
		return DrawLinesBetweenPoints(lineSize, -1f, raisePoints, points);
	}

	public static List<GameObject> DrawLinesBetweenPoints(params Vector3[] points)
	{
		return DrawLinesBetweenPoints(0.1f, -1f, 0f, points);
	}

	public static void DrawLinesToPoint(List<GameObject> list, Vector3 origin, Color color, float lineSize, float expireTime, float raisePoints, params Vector3[] points)
	{
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return;
		}
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 val = points[i];
			val.y += raisePoints;
			if (origin != points[i])
			{
				Vector3 direction = origin - val;
				float magnitude = ((Vector3)(ref direction)).magnitude;
				if (magnitude > 0.01f)
				{
					GameObject item = Ray(val, direction, color, magnitude, lineSize, expireTime > 0f, expireTime);
					list.Add(item);
				}
			}
		}
	}

	public static List<GameObject> DrawLinesToPoint(Vector3 origin, Color color, float lineSize, float expireTime, float raisePoints, params Vector3[] points)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return null;
		}
		List<GameObject> list = new List<GameObject>();
		DrawLinesToPoint(list, origin, color, lineSize, expireTime, raisePoints, points);
		return list;
	}

	public static void DrawSpheresAtPoints(List<GameObject> list, Color color, float size, float expireTime, float raisePoints, params Vector3[] points)
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		//IL_002b: Unknown result type (might be due to invalid IL or missing references)
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		if (DrawGizmos)
		{
			for (int i = 0; i < points.Length; i++)
			{
				Vector3 position = points[i];
				position.y += raisePoints;
				GameObject item = Sphere(position, size, color, expireTime);
				list.Add(item);
			}
		}
	}

	public static List<GameObject> DrawSpheresAtPoints(Color color, float size, float expireTime, float raisePoints, params Vector3[] points)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return null;
		}
		List<GameObject> list = new List<GameObject>();
		DrawSpheresAtPoints(list, color, size, expireTime, raisePoints, points);
		return list;
	}

	public static List<GameObject> DrawLinesBetweenPoints(float lineSize, float expireTime, float raisePoints, params Vector3[] points)
	{
		//IL_001f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0024: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_0069: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return null;
		}
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 val = points[i];
			val.y += raisePoints;
			Color randomColor = RandomColor;
			float size = Mathf.Clamp(lineSize * 1.5f, 0.15f, 10f);
			GameObject item = Sphere(val, size, randomColor, expireTime);
			list.Add(item);
			DrawLinesToPoint(list, val, randomColor, lineSize, expireTime, raisePoints, points);
		}
		return list;
	}

	public static List<GameObject> DrawLinesBetweenPoints(float lineSize, float expireTime, float raisePoints, Color color, params Vector3[] points)
	{
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		//IL_004b: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Unknown result type (might be due to invalid IL or missing references)
		if (!DrawGizmos)
		{
			return null;
		}
		List<GameObject> list = new List<GameObject>();
		for (int i = 0; i < points.Length; i++)
		{
			Vector3 val = points[i];
			val.y += raisePoints;
			float size = Mathf.Clamp(lineSize * 1.5f, 0.15f, 10f);
			GameObject item = Sphere(val, size, color, expireTime);
			list.Add(item);
			DrawLinesToPoint(list, val, color, lineSize, expireTime, raisePoints, points);
		}
		return list;
	}
}
