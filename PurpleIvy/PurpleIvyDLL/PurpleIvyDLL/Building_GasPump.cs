using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    public class Building_GasPump : Building
    {
        private int pumpfreq = 10;
        Faction factionDirect = Find.FactionManager.FirstFactionOfDef(DefDatabase<FactionDef>.GetNamed("Genny", true));
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.SetFactionDirect(factionDirect);
        }

        public override void TickRare()
        {
            base.TickRare();
            pumpfreq--;
            if (pumpfreq == 0)
            {
                MoteMaker.TryThrowSmoke(base.Position.ToVector3Shifted(), 2f);
                pumpfreq = 10;
            }
        }
    }
}