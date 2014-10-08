using System;
using UnityEngine;
using Verse;
using VerseBase;
using RimWorld;
namespace MorePower
{
    public class Building_MicroPowerPlantSolar : Building_PowerPlant
    {
        private const float FullSunPower = 400f;
        private const float NightPower = 0f;
        private static readonly Vector2 BarSize = new Vector2(1.15f, 0.10f);
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.5f, 0.475f, 0.1f));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.15f, 0.15f, 0.15f));
        public override void Tick()
        {
            base.Tick();
            if (Find.RoofGrid.Roofed(base.Position))
            {
                this.powerComp.powerOutput = 0f;
            }
            else
            {
                this.powerComp.powerOutput = Mathf.Lerp(0f, 400f, SkyManager.curSkyGlowPercent);
            }
        }
        public override void Draw()
        {
            base.Draw();
            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = this.DrawPos + Vector3.up * 0.1f;
            r.size = Building_MicroPowerPlantSolar.BarSize;
            r.fillPercent = this.powerComp.powerOutput / 400f;
            r.filledMat = Building_MicroPowerPlantSolar.BarFilledMat;
            r.unfilledMat = Building_MicroPowerPlantSolar.BarUnfilledMat;
            r.margin = 0.15f;
            IntRot rotation = this.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
        }
    }
}
