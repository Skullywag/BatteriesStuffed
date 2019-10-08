using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using RimWorld;
using UnityEngine;
using Verse;

namespace BatteriesStuffed
{
    public class BatteryMod : Mod
    {
        private static HarmonyInstance battery;

        public static HarmonyInstance Battery
        {
            get
            {
                if (battery == null)
                {
                    battery = HarmonyInstance.Create("com.batteriesstuffed.rimworld.mod");
                }

                return battery;
            }
        }

        public BatteryMod(ModContentPack content) : base(content)
        {
            Battery.PatchAll(Assembly.GetExecutingAssembly());
        }

        [HarmonyPatch(typeof(ShortCircuitUtility))]
        [HarmonyPatch("DrainBatteriesAndCauseExplosion")]
        public static class DrainBatteriesAndCauseExplosion_Patch
        {
            public static bool Prefix(PowerNet net, Building culprit, out float totalEnergy, out float explosionRadius)
            {
                totalEnergy = 0f;
                for (int i = 0; i < net.batteryComps.Count; i++)
                {
                    CompPowerBattery compPowerBattery = net.batteryComps[i];
                    if (compPowerBattery.parent is Building_StuffedBattery sb && !sb.Props.canShortCircuit) continue;
                    totalEnergy += compPowerBattery.StoredEnergy;
                    compPowerBattery.DrawPower(compPowerBattery.StoredEnergy);
                }
                explosionRadius = Mathf.Sqrt(totalEnergy) * 0.05f;
                explosionRadius = Mathf.Clamp(explosionRadius, 1.5f, 14.9f);
                GenExplosion.DoExplosion(culprit.Position, net.Map, explosionRadius, DamageDefOf.Flame, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                if (explosionRadius > 3.5f)
                {
                    GenExplosion.DoExplosion(culprit.Position, net.Map, explosionRadius * 0.3f, DamageDefOf.Bomb, null, -1, -1f, null, null, null, null, null, 0f, 1, false, null, 0f, 1, 0f, false);
                }
                return false;
            }
        }
    }
}
