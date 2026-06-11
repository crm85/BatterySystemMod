using EFT.UI;
using UnityEngine;

namespace SAIN.Editor.Util;

public static class MouseFunctions
{
	private const float MOUSE_FUNC_TIME = 0.2f;

	private static Vector2 _lastMousePos;

	private static float _mouseMoveTime;

	public static bool MouseIsMoving => _mouseMoveTime > Time.time;

	private static Vector2 MousePos => Event.current.mousePosition;

	public static void Update()
	{
		checkMouseEvents();
	}

	public static void OnGUI()
	{
	}

	public static bool CheckMouseDrag()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return (int)Event.current.type == 7 && CheckMouseDrag(GUILayoutUtility.GetLastRect());
	}

	public static bool CheckMouseDrag(Rect rect)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		return ((Rect)(ref MouseDragClass.DragRectangle)).Overlaps(rect);
	}

	public static bool IsMouseInside()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Invalid comparison between Unknown and I4
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		return (int)Event.current.type == 7 && IsMouseInside(GUILayoutUtility.GetLastRect());
	}

	private static void checkMouseEvents()
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_003c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0047: Unknown result type (might be due to invalid IL or missing references)
		Vector2 mousePosition = Event.current.mousePosition;
		Vector2 val = mousePosition - _lastMousePos;
		if (((Vector2)(ref val)).sqrMagnitude > 0.001f)
		{
			_mouseMoveTime = Time.time + 0.2f;
		}
		_lastMousePos = mousePosition;
		if ((int)Event.current.type == 0 && Event.current.button == 1)
		{
			Sounds.PlaySound((EUISoundType)2);
		}
	}

	public static bool IsMouseInside(Rect rect)
	{
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		return ((Rect)(ref rect)).Contains(Event.current.mousePosition);
	}

	public static bool IsNearMouse(Vector2 point, float distance = 20f)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		Vector2 val = MousePos - point;
		return ((Vector2)(ref val)).magnitude <= distance;
	}

	public static bool IsNearMouse(Rect rect, float widthDistance = 10f, float heightDistance = 10f)
	{
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_0042: Unknown result type (might be due to invalid IL or missing references)
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		//IL_0046: Unknown result type (might be due to invalid IL or missing references)
		if (((Rect)(ref rect)).Contains(MousePos))
		{
			return true;
		}
		Rect val = default(Rect);
		((Rect)(ref val))._002Ector(0f, 0f, widthDistance * 2f, heightDistance * 2f);
		((Rect)(ref val)).center = MousePos;
		Rect val2 = val;
		return ((Rect)(ref val2)).Overlaps(rect);
	}
}
