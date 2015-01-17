using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using VerseBase;
namespace RimWorld
{
    public class Projectile_Tomahawk : Bullet
    {
        private static readonly SoundDef TomaHit = SoundDef.Named("Pawn_Melee_BigBash_HitPawn");
        private float CurRotationInt = 0f;
        private int Rspeed = 3;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
        }

        public override void Tick()
        {
           base.Tick();
           this.CurRotationInt += (float)this.Rspeed;
        }

        public override void Draw()
        {
            Matrix4x4 matrix = default(Matrix4x4);
            Vector3 s = new Vector3(1f, 1f, 1f);
            matrix.SetTRS(this.DrawPos + Altitudes.AltIncVect, Gen.ToQuat(CurRotationInt), s);
            Graphics.DrawMesh(MeshPool.plane10, matrix, this.Graphic.MatAt(base.Rotation, null), 0);
        }

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);
            if (hitThing != null)
            {
                TomaHit.PlayOneShot(base.Position);
                GenSpawn.Spawn(ThingDef.Named("Weapon_Tomahawk"), base.Position);
            }
            else
            {
                MoteMaker.MakeShotHitDirt(this.ExactPosition);
                GenSpawn.Spawn(ThingDef.Named("Weapon_Tomahawk"), base.Position);

            }
        }
    }
}
