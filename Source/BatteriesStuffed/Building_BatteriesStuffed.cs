using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
namespace BatteriesStuffed
{
    public class Building_StuffedBattery : Building
    {
        private const float MinEnergyToExplode = 500f;
		private const float EnergyToLoseWhenExplode = 400f;
		private const float ExplodeChancePerDamage = 0.05f;
		private int ticksToExplode;
		private Sustainer wickSustainer;
		private static readonly Vector2 BarSize = new Vector2(1.3f, 0.4f);
		private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f));
		private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));
		private static readonly SoundDef WickSound = SoundDef.Named("HissSmall");
        private bool hasRand = false;
        private int chance;
        ThingDef moteThrown = ThingDef.Named("Mote_BlastEMP");
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
            r.size = Building_StuffedBattery.BarSize;
            r.fillPercent = comp.StoredEnergy / comp.props.storedEnergyMax;
            r.filledMat = Building_StuffedBattery.BarFilledMat;
            r.unfilledMat = Building_StuffedBattery.BarUnfilledMat;
            r.margin = 0.15f;
            Rot4 rotation = base.Rotation;
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
                    IntVec3 randomCell = this.OccupiedRect().RandomCell;
                    float radius = Rand.Range(0.5f, 1f) * 3f;
                    GenExplosion.DoExplosion(randomCell, radius, DamageDefOf.Flame, null, null, null, 1f);
                    base.GetComp<CompPowerBattery>().DrawPower(400f);
                }
            }         
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (this.def.defName != "PlasteelBattery" || this.def.defName != "UraniumBattery")
            {
                if (!base.Destroyed && this.ticksToExplode == 0 && dinfo.Def == DamageDefOf.Flame && Rand.Value < 0.05f && base.GetComp<CompPowerBattery>().StoredEnergy > 500f)
                {
                    this.ticksToExplode = Rand.Range(70, 150);
                    SoundInfo info = SoundInfo.InWorld(this, MaintenanceType.PerTick);
                    this.wickSustainer = Building_StuffedBattery.WickSound.TrySpawnSustainer(info);
                }
            }
        }
    }
}
