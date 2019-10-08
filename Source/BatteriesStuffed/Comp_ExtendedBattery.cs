using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace BatteriesStuffed
{
    public class Comp_ExtendedBattery : ThingComp
    {
        public CompProperties_ExtendedBattery Props => base.props as CompProperties_ExtendedBattery;
    }

    public class CompProperties_ExtendedBattery : CompProperties
    {
        public CompProperties_ExtendedBattery()
        {
            compClass = typeof(Comp_ExtendedBattery);
        }

        public bool canExplode = false;
        public bool canShortCircuit = true;
        //public float idlePowerDrainWatts = 5;

        public float fireExplodeSize = 2;
        public float empExplodeSize = 8;

        public float barMargin = 0.15f;
        public Vector2 barSize = new Vector2(1.3f, 0.4f);
        public Color barFilledColor = new Color(0.9f, 0.85f, 0.2f);
        public Color barUnfilledColor = new Color(0.3f, 0.3f, 0.3f);
    }
}
