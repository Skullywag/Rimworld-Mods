using Verse;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
namespace MoreWeapons
{
	public class Building_PestZapper : Building
	{
		public override void SpawnSetup()
		{
			base.SpawnSetup();
		}
		public override void TickRare()
		{
			base.TickRare();
            IEnumerable<Pawn> targets = FindAllAnimals(Position, 20);
            for (int i = targets.Count() - 1; i >= 0; i--)

		}
        public static IEnumerable<Pawn> FindAllAnimals(IntVec3 position, float distance)
        {
            return
                from p in Find.ListerPawns.AllPawns
                where p.RaceProps.IsAnimal && p.Position.WithinHorizontalDistanceOf(position, distance)
                select p;
        }
	}
}
