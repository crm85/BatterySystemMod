using SAIN.Models.Enums;
using UnityEngine;

namespace SAIN.SAINComponent.Classes;

public class SoundDispersionData
{
	public float Dispersion;

	public Vector3 EstimatedPosition;

	public float DistanceDispersion;

	public float AngleDispersionX;

	public float AngleDispersionY;

	public float DispersionModifier;

	public ESoundDispersionType DispersionType;

	public SoundDispersionData(float defaults = 1f)
	{
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		EstimatedPosition = Vector3.zero;
		DistanceDispersion = defaults;
		AngleDispersionX = defaults;
		AngleDispersionY = defaults;
		DispersionModifier = defaults;
		DispersionType = ESoundDispersionType.None;
		Dispersion = 0f;
	}
}
