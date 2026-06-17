# Changelog

This changelog summarizes the project work visible in git history. The current
top section is based on the `spt_3.10_upgrade` branch at `37df055`, compared
against `upstream/master` / the previous `v1.6.0` line.

## [1.7.0] - 2026-06-12

### SPT 3.11 upgrade

- Updated the client plugin, assembly metadata, and server package to version
  `1.7.0`.
- Updated the server mod metadata for SPT `3.11.*`.
- Moved the C# client project to .NET Framework 4.8 with local SPT 3.11
  references, including `spt-core`, `spt-reflection`, and required Unity
  modules.
- Removed checked-in dependency DLLs from `BatterySystemClient/Dependencies`;
  the project now expects local SPT install references for builds.
- Refreshed CR123 and CR2032 bundle assets for the updated SPT target.

### Battery and device behavior

- Reworked central battery tracking around `batteryDictionary` and per-item
  drain multipliers.
- Added a once-per-second heartbeat that refreshes battery drain state and
  drains active powered equipment while in raid.
- Added safer local-equipment detection for active weapon, earpiece, headwear,
  tactical vest, armband, and pockets.
- Added compound-item traversal so nested powered devices and batteries are
  detected more reliably.
- Removed stale battery tracking entries when items leave local equipment.
- Added per-device drain multiplier support, including custom drain rates for
  PNV-10T, GPNVG-18, N-15, PVS-14, and T-7 night/thermal devices.
- Ensured powered devices with missing or empty batteries stop functioning
  visually and mechanically.
- Delayed bot battery insertion slightly so other mods can finish populating
  bot equipment first.
- Added configurable spawned battery charge minimum and maximum values.
- Kept boss-spawned batteries at full charge.

### Server item database

- Kept AA, CR123, CR2032, and car batteries as resource items with 100 max
  charge.
- Assigned CR123 and CR2032 prefab bundle paths and battery item sounds.
- Added battery slots to powered equipment, including NVGs, thermals, powered
  sights, headsets, flashlights, lasers, tactical combos, and configured
  electronics.
- Added battery compatibility to pockets and the SICC case.
- Added item-description text showing which battery type a powered item uses.
- Set bot equipment and weapon battery-slot spawn chances to 50%.
- Added configurable weapon durability randomization for trader weapons and
  world loot.
- Patched dynamic and static location loot generation so generated weapon loot
  receives durability values per raid.

### Sights, NVGs, and tactical devices

- Reworked sight tracking for the active weapon and when aiming down sights.
- Disabled powered reticles and optic night-vision functionality when an
  installed battery is missing or empty.
- Tracked battery-backed sight components on the equipped weapon even when
  visual controllers are refreshed.
- Updated NVG and thermal handling to wait for camera switch transitions before
  deciding whether to drain power.
- Added tactical light/laser tracking for active weapon devices and
  headwear-mounted devices.
- Made tactical devices drain only while active and shut off lasers/lights when
  their battery runs out.
- Kept the foldable sight/auto-unfold work disabled because auto-unfold is not
  implemented yet.

### Headsets and Realism compatibility

- Added a config toggle for headset battery behavior.
- Supported both earpiece-slot headsets and helmet-mounted headphones.
- Captured and restored vanilla audio mixer values when headset power changes.
- Added a soft dependency on Realism Mod.
- Integrated with Realism `DeafenController` and `HeadsetGainController` so
  unpowered headsets behave like missing or low-gain headsets.
- Preserved helmet deafening protection when Realism headset power is removed.

### Realism analyzers

- Added battery gating for Realism gas analyzer and Geiger counter support.
- Searched tactical vest, armband, and pockets for equipped analyzer items.
- Synced Realism `GearController` state so analyzer/geiger availability only
  reports true when the equipped device has a charged battery.
- Blocked Realism gas analyzer and Geiger audio when the relevant powered
  device is missing or empty.

### Aquapeps purification

- Added Aquapeps purification support for Realism food poisoning.
- Allowed Aquapeps to be combined with eligible drink items.
- Consumed the Aquapeps item and marked the drink as purified on successful
  combine.
- Persisted purified drink item IDs in a BepInEx config-side text file.
- Reduced purified drink toxin chance to 20% of the original chance.
- Added reduced poisoning effects and Realism food-poisoning sound handling for
  purified drinks.

### Quest presence detector

- Integrated a loose quest item proximity detector.
- Embedded the quest presence detector UI asset bundle into the client plugin.
- Added runtime trackers to loose quest items as they initialize.
- Attached trackers to already-spawned quest loot when the local player loads.
- Added a proximity circle, optional directional arrow, and optional
  notification when near loose quest items.
- Added config entries for detector enablement, arrow display, notification
  display, and detection radius.

### White flare train summon

- Added white flare train summoning.
- Started all unstarted trains on the map when the local player fires a
  successful white flare.
- Matched known white flare template IDs and added name/short-name fallback
  detection.
- Initialized train and carriage depart times when summoning trains that had not
  started yet.

### Smoke and SAIN behavior

- Added smoke grenade AI occlusion by spawning invisible foliage-layer
  colliders inside active smoke.
- Added smoke occluder config for enablement, density, radius multiplier, M18
  radius multiplier, and debug visuals.
- Replaced M18 smoke visuals with configurable airdrop-style smoke plumes when
  the airdrop visual prefab is available.
- Suppressed the vanilla M18 smoke effect when the replacement prefab loads
  successfully.
- Added graceful fallback to vanilla M18 smoke when the replacement visual cannot
  be loaded.
- Added a soft dependency on SAIN.
- Added optional SAIN retreat smoke behavior: bots entering SAIN `Retreat` can
  force-throw a smoke grenade if they have one, pass chance/cooldown checks, can
  use their hands, and have a valid trajectory.
- Added retreat smoke config for enablement, chance, cooldown, target distance,
  emergency toss fallback, and debug logging.

### Packaging and repository layout

- Added an install-ready `BatterySystem-mod` layout with compiled client DLL,
  server files, config, source, and battery bundles.
- Added the generated server JavaScript output under the packaged mod layout.
- Embedded `qpd_assets_all.bundle` in the client project.
- Added imported/reference trees used during integration work:
  `sain`, `GamePanelHUD-3.3.0`, and `QuestPresenceDetector-master`.
- Updated `.gitignore` for generated output, SPT types, temporary server files,
  local reference folders, and build artifacts.

### Stability and refactoring

- Added safe patch enablement for optional integration patches so missing
  Realism, SAIN, QPD, or smoke internals do not stop the whole mod from loading.
- Reworked battery drain iteration to snapshot dictionary keys before modifying
  tracking state.
- Centralized helpers for battery resource lookup, slot checks, battery-template
  detection, compound item traversal, and local equipment checks.
- Added defensive null checks around reflected SPT/EFT methods and runtime game
  objects.
- Added supporting source for a battery resource attribute cleanup patch; it is
  present in the project but not currently enabled during plugin startup.

## Earlier history / v1.6.0 and older

### Core battery system

- Created the BatterySystem client/server mod.
- Added AA, CR123, CR2032, and car battery resource support.
- Added battery drain to powered equipment instead of treating batteries as
  cosmetic items.
- Added server-side item database patching for battery resources, item
  descriptions, battery slots, and powered gear support.
- Added battery storage support for player pockets and the SICC case.
- Turned off powered devices when their battery is missing or depleted.

### Powered gear support

- Added battery support for NVGs and thermals.
- Added powered sight support for collimators, holographics, and hybrid optics.
- Disabled sight reticles when their batteries are depleted.
- Added tactical device support and split it into a dedicated patch class.
- Added headset battery support.
- Added support for helmet-mounted earpieces.
- Removed strict active-slot assumptions so headsets and headwear-attached
  devices can work correctly.

### Bot battery behavior

- Added bot battery insertion for gear with battery slots.
- Randomized spawned bot battery charge.
- Kept boss batteries at 100%.
- Fixed collection-modified errors during battery dictionary updates.

### Code organization

- Split sight, headset, tactical-device, and core battery logic into separate
  classes.
- Introduced constants for battery template IDs.
- Refactored helper methods such as `UpdateBatteryDictionary`,
  `IsInActiveSlot`, and headset lookup.
- Cleaned unused code and reduced nesting across earlier patches.
