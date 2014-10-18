using System;
using Verse;
namespace RimWorld
{
    public static class ThingDefOfCustom
    {
        public static ThingDef Meteor;
        public static void RebindDefs()
        {
            DefOfHelper.BindDefsFor<ThingDef>(typeof(ThingDefOf));
        }
    }
}