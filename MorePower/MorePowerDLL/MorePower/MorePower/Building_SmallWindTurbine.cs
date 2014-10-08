using RimWorld;
using UnityEngine;
using Verse;
using VerseBase;
namespace MorePower
{
    public class Building_SmallWindTurbine : Building
    {
        private CompPowerTrader powerComp;
        private int ticksSinceUpdateGraphic;
        private int updateAnimationEveryXTicks = 5;
        private int activeGraphicFrame = 0;
        private bool disableAnimation = false;
        private const int arraySize = 12;
        private int timeSince = 250;
        public static Graphic_Single[] graphic = null;
        private string graphicPathAdditionWoNumber = "_frame";


        public override void SpawnSetup()
        {
            base.SpawnSetup();
            this.powerComp = base.GetComp<CompPowerTrader>();
            powerComp.PowerOn = true;
        }
        public override void Tick()
        {
            if (timeSince == 1)
            {
                CheckWeather();
            }
            DoTickerWork(1);
            timeSince--;
            base.TickRare();
        }

        public override Graphic Graphic
        {
            get
            {
                if (disableAnimation)
                    return base.Graphic;

                if (graphic == null || graphic[0] == null)
                {
                    UpdateGraphics();
                    if (graphic == null || graphic[0] == null)
                        return base.Graphic;
                }

                if (graphic[activeGraphicFrame] != null)
                    return graphic[activeGraphicFrame];

                return base.Graphic;
            }
        }

        private void UpdateGraphics()
        {
            graphic = new Graphic_Single[arraySize];

            int indexOf_frame = def.graphicPathSingle.ToLower().LastIndexOf(graphicPathAdditionWoNumber);
            string graphicRealPathBase = def.graphicPathSingle.Remove(indexOf_frame);

            for (int i = 0; i < arraySize; i++)
            {
                string graphicRealPath = graphicRealPathBase + graphicPathAdditionWoNumber + (i + 1).ToString();

                Material mat = MaterialPool.MatFrom(graphicRealPath, def.shader);
                graphic[i] = new Graphic_Single(mat);
            }
        }

        private void DoTickerWork( int ticks )
        {

            if ( powerComp == null || !powerComp.PowerOn ||
                Find.RoofGrid.Roofed( Position ) )
            {
                activeGraphicFrame = 0;
                powerComp.powerOutput = 0;
                return;
            }

            if (!disableAnimation)
            {
                ticksSinceUpdateGraphic += ticks;
                if (ticksSinceUpdateGraphic >= updateAnimationEveryXTicks)
                {
                    ticksSinceUpdateGraphic = 0;
                    activeGraphicFrame++;
                    if (activeGraphicFrame >= arraySize)
                    {
                        activeGraphicFrame = 0;
                    }
                    Find.MapDrawer.MapChanged(Position, MapChangeType.Things, true, false);
                }
            }

        }

        private void CheckWeather()
        {
            WeatherDef curWeather = Find.WeatherManager.curWeather;
            //Messages.Message("Current Weather - " + curWeather, MessageSound.Negative);
            if (curWeather == WeatherDef.Named("Rain") || curWeather == WeatherDef.Named("FoggyRain") || curWeather == WeatherDef.Named("SnowGentle"))
            {
                this.powerComp.powerOutput = 600;
            }
            else if (curWeather == WeatherDef.Named("RainyThunderstorm") || curWeather == WeatherDef.Named("DryThunderstorm") || curWeather == WeatherDef.Named("SnowHard"))
            {
                this.powerComp.powerOutput = 700;
                if ((float)UnityEngine.Random.Range(1, 5) == 1)
                {
                    DamageInfo damageInfo = new DamageInfo(DamageTypeDefOf.Bullet, 1, this, null, null);
                    base.TakeDamage(damageInfo);
                }

            }
            else
            {
                this.powerComp.powerOutput = 500;
            }
            this.timeSince = 250;
        }
    }
}
