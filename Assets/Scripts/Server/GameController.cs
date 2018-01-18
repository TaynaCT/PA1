using System;
using System.Collections;
using Assets.Scripts.Player;
using GameSparks.RT;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.Map;
using Assets.Scripts.Managers;

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

        private MatchType _matchType;
       
       // public Unit[] UnitPrefabs;
        public Unit[] UnitsPerPlayer = new Unit[3];
        private Unit[,] _playerUnitList;
        public Sprite[] _spriteList;
        public Text[] PlayerNameList;
        public Matrix Map { get; private set; }
        private DateTime _serverClock;
        private bool _clockStarted;
        private DateTime _endTime;

        public MatchType Match {
            get { return _matchType;}
            set { _matchType = value; }
        }

        void Start()
        {
            _playerUnitList = new Unit[GameSparksManager.Instance().RtSessionInfo.RtPlayerList.Count, UnitsPerPlayer.Length];
            int spawmIndice = 0; //------------------------
            Debug.Log(Match);
            int unitOffset = 1;
            for (int playerIndex = 0; playerIndex < GameSparksManager.Instance().RtSessionInfo.RtPlayerList.Count; playerIndex++)
            {                
                Debug.Log("RtPlayerList.Count" + GameSparksManager.Instance().RtSessionInfo.RtPlayerList.Count);
                //para cada player é atribuido 3 unidades
                for (int i = 0; i < UnitsPerPlayer.Length; i++)
                {
                    Debug.Log(spawmIndice);
                    //Debug.Log(Map);
                    //Vector2 inicialPos = Map.GetMatrixCell(2 + i * playerIndex, 3 + i * playerIndex).transform.position;
                    //Unit unit = GameObject.Instantiate((GameObject)Resources.Load("UnitWolf"), inicialPos, Quaternion.identity).GetComponent<Unit>();
                    //unit.UnitSprite = _spriteList[playerIndex];
                    //unit.UnitId = GameSparksManager.Instance().RtSessionInfo.RtPlayerList[playerIndex].DisplayName;
                    //unit.SetActionMenu(MainLoop.Instance().ActionMenuCanvas);
                    //unit.SetMoveButton(MainLoop.Instance().MoveButton);
                    //unit.CurrentTileCoords.SetIndice(2 + i, 3 + i);
                    //unit.Faction = Faction.Player0;
                    _playerUnitList[playerIndex, i] = MainLoop.Instance().SetUnit(
                        new Indice(i * unitOffset, i * unitOffset), _spriteList[playerIndex], 
                        spawmIndice,
                        GameSparksManager.Instance().RtSessionInfo.RtPlayerList[playerIndex].PeerId,
                        GameSparksManager.Instance().RtSessionInfo.RtPlayerList[playerIndex].DisplayName);
                    spawmIndice++;
                    if(_playerUnitList[playerIndex, i] == null)
                    {
                        Debug.Log(_playerUnitList[playerIndex, i] + "IS NULL");
                    }

                    //UnitsPerPlayer[i] = unit;
                    
                }

                unitOffset++;
            }
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
            Debug.Log("Instanciação das unidades");
            for (int i = 0; i < GameSparksManager.Instance().RtSessionInfo.RtPlayerList.Count; i++)
            {
                for (int u = 0; u < UnitsPerPlayer.Length; u++)
                {
                    if (_playerUnitList[i, u].UnitId == GameSparksManager.Instance().RtSessionInfo.RtPlayerList[i].PeerId.ToString())
                    {
                        //if (_playerUnitList[i, u].name == rtPacket.Sender.ToString())
                        //{
                        Debug.Log("CONDIÇÃO ACEITE!!");
                         //_playerUnitList[i, u].SetPosition(new Vector2(rtPacket.Data.GetVector2(1).Value.x, rtPacket.Data.GetVector2(1).Value.y));
                        MainLoop.Instance().UpdateUnitPos(_playerUnitList[i, u].UnitId, new Vector2(rtPacket.Data.GetVector2(1).Value.x, rtPacket.Data.GetVector2(1).Value.y));
                        //}
                    }
                }
            }
        }

        public void SetMap(RTPacket rtPacket)
        {
            //Map = rtPacket.Data.GetString;
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
                if (UnitsPerPlayer[i].name == peerId.ToString())
                {
                    UnitsPerPlayer[i].FinishedTurn = true;
                    break;
                }
            }
        }
    }
}
