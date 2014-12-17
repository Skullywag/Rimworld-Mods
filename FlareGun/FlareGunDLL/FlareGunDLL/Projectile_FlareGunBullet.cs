using System;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    public class Projectile_FlareGunBullet : Bullet
    {
        private DamageTypeDef dmgdef;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            MoteMaker.TryThrowSmoke(base.Position.ToVector3Shifted(), 3f);
        }

        public override void Tick()
        {
            base.Tick();
            MoteMaker.TryThrowMicroSparks(base.Position.ToVector3Shifted());
        }

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (hitThing != null)
            {
                hitThing.TryIgnite(0.3f);
                GenSpawn.Spawn(ThingDef.Named("Puddle_Fuel"), base.Position);
                FireUtility.TryStartFireIn(base.Position, 0.2f);
            }
            else
            {
                GenSpawn.Spawn(ThingDef.Named("FlareDeployed"), this.Position);
            }
            MoteMaker.ThrowFlash(base.Position, "ShotFlash", 6f);
            MoteMaker.TryThrowMicroSparks(base.Position.ToVector3Shifted());
        }
    }
}
