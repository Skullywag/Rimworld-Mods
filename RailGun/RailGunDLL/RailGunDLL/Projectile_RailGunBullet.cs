using System;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    public class Projectile_RailGunBullet : Bullet
    {
        private DamageTypeDef dmgdef;
        public int tickerType = 50;
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            dmgdef = DefDatabase<DamageTypeDef>.GetNamed("Vaporized", true);
            MoteMaker.ThrowFlash(base.Position, "SparkFlash", 20f);
        }
        public override void Tick()
        {
            base.Tick();
            MoteMaker.TryThrowMicroSparks(base.Position.ToVector3Shifted());
        }

        protected override void Impact(Thing hitThing)
        {
            if (hitThing != null)
            {
                int damageAmountBase = this.def.projectile.damageAmountBase;
                DamageInfo damageInfo = new DamageInfo(this.dmgdef, damageAmountBase, this.launcher, this.ExactRotation.eulerAngles.y, null, null);
                hitThing.TakeDamage(damageInfo);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null)
                {
                    IntVec3 pos = pawn.Position;
                    hitThing.TakeDamage(new DamageInfo(dmgdef, 0, this.launcher, null, null));
                    if (pawn.IsColonist)
                    {
                        Messages.Message(pawn.Name.first + " " + pawn.Name.last + " was vaporized.", MessageSound.Negative);
                    }                   
                    pawn.Destroy();
                }
            }
            else
            {
                MoteMaker.MakeShotHitDirt(this.ExactPosition);
            }
            this.Destroy();
        }
    }
}
