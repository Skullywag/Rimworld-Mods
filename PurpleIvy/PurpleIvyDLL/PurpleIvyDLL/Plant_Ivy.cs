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
        Thing stuckPawn = null;
        Thing stuckCorpse = null;
        Faction factionDirect = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("Genny", true));
        DamageTypeDef dmgdef = DefDatabase<DamageTypeDef>.GetNamed("Scratch", true);

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Random random = new Random();
            SpreadTick = random.Next(15, 25);
            OrigSpreadTick = SpreadTick;
            MutateTry = true;
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
                for (int i = 0; i < list.Count; i++)
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

        public void CheckThings(IntVec3 pos)
        {
            //List all things in that random direction cell
            List<Thing> list = Find.ThingGrid.ThingsListAt(pos);
            //Loop over things if there are things
            if (list.Count > 0)
            {
                //iterate through things
                //for (int i = list.Count - 1; i >= 0; i--) -- backwards iteration, incase i need it later
                for (int i = 0; i < list.Count; i++)
                {                  
                    //If we find a corpse
                    if (list[i].def.eType == EntityType.Corpse)
                    {
                        //Consume it - TODO see if i can decompose the corpse fast then destroy the skeleton
                        stuckCorpse = list[i];
                        //speedup the spread a little
                        SpreadTick--;
                        SpreadTick--;
                        SpreadTick--;
                    }
                    //If we find a pawn
                    if (list[i].def.eType == EntityType.Pawn)
                    {
                        //And its not a hatchling
                        if (list[i].def.defName != "Genny_Centipede")
                        {
                            //Could do some mutatey zombie stuff here, but for now save the pawn and injur it outside this loop
                            stuckPawn = list[i];
                        }
                    }
                }  
            }
        }

        public bool isSurroundedByIvy(IntVec3 dir)
        {
            foreach (IntVec3 current in GenAdj.AdjacentSquares8Way(dir))
			{
				if(!IvyInCell(current))
                {
                    return false;
                }
			}
            return true;
        }

        public bool hasNoBuildings(IntVec3 dir)
        {
            foreach (IntVec3 current in GenAdj.AdjacentSquares8Way(dir))
            {
                if (!current.Standable())
                {
                    return false;
                }
            }
            return true;
        }
        
        public override void TickRare()
        {
            base.TickRare();
            this.SpreadTick--;
            if (this.growthPercent >= 1)
            {
                //check things in cell and react
                CheckThings(Position);
            }
            if (this.SpreadTick == 0)
            {
                //Pick a random direction cell
                IntVec3 dir = new IntVec3();
                dir = GenAdj.RandomAdjSquareCardinal(Position);
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
                            //if theres no ivy here
                            if (!IvyInCell(dir))
                            {
                                if (dir.GetPlant() == null)
                                {
                                    //no plant, move on
                                }
                                else
                                {
                                    //Found plant, Kill it
                                    Plant plant = dir.GetPlant();
                                    plant.Destroy();
                                }
                                //Spawn more Ivy
                                SpawnIvy(dir);
                            }
                        }
                        //Its natural floor
                        else if (terrain.defName != "WaterDeep" &&
                                 terrain.defName != "WaterShallow" &&
                                 terrain.defName != "MarshyTerrain")
                            {
                            //if theres no ivy here
                            if (!IvyInCell(dir))
                            {
                                if (dir.GetPlant() == null)
                                 {
                                    //no plant, move on
                                 }
                                 else
                                 {
                                    //Found plant, Kill it
                                    Plant plant = dir.GetPlant();
                                    plant.Destroy();
                                 }
                                //Spawn more Ivy
                                SpawnIvy(dir);
                            }
                        }
                        //its water or something I dont know of
                        else
                        {

                        }
                    }
                }
                SpreadTick = OrigSpreadTick;                                     
            }
            if (this.MutateTry == true)
            {
                Random random = new Random();
                int MutateRate = random.Next(1, 200);
                if (MutateRate == 3 || MutateRate == 23)
                {
                    Building_GasPump GasPump = (Building_GasPump)ThingMaker.MakeThing(ThingDef.Named("GasPump"));
                    GasPump.SetFactionDirect(factionDirect);
                    if (hasNoBuildings(Position))
                    {
                        GenSpawn.Spawn(GasPump, Position);
                    }
                    this.MutateTry = false;
                    //Find.History.AddGameEvent("Gas here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                }
                else if (MutateRate == 4 || MutateRate == 24)
                {
                    Building_EggSac EggSac = (Building_EggSac)ThingMaker.MakeThing(ThingDef.Named("EggSac"));
                    EggSac.SetFactionDirect(factionDirect);
                    if (hasNoBuildings(Position))
                    {
                        GenSpawn.Spawn(EggSac, Position);
                    }
                    this.MutateTry = false;
                    //Find.History.AddGameEvent("Egg here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                }
                else if (MutateRate == 5)
                {
                    Building_Turret GenMortar = (Building_Turret)ThingMaker.MakeThing(ThingDef.Named("Turret_GenMortarSeed"));
                    GenMortar.SetFactionDirect(factionDirect);
                    if (hasNoBuildings(Position))
                    {
                        GenSpawn.Spawn(GenMortar, Position);
                    }
                    this.MutateTry = false;
                    //Find.History.AddGameEvent("Mortar here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                }
                else if (MutateRate == 6)
                {
                    Building_Turret GenTurret = (Building_Turret)ThingMaker.MakeThing(ThingDef.Named("GenTurretBase"));
                    GenTurret.SetFactionDirect(factionDirect);
                    if (hasNoBuildings(Position))
                    {
                        GenSpawn.Spawn(GenTurret, Position);
                    }
                    this.MutateTry = false;
                    //Find.History.AddGameEvent("Turret here", GameEventType.BadNonUrgent, true, Position, string.Empty);
                }
                else
                {
                    this.MutateTry = false;
                }
            }
            if (stuckPawn != null)
            {
                int damageAmountBase = 1;
                DamageInfo damageInfo = new DamageInfo(this.dmgdef, damageAmountBase, this, null, null);
                stuckPawn.TakeDamage(damageInfo);
                stuckPawn = null;
            }
            if (stuckCorpse != null)
            {
                stuckCorpse.Destroy();
                stuckCorpse = null;
            }
        }
    }
}
