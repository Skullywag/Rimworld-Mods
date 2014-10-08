using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
namespace MorePower
{
    public class Building_GasEngine : Building
    {
        private int Count = 0;
        private bool HasFuel = false;
        private CompPowerTrader powerComp;
        public bool GotCans
        {
            get
            {
                return this.HasCansInHopper;
            }
        }
        public bool HasCansInHopper
        {
            get
            {
                return this.CansInHopper != null;
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
        private Thing CansInHopper
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
                        if (current2.def == ThingDef.Named("Syngas"))
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
        }
        public override void Tick()
        {
            if (this.GotCans || this.HasFuel)
            {
                base.Tick();
                if (this.Count == 1)
                {
                    int num = 1;
                    int num2 = 0;
                    List<ThingDef> list = new List<ThingDef>();
                    Thing CansInHopper = this.CansInHopper;
                    do
                    {
                        int num3 = Mathf.Min(CansInHopper.stackCount, num);
                        num2 += num3;
                        list.Add(CansInHopper.def);
                        CansInHopper.SplitOff(num3);
                        if (num2 >= num)
                        {
                            break;
                        }
                        CansInHopper = this.CansInHopper;
                    }
                    while (CansInHopper != null);
                }
                this.Count++;
                this.powerComp.powerOutput = 300;
                this.HasFuel = true;
                if (this.Count >= 4800)
                {
                    this.Count = 0;
                    this.HasFuel = false;
                }
            }
            else
            {
                this.powerComp.powerOutput = 0;
            }
        }
    }
}
