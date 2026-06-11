using System.Runtime.CompilerServices;
using UnityEngine;

namespace SAIN.Editor.Util;

public static class MouseDragClass
{
	[CompilerGenerated]
	private static class _003C_003EO
	{
		public static WindowFunction _003C0_003E__EmptyWindowFunc;
	}

	private static Rect FullScreen = new Rect(0f, 0f, (float)Screen.width, (float)Screen.height);

	private static GUIStyle BlankStyle;

	public static Rect DragRectangle = Rect.zero;

	private static Rect DrawPosition = new Rect(193f, 148f, 56f, 44f);

	public static Color color = Color.white;

	private static readonly Vector3[] mousePositions = (Vector3[])(object)new Vector3[2];

	private static readonly Vector2[] mousePositions2D = (Vector2[])(object)new Vector2[2];

	private static bool drawRect = false;

	public static void OnGUI()
	{
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0022: Expected O, but got Unknown
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0056: Unknown result type (might be due to invalid IL or missing references)
		//IL_005c: Expected O, but got Unknown
		if (BlankStyle == null)
		{
			BlankStyle = new GUIStyle(GUI.skin.window);
		}
		if (SAINEditor.DisplayingWindow && drawRect)
		{
			Rect fullScreen = FullScreen;
			object obj = _003C_003EO._003C0_003E__EmptyWindowFunc;
			if (obj == null)
			{
				WindowFunction val = EmptyWindowFunc;
				_003C_003EO._003C0_003E__EmptyWindowFunc = val;
				obj = (object)val;
			}
			FullScreen = GUI.Window(999, fullScreen, (WindowFunction)obj, "", BlankStyle);
		}
	}

	private static void EmptyWindowFunc(int i)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		DrawRectangle(DrawPosition, 1, color);
	}

	private static void DrawRectangle(Rect area, int frameWidth, Color color)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Expected O, but got Unknown
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		Texture2D val = new Texture2D(1, 1);
		val.SetPixel(0, 0, color);
		val.Apply();
		Rect val2 = area;
		((Rect)(ref val2)).height = frameWidth;
		GUI.DrawTexture(val2, (Texture)(object)val);
		((Rect)(ref val2)).y = ((Rect)(ref area)).yMax - (float)frameWidth;
		GUI.DrawTexture(val2, (Texture)(object)val);
		val2 = area;
		((Rect)(ref val2)).width = frameWidth;
		GUI.DrawTexture(val2, (Texture)(object)val);
		((Rect)(ref val2)).x = ((Rect)(ref area)).xMax - (float)frameWidth;
		GUI.DrawTexture(val2, (Texture)(object)val);
	}
}
