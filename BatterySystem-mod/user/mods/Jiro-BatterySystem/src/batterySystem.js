"use strict";
var __createBinding = (this && this.__createBinding) || (Object.create ? (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    var desc = Object.getOwnPropertyDescriptor(m, k);
    if (!desc || ("get" in desc ? !m.__esModule : desc.writable || desc.configurable)) {
      desc = { enumerable: true, get: function() { return m[k]; } };
    }
    Object.defineProperty(o, k2, desc);
}) : (function(o, m, k, k2) {
    if (k2 === undefined) k2 = k;
    o[k2] = m[k];
}));
var __setModuleDefault = (this && this.__setModuleDefault) || (Object.create ? (function(o, v) {
    Object.defineProperty(o, "default", { enumerable: true, value: v });
}) : function(o, v) {
    o["default"] = v;
});
var __importStar = (this && this.__importStar) || function (mod) {
    if (mod && mod.__esModule) return mod;
    var result = {};
    if (mod != null) for (var k in mod) if (k !== "default" && Object.prototype.hasOwnProperty.call(mod, k)) __createBinding(result, mod, k);
    __setModuleDefault(result, mod);
    return result;
};
Object.defineProperty(exports, "__esModule", { value: true });
const config = __importStar(require("../config/config.json"));
class Mod {
    constructor() {
        this.batteryType = "";
        this.weaponBaseValues = {};
        this.weaponValueLow = 0;
        this.weaponValueHigh = 1;
        this.aaBatteryID = "5672cb124bdc2d1a0f8b4568";
        this.cr2032BatteryID = "5672cb304bdc2dc2088b456a";
        this.cr123BatteryID = "590a358486f77429692b2790";
        this.carBatteryID = "5733279d245977289b77ec24";
        this.weaponID = "5422acb9af1c889c16000029";
        this.specialScopeID = "55818aeb4bdc2ddc698b456a";
        this.nightVisionID = "5a2c3a9486f774688b05e574";
        this.thermalVisionID = "5d21f59b6dbe99052b54ef83";
        this.collimatorID = "55818ad54bdc2ddc698b4569";
        this.compactCollimatorID = "55818acf4bdc2dde698b456b";
        this.assaultScopeID = "55818add4bdc2d5b648b456f";
        this.opticScopeID = "55818ae44bdc2dde698b456c";
        this.headsetID = "5645bcb74bdc2ded0b8b4578";
        this.flashlightID = "55818b084bdc2d5b648b4571";
        this.lightLaserDesignatorID = "55818b0e4bdc2dde698b456e";
        this.tacticalComboID = "55818b164bdc2ddc698b456c";
        this.electronicsID = "57864a66245977548f04a81f";
        this.gearModID = "55802f3e4bdc2de7118b4584";
    }
    postDBLoad(container) {
        var _a;
        //const CustomItem = container.resolve<CustomItemService>("CustomItemService");
        const logger = container.resolve("WinstonLogger");
        const db = container.resolve("DatabaseServer");
        const tables = db.getTables();
        const locales = Object.values(tables.locales.global);
        const botDB = tables.bots.types;
        const items = tables.templates.items;
        const hideoutProduction = tables.hideout.production;
        const aaBatteryID = this.aaBatteryID;
        const dBatteryID = this.cr2032BatteryID;
        const rchblBatteryID = this.cr123BatteryID;
        const carBatteryID = this.carBatteryID;
        //const flirID = "5d1b5e94d7ad1a2b865a96b0";
        //Flir has a built-in battery. the battery doesn't show anywhere so no point
        //items[flirID]._props.MaxResource = 100;
        //items[flirID]._props.Resource = 0.05;
        if (config.BatteryFeatures?.Enabled !== false) {
            items[aaBatteryID]._props.MaxResource = 100;
            items[aaBatteryID]._props.Resource = 100;
            items[aaBatteryID]._props.ItemSound = "food_tin_can";
            items[rchblBatteryID]._props.MaxResource = 100;
            items[rchblBatteryID]._props.Resource = 100;
            items[rchblBatteryID]._props.Prefab.path = "batteries/cr123.bundle";
            items[rchblBatteryID]._props.ItemSound = "food_tin_can";
            items[dBatteryID]._props.MaxResource = 100;
            items[dBatteryID]._props.Resource = 100;
            items[dBatteryID]._props.Prefab.path = "batteries/cr2032.bundle";
            items[dBatteryID]._props.ItemSound = "food_tin_can";
            items[carBatteryID]._props.MaxResource = 100;
            items[carBatteryID]._props.Resource = 100;
            //Credit to Jehree! // 16 locales, wtf?
            for (const locale of locales) {
                locale[`${rchblBatteryID} Name`] = "CR123 Rechargeable Battery";
                locale[`${rchblBatteryID} ShortName`] = "CR123";
                locale[`${rchblBatteryID} Description`] = "A singular CR123A Battery. These are commonly used in military and hunting sights.";
                locale[`${dBatteryID} Name`] = "CR2032 Battery";
                locale[`${dBatteryID} ShortName`] = "CR2032";
                locale[`${dBatteryID} Description`] = "A multipurpose CR2032 Battery. Used from personal computers to military grade sights.";
            }
            ;
            // huge thanks and credit to jbs4mx! https://github.com/jbs4bmx/SpecialSlots/
            const pockets = items["627a4e6b255f7527fb05a0f6"];
            this.pushUnique(pockets._props.Slots[0]._props.filters[0].Filter, dBatteryID, rchblBatteryID, aaBatteryID);
            this.pushUnique(pockets._props.Slots[1]._props.filters[0].Filter, dBatteryID, rchblBatteryID, aaBatteryID);
            this.pushUnique(pockets._props.Slots[2]._props.filters[0].Filter, dBatteryID, rchblBatteryID, aaBatteryID);
            //S I C C case now fits batteries in it
            this.pushUnique(items["5d235bb686f77443f4331278"]._props.Grids[0]._props.filters[0].Filter, dBatteryID, rchblBatteryID, aaBatteryID);
            //add battery slots to wanted items
            for (let id in items) {
                if (this.shouldAddBatterySlot(id, items)) {
                    this.batteryType = this.getBatteryType(id, items);
                    if (this.hasConfiguredBatteryType(id)) {
                        this.prepareConfiguredBatterySlotItem(id, items);
                    }
                    for (const locale of locales) { // Item description now includes the battery type
                        const oldDescription = locale[`${id} Description`] ?? "";
                        const batteryName = locale[`${this.batteryType} Name`] ?? this.batteryType;
                        const newDescription = oldDescription.startsWith("Uses ")
                            ? oldDescription
                            : "Uses " + batteryName + "\n\n" + oldDescription;
                        locale[`${id} Description`] = newDescription;
                    }
                    const slots = (_a = items[id]._props).Slots ?? (_a.Slots = []);
                    if (slots.some(slot => slot._name === "mod_equipment"))
                        continue;
                    slots.push({
                        "_name": "mod_equipment",
                        "_id": "id_" + id.toLowerCase(),
                        "_parent": "parent_" + id.toLowerCase(),
                        "_props": {
                            "filters": [
                                {
                                    "Shift": 0,
                                    "Filter": [
                                        this.batteryType
                                    ]
                                }
                            ]
                        },
                        "_required": false,
                        "_mergeSlotWithChildren": false,
                        "_proto": "55d30c4c4bdc2db4468b457e"
                    });
                }
            }
            //change spawn% for batteries on bots. the durability is adjusted in a patch.
            //make spawn chance lower for scavs in the future?
            for (let bot in botDB) {
                if (botDB[bot].chances.equipmentMods != undefined) {
                    botDB[bot].chances.equipmentMods.mod_equipment = 50;
                }
                if (botDB[bot].chances.weaponMods != undefined) {
                    botDB[bot].chances.weaponMods.mod_equipment = 50;
                }
            }
        }
        else {
            logger.info("BatterySystem battery features are disabled by config.");
        }
        if (config.WeaponDurability?.Enabled !== false) {
            this.configureWeaponDurability(items, tables.templates.handbook);
            const traderWeaponCount = this.applyTraderWeaponDurability(tables.traders, items);
            const locationWeaponCount = this.applyLocationWeaponDurability(tables.locations, items);
            this.patchLocationLootGenerator(container, items, logger);
            logger.success(`BatterySystem weapon durability has been applied to ${traderWeaponCount} trader weapons and ${locationWeaponCount} world loot weapon templates!`);
        }
        //Jaeger trade for cr2032
        /*
        db.getTables().traders["5c0647fdd443bc2504c2d371"].assort.items.push({
            "_id": "cr2032barter1",
            "_tpl": dBatteryID,
            "parentId": "hideout",
            "slotId": "hideout",
            "upd": {
                "StackObjectsCount": 14993,
                "BuyRestrictionMax": 4,
                "BuyRestrictionCurrent": 0
            }
        })
        db.getTables().traders["5c0647fdd443bc2504c2d371"].assort.barter_scheme["cr2032barter1"] =
            [
                [
                    {
                        "count": 3,
                        "_tpl": dBatteryID
                    }
                ]
            ];
        db.getTables().traders["5c0647fdd443bc2504c2d371"].assort.loyal_level_items["cr2032barter1"] = 1;
        */
        //add hideout crafts for batteries
        /*hideoutProduction.push(
            {
                "_id": "cr2032Craft0",
                "areaType": 10,
                "requirements": [
                    {
                        "areaType": 10, //Lavatory = 2, WorkBench = 10
                        "requiredLevel": 1,
                        "type": "Area"
                    },
                    {
                        "templateId": "544fb5454bdc2df8738b456a", //multiTool
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Tool"
                    },
                    {
                        "templateId": aaBatteryID, //aa battery
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    }
                ],
                "productionTime": 600, // seconds
                "needFuelForAllProductionTime": false,
                "locked": false,
                "endProduct": dBatteryID,
                "continuous": false,
                "count": 2,
                "productionLimitCount": 0,
                "isEncoded": false
            },
            {
                // Induction!
                "_id": "cr123Recharge0",
                "areaType": 2,
                "requirements": [
                    {
                        "areaType": 2, //Lavatory = 2, WorkBench = 10
                        "requiredLevel": 1,
                        "type": "Area"
                    },
                    {
                        "templateId": "590a391c86f774385a33c404", //magnet
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Tool"
                    },
                    {
                        "templateId": "5c06779c86f77426e00dd782", //Bundle of wires
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    },
                    {
                        "templateId": rchblBatteryID,
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    }
                ],
                "productionTime": 600, // seconds
                "needFuelForAllProductionTime": false,
                "locked": false,
                "endProduct": rchblBatteryID,
                "continuous": false,
                "count": 1,
                "productionLimitCount": 0,
                "isEncoded": false
            },
            /*{ // Car Battery Recharge Test
                "_id": "carBatteryTest1",
                "areaType": 2,
                "requirements": [
                    {
                        "areaType": 2,
                        "requiredLevel": 2,
                        "type": "Area"
                    },
                    {
                        "templateId": carBatteryID, //Car battery
                        "count": 1,
                        "isFunctional": false,
                        "resource": 110,
                        "isEncoded": false,
                        "type": "Tool"
                    }
                ],
                "productionTime": 10, // seconds
                "needFuelForAllProductionTime": false,
                "locked": false,
                "endProduct": carBatteryID,
                "continuous": false,
                "count": 1,
                "productionLimitCount": 0,
                "isEncoded": false
            },
            { // Car Battery!
                "_id": "cr123Recharge1",
                "areaType": 2,
                "requirements": [
                    {
                        "areaType": 2,
                        "requiredLevel": 2,
                        "type": "Area"
                    },
                    {
                        "templateId": "5733279d245977289b77ec24", //Car battery
                        "count": 1,
                        "isFunctional": false,
                        "resource": 100,
                        "isEncoded": false,
                        "type": "Tool"
                    },
                    {
                        "templateId": "5c06779c86f77426e00dd782", //Bundle of wires
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    },
                    {
                        "templateId": rchblBatteryID,
                        "count": 2,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    }
                ],
                "productionTime": 600, // seconds
                "needFuelForAllProductionTime": false,
                "locked": false,
                "endProduct": rchblBatteryID,
                "continuous": false,
                "count": 2,
                "productionLimitCount": 0,
                "isEncoded": false
            },
            { // Normal charging :(
                "_id": "cr123Recharge2",
                "areaType": 10,
                "requirements": [
                    {
                        "areaType": 10,
                        "requiredLevel": 3,
                        "type": "Area"
                    },
                    {
                        "templateId": "5909e99886f7740c983b9984", //USB Adapter
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Tool"
                    },
                    {
                        "templateId": "5c06779c86f77426e00dd782", //Bundle of wires
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    },
                    {
                        "templateId": rchblBatteryID,
                        "count": 3,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    }
                ],
                "productionTime": 600, // seconds
                "needFuelForAllProductionTime": false,
                "locked": false,
                "endProduct": rchblBatteryID,
                "continuous": false,
                "count": 3,
                "productionLimitCount": 0,
                "isEncoded": false
            },
            { // Normal charging FLIR :(
                "_id": "flirRecharge0",
                "areaType": 10,
                "requirements": [
                    {
                        "areaType": 10,
                        "requiredLevel": 3,
                        "type": "Area"
                    },
                    {
                        "templateId": "5909e99886f7740c983b9984", //USB Adapter
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Tool"
                    },
                    {
                        "templateId": flirID,
                        "count": 1,
                        "isFunctional": false,
                        "isEncoded": false,
                        "type": "Item"
                    }
                ],
                "productionTime": 600, // seconds
                "needFuelForAllProductionTime": false,
                "locked": false,
                "endProduct": flirID,
                "continuous": false,
                "count": 1,
                "productionLimitCount": 0,
                "isEncoded": false
            },
        );
        */
        logger.success("BatterySystem has been applied!");
    }
    pushUnique(target, ...ids) {
        for (const id of ids) {
            if (!target.includes(id)) {
                target.push(id);
            }
        }
    }
    configureWeaponDurability(items, handbook) {
        this.weaponBaseValues = {};
        for (const handbookItem of handbook?.Items ?? []) {
            if (typeof handbookItem?.Id === "string" && typeof handbookItem?.Price === "number") {
                this.weaponBaseValues[handbookItem.Id] = handbookItem.Price;
            }
        }
        const weaponValues = [];
        for (const id in items) {
            if (!this.isWeaponTemplate(id, items))
                continue;
            const templateValue = Number(items[id]?._props?.CreditsPrice ?? 0);
            if (!this.weaponBaseValues[id] && templateValue > 0) {
                this.weaponBaseValues[id] = templateValue;
            }
            const value = this.weaponBaseValues[id] ?? 0;
            if (value > 0) {
                weaponValues.push(value);
            }
        }
        weaponValues.sort((a, b) => a - b);
        this.weaponValueLow = this.percentile(weaponValues, this.normalizePercentile(config.WeaponDurability?.ValuePercentileLow, 0.1));
        this.weaponValueHigh = this.percentile(weaponValues, this.normalizePercentile(config.WeaponDurability?.ValuePercentileHigh, 0.9));
        if (this.weaponValueHigh <= this.weaponValueLow) {
            this.weaponValueHigh = this.weaponValueLow + 1;
        }
    }
    applyTraderWeaponDurability(traders, items) {
        let changed = 0;
        for (const traderId in traders) {
            const assortItems = traders[traderId]?.assort?.items;
            if (!Array.isArray(assortItems))
                continue;
            for (const assortItem of assortItems) {
                if (assortItem?.parentId !== "hideout" || assortItem?.slotId !== "hideout")
                    continue;
                if (this.applyWeaponDurability(assortItem, items, "trader")) {
                    changed++;
                }
            }
        }
        return changed;
    }
    applyLocationWeaponDurability(locations, items) {
        let changed = 0;
        for (const location of Object.values(locations ?? {})) {
            changed += this.applyWorldWeaponDurabilityToLootTemplates(location?.looseLoot, items);
        }
        return changed;
    }
    patchLocationLootGenerator(container, items, logger) {
        try {
            const locationLootGenerator = container.resolve("LocationLootGenerator");
            const prototype = Object.getPrototypeOf(locationLootGenerator);
            if (prototype.__batterySystemWeaponDurabilityPatched)
                return;
            const mod = this;
            let patchedMethods = 0;
            for (const methodName of ["generateDynamicLoot", "generateStaticContainers"]) {
                const originalMethod = prototype[methodName];
                if (typeof originalMethod !== "function")
                    continue;
                prototype[methodName] = function (...args) {
                    const loot = originalMethod.apply(this, args);
                    if (typeof loot?.then === "function") {
                        return loot.then((resolvedLoot) => {
                            mod.applyWorldWeaponDurabilityToLootTemplates(resolvedLoot, items);
                            return resolvedLoot;
                        });
                    }
                    mod.applyWorldWeaponDurabilityToLootTemplates(loot, items);
                    return loot;
                };
                patchedMethods++;
            }
            if (patchedMethods === 0) {
                const warning = logger.warning ?? logger.error;
                warning.call(logger, "BatterySystem could not find LocationLootGenerator methods to patch for per-raid weapon durability.");
            }
            Object.defineProperty(prototype, "__batterySystemWeaponDurabilityPatched", { value: true });
        }
        catch (error) {
            const warning = logger.warning ?? logger.error;
            warning.call(logger, `BatterySystem could not patch world weapon durability: ${error?.message ?? error}`);
        }
    }
    applyWorldWeaponDurabilityToLootTemplates(lootTemplates, items) {
        let changed = 0;
        if (!lootTemplates)
            return changed;
        if (Array.isArray(lootTemplates)) {
            for (const lootTemplate of lootTemplates) {
                changed += this.applyWorldWeaponDurabilityToLootTemplates(lootTemplate, items);
            }
            return changed;
        }
        if (typeof lootTemplates !== "object")
            return changed;
        changed += this.applyWorldWeaponDurabilityToItems(lootTemplates.Items, lootTemplates.Root, items);
        changed += this.applyWorldWeaponDurabilityToItems(lootTemplates.template?.Items, lootTemplates.template?.Root, items);
        changed += this.applyWorldWeaponDurabilityToLootTemplates(lootTemplates.spawnpoints, items);
        changed += this.applyWorldWeaponDurabilityToLootTemplates(lootTemplates.spawnpointsForced, items);
        changed += this.applyWorldWeaponDurabilityToLootTemplates(lootTemplates.looseLoot, items);
        return changed;
    }
    applyWorldWeaponDurabilityToItems(itemList, rootId, items) {
        let changed = 0;
        if (!Array.isArray(itemList))
            return changed;
        for (const item of itemList) {
            if (!this.isWeaponTemplate(item?._tpl, items))
                continue;
            if (item._id !== rootId && this.hasWeaponParent(item, itemList, items))
                continue;
            if (this.applyWeaponDurability(item, items, "world")) {
                changed++;
            }
        }
        return changed;
    }
    applyWeaponDurability(item, items, source) {
        const template = items[item?._tpl];
        if (!template || !this.isWeaponTemplate(item._tpl, items))
            return false;
        const templateMaxDurability = Number(template._props?.MaxDurability ?? template._props?.Durability ?? 0);
        if (templateMaxDurability <= 0)
            return false;
        const range = this.getDurabilityRange(item._tpl, source);
        const chosenPercent = this.randomInt(range.min, range.max);
        const chosenMaxDurability = Math.max(1, Math.min(templateMaxDurability, Math.round(templateMaxDurability * chosenPercent / 100)));
        item.upd = item.upd ?? {};
        item.upd.Repairable = {
            Durability: chosenMaxDurability,
            MaxDurability: chosenMaxDurability
        };
        return true;
    }
    getDurabilityRange(itemTpl, source) {
        const settings = (config.WeaponDurability ?? {});
        const value = this.weaponBaseValues[itemTpl] ?? this.weaponValueLow;
        const valueFactor = this.clamp((value - this.weaponValueLow) / (this.weaponValueHigh - this.weaponValueLow), 0, 1);
        const lowMin = source === "trader" ? settings.TraderLowValueMin : settings.WorldLowValueMin;
        const lowMax = source === "trader" ? settings.TraderLowValueMax : settings.WorldLowValueMax;
        const highMin = source === "trader" ? settings.TraderHighValueMin : settings.WorldHighValueMin;
        const highMax = source === "trader" ? settings.TraderHighValueMax : settings.WorldHighValueMax;
        const min = Math.round(this.lerp(this.clampPercent(lowMin, 35), this.clampPercent(highMin, 80), valueFactor));
        const max = Math.round(this.lerp(this.clampPercent(lowMax, 95), this.clampPercent(highMax, 98), valueFactor));
        return {
            min: Math.min(min, max),
            max: Math.max(min, max)
        };
    }
    hasWeaponParent(item, itemList, items) {
        let parent = itemList.find((candidate) => candidate?._id === item?.parentId);
        while (parent) {
            if (this.isWeaponTemplate(parent._tpl, items))
                return true;
            parent = itemList.find((candidate) => candidate?._id === parent.parentId);
        }
        return false;
    }
    isWeaponTemplate(id, items) {
        return id === this.weaponID || this.isChildOf(id, this.weaponID, items);
    }
    percentile(sortedValues, percentile) {
        if (sortedValues.length === 0)
            return 0;
        const index = Math.round((sortedValues.length - 1) * percentile);
        return sortedValues[this.clamp(index, 0, sortedValues.length - 1)];
    }
    normalizePercentile(value, fallback) {
        if (typeof value !== "number" || !Number.isFinite(value))
            return fallback;
        return this.clamp(value > 1 ? value / 100 : value, 0, 1);
    }
    clampPercent(value, fallback) {
        if (typeof value !== "number" || !Number.isFinite(value))
            return fallback;
        return this.clamp(value, 0, 100);
    }
    clamp(value, min, max) {
        return Math.min(max, Math.max(min, value));
    }
    lerp(start, end, value) {
        return start + (end - start) * value;
    }
    randomInt(min, max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    }
    shouldAddBatterySlot(id, items) {
        if (config.NoBattery.includes(id))
            return false;
        if (this.hasConfiguredBatteryType(id))
            return true;
        if (this.isChildOf(id, this.nightVisionID, items))
            return true;
        if (this.isChildOf(id, this.thermalVisionID, items))
            return true;
        if (this.isChildOf(id, this.specialScopeID, items))
            return this.isPoweredSight(id, items);
        if (this.isChildOf(id, this.collimatorID, items))
            return this.isPoweredSight(id, items);
        if (this.isChildOf(id, this.compactCollimatorID, items))
            return this.isPoweredSight(id, items);
        if (this.isChildOf(id, this.assaultScopeID, items))
            return this.isPoweredSight(id, items);
        if (this.isChildOf(id, this.opticScopeID, items))
            return this.isPoweredSight(id, items);
        if (this.isChildOf(id, this.headsetID, items))
            return true;
        if (this.isChildOf(id, this.flashlightID, items))
            return true;
        if (this.isChildOf(id, this.lightLaserDesignatorID, items))
            return true;
        if (this.isChildOf(id, this.tacticalComboID, items))
            return true;
        return false;
    }
    hasConfiguredBatteryType(id) {
        return config.AA.includes(id)
            || config.CR123.includes(id)
            || config.CR2032.includes(id)
            || config.CR1225.includes(id)
            || config.CR1632.includes(id);
    }
    prepareConfiguredBatterySlotItem(id, items) {
        const item = items[id];
        const props = item?._props;
        if (!props)
            return;
        if (this.shouldUseCompoundBatterySlotParent(id, items)) {
            item._parent = this.gearModID;
            delete props.Grids;
            props.HideEntrails = false;
            props.MergesWithChildren = false;
        }
        props.Slots ?? (props.Slots = []);
        props.CanPutIntoDuringTheRaid ?? (props.CanPutIntoDuringTheRaid = true);
        props.ForbidMissingVitalParts ?? (props.ForbidMissingVitalParts = false);
        props.ForbidNonEmptyContainers ?? (props.ForbidNonEmptyContainers = false);
        props.RaidModdable ?? (props.RaidModdable = true);
        props.ToolModdable ?? (props.ToolModdable = true);
    }
    shouldUseCompoundBatterySlotParent(id, items) {
        const item = items[id];
        const props = item?._props;
        if (!props)
            return false;
        return this.hasConfiguredBatteryType(id)
            && this.isChildOf(id, this.electronicsID, items)
            && !Object.prototype.hasOwnProperty.call(props, "Slots");
    }
    getBatteryType(id, items) {
        if (config.AA.includes(id))
            return this.aaBatteryID;
        if (config.CR123.includes(id))
            return this.cr123BatteryID;
        if (config.CR2032.includes(id))
            return this.cr2032BatteryID;
        if (config.CR1225.includes(id))
            return this.cr2032BatteryID;
        if (config.CR1632.includes(id))
            return this.cr2032BatteryID;
        if (this.isChildOf(id, this.headsetID, items))
            return this.aaBatteryID;
        if (this.isChildOf(id, this.flashlightID, items))
            return this.cr123BatteryID;
        if (this.isChildOf(id, this.lightLaserDesignatorID, items))
            return this.cr123BatteryID;
        if (this.isChildOf(id, this.tacticalComboID, items))
            return this.cr123BatteryID;
        if (this.isChildOf(id, this.specialScopeID, items))
            return this.cr123BatteryID;
        if (this.isChildOf(id, this.thermalVisionID, items))
            return this.cr123BatteryID;
        if (this.isChildOf(id, this.nightVisionID, items))
            return this.aaBatteryID;
        return this.cr2032BatteryID;
    }
    isPoweredSight(id, items) {
        const sightModType = items[id]?._props?.sightModType;
        return sightModType === "holo"
            || sightModType === "reflex"
            || sightModType === "hybrid";
    }
    isChildOf(id, parentId, items) {
        let cursor = items[id];
        while (cursor?._parent) {
            if (cursor._parent === parentId)
                return true;
            cursor = items[cursor._parent];
        }
        return false;
    }
}
module.exports = { mod: new Mod() };
