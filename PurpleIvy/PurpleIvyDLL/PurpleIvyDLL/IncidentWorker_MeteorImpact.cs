using System;
using Verse;
namespace RimWorld
{
    public class IncidentWorker_MeteorImpact : IncidentWorker
    {
        private const float FogClearRadius = 4.5f;
        public override bool TryExecute(IncidentParms parms)
        {
            ThingDef thingDef = ThingDef.Named("Meteorite");
            Thing singleContainedThing = ThingMaker.MakeThing(thingDef);
            IntVec3 intVec = GenCellFinder.RandomCellWith((IntVec3 sq) => GenGrid.Standable(sq) && !Find.RoofGrid.Roofed(sq) && !FogUtility.Fogged(sq));
            MeteorUtility.MakeMeteorAt(intVec, new MeteorInfo
            {
                SingleContainedThing = singleContainedThing,
                openDelay = 1,
                leaveSlag = false
            });
            Find.History.AddGameEvent("Look a giant flying purple rock....its purple it has to be something good...right?", GameEventType.BadNonUrgent, true, intVec, string.Empty);
            return true;
        }
    }
}
