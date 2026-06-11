using System.Text;
using UnityEngine;

namespace SAIN.Helpers;

public sealed class GUIObject
{
	public Vector3 WorldPos;

	public string Text;

	public GUIStyle Style;

	public float Scale = 1f;

	public StringBuilder StringBuilder = new StringBuilder();

	public bool Enabled = true;
}
