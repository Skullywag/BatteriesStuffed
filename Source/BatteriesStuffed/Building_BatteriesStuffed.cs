using System;
using UnityEngine;
using Verse;
using Verse.Sound;
using RimWorld;
namespace BatteriesStuffed
{
    public class Building_StuffedBattery : Building
    {
        private static readonly Vector2 BarSize = new Vector2(1.3f, 0.4f);
        private static readonly Material BarFilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.9f, 0.85f, 0.2f));
        private static readonly Material BarUnfilledMat = SolidColorMaterials.SimpleSolidColorMaterial(new Color(0.3f, 0.3f, 0.3f));
        public override void ExposeData()
        {
            base.ExposeData();
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
        }
        public override void Tick()
        {
            base.Tick();            
        }
    }
}
