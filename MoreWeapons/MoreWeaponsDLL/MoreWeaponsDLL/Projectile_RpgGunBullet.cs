using MoreWeapons;
using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
namespace MoreWeapons
{
    public class Projectile_RpgGunBullet : Bullet
    {
        private DamageTypeDef dmgdef;
        private int ticksToDetonation;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
            MoteMaker.TryThrowSmoke(base.Position.ToVector3Shifted(), 5f);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<int>(ref this.ticksToDetonation, "ticksToDetonation", 0, false);
        }

        public override void Tick()
        {
            base.Tick();
            MoteMaker.TryThrowSmoke(base.Position.ToVector3Shifted(), 1f);
            if (this.ticksToDetonation > 0)
            {
                this.ticksToDetonation--;
                if (this.ticksToDetonation <= 0)
                {
                    this.Explode();
                }
            }
        }

        protected virtual void Explode()
        {
            this.Destroy(DestroyMode.Vanish);
            BodyPartDamageInfo value = new BodyPartDamageInfo(null, new BodyPartDepth?(BodyPartDepth.Outside));
            ExplosionInfo explosionInfo = default(ExplosionInfo);
            explosionInfo.center = base.Position;
            explosionInfo.radius = this.def.projectile.explosionRadius;
            explosionInfo.dinfo = new DamageInfo(this.def.projectile.damageDef, 999, this.launcher, new BodyPartDamageInfo?(value), null);
            explosionInfo.preExplosionSpawnThingDef = this.def.projectile.preExplosionSpawnThingDef;
            explosionInfo.postExplosionSpawnThingDef = this.def.projectile.postExplosionSpawnThingDef;
            explosionInfo.explosionSpawnChance = this.def.projectile.explosionSpawnChance;
            explosionInfo.explosionSound = this.def.projectile.soundExplode;
            explosionInfo.projectile = this.def;
            explosionInfo.Explode();
        }

        protected override void Impact(Thing hitThing)
        {
            if (this.def.projectile.explosionDelay == 0)
            {
                this.Explode();
                return;
            }
            this.landed = true;
            this.ticksToDetonation = this.def.projectile.explosionDelay;
        }
    }
}
