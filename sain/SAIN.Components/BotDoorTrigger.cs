using EFT.Interactive;
using UnityEngine;

namespace SAIN.Components;

public class BotDoorTrigger : MonoBehaviour
{
	private Door _door;

	private DoorHandler _doorHandler;

	public SphereCollider SphereCollider { get; private set; }

	public void Awake()
	{
		SphereCollider = ((Component)this).gameObject.AddComponent<SphereCollider>();
		((Collider)SphereCollider).isTrigger = true;
		SphereCollider.radius = 10f;
		((Collider)SphereCollider).enabled = true;
		_doorHandler = GameWorldComponent.Instance.Doors;
	}

	public void Update()
	{
	}

	public void initDoor(Door door)
	{
		//IL_0014: Unknown result type (might be due to invalid IL or missing references)
		//IL_0030: Unknown result type (might be due to invalid IL or missing references)
		_door = door;
		((Component)this).transform.position = ((Component)door).transform.position;
		((Component)SphereCollider).transform.position = ((Component)this).transform.position;
	}

	public void OnDestroy()
	{
	}

	public void OnTriggerEnter(Collider other)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		//IL_0043: Unknown result type (might be due to invalid IL or missing references)
		GameObject gameObject = ((Component)other).gameObject;
		Logger.LogDebug("Enter: " + ((gameObject != null) ? ((Object)gameObject).name : null));
		if ((int)((WorldInteractiveObject)_door).DoorState == 2)
		{
			bool shallInvert = shallInvertDoorAngle(_door, ((Component)other).transform.position);
			_doorHandler.ChangeDoorState(_door, (EDoorState)4, shallInvert);
			Logger.LogInfo("open");
		}
	}

	public void OnTriggerStay(Collider other)
	{
		//IL_0007: Unknown result type (might be due to invalid IL or missing references)
		//IL_000d: Invalid comparison between Unknown and I4
		if ((int)((WorldInteractiveObject)_door).DoorState == 4)
		{
			Collider[] components = ((Component)_door).gameObject.GetComponents<Collider>();
			Collider[] array = components;
			foreach (Collider val in array)
			{
				Physics.IgnoreCollision(other, val, true);
			}
		}
	}

	public void OnTriggerExit(Collider other)
	{
		//IL_0029: Unknown result type (might be due to invalid IL or missing references)
		//IL_002f: Invalid comparison between Unknown and I4
		GameObject gameObject = ((Component)other).gameObject;
		Logger.LogDebug("Exit: " + ((gameObject != null) ? ((Object)gameObject).name : null));
		if ((int)((WorldInteractiveObject)_door).DoorState == 4)
		{
			_doorHandler.ChangeDoorState(_door, (EDoorState)2, shallInvert: false);
			Logger.LogInfo("close");
		}
		Collider[] components = ((Component)_door).gameObject.GetComponents<Collider>();
		Collider[] array = components;
		foreach (Collider val in array)
		{
			Physics.IgnoreCollision(other, val, false);
		}
	}

	private bool shallInvertDoorAngle(Door door, Vector3 colliderPosition)
	{
		//IL_0002: Unknown result type (might be due to invalid IL or missing references)
		//IL_0003: Unknown result type (might be due to invalid IL or missing references)
		//IL_0008: Unknown result type (might be due to invalid IL or missing references)
		//IL_0009: Unknown result type (might be due to invalid IL or missing references)
		//IL_0010: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Invalid comparison between Unknown and I4
		//IL_0022: Unknown result type (might be due to invalid IL or missing references)
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		GStruct425 interactionParameters = ((WorldInteractiveObject)door).GetInteractionParameters(colliderPosition);
		if (interactionParameters.AnimationId == (((int)((WorldInteractiveObject)door).DoorState != 1) ? ((WorldInteractiveObject)door).CalculateInteractionIndex(colliderPosition) : ((int)((WorldInteractiveObject)door).DoorKeyOpenInteraction)))
		{
			return false;
		}
		return true;
	}
}
