using System;
using System.Collections.Generic;
using System.Diagnostics;
using Verse;
using Verse.AI;
using VerseBase;
using RimWorld;
namespace MoreWeapons
{
	public class Building_APM : Building
	{
        private float radius;
        private DamageTypeDef dmgdef;
        private int delay = 30;
        private int start = 1800;
        private bool armed = false;
        private Graphic_Single armedgfc;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.armedgfc = new Graphic_Single(MaterialPool.MatFrom("Things/Weapons/APM_disarmed", true));
        }

        private void Command_Detonate()
        {
            radius = base.GetComp<CompExplosive>().props.explosiveRadius;
            dmgdef = base.GetComp<CompExplosive>().props.explosiveDamageType;
            this.Destroy(DestroyMode.Vanish);
            BodyPartDamageInfo value = new BodyPartDamageInfo(null, new BodyPartDepth?(BodyPartDepth.Outside));
            ExplosionInfo explosionInfo = default(ExplosionInfo);
            explosionInfo.center = base.Position;
            explosionInfo.radius = radius;
            explosionInfo.dinfo = new DamageInfo(dmgdef, 30, this, new BodyPartDamageInfo?(value), null);
            explosionInfo.Explode();
        }

        public override Graphic Graphic
        {
            get
            {
                return this.armedgfc;
            }
        }


        public override void Tick()
        {
            base.Tick();
            start--;
            if(start == 0)
            {
                armed = true;
                this.armedgfc = new Graphic_Single(MaterialPool.MatFrom("Things/Weapons/APM_armed", true));
            }
            if (armed == true)
            {
                delay--;
                if (delay == 1)
                {
                    foreach (IntVec3 current in GenAdj.AdjacentSquaresCardinal(this))
                    {
                        foreach (Thing thing in Find.ThingGrid.ThingsAt(current))
                        {
                            if (thing is Pawn)
                            {
                                this.Command_Detonate();
                                delay = 30;
                            }
                        }
                    }
                }
                else if (delay == 0)
                {
                    delay = 30;
                }
            }
        }
	}
}