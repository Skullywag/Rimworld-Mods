using System;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    public class Building_BatteryMk2 : Building
    {
        private const float MinEnergyToExplode = 500f;
        private const float EnergyToLoseWhenExplode = 400f;
        private const float ExplodeChancePerDamage = 0.05f;
        private int ticksToExplode;
        private Sustainer wickSustainer;
        private static readonly Vector2 BarSize = new Vector2(1.3f, 0.4f);
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.4f, 0.35f, 0.8f));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));
        private static readonly SoundDef WickSound = SoundDef.Named("HissSmall");
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<int>(ref this.ticksToExplode, "ticksToExplode", 0, false);
        }
        public override void Draw()
        {
            base.Draw();
            CompPowerBattery comp = base.GetComp<CompPowerBattery>();
            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = this.DrawPos + Vector3.up * 0.1f;
            r.size = Building_BatteryMk2.BarSize;
            r.fillPercent = comp.StoredEnergy / comp.props.storedEnergyMax;
            r.filledMat = Building_BatteryMk2.BarFilledMat;
            r.unfilledMat = Building_BatteryMk2.BarUnfilledMat;
            r.margin = 0.15f;
            IntRot rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
            if (this.ticksToExplode > 0)
            {
                OverlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
            }
        }
        public override void Tick()
        {
            base.Tick();
            if (this.ticksToExplode > 0)
            {
                this.wickSustainer.Maintain();
                this.ticksToExplode--;
                if (this.ticksToExplode == 0)
                {
                    IntVec3 loc = GenAdj.CellsOccupiedBy(this).ToList<IntVec3>().RandomListElement<IntVec3>();
                    float radius = Rand.Range(0.5f, 1f) * 3f;
                    GenExplosion.DoExplosion(loc, radius, DamageTypeDefOf.Flame, null, null, null);
                    base.GetComp<CompPowerBattery>().DrawPower(400f);
                }
            }
        }
        public override void PostApplyDamage(DamageInfo dinfo)
        {
            if (!base.Destroyed && this.ticksToExplode == 0 && dinfo.Def == DamageTypeDefOf.Flame && Rand.Value < 0.05f && base.GetComp<CompPowerBattery>().StoredEnergy > 500f)
            {
                this.ticksToExplode = Rand.Range(70, 150);
                SoundInfo info = SoundInfo.InWorld(this, MaintenanceType.PerTick);
                this.wickSustainer = Building_BatteryMk2.WickSound.TrySpawnSustainer(info);
            }
        }
    }
}
