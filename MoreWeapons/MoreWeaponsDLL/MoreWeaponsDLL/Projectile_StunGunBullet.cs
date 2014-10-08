using RimWorld;
using System;
using UnityEngine;
using Verse;
namespace MoreWeapons
{
    public class Projectile_StunGunBullet : Bullet
    {
        private const float StunChance = 1f;
        private DamageTypeDef dmgdef;
        public override void SpawnSetup()
        {
            base.SpawnSetup();
            dmgdef = DefDatabase<DamageTypeDef>.GetNamed("Spasm", true);
        }
        protected override void Impact(Thing hitThing)
        {
            if (hitThing != null)
            {
                int damageAmountBase = this.def.projectile.damageAmountBase;
                DamageInfo damageInfo = new DamageInfo(this.dmgdef, damageAmountBase, this.launcher, this.ExactRotation.eulerAngles.y, null, null);
                hitThing.TakeDamage(damageInfo);
                Pawn pawn = hitThing as Pawn;
                if (pawn != null && !pawn.healthTracker.Downed)
                {
                    hitThing.TakeDamage(new DamageInfo(dmgdef, 0, this.launcher, null, null));
                    pawn.healthTracker.ForceDowned();
                    if (pawn.def.race.humanoid)
                    {
                        //pawn.health = UnityEngine.Random.Range(80, 99);
                    }
                }
            }
            else
            {
                MoteMaker.MakeShotHitDirt(this.ExactPosition);
            }
            this.Destroy(0);
        }
    }
}
