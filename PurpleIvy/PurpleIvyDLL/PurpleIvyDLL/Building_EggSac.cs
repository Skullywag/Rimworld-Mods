using System;
using System.Collections.Generic;
using System.Text;
using Verse;
using Verse.AI;
using Verse.Sound;
namespace RimWorld
{
    public class Building_EggSac : Building
    {
        private int Spawnrate;
        private int Origspawnrate;
        Faction factionDirect = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("Genny", true));
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            Random random = new Random();
            Spawnrate = random.Next(30, 50);
            Origspawnrate = Spawnrate;
            this.SetFactionDirect(factionDirect);
        }

        public override void TickRare()
        {
            base.TickRare();
            Spawnrate--;
            if (EcosystemFull()) 
            {
                return;
            }
            if (Spawnrate == 0)
            {
                PawnKindDef pawnKindDef = PawnKindDef.Named("Genny_Centipede");
                Pawn NewPawn = PawnGenerator.GeneratePawn(pawnKindDef, null);
                NewPawn.kindDef = pawnKindDef;
                NewPawn.SetFactionDirect(factionDirect);
                NewPawn.thinker = new Pawn_Thinker(NewPawn);
                GenSpawn.Spawn(NewPawn, Position);
                Spawnrate = Origspawnrate;
            }
            
        }

        public static bool EcosystemFull()
        {
            float num = 0f;
            foreach (Pawn current in Find.ListerPawns.AllPawns)
            {
                if (current.kindDef.race.defName.Equals("Genny_Centipede"))
                {
                    num += 1;
                }
            }
            return num >= 20;
        }
    }
}