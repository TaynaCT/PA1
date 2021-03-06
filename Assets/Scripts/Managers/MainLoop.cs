﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Map;
using Assets.Scripts.Player;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.Interface;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.AI;
using System.Threading;
using Assets.Scripts.Server;
using GameSparks.RT;
using System;

namespace Assets.Scripts.Managers
{
    public class MainLoop : MonoBehaviour
    {                
        private static MainLoop _instance;
        public static MainLoop Instance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            Debug.Log("Is Null!!");
            return null;
        }
        
        public StatsManager StatsManager;
        public GameObject ActionMenuCanvas;
        public GameObject MoveButton;
        
        public Unit actualUnit;
        public Unit Enemy;
        public List<Unit> Player0;
        
        private Matrix _map;
        private GameObject _actionMenu;
        private bool _isActionMenuActive;
        private Thread aiThread;
        /// <summary>
        /// Unidade quer se mover ?
        /// </summary>
        private bool _wantToMove = false;
        private Vector2 _newpos = new Vector2();
        private Vector2 _oldPos = new Vector2(); 
        private Indice _newlIndice = new Indice(0,0);

        private Indice[] _spawmPos = { new Indice(5, 5), new Indice(5, 4), new Indice(5, 6), new Indice(10, 10), new Indice(10, 11), new Indice(10, 12) };
        public MatchType Match { get;  set; }

        public List<IDeselect> interfaceList = new List<IDeselect>();

        private List<Unit> _units;

        public void Awake()
        {
            Match = MatchType.MultiPlayer;
            Debug.Log("------------------> " + Match);
            _instance = this;
            _units = new List<Unit>();

            //definir o tamanho do mapa
            _map = new Matrix(24, 16);

            //Debug.Log("map" + _map.ToString());            
            
            if (Match == MatchType.SinglePlayer)
            {
                //Posições iniciais do player
                Vector2 playerInicialPos = _map.GetMatrixCell(2, 3).transform.position;
                Vector2 enemyInicialPos = _map.GetMatrixCell(6, 6).transform.position;

                actualUnit = GameObject.Instantiate((GameObject)Resources.Load("UnitWolf"), playerInicialPos, Quaternion.identity).GetComponent<Unit>();

                actualUnit.SetActionMenu(ActionMenuCanvas);
                actualUnit.SetMoveButton(MoveButton);
                actualUnit.CurrentTileCoords.SetIndice(2, 3);
                actualUnit.Faction = Faction.Player0;
                actualUnit.Behaviour = UnitBehaviour.AttackerAgressive;

                Enemy = GameObject.Instantiate((GameObject)Resources.Load("UnitEnemy"), enemyInicialPos, Quaternion.identity).GetComponent<Unit>();

                Enemy.SetActionMenu(ActionMenuCanvas);
                Enemy.SetMoveButton(MoveButton);
                Enemy.CurrentTileCoords.SetIndice(6, 6);
                Enemy.Faction = Faction.World0Enemy;
                Enemy.Behaviour = UnitBehaviour.AttackerAgressive;

                _units.Add(Enemy);
                _units.Add(actualUnit);

                _map.Units = _units;
            }

            else if (Match == MatchType.MultiPlayer) //multiplayer
            {
               // Player0 = new List<Unit>();
                //Player1 = new List<Unit>();
                //for (int i = 0; i < 3; i++)
                //{
                //    Vector2 inicialPos = _map.GetMatrixCell(2 + i, 3 + i).transform.position;
                //    Unit unit = GameObject.Instantiate((GameObject)Resources.Load("UnitWolf"), inicialPos, Quaternion.identity).GetComponent<Unit>();

                //    unit.SetActionMenu(ActionMenuCanvas);
                //    unit.SetMoveButton(MoveButton);
                //    unit.CurrentTileCoords.SetIndice(2 + i, 3 + i);
                //    unit.Faction = Faction.Player0;

                //    Player0.Add(unit);
                //    units.Add(unit);
                //}
                //_map.Units = units;

                //for (int i = 0; i < 3; i++)
                //{
                //    Vector2 inicialPos = _map.GetMatrixCell(2 * i, 3 * i).transform.position;
                //    Unit unit = GameObject.Instantiate((GameObject)Resources.Load("UnitLucky"), inicialPos, Quaternion.identity).GetComponent<Unit>();

                //    unit.SetActionMenu(ActionMenuCanvas);
                //    unit.SetMoveButton(MoveButton);
                //    unit.CurrentTileCoords.SetIndice(2 * i+5, 3 * i+5);
                //    unit.Faction = Faction.Player1;

                //    Player1.Add(unit);
                //    units.Add(unit);
                //}
            }
            else { Debug.Log("No Match"); }
        }

        public Unit SetUnit(Indice i, Sprite sprite, int spawmpointIndice, int peerId, string playerName)
        {
            //Vector2 inicialPos = _map.GetMatrixCell(2 * i.X, 3 * i.Y).transform.position;
            Vector2 inicialPos = _map.GetMatrixCell(_spawmPos[spawmpointIndice].X, _spawmPos[spawmpointIndice].Y).transform.position;
            Debug.Log("inicialPos -- " + inicialPos);
            Unit unit = GameObject.Instantiate((GameObject)Resources.Load("UnitLucky"), inicialPos, Quaternion.identity).GetComponent<Unit>();
            unit.UnitSprite = sprite;            
            unit.SetActionMenu(ActionMenuCanvas);
            unit.SetMoveButton(MoveButton);
            //unit.CurrentTileCoords.SetIndice(2 * i.X , 3 * i.Y );
            unit.SetPosition(inicialPos);
            unit.Faction = Faction.Player1;
            unit.UnitIdText(peerId.ToString(), playerName);
            
            Debug.Log("SetUnit - unit" + i + unit);

            Player0.Add(unit);
            _units.Add(unit);
            _map.Units = _units;
            return unit;
        }

        public void UpdateUnitPos(string unitId, Vector2 newpos)
        {
            foreach(Unit u in _units)
            {
                if(u.UnitId == unitId)
                {
                    u.SetPosition(newpos);
                }
            }
        }

        public Matrix GameMap
        {
            get { return _map; }
        }
        // Use this for initialization
        void Start()
        {
            Debug.Log(Match);
            StartCoroutine(SendUnitNewPos());
            //_instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            //UnitPlayer.SetText(UnitText);           
        }

        private int _packetSizeIncomming;
        public void PacketReceived(int packetSize)
        {
            _packetSizeIncomming = packetSize;
        }

        public void Deselect()
        {
            int interfacesCount = interfaceList.Count;

            for (int i = 0; i < interfacesCount; i++)
            {
                interfaceList[i].Deselect();
            }
        }

        public void MoveUnit(Vector2 newPos/*, int indiceX, int indiceY*/)
        {
            foreach (Unit u in Player0)
            {
                Debug.Log("foreach" + u.IsSelected);

                if (u.IsSelected == true)
                {                    
                    _oldPos = u.Position;
                    //UnitPlayer.Position = newPos;
                    u.SetPosition(newPos/*, indiceX, indiceY*/);
                    _newpos = newPos;
                    //_newlIndice.X = indiceX;
                    //_newlIndice.Y = indiceY;
                    actualUnit = u;
                    _wantToMove = true;
                }
            }
        }

        public IEnumerator SendUnitNewPos()
        {
            Debug.Log("Want to move" + _wantToMove);
            Debug.Log("gamesparksmanager>" + GameSparksManager.Instance().GameSparksRtUnity);

            if (_wantToMove)
            {
                using (RTData data = RTData.Get())
                {
                    data.SetVector2(1, actualUnit.Position);                       
                   // data.SetString(1, actualUnit.gameObject.name);
                   
                    GameSparksManager.Instance().GameSparksRtUnity.SendData(1, GameSparksRT.DeliveryIntent.UNRELIABLE_SEQUENCED, data);
                    Debug.Log(actualUnit.name + "has moved");
                }                
            }

            _wantToMove = false;
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(SendUnitNewPos());
        }

        public void AttackUnit()
        {
            Unit.Interact(actualUnit, Enemy);
        }

        public void ExitGame()
        {
            SceneManager.LoadScene("StartMenu");
        }

        public void EndTurn(/*não ponha nada aqui*/)
        {
            /*
             * 
             * change Actualplayer
             * */
            Debug.Log("starting AI");
            Brain.ProcessTurn(_map, Faction.World0Enemy);
            Debug.Log("ending AI");
        }

        /// <summary>
        /// Método que faz o HightLight das posições possiveis do player
        /// </summary>
        public void HighLightUnitRange()
        {
            Debug.Log("HighLightUnitRange");

            foreach (Unit u in Player0)
            {
                Debug.Log("foreach" + u.IsSelected);

                if (u.IsSelected == true)
                {
                    actualUnit = u;
                    if(actualUnit == null)
                    {
                        Debug.Log("Actual Unit Is null !!");
                    }
                    u.MovementType = MovementType.Foot;
                    _map.CalculateUnitMovementRange(u);

                    for (int y = 0; y < _map.MatrixHeight; y++)
                    {
                        for (int x = 0; x < _map.MatrixWidth; x++)
                        {
                            Indice p = new Indice(x, y);
                            //Debug.Log(p.X + "," + p.Y);

                            if (u.InMovementRangeCoords.Contains(p) /*UnitPlayer.InRangeCoordsList.Contains(p)*/)
                            {
                                Debug.Log("ContemP!!!! ");
                                Debug.Log(u.InMovementRangeCoords.Contains(p));
                                _map.GetMatrixCell(p.X, p.Y).HighLight();
                            }
                        }
                    }

                }
            }

            Debug.Log("Enters");
            //UnitPlayer.MovementType = MovementType.Foot;
            // _map.CalculateUnitRange(UnitPlayer);
            //_map.CalculateUnitMovementRange(UnitPlayer);

           
        }        
    }
}
