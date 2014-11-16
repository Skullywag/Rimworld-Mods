using MoreWeapons;
using System;
using UnityEngine;
using System.Collections.Generic;
using Verse;
using Verse.Sound;
using RimWorld;
namespace MoreWeapons
{
    public class Projectile_FireExtinguisher : Bullet
    {
        public override void SpawnSetup()
        {
            base.SpawnSetup();
        }

        public override void Tick()
        {
            base.Tick();
        }

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            MoteMaker.TryThrowSmoke(base.Position.ToVector3Shifted(), 1f);
            foreach (IntVec3 current in GenAdj.AdjacentSquares8Way(this))
            {
                List<Thing> list = Find.ThingGrid.ThingsListAt(current);
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].def.eType == EntityType.Fire)
                    {
                        list[i].TakeDamage(new DamageInfo(DamageTypeDefOf.Extinguish, 5, this, null, null));
                    }
                }
            }
        }
    }
}
