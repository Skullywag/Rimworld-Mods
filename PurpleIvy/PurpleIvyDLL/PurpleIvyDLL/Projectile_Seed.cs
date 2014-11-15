using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;
using System.Linq;
using Verse.AI;
namespace RimWorld
{
    public class Projectile_Seed : Projectile
    {
        public Equipment gun;
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.gun = (Equipment)ThingMaker.MakeThing(this.def.building.turretGunDef, null);
        }

        public override void Tick()
        {
            base.Tick();
        }

        private bool IsValidTarget(Thing t)
        {
            Pawn pawn = t as Pawn;
            if (pawn != null)
            {
                if (this.gun.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead)
                {
                    RoofDef roofDef = Find.RoofGrid.RoofDefAt(t.Position);
                    if (roofDef != null && roofDef.isThickRoof)
                    {
                        return false;
                    }
                }
                return !GenAI.MachinesLike(base.Faction, pawn);
            }
            return true;
        }

        protected override void Impact(Thing hitThing)
        {
            if (hitThing == null)
            {

            }
            else
            {
                foreach (IntVec3 current in GenAdj.AdjacentSquares8WayAndInside(hitThing.Position))
                {
                    MoteMaker.ThrowDustPuff(current, 2f);
                    Thing t = GenAI.BestAttackTarget(hitThing.Position, this, new Predicate<Thing>(this.IsValidTarget), 2f, 0f, false, false, false, true);
                    Pawn pawn = t as Pawn;
                    pawn.thinker.mindState.Sanity.Equals(SanityState.Psychotic);
                }
            }
		}
    }
}
