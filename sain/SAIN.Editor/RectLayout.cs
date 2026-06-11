using SAIN.Plugin;
using UnityEngine;

namespace SAIN.Editor;

public static class RectLayout
{
	public static Rect MainWindow = new Rect(0f, 0f, 1920f, 1080f);

	private const float RectHeight = 30f;

	private const float ExitWidth = 30f;

	private const float SaveAllWidth = 175f;

	private const float AdvWidth = 225f;

	private static readonly float ExitStartX = ((Rect)(ref MainWindow)).width - 30f;

	private static readonly float SaveAllStartX = ExitStartX - 175f - 5f;

	private static readonly float AdvRectStartX = SaveAllStartX - 225f - 5f;

	private static readonly float DragWidth = AdvRectStartX - 5f;

	public static Rect ExitRect = new Rect(ExitStartX, 0f, 30f, 30f);

	public static Rect DragRect = new Rect(0f, 0f, DragWidth, 30f);

	public static Rect SaveAllRect = new Rect(SaveAllStartX, 0f, 175f, 30f);

	public static Rect AdvRect = new Rect(AdvRectStartX, 0f, 225f, 30f);

	public static Vector2 ScaledPivot => GetScaling();

	private static float ReferenceResX => 1920f * PresetHandler.EditorDefaults.ConfigScaling;

	private static float ReferenceResY => 1080f * PresetHandler.EditorDefaults.ConfigScaling;

	private static Vector2 GetScaling()
	{
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		float num = Mathf.Min((float)Screen.width / ReferenceResX, (float)Screen.height / ReferenceResY);
		return new Vector2(num, num);
	}
}
