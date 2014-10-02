using System;
using Verse;
public class CompGlowerX : CompGlower
{
    private bool glowOnInt;
    public bool LitX
    {
        get
        {
            return this.glowOnInt;
        }
        set
        {
            if (this.glowOnInt == value)
            {
                return;
            }
            this.glowOnInt = value;
            if (!value)
            {
                Find.MapDrawer.MapChanged(this.parent.Position, MapChangeType.Things);
                Find.GlowGrid.DeRegisterGlower(this);
                return;
            }
            Find.MapDrawer.MapChanged(this.parent.Position, MapChangeType.Things);
            Find.GlowGrid.RegisterGlower(this);
        }
    }
    public int RadiusIntCeilingX
    {
        get
        {
            return (int)Math.Ceiling((double)this.props.glowRadius);
        }
    }
    public override void PostSpawnSetup()
    {
        if (this.Lit)
        {
            Find.GlowGrid.RegisterGlower(this);
        }
        this.Lit = true;
    }
    public override void ReceiveCompSignal(string signal)
    {
    }
    public override void PostExposeData()
    {
        Scribe_Values.LookValue<bool>(ref this.glowOnInt, "glowOn", false, false);
    }
    public override void PostDestroy(DestroyMode mode = DestroyMode.Vanish)
    {
        base.PostDestroy(mode);
        this.Lit = false;
    }
}
