using System;
using System.Collections;
using Assets.Scripts.Player;
using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.Map;
using Assets.Scripts.Managers;
using Assets.Scripts.Player;


namespace Assets.Scripts.Server
{
    public class GameController : MonoBehaviour
    {
        private static GameController _instance;
        public static GameController Instance()
        {
            return _instance;
        }
        void Awake()
        {
            _instance = this;
        }

        public Unit[] UnitPrefabs;
        public Unit[] Units = new Unit[3];
        private Unit[] _playerUnitList;
        public Text[] PlayerNameList;
        public Matrix Map;
        private DateTime _serverClock;
        private bool _clockStarted;
        private DateTime _endTime;

        void Start()
        {
            for (int i = 0; i < Units.Length; i++)
            {
                Vector2 inicialPos = MainLoop.Instance().GameMap.GetMatrixCell(2 + i, 3 + i).transform.position;
                Unit unit = GameObject.Instantiate((GameObject)Resources.Load("UnitWolf"), inicialPos, Quaternion.identity).GetComponent<Unit>();

                unit.SetActionMenu(MainLoop.Instance().ActionMenuCanvas);
                unit.SetMoveButton(MainLoop.Instance().MoveButton);
                unit.CurrentTileCoords.SetIndice(2 + i, 3 + i);
                unit.Faction = Faction.Player0;

                _playerUnitList[i] = unit;
            }

            _playerUnitList = new Unit[GameSparksManager.Instance().RtSessionInfo.RtPlayerList.Count];

        }
    
    // Analytics: Use if you want to analize the packet information
        private int _packetSizeIncomming;
        private int _packetSizeSent;

        public void PacketReceived(int packetSize)
        {
            _packetSizeIncomming = packetSize;
        }

        public void SendRTData(int opCode, GameSparksRT.DeliveryIntent intent, RTData data, int[] targetPeers)
        {
            _packetSizeSent = GameSparksManager.Instance().GameSparksRtUnity
                .SendData(opCode, intent, data, targetPeers);
        }
        public void SendRTData(int opCode, GameSparksRT.DeliveryIntent intent, RTData data)
        {
            _packetSizeSent = GameSparksManager.Instance().GameSparksRtUnity.SendData(opCode, intent, data);
        }


        public void UpdateOpponentUnit(RTPacket rtPacket)
        {            
            for (int i = 0; i < _playerUnitList.Length; i++)
            {
                if (_playerUnitList[i].name == rtPacket.Sender.ToString())
                {
                    //_playerUnitList[i].GoToPosition = new Vector2(rtPacket.Data.GetVector4(1).Value.x, rtPacket.Data.GetVector4(1).Value.y) + new Vector2(rtPacket.Data.GetVector4(1).Value.z, rtPacket.Data.GetVector4(1).Value.w);
                    //_playerUnitList[i].GotoRotation = rtPacket.Data.GetFloat(2).Value;
                    int x = Map.FindTilebyPos(new Vector2(rtPacket.Data.GetVector4(1).Value.x, rtPacket.Data.GetVector4(1).Value.y)).Indice.X;
                    int y = Map.FindTilebyPos(new Vector2(rtPacket.Data.GetVector4(1).Value.x, rtPacket.Data.GetVector4(1).Value.y)).Indice.Y;

                    _playerUnitList[i].SetPosition(new Vector2(rtPacket.Data.GetVector4(1).Value.x, rtPacket.Data.GetVector4(1).Value.y), x, y);
                    Map.MoveUnit(_playerUnitList[i], Map.FindTilebyPos(new Vector2(rtPacket.Data.GetVector4(1).Value.x, rtPacket.Data.GetVector4(1).Value.y)).Indice);
                 
                }
            }
        }     

        //public void RegisterOpponentAttack(RTPacket rtPacket)
        //{
        //    for (int i = 0; i < _playerUnitList.Length; i++)
        //    {
        //        if (_playerUnitList[i].name == rtPacket.Data.GetString(1))
        //        {
        //            _playerUnitList[i].
        //        }
        //        if (rtPacket.Sender != GameSparksManager.Instance().GameSparksRtUnity.PeerId &&
        //            _playerUnitList[i].name == rtPacket.Data.GetString(2))
        //        {
        //            _playerUnitList[i].UpdateScore();
        //        }
        //    }
        //}

        public void OnOpponentDisconnected(int peerId)
        {
            for (int i = 0; i < _playerUnitList.Length; i++)
            {
                if (_playerUnitList[i].name == peerId.ToString())
                {
                    _playerUnitList[i].FinishedTurn = true;
                    break;
                }
            }
        }
    }
}
