using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;

namespace MentalrobClient.HarmonyPatches
{
    [HarmonyPatch(typeof(Mission), "OnAgentShootMissile")]
    class MissionMissilePatch
    {
        public static bool Prefix(Mission __instance , Agent shooterAgent, EquipmentIndex weaponIndex, Vec3 position, Vec3 velocity, Mat3 orientation, bool hasRigidBody, bool isPrimaryWeaponShot, int forcedMissileIndex)
        {
            // Create a Missile packet and send to server
            MethodInfo dynMethod = __instance.GetType().GetMethod("AddMissileAux", BindingFlags.NonPublic | BindingFlags.Instance);
            MissionWeapon weapon;
            if (shooterAgent.Equipment[weaponIndex].CurrentUsageItem.IsRangedWeapon && shooterAgent.Equipment[weaponIndex].CurrentUsageItem.IsConsumable)
            {
                weapon = shooterAgent.Equipment[weaponIndex];
            }
            else
            {
                weapon = shooterAgent.Equipment[weaponIndex].AmmoWeapon;
            }
            weapon.Amount = 1;
            WeaponData weaponData = weapon.GetWeaponData(true);
            WeaponStatsData[] weaponStatsData = weapon.GetWeaponStatsData();
            Vec3 direction = velocity;
            float num = direction.Normalize();
            WeaponComponentData currentUsageItem = shooterAgent.Equipment[shooterAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].CurrentUsageItem;
            num = MissionGameModels.Current.AgentApplyDamageModel.CalculateEffectiveMissileSpeed(shooterAgent, currentUsageItem, ref direction, num);
            float num2 = (float)shooterAgent.Equipment[shooterAgent.GetWieldedItemIndex(Agent.HandIndex.MainHand)].GetModifiedMissileSpeedForCurrentUsage();
            object[] args = new object[] {
                0, true, shooterAgent, weaponData, weaponStatsData, 0, position, direction, orientation, num2, num, false, null, isPrimaryWeaponShot, null
            };
            dynMethod.Invoke(__instance, args);
            return false;
        }
    }
}
