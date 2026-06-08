import { DependencyContainer } from "tsyringe";

import { ILogger } from "@spt/models/spt/utils/ILogger"
import { IPostDBLoadMod } from "@spt/models/external/IPostDBLoadMod";
//import { CustomItemService } from "@spt/services/mod/CustomItemService";
//import { NewItemFromCloneDetails } from "@spt/models/spt/mod/NewItemDetails";
import { DatabaseServer } from "@spt/servers/DatabaseServer";
import * as config from "../config/config.json";
import { TraderPurchaseData } from "@spt/models/eft/profile/ISptProfile";

class Mod implements IPostDBLoadMod {
    private batteryType = "";
    private readonly aaBatteryID = "5672cb124bdc2d1a0f8b4568";
    private readonly cr2032BatteryID = "5672cb304bdc2dc2088b456a";
    private readonly cr123BatteryID = "590a358486f77429692b2790";
    private readonly carBatteryID = "5733279d245977289b77ec24";
    private readonly specialScopeID = "55818aeb4bdc2ddc698b456a";
    private readonly nightVisionID = "5a2c3a9486f774688b05e574";
    private readonly thermalVisionID = "5d21f59b6dbe99052b54ef83";
    private readonly collimatorID = "55818ad54bdc2ddc698b4569";
    private readonly compactCollimatorID = "55818acf4bdc2dde698b456b";
    private readonly assaultScopeID = "55818add4bdc2d5b648b456f";
    private readonly opticScopeID = "55818ae44bdc2dde698b456c";
    private readonly headsetID = "5645bcb74bdc2ded0b8b4578";
    private readonly flashlightID = "55818b084bdc2d5b648b4571";
    private readonly lightLaserDesignatorID = "55818b0e4bdc2dde698b456e";
    private readonly tacticalComboID = "55818b164bdc2ddc698b456c";

    public postDBLoad(container: DependencyContainer): void {
        //const CustomItem = container.resolve<CustomItemService>("CustomItemService");
        const logger = container.resolve<ILogger>("WinstonLogger");
        const db = container.resolve<DatabaseServer>("DatabaseServer");
        const locales = Object.values(db.getTables().locales.global) as Record<string, string>[];
        const botDB = db.getTables().bots.types;
        const items = db.getTables().templates.items;
        const hideoutProduction = db.getTables().hideout.production;
        const aaBatteryID = this.aaBatteryID;
        const dBatteryID = this.cr2032BatteryID;
        const rchblBatteryID = this.cr123BatteryID;
        const carBatteryID = this.carBatteryID;
        //const flirID = "5d1b5e94d7ad1a2b865a96b0";
        //Flir has a built-in battery. the battery doesn't show anywhere so no point
        //items[flirID]._props.MaxResource = 100;
        //items[flirID]._props.Resource = 0.05;


        items[aaBatteryID]._props.MaxResource = 100;
        items[aaBatteryID]._props.Resource = 100;
        items[aaBatteryID]._props.ItemSound = "food_tin_can";
        
        items[rchblBatteryID]._props.MaxResource = 100;
        items[rchblBatteryID]._props.Resource = 100;
        items[rchblBatteryID]._props.Prefab.path = "batteries/cr123.bundle"
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
        };

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
                for (const locale of locales) { // Item description now includes the battery type
                    const oldDescription = locale[`${id} Description`] ?? "";
                    const batteryName = locale[`${this.batteryType} Name`] ?? this.batteryType;
                    const newDescription = oldDescription.startsWith("Uses ")
                        ? oldDescription
                        : "Uses " + batteryName + "\n\n" + oldDescription;
                    locale[`${id} Description`] = newDescription;
                }
                if (items[id]._props.Slots.some(slot => slot._name === "mod_equipment")) continue;

                items[id]._props.Slots.push(
                    {
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
                    }
                );
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

    private pushUnique(target: string[], ...ids: string[]): void {
        for (const id of ids) {
            if (!target.includes(id)) {
                target.push(id);
            }
        }
    }

    private shouldAddBatterySlot(id: string, items: Record<string, any>): boolean {
        if (config.NoBattery.includes(id)) return false;

        const batteryParentIds = [
            this.specialScopeID,
            this.nightVisionID,
            this.thermalVisionID,
            this.collimatorID,
            this.compactCollimatorID,
            this.assaultScopeID,
            this.opticScopeID,
            this.headsetID,
            this.flashlightID,
            this.lightLaserDesignatorID,
            this.tacticalComboID
        ];

        if (batteryParentIds.includes(id)) return false;

        return batteryParentIds.some(parentId => this.isChildOf(id, parentId, items));
    }

    private getBatteryType(id: string, items: Record<string, any>): string {
        if (config.AA.includes(id)) return this.aaBatteryID;
        if (config.CR123.includes(id)) return this.cr123BatteryID;
        if (config.CR2032.includes(id)) return this.cr2032BatteryID;
        if (config.CR1225.includes(id)) return this.cr2032BatteryID;
        if (config.CR1632.includes(id)) return this.cr2032BatteryID;

        if (this.isChildOf(id, this.headsetID, items)) return this.aaBatteryID;
        if (this.isChildOf(id, this.flashlightID, items)) return this.cr123BatteryID;
        if (this.isChildOf(id, this.lightLaserDesignatorID, items)) return this.cr123BatteryID;
        if (this.isChildOf(id, this.tacticalComboID, items)) return this.cr123BatteryID;
        if (this.isChildOf(id, this.specialScopeID, items)) return this.cr123BatteryID;
        if (this.isChildOf(id, this.thermalVisionID, items)) return this.cr123BatteryID;
        if (this.isChildOf(id, this.nightVisionID, items)) return this.aaBatteryID;

        return this.cr2032BatteryID;
    }

    private isChildOf(id: string, parentId: string, items: Record<string, any>): boolean {
        let cursor = items[id];
        while (cursor?._parent) {
            if (cursor._parent === parentId) return true;
            cursor = items[cursor._parent];
        }

        return false;
    }
}

module.exports = { mod: new Mod() }
