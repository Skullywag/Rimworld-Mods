using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Verse;
using Verse.AI;
using RimWorld;
namespace MoreWeapons
{
	public class Building_PestZapper : Building_Turret
	{
		public Equipment gun;
		protected TurretTop top;
		protected CompPowerTrader powerComp;
		protected CompMannable mannableComp;
		protected TargetPack currentTargetInt = TargetPack.Invalid;
		protected int burstWarmupTicksLeft;
		protected int burstCooldownTicksLeft;
		public override TargetPack CurrentTarget
		{
			get
			{
				return this.currentTargetInt;
			}
		}
		private bool WarmingUp
		{
			get
			{
				return this.burstWarmupTicksLeft > 0;
			}
		}
		public override void SpawnSetup()
		{
			base.SpawnSetup();
			this.powerComp = base.GetComp<CompPowerTrader>();
			this.mannableComp = base.GetComp<CompMannable>();
			this.gun = (Equipment)ThingMaker.MakeThing(this.def.building.turretGunDef, null);
			this.gun.InitVerbs();
			foreach (Verb current in this.gun.AllVerbs)
			{
				current.caster = this;
				current.castCompleteCallback = new Action(this.BurstComplete);
			}
			this.top = new TurretTop(this);
		}
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.LookValue<int>(ref this.burstCooldownTicksLeft, "burstCooldownTicksLeft", 0, false);
		}
		public override void OrderAttack(TargetPack targ)
		{
			if ((targ.Loc - base.Position).LengthHorizontal < this.gun.PrimaryVerb.verbProps.minRange)
			{
				Messages.Message("MessageTargetBelowMinimumRange".Translate());
				return;
			}
			if ((targ.Loc - base.Position).LengthHorizontal > this.gun.PrimaryVerb.verbProps.range)
			{
				Messages.Message("MessageTargetBeyondMaximumRange".Translate());
				return;
			}
			this.forcedTarget = targ;
		}
		public override void Tick()
		{
			base.Tick();
			if (this.powerComp != null && !this.powerComp.PowerOn)
			{
				return;
			}
			this.gun.verbTracker.VerbsTick();
			if (this.stunner.Stunned)
			{
				return;
			}
			if (this.gun.PrimaryVerb.state == VerbState.Bursting)
			{
				return;
			}
			if (this.WarmingUp)
			{
				this.burstWarmupTicksLeft--;
				if (this.burstWarmupTicksLeft == 0)
				{
					this.BeginBurst();
				}
			}
			else
			{
				if (this.burstCooldownTicksLeft > 0)
				{
					this.burstCooldownTicksLeft--;
				}
				if (this.burstCooldownTicksLeft == 0)
				{
					this.TryStartShootSomething();
				}
			}
			this.top.TurretTopTick();
		}
		protected void TryStartShootSomething()
		{
			this.currentTargetInt = this.TryFindNewTarget();
			if (this.currentTargetInt.Valid)
			{
				if (this.def.building.turretBurstWarmupTicks > 0)
				{
					this.burstWarmupTicksLeft = this.def.building.turretBurstWarmupTicks;
				}
				else
				{
					this.BeginBurst();
				}
			}
		}
		protected TargetPack TryFindNewTarget()
		{
			Thing searcher;
			searcher = this;
            //Messages.Message("Spotting target", MessageSound.Silent);
			return GenAI.BestShootTargetFromCurrentPosition(base.Position, searcher, new Predicate<Thing>(this.IsValidTarget), this.gun.PrimaryVerb.verbProps.range, this.gun.PrimaryVerb.verbProps.minRange, !this.gun.PrimaryVerb.verbProps.projectileDef.projectile.flyOverhead, !this.gun.PrimaryVerb.verbProps.ai_IsIncendiary);
        }
		private bool IsValidTarget(Thing t)
		{
            //Messages.Message("Thing is "+t, MessageSound.Silent);
			Pawn pawn = t as Pawn;
            //Messages.Message("Checking target", MessageSound.Silent);
			if (pawn != null)
			{
                //Messages.Message("pawn is an animal = " + pawn.def.race.IsAnimal, MessageSound.Silent);
                if (pawn.def.race.IsAnimal == true)
                {
                    //Messages.Message("DIE MUTHAFUCKA!", MessageSound.Silent);
                    return true;
                }
			}
			return false;
		}
		protected void BeginBurst()
		{
			this.gun.PrimaryVerb.TryStartCastOn(this.CurrentTarget);
		}
		protected void BurstComplete()
		{
			if (this.def.building.turretBurstCooldownTicks >= 0)
			{
				this.burstCooldownTicksLeft = this.def.building.turretBurstCooldownTicks;
			}
			else
			{
				this.burstCooldownTicksLeft = this.gun.PrimaryVerb.verbProps.cooldownTicks;
			}
		}
		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			string inspectString = base.GetInspectString();
			if (!inspectString.NullOrEmpty())
			{
				stringBuilder.AppendLine(inspectString);
			}
			stringBuilder.AppendLine("GunInstalled".Translate() + ": " + this.gun.LabelCap);
			if (this.gun.PrimaryVerb.verbProps.minRange > 0f)
			{
				stringBuilder.AppendLine("MinimumRange".Translate() + ": " + this.gun.PrimaryVerb.verbProps.minRange.ToString("F0"));
			}
			if (this.burstCooldownTicksLeft > 0)
			{
				stringBuilder.AppendLine("CanFireIn".Translate() + ": " + this.burstCooldownTicksLeft.TickstoSecondsString());
			}
			return stringBuilder.ToString();
		}
		public override void Draw()
		{
			this.top.DrawTurret();
			base.Draw();
		}
		public override void DrawExtraSelectionOverlays()
		{
			float range = this.gun.PrimaryVerb.verbProps.range;
			if (range < 90f)
			{
				GenDraw.DrawRadiusRing(base.Position, range);
			}
			float minRange = this.gun.PrimaryVerb.verbProps.minRange;
			if (minRange < 90f && minRange > 0.1f)
			{
				GenDraw.DrawRadiusRing(base.Position, minRange);
			}
			if (this.burstWarmupTicksLeft > 0)
			{
				int degreesWide = (int)((float)this.burstWarmupTicksLeft * 0.5f);
				GenDraw.DrawAimPie(this, this.CurrentTarget, degreesWide, (float)this.def.size.x * 0.5f);
			}
		}
	}
}
