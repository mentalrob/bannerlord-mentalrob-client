using BannerlordDedicatedServer.Helpers;
using BannerlordDedicatedServer.NetworkPackets;
using MentalrobClient.Networking;
using System;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using System.Linq;
using BannerlordDedicatedServer.NetworkPackets.ServerPackets;
using BannerlordDedicatedServer.NetworkPackets.ClientPackets;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using System.Reflection;
using BannerlordDedicatedServer.PacketHandlers;
using MentalrobClient.NetworkPackets;
using static TaleWorlds.MountAndBlade.Agent;
using System.Threading.Tasks;
using MentalrobClient.NetworkPackets.ClientPackets;
using System.Collections.Generic;
using System.Threading;
using MentalrobClient.Helpers;
using System.Runtime.InteropServices;

namespace MentalrobClient.MissionManager
{
    class MentalrobMissionController : MissionLogic
    {


        public const int BUFFER_SIZE = 1024;
        private const int SERVER_TICK_RATE = 60;
        private int currentTick;

        public static Agent ControlledAgent;
        public static int controlledAgentIndex = -1;
        public static Vec3 latestProcessedState;
        public static Vec3 latestServerState;
        public static Vec3[] serverPositions;
        public static Vec3[] clientPositions;

        public static Dictionary<int, Agent> PlayerControlledAgents = new Dictionary<int, Agent>();

        private Agent DebugAgent;


        public MentalrobMissionController(String ip, int port)
        {
            this._game = Game.Current;
            this._ip = ip;
            this._port = port;
        }

        internal unsafe void UnsafeTeleport(Agent agent, ref Vec3 newPosition)
        {
            UIntPtr positionPtr = (UIntPtr)typeof(Agent).GetField("_positionPointer", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(agent);
            Vec3* ptr = (Vec3*)positionPtr.ToPointer();
            *ptr = newPosition;
        }


        public string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[8];
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);
            return finalString;
        }

        private void LogMessage(String s)
        {
            InformationManager.DisplayMessage(new InformationMessage(s));
        }

       
        public override void AfterStart()
        {
            base.AfterStart();
            
            serverPositions = new Vec3[BUFFER_SIZE];
            clientPositions = new Vec3[BUFFER_SIZE];
            // minTimeBetweenTicks = 1f / SERVER_TICK_RATE;

            Team team = base.Mission.Teams.Add(BattleSideEnum.Attacker);
            team.SetIsEnemyOf(team, true);
            base.Mission.SetMissionMode(MissionMode.Battle, true);

            Telepathy.Log.Info = LogMessage;
            Telepathy.Log.Error = LogMessage;
            Telepathy.Log.Warning = LogMessage;

            NetworkClient.Current.OnConnected = this.OnConnected;
            NetworkClient.Current.OnData = this.OnData;
            NetworkClient.Current.OnDisconnected = this.OnDisconnected;

            // connect
            NetworkClient.Current.Connect(this._ip, this._port);
            EntityInterpolationManager entityInterpolationManager = new EntityInterpolationManager();
            
        }

        public void OnData(ArraySegment<byte> message)
        {
            byte[] serializedPacket = message.Skip(message.Offset).Take(message.Count).ToArray();
            Packet packet = ProtobufHelper.ProtoDeserialize<Packet>(serializedPacket);
            Type packetType = NetworkPacketRegistry.storage[packet.Id];
            MethodInfo methodInfo = typeof(ProtobufHelper).GetMethod("ProtoDeserialize", BindingFlags.Public | BindingFlags.Static);
            IServerDataPacket dataPacket = (IServerDataPacket)methodInfo.MakeGenericMethod(packetType).Invoke(null, new object[] { packet.Data });
            HandlerRegistry.storage[packetType].Handle(base.Mission, dataPacket);
        }

        public void OnConnected()
        {
            JoinGamePacket jgp = new JoinGamePacket()
            {
                PlayerName = "deneme123-" + this.RandomString(3)
            };
            CommunicatorHelper.ClientSendPacket<JoinGamePacket>(jgp);
        }

        public void OnDisconnected()
        {
            base.Mission.EndMission();
        }

        private MovementControlFlag CalculateMovementFlag()
        {
            if (ControlledAgent == null) return 0;
            MovementControlFlag currentFlag = ControlledAgent.MovementFlags;
            Vec2 inputDirection = ControlledAgent.MovementInputVector;
            if (inputDirection.X == 1)
            {
                currentFlag |= MovementControlFlag.StrafeRight;
            }
            else if (inputDirection.X == -1)
            {
                currentFlag |= MovementControlFlag.StrafeLeft;
            }
            if (inputDirection.Y == 1)
            {
                currentFlag |= MovementControlFlag.Forward;
            }
            else if (inputDirection.Y == -1)
            {
                currentFlag |= MovementControlFlag.Backward;
            }

            return currentFlag;
        }


        public override void OnRegisterBlow(
            Agent attacker, 
            Agent victim, 
            GameEntity realHitEntity, 
            Blow b,
            ref AttackCollisionData collisionData, 
            in MissionWeapon attackerWeapon)
        {
            if(MentalrobMissionController.ControlledAgent != null && attacker.Equals(MentalrobMissionController.ControlledAgent) && victim != null)
            {
                int victimIndex = MentalrobMissionController.PlayerControlledAgents.FirstOrDefault(x => x.Value.Equals(victim)).Key;
                int size = Marshal.SizeOf(b);
                byte[] blowData = new byte[size];
                IntPtr ptr = Marshal.AllocHGlobal(size);
                Marshal.StructureToPtr(b, ptr, true);
                Marshal.Copy(ptr, blowData, 0, size);
                Marshal.FreeHGlobal(ptr);
                AgentHitPacket ahp = new AgentHitPacket()
                {
                    blowData = blowData,
                    victimIndex = victimIndex
                };
                CommunicatorHelper.ClientSendPacket<AgentHitPacket>(ahp);
            }
        }

        public void OnClientTick(float dt)
        {
            if (!NetworkClient.Current.Connected) return;
            NetworkClient.Current.Tick(100);

            if (ControlledAgent == null || !ControlledAgent.IsActive()) return;
            int bufferIndex = currentTick % BUFFER_SIZE;
            MentalrobMissionController.clientPositions[bufferIndex] = ControlledAgent.Position;
            AgentDataPacket adp = new AgentDataPacket()
            {
                eventControlFlag = ControlledAgent.EventControlFlags,
                movementControlFlag = CalculateMovementFlag(),
                lookPosition = new float[3] { ControlledAgent.LookDirection.X, ControlledAgent.LookDirection.Y, ControlledAgent.LookDirection.Z },
                position = new float[3] { ControlledAgent.Position.X, ControlledAgent.Position.Y, ControlledAgent.Position.Z },
                actionCodeType = ControlledAgent.GetCurrentActionType(0),
                agentCrouching = ControlledAgent.CrouchMode,
                agentLeftStance = ControlledAgent.GetIsLeftStance(),
                tick = this.currentTick
            };
            CommunicatorHelper.ClientSendPacket<AgentDataPacket>(adp);
        }
        public override void OnPreMissionTick(float dt)
        {
            // Same tick with controlTick
            base.OnPreMissionTick(dt);
            OnClientTick(dt);
            currentTick++;
        }
        public override void OnMissionTick(float dt)
        {
            base.OnMissionTick(dt);

            

            if (base.Mission.InputManager.IsKeyPressed(TaleWorlds.InputSystem.InputKey.Y))
            {
                if(!ControlledAgent.IsActive())
                {
                    RequestAgentSpawnPacket rasp = new RequestAgentSpawnPacket()
                    {
                        characterId = "commander_2",
                    };
                    CommunicatorHelper.ClientSendPacket<RequestAgentSpawnPacket>(rasp);
                }
                /*ControlledAgent.MovementFlags = MovementControlFlag.StrafeLeft;
                LogMessage(ControlledAgent.GetIsLeftStance().ToString());
                LogMessage("My Position On Client: "+ ControlledAgent.Position);
                LogMessage("My Server Position: "+ latestServerState);*/ 
            }
            else if (base.Mission.InputManager.IsKeyDown(TaleWorlds.InputSystem.InputKey.U))
            {
                Agent targetAgent = MentalrobMissionController.PlayerControlledAgents.Values.ToList().Where((Agent a) => {
                    return !a.Equals(ControlledAgent);
                }).First();
                int index = MentalrobMissionController.PlayerControlledAgents.FirstOrDefault(x => !x.Value.Equals(ControlledAgent)).Key;
                
                LogMessage("Target Ent Position On Client: " + targetAgent.Position);
                LogMessage("Target Ent Position On Server: " + Utils.GetPositionFromUpdateAgentPacket(EntityInterpolationManager.Current.currentPacket[index]));
            }
        }

        protected override void OnEndMission()
        {
            base.OnEndMission();
            ControlledAgent = null;
            controlledAgentIndex = -1;
            PlayerControlledAgents.Clear();

            if (NetworkClient.Current.Connected)
            {
                NetworkClient.Current.Disconnect();
            }
        }

        // Token: 0x0400150D RID: 5389
        private Game _game;
        private String _ip;
        private int _port;
    }
}
