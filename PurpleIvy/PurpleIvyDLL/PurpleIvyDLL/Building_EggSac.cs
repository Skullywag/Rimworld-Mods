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
        Faction factionDirect = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("Genny", true));
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.SetFactionDirect(factionDirect);
        }

        public override void TickRare()
        {
            base.TickRare();
            if (EcosystemFull()) 
            {
                return;
            }
            Random random = new Random();
            int Spawnrate = random.Next(1, 50);
            if (Spawnrate == 5)
            {
                PawnKindDef pawnKindDef = PawnKindDef.Named("Genny_Centipede");
                Pawn NewPawn = PawnGenerator.GeneratePawn(pawnKindDef, null);
                NewPawn.kindDef = pawnKindDef;
                NewPawn.SetFactionDirect(factionDirect);
                NewPawn.thinker = new Pawn_Thinker(NewPawn);
                GenSpawn.Spawn(NewPawn, Position);
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