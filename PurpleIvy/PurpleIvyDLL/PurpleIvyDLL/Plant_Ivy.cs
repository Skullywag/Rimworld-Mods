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

        public void SpawnIvy(IntVec3 dir)
        {
            Plant newivy = new Plant();
            newivy = (Plant)ThingMaker.MakeThing(ThingDef.Named("PurpleIvy"));
            GenSpawn.Spawn(newivy, dir);
        }

        public bool IvyInCell(IntVec3 dir)
        {
            //List all things in that random direction cell
            List<Thing> list = Find.ThingGrid.ThingsListAt(dir);
            if (list.Count > 0)
            {
                //Loop over things
                for (int i = 0; i < list.Count - 1; i++)
                {
                    //If we find a plant
                    if (list[i].def.eType == EntityType.Plant)
                    {
                        //If the plant is Ivy
                        if (list[i].def.defName == "PurpleIvy")
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public void CheckThings(IntVec3 dir)
        {
            //List all things in that random direction cell
            List<Thing> list = Find.ThingGrid.ThingsListAt(dir);
            //Loop over things if there are things
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count - 1; i++)
                {
                    //If we find a plant
                    if (list[i].def.eType == EntityType.Plant)
                    {
                        //If the plant isnt Ivy
                        if (list[i].def.defName != "PurpleIvy")
                        {
                            //kill it
                            list[i].Destroy();
                        }
                    }
                    //If we find a corpse
                    if (list[i].def.eType == EntityType.Corpse)
                    {
                        //Consume it - TODO see if i can decompose the corpse fast then destroy the skeleton
                        list[i].Destroy();
                        //speedup the spread a little
                        this.SpreadTick--;
                        this.SpreadTick--;
                        this.SpreadTick--;
                    }
                }
            }
        }
        
        public override void TickRare()
        {
            base.TickRare();
            if(this.growthPercent >= 1)
            {
                if (this.MutateTry == true)
                {
                    Random random = new Random();
                    int MutateRate = random.Next(1, 200);
                    if (MutateRate == 3 || MutateRate == 23)
                    {
                        this.Destroy();
                        Building_GasPump GasPump = (Building_GasPump)ThingMaker.MakeThing(ThingDef.Named("GasPump"));
                        GasPump.SetFactionDirect(factionDirect);
                        GenSpawn.Spawn(GasPump, Position);
                        this.MutateTry = false;
                        //Find.History.AddGameEvent("Gas here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                    }
                    else if (MutateRate == 4 || MutateRate == 24)
                    {
                        this.Destroy();
                        Building_EggSac EggSac = (Building_EggSac)ThingMaker.MakeThing(ThingDef.Named("EggSac"));
                        EggSac.SetFactionDirect(factionDirect);
                        GenSpawn.Spawn(EggSac, Position);
                        this.MutateTry = false;
                        //Find.History.AddGameEvent("Egg here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                    }
                    else if (MutateRate == 5)
                    {
                        this.Destroy();
                        Building_Turret GenMortar = (Building_Turret)ThingMaker.MakeThing(ThingDef.Named("Turret_GenMortarSeed"));
                        GenMortar.SetFactionDirect(factionDirect);
                        GenSpawn.Spawn(GenMortar, Position);
                        this.MutateTry = false;
                        //Find.History.AddGameEvent("Mortar here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                    }
                    else if (MutateRate == 6)
                    {
                        this.Destroy();
                        Building_Turret GenTurret = (Building_Turret)ThingMaker.MakeThing(ThingDef.Named("GenTurretBase"));
                        GenTurret.SetFactionDirect(factionDirect);
                        GenSpawn.Spawn(GenTurret, Position);
                        this.MutateTry = false;
                        //Find.History.AddGameEvent("Turret here", GameEventType.BadNonUrgent, true, Position, string.Empty);
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
                //Pick a random direction cell
                IntVec3 dir = GenAdj.RandomAdjSquareCardinal(Position);
                //If in bounds
                if (dir.InBounds())
                {
                    //If we find a tasty floor lets eat it nomnomnom
                    TerrainDef terrain = dir.GetTerrain();
                    if (terrain != null)
                    {
                        //Only eat floor if not natural
                        if (terrain.defName != "Sand" &&
                            terrain.defName != "Soil" &&
                            terrain.defName != "MarshyTerrain" &&
                            terrain.defName != "SoilRich" &&
                            terrain.defName != "Mud" &&
                            terrain.defName != "Marsh" &&
                            terrain.defName != "Gravel" &&
                            terrain.defName != "RoughStone" &&
                            terrain.defName != "WaterDeep" &&
                            terrain.defName != "WaterShallow" &&
                            terrain.defName != "RoughHewnRock")
                        {
                            //And by eat i mean replace - TODO can you damage floors over time?                   
                            //Replace with soil - TODO for now, maybe change to regen tile later if possible
                            Find.TerrainGrid.SetTerrain(dir, TerrainDef.Named("Soil"));
                            //check things in cell and react
                            CheckThings(dir);
                            //if theres no ivy here
                            if (!IvyInCell(dir))
                            {
                                //Spawn more Ivy
                                SpawnIvy(dir);
                            }
                        }
                        //Its natural floor
                        else if (terrain.defName != "WaterDeep" &&
                                 terrain.defName != "WaterShallow" &&
                                 terrain.defName != "MarshyTerrain")
                            {
                            //check things in cell and react
                            CheckThings(dir);
                            //if theres no ivy here
                            if (!IvyInCell(dir))
                            {
                                //Spawn more Ivy
                                SpawnIvy(dir);
                            }
                        }
                        //its water
                        else
                        {

                        }
                    }
                }                  
                                          
                //Old Spread code
                /*if (dir.GetPlant() == null)
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
                }*/
                SpreadTick = OrigSpreadTick;
            }
        }
    }
}
