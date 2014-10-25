using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    public class Plant_Ivy : Plant
    {
        private int SpreadTick;
        private int OrigSpreadTick;
        private int EggSackRate;
        private bool EggTry;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Random random = new Random();
            SpreadTick = random.Next(25, 35);
            OrigSpreadTick = SpreadTick;
            EggTry = true;
        }
        
        public override void TickRare()
        {
            base.TickRare();
            if(this.growthPercent >= 1)
            {
                if (EggTry == true)
                {
                    Random random = new Random();
                    EggSackRate = random.Next(1, 20);
                    if (EggSackRate == 5)
                    {
                        Building_EggSack EggSack = (Building_EggSack)ThingMaker.MakeThing(ThingDef.Named("EggSack"));
                        GenSpawn.Spawn(EggSack, Position);
                    }
                    else
                    {
                        EggTry = false;
                    }
                }
            }
            SpreadTick--;
            if (SpreadTick == 0)
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
                SpreadTick = OrigSpreadTick;
            } 
        }
    }
}
