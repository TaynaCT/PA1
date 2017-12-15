using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Managers;
using Assets.Scripts.Interface;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.Player;

namespace Assets.Scripts.Map
{    
    public class HexagonTile : MonoBehaviour, IDeselect
    {
        public enum TileType { Normal, Forest, River, Road }
        [SerializeField]
        Sprite[] TileSprite;

        private Indice _indice;
        private Unit _currentUnit;
        private bool _isHighLight;
        /// <summary>
        /// valor de movimento nessa tile
        /// Range [0,1]
        /// </summary>
        private float _walkCost;
        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _indice = new Indice(/*0, 0*/);
        }

        private void Start()
        {            
            _isHighLight = false;
            
            MainLoop.Instance().interfaceList.Add(this);
        }        

        private void OnMouseDown()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Tile => x " + _indice.X + "y " + _indice.Y);
                if (_isHighLight)
                {
                    MainLoop.Instance().HighLightUnitRange();
                    //move o player para o tile pretendido.
                    MainLoop.Instance().MoveUnit(this.transform.position, _indice.X, _indice.Y);
                    //gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                    return;
                }
                MainLoop.Instance().Deselect();
            }
        }

        public void SetTileProperties(TileType tileType)
        {
            switch (tileType)
            {
                case TileType.Normal:
                    _walkCost = 1f;
                    gameObject.GetComponent<SpriteRenderer>().sprite = TileSprite[0];
                    break;
                case TileType.Forest:
                    _walkCost = 0.5f;
                    gameObject.GetComponent<SpriteRenderer>().sprite = TileSprite[1];
                    break;
                case TileType.River:
                    _walkCost = 0f;
                    gameObject.GetComponent<SpriteRenderer>().sprite = TileSprite[2];
                    break;
                case TileType.Road:
                    _walkCost = 2f;
                    gameObject.GetComponent<SpriteRenderer>().sprite = TileSprite[3];
                    break;
                default:
                    _walkCost = 1f;
                    break;
            }
        }

        public Vector2 SendNewPos()
        {
            return transform.position;         
        }
        
        public GameObject Tile
        {
            get { return this.gameObject; }
        }
        public Vector2 Position
        {
            get { return (Vector2)this.transform.position; }
        }

        /// <summary>
        /// Função que verifica se o tile é acessivel ou não para a unidade
        /// </summary>
        /// <param name="unitRange"></param>
        /// <returns></returns>
        public bool IsAccessible(float unitRange)
        {
            float level = unitRange / _walkCost;

            if (level > 0)
            {
                return true;
            }
            else return false;
        }

        public void HighLight()
        {
            if (!_isHighLight)
            {
                this.gameObject.GetComponent<SpriteRenderer>().color = Color.black;
                _isHighLight = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                _isHighLight = false;
            }
        }

        public void Deselect()
        {
            
        }

        public float WalkCost
        {
            get { return _walkCost; }
            set { _walkCost = value; }
        }
        public Unit CurrentUnit
        {
            get { return _currentUnit; }
            set { _currentUnit = value; }
        }

        public Indice Indice
        {
            get { return _indice; }            
        }
    }
}
