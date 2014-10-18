using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    public class Building_Meteorite : Building
    {
        private int spawnticks = 1200;
        public override void SpawnSetup()
        {
            base.SpawnSetup();
        }

        public override void Tick()
        {
            base.Tick();
            spawnticks--;
            if (spawnticks == 0)
            {
                foreach (IntVec3 current in GenAdj.AdjacentSquaresCardinal(this))
                {
                    if (current.GetPlant() == null)
                    {
                        //not a plant, move on
                    }
                    else
                    {
                        Plant plant = current.GetPlant();
                        if (plant.def.defName != "PurpleIvy")
                        {
                            plant.Destroy();
                            Plant newivy = (Plant)ThingMaker.MakeThing(ThingDef.Named("PurpleIvy"));
                            GenSpawn.Spawn(newivy, current);
                        }
                        else
                        {
                            //dont destroy other ivy
                        }
                    }
                }
                spawnticks = 1200;
            }
        }
    }
}