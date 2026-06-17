using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SmokeGrenadeClient.Configs;
using Comfort.Common;
using EFT;
using EFT.AssetsManager;
using EFT.SynchronizableObjects;
using HarmonyLib;
using SPT.Reflection.Patching;
using Systems.Effects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SmokeGrenadeClient
{
	public class SmokeGrenadeExplosionPatch : ModulePatch
	{
		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(SmokeGrenade), nameof(SmokeGrenade.OnExplosion));
		}

		[PatchPostfix]
		private static void Postfix(SmokeGrenade __instance)
		{
			if (__instance == null || !SmokeGrenadePlugin.InGame()) return;

			if (SmokeGrenadeConfig.EnableSmokeOccluders.Value
				&& __instance.GetComponent<SmokeOcclusionController>() == null)
			{
				__instance.gameObject.AddComponent<SmokeOcclusionController>().Init(__instance);
			}

			if (!SmokeVisualReplacement.ShouldReplaceM18Visual(__instance)) return;
			if (!AirdropSmokeVisualCache.TryEnsurePrefab()) return;
			if (__instance.GetComponent<M18AirdropSmokeController>() != null) return;

			__instance.gameObject.AddComponent<M18AirdropSmokeController>().Init(__instance);
		}
	}

	public class M18SmokeEffectSuppressionPatch : ModulePatch
	{
		private static bool _loggedSuppression;

		protected override MethodBase GetTargetMethod()
		{
			return AccessTools.Method(typeof(Effects), nameof(Effects.EmitGrenade),
				new[] { typeof(string), typeof(Vector3), typeof(Vector3) })
				?? AccessTools.GetDeclaredMethods(typeof(Effects)).FirstOrDefault(method =>
					method.Name == nameof(Effects.EmitGrenade)
					&& IsGrenadeEffectMethod(method));
		}

		[PatchPrefix]
		private static bool Prefix(object[] __args)
		{
			if (!SmokeGrenadeConfig.ReplaceM18SmokeVisual.Value) return true;
			if (!SmokeGrenadePlugin.InGame()) return true;
			if (__args == null || __args.Length == 0) return true;
			if (!SmokeVisualReplacement.IsM18SmokeEffect(__args[0] as string)) return true;

			bool canReplace = AirdropSmokeVisualCache.TryEnsurePrefab();
			if (canReplace && !_loggedSuppression)
			{
				Debug.Log("[SmokeGrenades] Suppressing vanilla M18 smoke effect; using airdrop plume replacement.");
				_loggedSuppression = true;
			}

			return !canReplace;
		}

		private static bool IsGrenadeEffectMethod(MethodInfo method)
		{
			ParameterInfo[] parameters = method.GetParameters();
			return (parameters.Length == 3 || parameters.Length == 4)
				&& parameters[0].ParameterType == typeof(string)
				&& parameters[1].ParameterType == typeof(Vector3)
				&& parameters[2].ParameterType == typeof(Vector3);
		}
	}

	internal static class SmokeVisualReplacement
	{
		internal const string M18TemplateId = "617aa4dd8166f034d57de9c5";
		internal const string M18SmokeEffectType = "grenade_smoke_m18_green";

		internal static bool ShouldReplaceM18Visual(SmokeGrenade grenade)
		{
			return SmokeGrenadeConfig.ReplaceM18SmokeVisual.Value
				&& SmokeGrenadePlugin.InGame()
				&& IsM18SmokeGrenade(grenade);
		}

		internal static bool IsM18SmokeGrenade(SmokeGrenade grenade)
		{
			if (grenade?.WeaponSource == null) return false;

			return string.Equals(grenade.WeaponSource.TemplateId, M18TemplateId, StringComparison.OrdinalIgnoreCase)
				|| IsM18SmokeEffect(grenade.WeaponSource.ExplosionEffectType);
		}

		internal static bool IsM18SmokeEffect(string effectType)
		{
			return string.Equals(effectType, M18SmokeEffectType, StringComparison.OrdinalIgnoreCase);
		}

		internal static float GetApproxSmokeRadius(SmokeGrenade grenade)
		{
			if (grenade == null) return 2f;

			float radius = Mathf.Max(1f, grenade.Radius);
			SmokeGrenadeSettings settings = grenade.SmokeGrenadeSettings_0;
			if (settings != null)
			{
				float settingsRadius = settings._initialRadius * Mathf.Max(0.01f, settings._radiusMultiplier);
				radius = Mathf.Max(radius, settingsRadius);

				SphereCollider area = settings._emissionArea;
				if (area != null)
				{
					Vector3 scale = area.transform.lossyScale;
					float maxScale = Mathf.Max(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z));
					radius = Mathf.Max(radius, area.radius * maxScale);
				}
			}

			return Mathf.Clamp(radius, 1.5f, 12f);
		}

		internal static float EvaluateSmokeSize(SmokeGrenade grenade, float elapsed)
		{
			if (grenade == null) return 0f;

			float normalTime = grenade.EmitTime > 0f ? elapsed / grenade.EmitTime : 1f;
			SmokeGrenadeSettings settings = grenade.SmokeGrenadeSettings_0;
			if (settings?._sizeOverTime != null && settings._sizeOverTime.length > 0)
			{
				return Mathf.Clamp01(settings._sizeOverTime.Evaluate(normalTime));
			}

			float lifeTime = Mathf.Max(1f, grenade.LifeTime);
			return Mathf.Clamp01(elapsed / Mathf.Min(5f, lifeTime));
		}
	}

	internal sealed class SmokeOcclusionController : MonoBehaviour
	{
		private sealed class Occluder
		{
			public Transform Transform;
			public CapsuleCollider Collider;
			public Vector2 UnitOffset;
			public float WidthScale;
			public GameObject DebugVisual;
		}

		private const float GoldenAngle = 137.507764f;
		private static bool _warnedMissingFoliageLayer;
		private static Material _debugMaterial;

		private readonly List<Occluder> _occluders = new List<Occluder>();
		private SmokeGrenade _grenade;
		private GameObject _root;
		private float _startTime;
		private float _lifeTime;
		private float _baseRadius;
		private float _radiusMultiplier;
		private float _heightMultiplier;

		public void Init(SmokeGrenade grenade)
		{
			if (_root != null) return;

			int foliageLayer = LayerMask.NameToLayer("Foliage");
			if (foliageLayer < 0)
			{
				if (!_warnedMissingFoliageLayer)
				{
					Debug.LogWarning("[SmokeGrenades] Smoke occluders disabled: Foliage layer was not found.");
					_warnedMissingFoliageLayer = true;
				}

				Destroy(this);
				return;
			}

			_grenade = grenade;
			_startTime = Time.time;
			_lifeTime = Mathf.Max(1f, grenade.LifeTime);
			_baseRadius = SmokeVisualReplacement.GetApproxSmokeRadius(grenade);
			bool isM18 = SmokeVisualReplacement.IsM18SmokeGrenade(grenade);
			_radiusMultiplier = SmokeGrenadeConfig.SmokeOccluderRadiusMultiplier.Value
				* (isM18 ? SmokeGrenadeConfig.M18SmokeOccluderRadiusMultiplier.Value : 1f);
			_heightMultiplier = isM18 ? 1.35f : 1f;

			_root = new GameObject("SmokeGrenades_SmokeOccluders");
			_root.layer = foliageLayer;
			_root.transform.SetPositionAndRotation(grenade.transform.position, Quaternion.identity);

			int baseCount = Mathf.Clamp(SmokeGrenadeConfig.SmokeOccluderDensity.Value, 4, 48);
			int count = Mathf.Clamp(Mathf.RoundToInt(baseCount * (isM18 ? 1.5f : 1f)), 4, 72);
			for (int i = 0; i < count; i++)
			{
				Occluder occluder = CreateOccluder(i, count, foliageLayer);
				_occluders.Add(occluder);
			}
		}

		private Occluder CreateOccluder(int index, int count, int foliageLayer)
		{
			GameObject obj = new GameObject("smoke_ai_foliage_occluder");
			obj.layer = foliageLayer;
			obj.transform.parent = _root.transform;
			obj.transform.localRotation = Quaternion.Euler(0f, index * GoldenAngle, 0f);

			CapsuleCollider collider = obj.AddComponent<CapsuleCollider>();
			collider.direction = 1;
			collider.isTrigger = true;
			collider.enabled = false;

			Vector2 unitOffset;
			if (index == 0)
			{
				unitOffset = Vector2.zero;
			}
			else
			{
				float angle = index * GoldenAngle * Mathf.Deg2Rad;
				float ring = Mathf.Sqrt((index - 0.25f) / Mathf.Max(1f, count - 1f));
				unitOffset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * ring;
			}

			return new Occluder
			{
				Transform = obj.transform,
				Collider = collider,
				UnitOffset = unitOffset,
				WidthScale = 0.85f + 0.3f * (((index * 37) % 100) / 100f)
			};
		}

		private void Update()
		{
			if (_grenade == null || _root == null)
			{
				Cleanup();
				return;
			}

			float elapsed = Time.time - _startTime;
			if (elapsed >= _lifeTime)
			{
				Cleanup();
				return;
			}

			float size = SmokeVisualReplacement.EvaluateSmokeSize(_grenade, elapsed);
			bool enabledState = size > 0.05f;
			bool showDebugVisuals = SmokeGrenadeConfig.SmokeOccluderDebugVisuals.Value;
			float currentRadius = _baseRadius * _radiusMultiplier * size;
			float colliderRadius = Mathf.Clamp(currentRadius / Mathf.Sqrt(Mathf.Max(1, _occluders.Count)) * 0.85f, 0.2f, 2.2f);
			float height = Mathf.Lerp(1.4f, 3.6f, size) * _heightMultiplier;

			_root.transform.position = _grenade.transform.position;
			_root.transform.rotation = Quaternion.identity;

			for (int i = 0; i < _occluders.Count; i++)
			{
				Occluder occluder = _occluders[i];
				if (occluder.Collider == null) continue;

				Vector2 offset = occluder.UnitOffset * currentRadius;
				occluder.Transform.localPosition = new Vector3(offset.x, 0f, offset.y);
				occluder.Collider.enabled = enabledState;
				occluder.Collider.radius = colliderRadius * occluder.WidthScale;
				occluder.Collider.height = Mathf.Max(height, occluder.Collider.radius * 2f);
				occluder.Collider.center = new Vector3(0f, occluder.Collider.height * 0.5f, 0f);
				UpdateDebugVisual(occluder, enabledState && showDebugVisuals);
			}
		}

		private static void UpdateDebugVisual(Occluder occluder, bool visible)
		{
			if (!visible)
			{
				if (occluder.DebugVisual != null)
				{
					occluder.DebugVisual.SetActive(false);
				}

				return;
			}

			if (occluder.DebugVisual == null)
			{
				occluder.DebugVisual = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
				occluder.DebugVisual.name = "smoke_ai_foliage_occluder_debug";
				occluder.DebugVisual.transform.parent = occluder.Transform;
				Collider debugCollider = occluder.DebugVisual.GetComponent<Collider>();
				if (debugCollider != null)
				{
					Destroy(debugCollider);
				}

				MeshRenderer renderer = occluder.DebugVisual.GetComponent<MeshRenderer>();
				if (renderer != null)
				{
					renderer.sharedMaterial = DebugMaterial;
				}
			}

			occluder.DebugVisual.SetActive(true);
			occluder.DebugVisual.transform.localPosition = occluder.Collider.center;
			occluder.DebugVisual.transform.localRotation = Quaternion.identity;
			occluder.DebugVisual.transform.localScale = new Vector3(
				occluder.Collider.radius * 2f,
				occluder.Collider.height * 0.5f,
				occluder.Collider.radius * 2f);
		}

		private static Material DebugMaterial
		{
			get
			{
				if (_debugMaterial != null) return _debugMaterial;

				Shader shader = Shader.Find("Hidden/Internal-Colored") ?? Shader.Find("Sprites/Default") ?? Shader.Find("Unlit/Color");
				_debugMaterial = new Material(shader)
				{
					hideFlags = HideFlags.HideAndDontSave,
					color = new Color(0.1f, 1f, 0.2f, 0.22f)
				};
				_debugMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
				_debugMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
				_debugMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
				_debugMaterial.SetInt("_ZWrite", 0);
				_debugMaterial.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
				return _debugMaterial;
			}
		}

		private void Cleanup()
		{
			if (_root != null)
			{
				Destroy(_root);
				_root = null;
			}

			_occluders.Clear();
			Destroy(this);
		}

		private void OnDestroy()
		{
			if (_root != null)
			{
				Destroy(_root);
				_root = null;
			}

			_occluders.Clear();
		}
	}

	internal sealed class M18AirdropSmokeController : MonoBehaviour
	{
		private const float GoldenAngle = 137.507764f;

		private SmokeGrenade _grenade;
		private GameObject _root;
		private float _startTime;
		private float _lifeTime;

		public void Init(SmokeGrenade grenade)
		{
			if (_root != null) return;

			_grenade = grenade;
			_startTime = Time.time;
			_lifeTime = Mathf.Max(1f, grenade.LifeTime);

			_root = new GameObject("SmokeGrenades_M18AirdropSmoke");
			_root.transform.SetPositionAndRotation(grenade.transform.position, Quaternion.identity);

			SpawnPlumes(grenade);
		}

		private void SpawnPlumes(SmokeGrenade grenade)
		{
			int plumeCount = Mathf.Clamp(SmokeGrenadeConfig.M18AirdropPlumeCount.Value, 1, 8);
			float plumeScale = Mathf.Clamp(SmokeGrenadeConfig.M18AirdropPlumeScale.Value, 0.5f, 4f);
			float offsetRadius = Mathf.Clamp(SmokeVisualReplacement.GetApproxSmokeRadius(grenade) * 0.25f, 0.5f, 2.5f);
			int spawned = 0;

			for (int i = 0; i < plumeCount; i++)
			{
				GameObject plume = AirdropSmokeVisualCache.InstantiatePlume(_root.transform);
				if (plume == null) continue;

				Vector3 offset = GetPlumeOffset(i, plumeCount, offsetRadius);
				float variantScale = plumeScale * (0.9f + 0.08f * (i % 3));
				plume.transform.localPosition = offset;
				plume.transform.localRotation = Quaternion.Euler(0f, i * GoldenAngle, 0f);
				plume.transform.localScale = Vector3.one * variantScale;
				AirdropSmokeVisualCache.ConfigureRuntimePlume(plume, variantScale);
				spawned++;
			}

			if (spawned == 0)
			{
				Cleanup();
				return;
			}

			Debug.Log("[SmokeGrenades] Spawned " + spawned + " airdrop smoke plume(s) for M18 smoke.");
		}

		private static Vector3 GetPlumeOffset(int index, int count, float radius)
		{
			if (count <= 1 || index == 0) return Vector3.zero;

			float angle = index * GoldenAngle * Mathf.Deg2Rad;
			float ring = Mathf.Sqrt((index - 0.25f) / Mathf.Max(1f, count - 1f));
			return new Vector3(Mathf.Cos(angle), 0f, Mathf.Sin(angle)) * radius * ring;
		}

		private void Update()
		{
			if (_grenade == null || _root == null)
			{
				Cleanup();
				return;
			}

			if (Time.time - _startTime >= _lifeTime)
			{
				Cleanup();
				return;
			}

			_root.transform.position = _grenade.transform.position;
		}

		private void Cleanup()
		{
			if (_root != null)
			{
				Destroy(_root);
				_root = null;
			}

			Destroy(this);
		}

		private void OnDestroy()
		{
			if (_root != null)
			{
				Destroy(_root);
				_root = null;
			}
		}
	}

	internal static class AirdropSmokeVisualCache
	{
		private static GameObject _plumePrefab;
		private static bool _warnedLoadFailure;
		private static float _nextLoadAttemptTime;
		private static bool _loggedStrippedIgnitionVisuals;

		internal static bool TryEnsurePrefab()
		{
			if (_plumePrefab != null) return true;
			if (_nextLoadAttemptTime > Time.realtimeSinceStartup) return false;

			SynchronizableObject syncObject = null;
			try
			{
				if (!Singleton<PoolManagerClass>.Instantiated || Singleton<PoolManagerClass>.Instance == null)
				{
					return FailLoad("PoolManager is not available yet.");
				}

				ResourceKey resourceKey;
				if (!ResourceKeyManagerAbstractClass.SynchronizableObjectPath.TryGetValue(SynchronizableObjectType.AirDrop, out resourceKey))
				{
					return FailLoad("Airdrop synchronizable object resource key was not found.");
				}

				syncObject = Singleton<PoolManagerClass>.Instance.CreateFromPool<SynchronizableObject>(resourceKey);
				AirdropSynchronizableObject airdrop = syncObject as AirdropSynchronizableObject;
				if (airdrop == null)
				{
					return FailLoad("Airdrop synchronizable object could not be created from the pool.");
				}

				if (airdrop.AirdropFlare == null)
				{
					return FailLoad("Airdrop flare visual was not found on the pooled airdrop object.");
				}

				_plumePrefab = Object.Instantiate(airdrop.AirdropFlare);
				_plumePrefab.name = "SmokeGrenades_AirdropSmokePlumePrefab";
				SanitizeNonVisualComponents(_plumePrefab);
				StripIgnitionVisuals(_plumePrefab);
				_plumePrefab.SetActive(false);
				Debug.Log("[SmokeGrenades] Loaded airdrop flare visual for M18 smoke replacement.");
				return true;
			}
			catch (Exception ex)
			{
				return FailLoad(ex.Message);
			}
			finally
			{
				if (syncObject != null)
				{
					try
					{
						AssetPoolObject.ReturnToPool(syncObject.gameObject);
					}
					catch (Exception ex)
					{
						Debug.LogWarning("[SmokeGrenades] Failed to return temporary airdrop object to pool: " + ex.Message);
					}
				}
			}
		}

		internal static GameObject InstantiatePlume(Transform parent)
		{
			if (!TryEnsurePrefab()) return null;

			GameObject plume = Object.Instantiate(_plumePrefab, parent);
			plume.name = "SmokeGrenades_M18_AirdropSmokePlume";
			SanitizeNonVisualComponents(plume);
			StripIgnitionVisuals(plume);
			plume.SetActive(true);
			return plume;
		}

		internal static void ConfigureRuntimePlume(GameObject plume, float scale)
		{
			if (plume == null) return;

			float smokeAmount = Mathf.Clamp(SmokeGrenadeConfig.M18AirdropSmokeAmount.Value, 0.5f, 6f);
			float particleScale = Mathf.Sqrt(Mathf.Max(0.1f, scale)) * Mathf.Sqrt(smokeAmount);
			ParticleSystem[] systems = plume.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = 0; i < systems.Length; i++)
			{
				ParticleSystem system = systems[i];
				if (IsIgnitionVisual(system.gameObject, system.GetComponent<Renderer>())) continue;

				ParticleSystem.MainModule main = system.main;
				main.loop = true;
				main.scalingMode = ParticleSystemScalingMode.Hierarchy;
				main.startSizeMultiplier *= particleScale * 1.15f;
				main.startSpeedMultiplier *= Mathf.Lerp(1f, particleScale, 0.45f);
				main.maxParticles = Mathf.CeilToInt(main.maxParticles * Mathf.Max(1f, smokeAmount));

				ParticleSystem.EmissionModule emission = system.emission;
				emission.rateOverTimeMultiplier *= Mathf.Max(1f, scale * smokeAmount * 1.35f);
				emission.rateOverDistanceMultiplier *= Mathf.Max(1f, scale * smokeAmount);

				ParticleSystem.ShapeModule shape = system.shape;
				if (shape.enabled)
				{
					shape.radius *= Mathf.Sqrt(smokeAmount);
				}

				system.Clear(true);
				system.Play(true);
			}
		}

		private static bool FailLoad(string reason)
		{
			_nextLoadAttemptTime = Time.realtimeSinceStartup + 10f;
			if (!_warnedLoadFailure)
			{
				Debug.LogWarning("[SmokeGrenades] M18 airdrop smoke replacement unavailable; vanilla M18 smoke will be used. " + reason);
				_warnedLoadFailure = true;
			}

			return false;
		}

		private static void SanitizeNonVisualComponents(GameObject obj)
		{
			AudioSource[] audioSources = obj.GetComponentsInChildren<AudioSource>(true);
			for (int i = 0; i < audioSources.Length; i++)
			{
				audioSources[i].Stop();
				audioSources[i].enabled = false;
			}

			Light[] lights = obj.GetComponentsInChildren<Light>(true);
			for (int i = 0; i < lights.Length; i++)
			{
				lights[i].enabled = false;
			}

			Collider[] colliders = obj.GetComponentsInChildren<Collider>(true);
			for (int i = 0; i < colliders.Length; i++)
			{
				colliders[i].enabled = false;
			}

			Rigidbody[] rigidbodies = obj.GetComponentsInChildren<Rigidbody>(true);
			for (int i = 0; i < rigidbodies.Length; i++)
			{
				rigidbodies[i].isKinematic = true;
				rigidbodies[i].detectCollisions = false;
			}
		}

		private static void StripIgnitionVisuals(GameObject obj)
		{
			int stripped = 0;
			ParticleSystem[] systems = obj.GetComponentsInChildren<ParticleSystem>(true);
			for (int i = 0; i < systems.Length; i++)
			{
				ParticleSystem system = systems[i];
				Renderer renderer = system.GetComponent<Renderer>();
				if (!IsIgnitionVisual(system.gameObject, renderer)) continue;

				ParticleSystem.EmissionModule emission = system.emission;
				emission.enabled = false;
				system.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
				if (renderer != null)
				{
					renderer.enabled = false;
				}
				stripped++;
			}

			Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				if (renderer is ParticleSystemRenderer) continue;
				if (!IsIgnitionVisual(renderer.gameObject, renderer)) continue;

				renderer.enabled = false;
				stripped++;
			}

			if (stripped > 0 && !_loggedStrippedIgnitionVisuals)
			{
				Debug.Log("[SmokeGrenades] Disabled " + stripped + " non-smoke flare/fire visual component(s) from M18 airdrop smoke.");
				_loggedStrippedIgnitionVisuals = true;
			}
		}

		private static bool IsIgnitionVisual(GameObject obj, Renderer renderer)
		{
			string descriptor = BuildVisualDescriptor(obj, renderer);
			if (ContainsAny(descriptor, "smoke", "smog", "dust", "cloud", "fog")) return false;

			return ContainsAny(descriptor,
				"fire",
				"flame",
				"flare",
				"spark",
				"ember",
				"burn",
				"glow",
				"flash",
				"ignit",
				"light");
		}

		private static string BuildVisualDescriptor(GameObject obj, Renderer renderer)
		{
			string descriptor = obj != null ? obj.name.ToLowerInvariant() : string.Empty;

			if (renderer != null)
			{
				Material[] materials = renderer.sharedMaterials;
				for (int i = 0; i < materials.Length; i++)
				{
					Material material = materials[i];
					if (material == null) continue;

					descriptor += " " + material.name.ToLowerInvariant();
					if (material.shader != null)
					{
						descriptor += " " + material.shader.name.ToLowerInvariant();
					}
				}
			}

			Transform parent = obj != null ? obj.transform.parent : null;
			for (int depth = 0; parent != null && depth < 2; depth++)
			{
				descriptor += " " + parent.name.ToLowerInvariant();
				parent = parent.parent;
			}

			return descriptor;
		}

		private static bool ContainsAny(string text, params string[] tokens)
		{
			for (int i = 0; i < tokens.Length; i++)
			{
				if (text.Contains(tokens[i])) return true;
			}

			return false;
		}
	}
}
