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
        private int MutateRate;
        private bool MutateTry;
        Faction factionDirect = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("Genny", true));

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Random random = new Random();
            this.SpreadTick = random.Next(15, 25);
            this.OrigSpreadTick = SpreadTick;
            this.MutateTry = true;
        }
        
        public override void TickRare()
        {
            base.TickRare();
            if(this.growthPercent >= 1)
            {
                if (this.MutateTry == true)
                {
                    Random random = new Random();
                    MutateRate = random.Next(1, 25);
                    if (MutateRate == 5)
                    {
                        this.Destroy();
                        Building_GasPump GasPump = (Building_GasPump)ThingMaker.MakeThing(ThingDef.Named("GasPump"));
                        GasPump.SetFactionDirect(factionDirect);
                        GenSpawn.Spawn(GasPump, Position);
                        this.MutateTry = false;
                        //Find.History.AddGameEvent("Gas here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                        return;
                    }
                    else if (MutateRate == 10)
                    {
                        this.Destroy();
                        Building_EggSac EggSac = (Building_EggSac)ThingMaker.MakeThing(ThingDef.Named("EggSac"));
                        EggSac.SetFactionDirect(factionDirect);
                        GenSpawn.Spawn(EggSac, Position);
                        this.MutateTry = false;
                        //Find.History.AddGameEvent("Egg here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                        return;
                    }
                    else
                    {
                        this.MutateTry = false;
                    }
                }
            }
            this.SpreadTick--;
            if (this.SpreadTick == 0)
            {
                IntVec3 dir = GenAdj.RandomAdjSquareCardinal(Position);
                if (dir.GetPlant() == null)
                {
                    //not a plant, move on
                }
                else
                {
                    Plant plant = dir.GetPlant();
                    //Log.Error("" + plant.def.defName);
                    if (plant.def.defName != "PurpleIvy")
                    {
                        plant.Destroy();
                        Plant newivy = new Plant();
                        newivy = (Plant)ThingMaker.MakeThing(ThingDef.Named("PurpleIvy"));
                        GenSpawn.Spawn(newivy, dir);
                    }
                    else
                    {
                        //dont destroy other ivy
                    }
                }
                SpreadTick = OrigSpreadTick;
            } 
        }
    }
}
