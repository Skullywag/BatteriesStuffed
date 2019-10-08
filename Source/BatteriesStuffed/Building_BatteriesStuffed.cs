using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
namespace BatteriesStuffed
{
    [StaticConstructorOnStartup]
    public class Building_StuffedBattery : Building
    {
        private const float MinEnergyToExplode = 500f; //40% of battery capacity
		private const float EnergyToLoseWhenExplode = 400f; //35% of current energy
		private const float ExplodeChancePerDamage = 0.05f; // Should begin to risk exploding at 75% health, each damage after that should have 20% chance of lighting the wick code.
		private int ticksToExplode; // Batteries will have 2-3 seconds before exploding, can add this as an option in my extended comps however. Two layer explosion, small range fire large range EMP.
		private Sustainer wickSustainer;
        private Material BarFilledMat;
        private Material BarUnfilledMat;

        public CompProperties_ExtendedBattery Props => this.GetComp<Comp_ExtendedBattery>().Props;

        private Material BarFilled
        {
            get
            {
                if (BarFilledMat == null)
                {
                    BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(Props.barFilledColor);
                }
                return BarFilledMat;
            }
        }

        private Material BarUnfilled
        {
            get
            {
                if (BarUnfilledMat == null)
                {
                    BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(Props.barUnfilledColor);
                }
                return BarUnfilledMat;
            }
        }


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.ticksToExplode, "ticksToExplode", 0, false);
        }

        public override void Draw()
        {
            base.Draw();
            CompPowerBattery comp = base.GetComp<CompPowerBattery>();
            GenDraw.FillableBarRequest r = default(GenDraw.FillableBarRequest);
            r.center = this.DrawPos + Vector3.up * 0.1f;
            r.size = Props.barSize;
            /* Obsolete
             if (this.def.defName == "LargePlasteelBattery" || this.def.defName == "LargeSteelBattery" || this.def.defName == "LargeGoldBattery" || this.def.defName == "LargeSilverBattery" || this.def.defName == "LargeUraniumBattery")
                {
                    r.size = Building_StuffedBattery.LargeBarSize;
                }
                else
                {
                    r.size = Building_StuffedBattery.BarSize;
                }
            */
            r.fillPercent = comp.StoredEnergy / comp.Props.storedEnergyMax;
            r.filledMat = BarFilled;
            r.unfilledMat = BarUnfilled;
            r.margin = Props.barMargin;
            Rot4 rotation = base.Rotation;
            rotation.Rotate(RotationDirection.Clockwise);
            r.rotation = rotation;
            GenDraw.DrawFillableBar(r);
            if (this.ticksToExplode > 0 && base.Spawned)
            {
                base.Map.overlayDrawer.DrawOverlay(this, OverlayTypes.BurningWick);
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (this.ticksToExplode > 0)
            {
                if (this.wickSustainer == null)
                {
                    this.StartWickSustainer();
                }
                else
                {
                    this.wickSustainer.Maintain();
                }
                this.ticksToExplode--;
                if (this.ticksToExplode == 0)
                {
                    TryExplode();
                    base.GetComp<CompPowerBattery>().DrawPower(400f);
                }
            }
        }

        private void TryExplode()
        {
            if (!Props.canExplode) return;
            IntVec3 randomCell = this.OccupiedRect().RandomCell;
            GenExplosion.DoExplosion(randomCell, Map, Props.fireExplodeSize, DamageDefOf.Flame, this);
            GenExplosion.DoExplosion(randomCell, Map, Props.empExplodeSize, DamageDefOf.EMP, this);
        }

        private void StartWickSustainer()
        {
            SoundInfo info = SoundInfo.InMap(this, MaintenanceType.PerTick);
            this.wickSustainer = SoundDefOf.HissSmall.TrySpawnSustainer(info);
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            //if (this.def.defName != "PlasteelBattery" || this.def.defName != "UraniumBattery" || this.def.defName != "LargePlasteelBattery" || this.def.defName != "LargeUraniumBattery")
            if(Props.canExplode)
            {
                if (!base.Destroyed && this.ticksToExplode == 0 && dinfo.Def == DamageDefOf.Flame && Rand.Value < 0.05f && base.GetComp<CompPowerBattery>().StoredEnergy > 500f)
                {
                    this.ticksToExplode = Rand.Range(70, 150);
                    this.StartWickSustainer();
                }
            }
        }
    }
}
