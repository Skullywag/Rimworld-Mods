using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Verse;
using Verse.AI;
using VerseBase;
using RimWorld;
using UnityEngine;
namespace MoreDeco
{
	public class Building_ThermiteBomb : Building
	{
        private float radius;
        private DamageTypeDef dmgdef;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
        }

        public override IEnumerable<Command> GetCommands()
        {
            Command_Action com = new Command_Action();
            com.hotKey = KeyCode.V;

            com.defaultLabel = "CommandDetonateLabel".Translate();
            com.icon = ContentFinder<Texture2D>.Get("UI/Commands/Detonate");
            com.defaultDesc = "CommandDetonateDesc".Translate();
            com.action = () => Command_Detonate();
           
            yield return com;
        }

        private void Command_Detonate()
        {
            radius = base.GetComp<CompExplosive>().props.explosiveRadius;
            dmgdef = base.GetComp<CompExplosive>().props.explosiveDamageType;
            this.Destroy(DestroyMode.Vanish);
            BodyPartDamageInfo value = new BodyPartDamageInfo(null, new BodyPartDepth?(BodyPartDepth.Outside));
            ExplosionInfo explosionInfo = default(ExplosionInfo);
            explosionInfo.center = Position;
            explosionInfo.radius = radius;
            explosionInfo.dinfo = new DamageInfo(dmgdef, 100, this, new BodyPartDamageInfo?(value), null);
            explosionInfo.Explode();
            explosionInfo.dinfo = new DamageInfo(dmgdef, 100, this, new BodyPartDamageInfo?(value), null);
            explosionInfo.dinfo = new DamageInfo(dmgdef, 100, this, new BodyPartDamageInfo?(value), null);
            explosionInfo.dinfo = new DamageInfo(dmgdef, 100, this, new BodyPartDamageInfo?(value), null);
            MoteMaker.TryThrowMicroSparks(Position.ToVector3Shifted());
            if(Position.GetRoof() != null)
            {
                if (Find.RoofGrid.RoofDefAt(Position).isThickRoof == true)
                {
                    RoofDef roofType = DefDatabase<RoofDef>.GetNamed("RoofRockThin");
                    Find.RoofGrid.SetRoof(Position, roofType);
                }
                else
                {
                    Find.RoofGrid.SetRoof(Position, null);
                }    
            }
            foreach (IntVec3 current in GenAdj.AdjacentSquares8Way(this))
            {
                if (current.GetRoof() != null)
                {
                    if (Find.RoofGrid.RoofDefAt(current).isThickRoof == true)
                    {
                        RoofDef roofType = DefDatabase<RoofDef>.GetNamed("RoofRockThin");
                        Find.RoofGrid.SetRoof(current, roofType);
                    }
                    else
                    {
                        Find.RoofGrid.SetRoof(current, null);
                    }
                }
            }
        }
	}
}