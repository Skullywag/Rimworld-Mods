using Verse;
namespace RimWorld
{
	public class Building_Flare : Building
	{
        private int Burnticks = 1800;

        public override void SpawnSetup()
        {
            base.SpawnSetup();
        }

        public override void Tick()
        {
            base.Tick();
            Burnticks--;
            if (Burnticks == 0)
            {
                this.Destroy();
            }
            else
            {
                MoteMaker.ThrowFlash(base.Position, "SparkFlash", 2f);
                MoteMaker.TryThrowMicroSparks(base.Position.ToVector3Shifted());
            }
        }
	}
}