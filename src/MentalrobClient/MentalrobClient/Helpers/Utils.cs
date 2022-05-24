using BannerlordDedicatedServer.NetworkPackets.ServerPackets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace MentalrobClient.Helpers
{
    class Utils
    {
        public static float CalculateDistance(Vec3 a, Vec3 b)
        {
            float aX = a.x;
            float aY = a.y;
            float aZ = a.z;

            float bX = b.x;
            float bY = b.y;
            float bZ = b.z;

            float distanceX = MathF.Pow(aX - bX, 2);
            float distanceY = MathF.Pow(aY - bY, 2);
            float distanceZ = MathF.Pow(aZ - bZ, 2);
            return MathF.Sqrt(distanceX + distanceY + distanceZ);
        }

        public static float CalculateDistance(Vec2 a, Vec2 b)
        {
            float aX = a.x;
            float aY = a.y;

            float bX = b.x;
            float bY = b.y;

            float distanceX = MathF.Pow(aX - bX, 2);
            float distanceY = MathF.Pow(aY - bY, 2);
            return MathF.Sqrt(distanceX + distanceY);
        }
        public static bool HasFlagOne<T>(T value, T[] flags) where T : struct
        {
            foreach(T flag in flags)
            {
                return ((uint)(object)value & (uint)(object)flag) == (uint)(object)flag;
            }
            return false;
        }

        public static Vec3 GetPositionFromUpdateAgentPacket(UpdateAgentPacket uap) {
            Vec3 v = new Vec3(uap.position[0], uap.position[1], uap.position[2]);
            return v;
        }
        public static Vec3 GetLookPositionFromUpdateAgentPacket(UpdateAgentPacket uap)
        {
            Vec3 v = new Vec3(uap.lookPosition[0], uap.lookPosition[1], uap.lookPosition[2]);
            return v;
        }
    }
}
