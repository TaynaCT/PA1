using System.Collections;
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
        
        public Unit UnitPlayer;
        public Unit Enemy;
        
        private Matrix _map;
        private GameObject _actionMenu;
        private bool _isActionMenuActive;
        private Thread aiThread;

        public List<IDeselect> interfaceList = new List<IDeselect>();

        public void Awake()
        {
            List<Unit> units = new List<Unit>();

            //definir o tamanho do mapa
            _map = new Matrix(24, 16);

            //Posições iniciais do player
            Vector2 playerInicialPos = _map.GetMatrixCell(2, 3).transform.position;
            Vector2 enemyInicialPos = _map.GetMatrixCell(6, 6).transform.position;

            UnitPlayer = GameObject.Instantiate((GameObject)Resources.Load("UnitWolf"), playerInicialPos, Quaternion.identity).GetComponent<Unit>();

            UnitPlayer.SetActionMenu(ActionMenuCanvas);
            UnitPlayer.SetMoveButton(MoveButton);
            UnitPlayer.CurrentTileCoords.SetIndice(2, 3);
            UnitPlayer.Faction = Faction.Player0;
            UnitPlayer.Behaviour = UnitBehaviour.Defender;

            Enemy = GameObject.Instantiate((GameObject)Resources.Load("UnitEnemy"), enemyInicialPos, Quaternion.identity).GetComponent<Unit>();

            Enemy.SetActionMenu(ActionMenuCanvas);
            Enemy.SetMoveButton(MoveButton);
            Enemy.CurrentTileCoords.SetIndice(6, 6);
            Enemy.Faction = Faction.World0Enemy;
            Enemy.Behaviour = UnitBehaviour.Defender;

            units.Add(Enemy);
            units.Add(UnitPlayer);

            _map.Units = units;

        }

        // Use this for initialization
        void Start()
        {
            _instance = this;
        }

        // Update is called once per frame
        void Update()
        {
            //UnitPlayer.SetText(UnitText);
        }

        public void Deselect()
        {
            int interfacesCount = interfaceList.Count;

            for (int i = 0; i < interfacesCount; i++)
            {
                interfaceList[i].Deselect();
            }
        }

        public void MoveUnit(Vector2 newPos, int indiceX, int indiceY)
        {
            //UnitPlayer.Position = newPos;
            UnitPlayer.SetPosition(newPos, indiceX, indiceY);
        }

        public void AttackUnit()
        {

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
            aiThread = new Thread(() => Brain.ProcessTurn(_map, Faction.Player0));
            aiThread.IsBackground = true;
            if (!aiThread.IsAlive)
            {
                aiThread.Start();
            }
            Debug.Log("ending AI");
        }

        /// <summary>
        /// Método que faz o HightLight das posições possiveis do player
        /// </summary>
        public void HighLightUnitRange()
        {
            Debug.Log("Enters");
            UnitPlayer.MovementType = MovementType.Foot;
            // _map.CalculateUnitRange(UnitPlayer);
            _map.CalculateUnitMovementRange(UnitPlayer);
            
            for (int y = 0; y < _map.MatrixHeight; y++)
            {
                for (int x = 0; x < _map.MatrixWidth; x++)
                {
                    Indice p = new Indice(x, y);
                    Debug.Log(p.X + "," + p.Y);

                    if (UnitPlayer.InMovementRangeCoords.Contains(p) /*UnitPlayer.InRangeCoordsList.Contains(p)*/)
                    {
                        Debug.Log("ContemP!!!! ");
                        Debug.Log(UnitPlayer.InMovementRangeCoords.Contains(p));
                        _map.GetMatrixCell(p.X, p.Y).HighLight();
                    }
                }
            }
        }        
    }
}
