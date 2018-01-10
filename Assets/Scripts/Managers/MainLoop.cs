﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Map;
using Assets.Scripts.Player;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Assets.Scripts.Interface;
using Assets.Scripts.SupportClasses;

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
        
        private Matrix _map;
        private GameObject _actionMenu;
        private bool _isActionMenuActive;

        public List<IDeselect> interfaceList = new List<IDeselect>();

        public void Awake()
        {
            //definir o tamanho do mapa
            _map = new Matrix(16, 16);

            Vector2 unitInicialPos = _map.GetMatrixCell(2, 3).transform.position;

            UnitPlayer = GameObject.Instantiate((GameObject)Resources.Load("Unit"), unitInicialPos, Quaternion.identity).GetComponent<Unit>();

            UnitPlayer.SetActionMenu(ActionMenuCanvas);
            UnitPlayer.SetMoveButton(MoveButton);
            UnitPlayer.Indice.SetIndice(2, 3);
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
        }

        /// <summary>
        /// Método que faz o HightLight das posições possiveis do player
        /// </summary>
        public void HighLightUnitRange()
        {
            Debug.Log("Enters");

            _map.CalculateUnitRange(UnitPlayer);

            for (int y = 0; y < _map.MatrixHeight; y++)
            {
                for (int x = 0; x < _map.MatrixWidth; x++)
                {
                    Indice p = new Indice(x, y);
                    Debug.Log(p.X + "," + p.Y);

                    if (UnitPlayer.InRangeCoordsList.Contains(p))
                    {
                        Debug.Log("ContemP!!!! ");
                        Debug.Log(UnitPlayer.InRangeCoordsList.Contains(p));
                        _map.GetMatrixCell(p.X, p.Y).HighLight();
                    }
                }
            }
        }        
    }
}
