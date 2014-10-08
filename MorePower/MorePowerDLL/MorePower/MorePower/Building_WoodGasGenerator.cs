using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace MorePower
{
    public class Building_WoodGasGenerator : Building
    {
        private int GasCount = 0;
        public IntVec3 GasOutPut;
        private CompPowerTrader powerComp;
        public bool GotLogs
        {
            get
            {
                return this.powerComp.PowerOn && this.HasLogsInHopper;
            }
        }
        public bool HasLogsInHopper
        {
            get
            {
                return this.LogsInHopper != null;
            }
        }
        public Building AnyAdjacentHopper
        {
            get
            {
                ThingDef thingDef = ThingDef.Named("Hopper");
                Building result;
                foreach (IntVec3 current in GenAdj.AdjacentSquaresCardinal(this))
                {
                    Building building = Find.BuildingGrid.BuildingAt(current);
                    if (building != null && building.def == thingDef)
                    {
                        result = (Building_Storage)building;
                        return result;
                    }
                }
                result = null;
                return result;
            }
        }
        private Thing LogsInHopper
        {
            get
            {
                ThingDef thingDef = ThingDef.Named("Hopper");
                Thing result;
                foreach (IntVec3 current in GenAdj.AdjacentSquaresCardinal(this))
                {
                    Thing thing = null;
                    Thing thing2 = null;
                    foreach (Thing current2 in Find.ThingGrid.ThingsAt(current))
                    {
                        if (current2.def == ThingDef.Named("WoodLog"))
                        {
                            thing = current2;
                        }
                        if (current2.def == thingDef)
                        {
                            thing2 = current2;
                        }
                    }
                    if (thing != null && thing2 != null)
                    {
                        result = thing;
                        return result;
                    }
                }
                result = null;
                return result;
            }
        }
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.powerComp = base.GetComp<CompPowerTrader>();
            if (this.Rotation == IntRot.north)
            {
                this.GasOutPut = base.Position + IntVec3.north * 2;
            }
            else
            {
                if (this.Rotation == IntRot.south)
                {
                    this.GasOutPut = base.Position + IntVec3.south * 2;
                }
                else
                {
                    if (this.Rotation == IntRot.east)
                    {
                        this.GasOutPut = base.Position + IntVec3.east * 2;
                    }
                    else
                    {
                        if (this.Rotation == IntRot.west)
                        {
                            this.GasOutPut = base.Position + IntVec3.west * 2;
                        }
                    }
                }
            }
        }
        public override void Tick()
        {
            if (this.GotLogs)
            {
                base.Tick();
                this.GasCount++;
                if (this.GasCount >= 2400)
                {
                    int num = 5;
                    int num2 = 0;
                    List<ThingDef> list = new List<ThingDef>();
                    Thing LogsInHopper = this.LogsInHopper;
                    do
                    {
                        int num3 = Mathf.Min(LogsInHopper.stackCount, num);
                        num2 += num3;
                        list.Add(LogsInHopper.def);
                        LogsInHopper.SplitOff(num3);
                        if (num2 >= num)
                        {
                            break;
                        }
                        LogsInHopper = this.LogsInHopper;
                    }
                    while (LogsInHopper != null);
                    ThingMaker.MakeThing(ThingDef.Named("Syngas"));
                    GenSpawn.Spawn(ThingDef.Named("Syngas"), this.GasOutPut).stackCount = 1;
                    this.GasCount = 0;
                }
            }
        }
    }
}
