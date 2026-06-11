using EFT;
using EFT.EnvironmentEffect;
using SAIN.Components.PlayerComponentSpace;
using SAIN.Editor;
using SAIN.Helpers;
using SAIN.Models.Enums;
using SAIN.SAINComponent;
using UnityEngine;

namespace SAIN.Components;

public class SAINCamoClass : PlayerComponentBase
{
	public LayerMask GrassLayer = LayerMask.op_Implicit(0);

	public LayerMask BushLayer = LayerMask.op_Implicit(0);

	private float FreqencyTimer;

	private Collider[] BushColliders = (Collider[])(object)new Collider[5];

	private Collider[] GrassColliders = (Collider[])(object)new Collider[1];

	public ETimeOfDay TimeOfDay { get; private set; }

	public bool NearBush { get; private set; }

	public bool InsideBush { get; private set; }

	public bool OnGrass { get; private set; }

	public bool IsProne => base.Player.IsInPronePose;

	public EnvironmentType EnvironmentType { get; private set; }

	public SAINCamoClass(PlayerComponent playerComp)
		: base(playerComp)
	{
	}//IL_0002: Unknown result type (might be due to invalid IL or missing references)
	//IL_0007: Unknown result type (might be due to invalid IL or missing references)
	//IL_000e: Unknown result type (might be due to invalid IL or missing references)
	//IL_0013: Unknown result type (might be due to invalid IL or missing references)


	public void Start()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0021: Unknown result type (might be due to invalid IL or missing references)
		//IL_0026: Unknown result type (might be due to invalid IL or missing references)
		BushLayer = LayerMask.op_Implicit(LayerMask.NameToLayer("Foliage"));
		GrassLayer = LayerMask.op_Implicit(LayerMask.NameToLayer("Grass"));
	}

	public void Update()
	{
		//IL_002d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_0077: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ca: Unknown result type (might be due to invalid IL or missing references)
		//IL_00da: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0124: Unknown result type (might be due to invalid IL or missing references)
		//IL_0125: Unknown result type (might be due to invalid IL or missing references)
		//IL_012a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_01a0: Unknown result type (might be due to invalid IL or missing references)
		Player player = base.Player;
		if (!((Object)(object)player != (Object)null) || !(FreqencyTimer < Time.time))
		{
			return;
		}
		Vector3 position = base.Position;
		BotManagerComponent instance = BotManagerComponent.Instance;
		if ((Object)(object)instance != (Object)null)
		{
			TimeOfDay = instance.TimeVision.TimeOfDay;
		}
		EnvironmentManager instance2 = EnvironmentManager.Instance;
		if ((Object)(object)instance2 != (Object)null)
		{
			EnvironmentType = instance2.GetEnvironmentByPos(position);
		}
		FreqencyTimer = Time.time + 0.5f;
		for (int i = 0; i < BushColliders.Length; i++)
		{
			BushColliders[i] = null;
		}
		NearBush = Physics.OverlapSphereNonAlloc(player.MainParts[(BodyPartType)1].Position, 2f, BushColliders, LayerMask.op_Implicit(LayerMaskClass.HighPolyWithTerrainMaskAI)) > 0;
		bool insideBush = false;
		for (int j = 0; j < BushColliders.Length; j++)
		{
			if ((Object)(object)BushColliders[j] != (Object)null)
			{
				Vector3 val = ((Component)BushColliders[j]).transform.position - position;
				if (((Vector3)(ref val)).magnitude < 0.75f)
				{
					insideBush = true;
					break;
				}
			}
		}
		InsideBush = insideBush;
		for (int k = 0; k < GrassColliders.Length; k++)
		{
			GrassColliders[k] = null;
		}
		OnGrass = Physics.OverlapSphereNonAlloc(position, 0.5f, GrassColliders, LayerMask.op_Implicit(GrassLayer)) > 0;
	}

	public void OnDestroy()
	{
	}

	public bool IsBushBetween(Vector3 start)
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0017: Unknown result type (might be due to invalid IL or missing references)
		//IL_0018: Unknown result type (might be due to invalid IL or missing references)
		//IL_0020: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Unknown result type (might be due to invalid IL or missing references)
		Vector3 val = base.PlayerComponent.Transform.BodyPosition - start;
		RaycastHit val2 = default(RaycastHit);
		return Physics.SphereCast(start, 0.1f, ((Vector3)(ref val)).normalized, ref val2, ((Vector3)(ref val)).magnitude, LayerMask.op_Implicit(BushLayer));
	}

	public void OnGUI()
	{
		//IL_000e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0013: Unknown result type (might be due to invalid IL or missing references)
		//IL_0044: Unknown result type (might be due to invalid IL or missing references)
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_004c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0051: Unknown result type (might be due to invalid IL or missing references)
		//IL_0055: Unknown result type (might be due to invalid IL or missing references)
		//IL_005a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006a: Unknown result type (might be due to invalid IL or missing references)
		//IL_006c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0071: Unknown result type (might be due to invalid IL or missing references)
		//IL_0076: Unknown result type (might be due to invalid IL or missing references)
		//IL_007b: Unknown result type (might be due to invalid IL or missing references)
		//IL_007d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0153: Unknown result type (might be due to invalid IL or missing references)
		//IL_0156: Unknown result type (might be due to invalid IL or missing references)
		//IL_015b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0160: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0170: Unknown result type (might be due to invalid IL or missing references)
		//IL_017c: Unknown result type (might be due to invalid IL or missing references)
		//IL_017e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0181: Unknown result type (might be due to invalid IL or missing references)
		//IL_0186: Unknown result type (might be due to invalid IL or missing references)
		//IL_018a: Unknown result type (might be due to invalid IL or missing references)
		//IL_018f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ee: Expected O, but got Unknown
		//IL_00f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fa: Unknown result type (might be due to invalid IL or missing references)
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0103: Unknown result type (might be due to invalid IL or missing references)
		//IL_010a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0126: Unknown result type (might be due to invalid IL or missing references)
		//IL_012d: Unknown result type (might be due to invalid IL or missing references)
		//IL_013e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0146: Unknown result type (might be due to invalid IL or missing references)
		if (!SAINPlugin.DebugMode)
		{
			return;
		}
		GUIUtility.ScaleAroundPivot(RectLayout.ScaledPivot, Vector2.zero);
		for (int i = 0; i < BushColliders.Length; i++)
		{
			Collider val = BushColliders[i];
			if ((Object)(object)val != (Object)null)
			{
				Vector3 position = ((Component)val).transform.position;
				Bounds bounds = val.bounds;
				Vector3 val2 = ((Bounds)(ref bounds)).size;
				float magnitude = ((Vector3)(ref val2)).magnitude;
				Vector3 val3 = Camera.main.WorldToScreenPoint(position + Vector3.up);
				if (val3.z > 0f)
				{
					GUIStyle box = GUI.skin.box;
					object[] obj = new object[4]
					{
						((Object)val).name,
						null,
						null,
						null
					};
					PhysicMaterial material = val.material;
					obj[1] = ((material != null) ? ((Object)material).name : null);
					obj[2] = magnitude;
					obj[3] = EnvironmentType;
					GUIContent val4 = new GUIContent(string.Format("{0} : {1} : {2} : {3}", obj));
					Rect val5 = default(Rect);
					Vector2 val6 = box.CalcSize(val4);
					((Rect)(ref val5)).x = val3.x - val6.x / 2f;
					((Rect)(ref val5)).y = (float)Screen.height - (val3.y + val6.y);
					((Rect)(ref val5)).size = val6;
					GUI.Box(val5, val4, box);
				}
				val2 = position - base.Position;
				Color color = ((((Vector3)(ref val2)).magnitude < 0.75f) ? Color.blue : Color.green);
				bounds = val.bounds;
				val2 = ((Bounds)(ref bounds)).size;
				DebugGizmos.Sphere(position, ((Vector3)(ref val2)).magnitude, color, 1f);
			}
		}
	}
}
