using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    internal class ThingDef_Torch : ThingDef
    {
        public int MaxBurnTime;
    }

    public class Torch : Building_TempControl
    {
        private int ticksSinceRoomHeat;
        public float fireSize = 0.5f;
        public int BurnTime { get; set; }
        public override void SpawnSetup()
        {
            base.SpawnSetup();

            ThingDef_Torch TorchDef = (ThingDef_Torch)this.def;
            BurnTime = TorchDef.MaxBurnTime;
        }

        public override void Tick()
        {
            base.Tick();
            BurnTime--;
            if(BurnTime == 0)
            {
                base.GetComp<CompGlower>().Lit = false;
                MoteMaker.ThrowDustPuff(Position, 0.3f);
                this.Destroy();
            }
            else
            {
                base.GetComp<CompGlower>().Lit = true;
            }
        }
    }
}
