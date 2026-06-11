using BatterySystem.Configs;
using BepInEx.Logging;
using Comfort.Common;
using EFT;
using EFT.Communications;
using EFT.Interactive;
using EFT.InventoryLogic;
using HarmonyLib;
using SPT.Reflection.Patching;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace BatterySystem
{
    internal static class QuestPresenceDetector
    {
        private static ManualLogSource _logger;
        private static GameObject _interfacePrefab;
        private static bool _loadAttempted;
        private static readonly HashSet<string> _trackedLootItemIds = new HashSet<string>();

        public static void LoadInterfaceBundle(ManualLogSource logger)
        {
            _logger = logger;
            if (_loadAttempted) return;

            _loadAttempted = true;

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetManifestResourceNames()
                    .FirstOrDefault(name => name.EndsWith("qpd_assets_all.bundle", StringComparison.OrdinalIgnoreCase));
                if (resourceName == null)
                {
                    LogWarning("Quest presence detector bundle resource was not found.");
                    return;
                }

                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    if (stream == null)
                    {
                        LogWarning("Quest presence detector bundle stream was not available.");
                        return;
                    }

                    stream.CopyTo(memoryStream);
                    AssetBundle bundle = AssetBundle.LoadFromMemory(memoryStream.ToArray());
                    if (bundle == null)
                    {
                        LogWarning("Quest presence detector bundle failed to load.");
                        return;
                    }

                    _interfacePrefab = bundle.LoadAsset<GameObject>("QPD.prefab");
                    if (_interfacePrefab == null)
                        LogWarning("Quest presence detector interface prefab was not found in the bundle.");
                }
            }
            catch (Exception ex)
            {
                LogWarning("Quest presence detector bundle load failed: " + ex.Message);
            }
        }

        public static void CreateInterface()
        {
            if (!BatterySystemConfig.EnableQuestPresenceDetector.Value) return;
            if (QuestPresenceInterface.Instance != null) return;

            if (!_loadAttempted)
                LoadInterfaceBundle(_logger);

            if (_interfacePrefab == null) return;

            GameObject interfaceObject = UnityEngine.Object.Instantiate(_interfacePrefab);
            interfaceObject.AddComponent<QuestPresenceInterface>();
        }

        public static void AttachToExistingQuestLootItems()
        {
            if (!BatterySystemConfig.EnableQuestPresenceDetector.Value) return;

            foreach (LootItem lootItem in UnityEngine.Object.FindObjectsOfType<LootItem>())
                AttachToLootItem(lootItem, lootItem?.Item);
        }

        public static void AttachToLootItem(LootItem lootItem, Item item)
        {
            if (!BatterySystemConfig.EnableQuestPresenceDetector.Value) return;
            if (lootItem == null || item == null || !IsQuestItem(item)) return;
            if (!lootItem.isActiveAndEnabled) return;

            string trackingId = item.Id ?? lootItem.GetInstanceID().ToString();
            if (!_trackedLootItemIds.Add(trackingId)) return;

            GameObject tracker = new GameObject(lootItem.gameObject.name + "_qpd_tracker");
            QuestPresenceComponent component = tracker.AddComponent<QuestPresenceComponent>();
            component.SetItem(lootItem.gameObject, GetItemName(item));
            tracker.transform.SetPositionAndRotation(lootItem.gameObject.transform.position, lootItem.gameObject.transform.rotation);
        }

        public static bool IsQuestItem(Item item)
        {
            if (item == null) return false;

            PropertyInfo questItemProperty = item.GetType().GetProperty("QuestItem", BindingFlags.Instance | BindingFlags.Public);
            if (questItemProperty != null && questItemProperty.PropertyType == typeof(bool))
                return (bool)questItemProperty.GetValue(item, null);

            return false;
        }

        public static string GetItemName(Item item)
        {
            if (item == null) return "quest item";

            Type localizationType = AccessTools.TypeByName("GClass2112");
            MethodInfo localizedNameMethod = AccessTools.Method(localizationType, "LocalizedName", new[] { typeof(Item) });
            object helperName = localizedNameMethod?.Invoke(null, new object[] { item });
            if (helperName is string helperNameText && !string.IsNullOrWhiteSpace(helperNameText))
                return helperNameText;

            PropertyInfo localizedNameProperty = item.GetType().GetProperty("LocalizedName", BindingFlags.Instance | BindingFlags.Public);
            object localizedName = localizedNameProperty?.GetValue(item, null);
            if (localizedName is string localizedNameText && !string.IsNullOrWhiteSpace(localizedNameText))
                return localizedNameText;

            PropertyInfo nameProperty = item.GetType().GetProperty("Name", BindingFlags.Instance | BindingFlags.Public);
            object name = nameProperty?.GetValue(item, null);
            if (name is string nameText && !string.IsNullOrWhiteSpace(nameText))
                return nameText;

            return item.StringTemplateId ?? "quest item";
        }

        public static void LogWarning(string message)
        {
            _logger?.LogWarning(message);
        }
    }

    internal sealed class QuestPresenceComponent : TriggerWithId
    {
        private GameObject _item;
        private float _radius;
        private string _name;

        public void SetItem(GameObject item, string name)
        {
            _item = item;
            _name = name;
        }

        public override void Awake()
        {
            SphereCollider trigger = gameObject.AddComponent<SphereCollider>();
            trigger.isTrigger = true;
            trigger.radius = BatterySystemConfig.QuestPresenceAreaSize.Value;
            trigger.center = Vector3.zero;
            _radius = trigger.radius;

            base.Awake();
            SetId(gameObject.name + "_qpd_tracker");
        }

        public override void TriggerEnter(Player player)
        {
            if (!HasTrackedItem())
            {
                DisableInterfaceAndDestroy();
                return;
            }

            if (player == null || !player.IsYourPlayer) return;

            Show(player);
        }

        public override void TriggerExit(Player player)
        {
            if (!HasTrackedItem())
            {
                DisableInterfaceAndDestroy();
                return;
            }

            if (player == null || !player.IsYourPlayer) return;

            Hide();
        }

        private void Update()
        {
            if (!HasTrackedItem())
            {
                DisableInterfaceAndDestroy();
                return;
            }

            Player player = Singleton<GameWorld>.Instance?.MainPlayer;
            if (player == null || !player.IsYourPlayer) return;

            bool inside = (_item.transform.position - player.Position).sqrMagnitude <= _radius * _radius;
            if (inside && !_localPlayerInside)
                Show(player);
            else if (!inside && _localPlayerInside)
                Hide();
        }

        private bool _localPlayerInside;

        private void Show(Player player)
        {
            _localPlayerInside = true;
            QuestPresenceInterface.Instance?.Toggle(true);
            QuestPresenceInterface.Instance?.SetData(player, _item.transform, _radius);
            if (BatterySystemConfig.QuestPresenceShowNotification.Value)
                NotificationManagerClass.DisplayMessageNotification("Nearing objective item '" + _name + "'");
        }

        private void Hide()
        {
            _localPlayerInside = false;
            if (QuestPresenceInterface.Instance?.IsTracking(_item.transform) == true)
                QuestPresenceInterface.Instance.Toggle(false);
        }

        private bool HasTrackedItem()
        {
            return _item != null && _item.gameObject.activeSelf;
        }

        private void DisableInterfaceAndDestroy()
        {
            QuestPresenceInterface.Instance?.Toggle(false);
            Destroy(gameObject);
        }
    }

    internal sealed class QuestPresenceInterface : MonoBehaviour
    {
        public static QuestPresenceInterface Instance { get; private set; }

        private Image _circle;
        private Image _arrow;
        private bool _showArrow;
        private Transform _currentItem;
        private Player _player;
        private float _radius;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (!TryBindImages())
            {
                QuestPresenceDetector.LogWarning("Quest presence detector interface prefab did not match the expected layout.");
                Destroy(gameObject);
                return;
            }

            _showArrow = BatterySystemConfig.QuestPresenceShowArrow.Value;
            Instance = this;

            BatterySystemConfig.QuestPresenceShowArrow.SettingChanged += ShowArrow_SettingChanged;
            Toggle(false);
        }

        private bool TryBindImages()
        {
            if (transform.childCount == 0) return false;

            Transform firstChild = transform.GetChild(0);
            if (firstChild.childCount == 0) return false;

            Transform circleTransform = firstChild.GetChild(0);
            _circle = circleTransform.GetComponent<Image>();
            if (_circle == null || circleTransform.childCount == 0) return false;

            _arrow = circleTransform.GetChild(0).GetComponent<Image>();
            return _arrow != null;
        }

        private void LateUpdate()
        {
            if (_currentItem == null || _player == null || !_currentItem.gameObject.activeSelf)
            {
                Toggle(false);
                return;
            }

            Vector3 offset = _currentItem.position - _player.Position;
            float distance = Mathf.Sqrt(offset.sqrMagnitude);
            if (distance > _radius) return;

            const float fullScaleDistance = 1.2f;
            float percentage = Mathf.Clamp01((_radius - distance) / (_radius - fullScaleDistance));
            _circle.fillAmount = percentage;

            if (!_showArrow || Camera.main == null) return;

            Vector3 localTarget = Camera.main.transform.InverseTransformPoint(_currentItem.position);
            float angleRadians = Mathf.Atan2(localTarget.x, localTarget.z);
            float angleDegrees = angleRadians * Mathf.Rad2Deg;
            _arrow.rectTransform.localRotation = Quaternion.Euler(0f, 0f, -angleDegrees);
        }

        public void Toggle(bool state)
        {
            enabled = state;
            if (_circle != null)
                _circle.enabled = state;
            if (_arrow != null)
                _arrow.enabled = state && _showArrow;

            if (!state)
            {
                if (_circle != null)
                    _circle.fillAmount = 0f;
                ClearData();
            }
        }

        public void SetData(Player player, Transform item, float radius)
        {
            _player = player;
            _currentItem = item;
            _radius = radius;
        }

        public bool IsTracking(Transform item)
        {
            return _currentItem == item;
        }

        private void ClearData()
        {
            _player = null;
            _currentItem = null;
            _radius = 0f;
        }

        private void ShowArrow_SettingChanged(object sender, EventArgs e)
        {
            _showArrow = BatterySystemConfig.QuestPresenceShowArrow.Value;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;

            if (BatterySystemConfig.QuestPresenceShowArrow != null)
                BatterySystemConfig.QuestPresenceShowArrow.SettingChanged -= ShowArrow_SettingChanged;
        }
    }

    internal sealed class QuestPresenceLootItemPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(LootItem).GetMethod(nameof(LootItem.Init));
        }

        [PatchPostfix]
        public static void PatchPostfix(LootItem __instance, Item item)
        {
            QuestPresenceDetector.AttachToLootItem(__instance, item);
        }
    }

    internal sealed class QuestPresencePlayerPatch : ModulePatch
    {
        protected override MethodBase GetTargetMethod()
        {
            return typeof(Player).GetMethod(nameof(Player.method_83));
        }

        [PatchPostfix]
        public static void PatchPostfix(Player __instance)
        {
            if (!BatterySystemConfig.EnableQuestPresenceDetector.Value) return;
            if (__instance == null || !__instance.IsYourPlayer || __instance is HideoutPlayer) return;
            if (Singleton<GameWorld>.Instance is HideoutGameWorld) return;

            QuestPresenceDetector.CreateInterface();
            QuestPresenceDetector.AttachToExistingQuestLootItems();
        }
    }
}
