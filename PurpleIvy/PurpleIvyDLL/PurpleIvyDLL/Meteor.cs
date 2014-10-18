using System;
using Verse;
using Verse.Sound;
namespace RimWorld
{
    public class Meteor : Thing
    {
        public int age;
        public MeteorInfo info;
        private static readonly SoundDef OpenSound = SoundDef.Named("DropPodOpen");
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.LookValue<int>(ref this.age, "age", 0, false);
            Scribe_Deep.LookDeep<MeteorInfo>(ref this.info, "info");
        }
        public override void Tick()
        {
            this.age++;
            if (this.age > this.info.openDelay)
            {
                this.PodOpen();
            }
        }
        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            foreach (Thing current in this.info.containedThings)
            {
                current.Destroy(DestroyMode.Vanish);
            }
            base.Destroy(mode);
            if (mode == DestroyMode.Kill)
            {
                for (int i = 0; i < 1; i++)
                {
                    Thing thing = ThingMaker.MakeThing(ThingDef.Named("ChunkSlag"), null);
                    GenPlace.TryPlaceThing(thing, base.Position, ThingPlaceMode.Near);
                }
            }
        }
        private void PodOpen()
        {
            foreach (Thing current in this.info.containedThings)
            {
                GenPlace.TryPlaceThing(current, base.Position, ThingPlaceMode.Near);
            }
            this.info.containedThings.Clear();
            if (this.info.leaveSlag)
            {
                for (int i = 0; i < 1; i++)
                {
                    Thing thing = ThingMaker.MakeThing(ThingDef.Named("ChunkSlag"), null);
                    GenPlace.TryPlaceThing(thing, base.Position, ThingPlaceMode.Near);
                }
            }
            Meteor.OpenSound.PlayOneShot(base.Position);
            this.Destroy(DestroyMode.Vanish);
        }
    }
}
