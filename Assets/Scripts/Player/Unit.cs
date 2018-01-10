using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Map;
using UnityEngine.UI;
using Assets.Scripts.Managers;
using Assets.Scripts.Interface;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.AI;

namespace Assets.Scripts.Player
{ 
    public class Unit : MonoBehaviour, IDeselect
    {
        const int MAXHP = 50;
        const int MAXMP = 30;

        private UnitBehaviour behaviour;
        private bool finishedTurn;

        struct Atributes
        {
            private float _hP;
            private int _maxHp;
            private int _mP;
            private int _maxMp;
            private int _attack;
            private int _armor;
            private float _resistance;
            private float _hit;
            private float _evade;
            private int _speed;
            public Atributes(float hp, int extraHealth, int mp, int extraMp, int attack, int armor, float resistance, float hit, float evade, int speed)
            {
                _hP = hp;
                _maxHp = MAXHP + extraHealth;
                _mP = mp;
                _maxMp = MAXMP + extraMp;
                _attack = attack;
                _armor = armor;
                _resistance = resistance;
                _hit = hit;
                _evade = evade;
                _speed = speed;
            }

            public float GetHealthValue
            {
                get { return _hP; }
            }

            public int GetAttackValue
            {
                get { return _attack; }
            }

            public int GetArmorValue
            {
                get { return _armor; }
            }

            public float GetResistenceValue
            {
                get { return _resistance; }
            }

            public float GetHitValue
            {
                get { return _hit; }
            }

            public float GetEvadeValue
            {
                get { return _evade; }
            }

            public int GetSpeedValue
            {
                get { return _speed; }
            }

            public float CurrentHealth()
            {
                return _hP / _maxHp;
            }

            public void ApplyDamage(float amount)
            {
                if (_hP > 0)
                {
                    _hP -= amount;

                    if (_hP < 0)
                    {
                        _hP = 0;
                    }
                }
                //(Ataque−defesa )±(Modificadores de tecnicas e terrenos)= dano no oponente                
            }

            public float CurrentMp()
            {
                return _mP / _maxMp;
            }

            public void UseMp()
            {
                _mP -= _attack;
                //statsManager.SetHpBar(CurrentMp());
            }
        }
        
        StatsManager statsManager;

        private SpriteRenderer _spriteRender;
        private GameObject _moveButton;
        private GameObject _actionMenu;
        private UnitClass _class;
        private UnitOwner _owner;
        private bool _isActionMenuActive;        
        private bool _isSelected;
        private Vector2 _lastPos;
        /// <summary>
        /// Range do movimento da unidade atual
        /// </summary>
        private float _walkRange;
        private List<Indice> _inRangeCoords;
        private List<Indice> _enemyInRange;
        
        /// <summary>
        /// Array de itens disponiveis para a unidade
        /// </summary>
        private GameObject[] _itens;
        private Atributes _atributes;
        private Indice _indice;

        public void Awake()
        {
            _indice = new Indice();
        }
        void Start()
        {
            _actionMenu.SetActive(false);
            _isActionMenuActive = false;
            this.gameObject.tag = "Unit";
            _isSelected = false;
            _lastPos = transform.position;
            _inRangeCoords = new List<Indice>();
            _itens = new GameObject[3];
            _spriteRender = GetComponent<SpriteRenderer>();
            statsManager = MainLoop.Instance().StatsManager;

            _walkRange = 2;
            _atributes = new Atributes(50f, 0, 30, 0, 15, 10, 0.3f, 0.5f, 0.4f, 5);
                       
            _class = UnitClass.Archer;
            //_owner = UnitOwner.Player;

            MainLoop.Instance().interfaceList.Add(this);
        }
        
        private void Update()
        {
            if (_lastPos != (Vector2)transform.position)
            {
                DeactivateActionMenu();
                _lastPos = transform.position;
            }            
        }

        public void OnMouseOver()
        {
            //ativa menu de ação
            if (Input.GetMouseButtonDown(1) && _isSelected)
            {
                if (!_isActionMenuActive)
                {
                    ShowActionMenu(this.transform.position);
                }
            }
        }

        public void OnMouseDown()
        {            
            SelectUnit();
            DeactivateActionMenu();
            SetStatsValues();            
        }

        /// <summary>
        /// Coloca a unidade no em uma posição da matriz
        /// </summary>
        public void SetUnitIndice(int x, int y)
        {
            _indice.SetIndice(x, y);
        }

        private void SetStatsValues()
        {
            statsManager.SetHpBar(_atributes.CurrentHealth());
            statsManager.SetMpBar(_atributes.CurrentMp());
            statsManager.SetArmorBar(_atributes.GetArmorValue);
            statsManager.SetAttackText(_atributes.GetAttackValue);
            statsManager.SetEvadeText(_atributes.GetEvadeValue);
            statsManager.SetHitText(_atributes.GetHitValue);
            statsManager.SetResistanceText(_atributes.GetResistenceValue);
            statsManager.SetSpeedText(_atributes.GetSpeedValue);

            statsManager.IsActive(_isSelected);
        }

        public void SelectUnit()
        {
            if (!_isSelected)
            {
                _spriteRender.color = Color.gray;
                _isSelected = true;
            }
            else
            {
                _spriteRender.color = Color.white;
                _isSelected = false;
            }
        }

        public void DeactivateActionMenu()
        {
            if (_isActionMenuActive)
            {
                _actionMenu.SetActive(false);
                _isActionMenuActive = false;
            }
        }

        public void ShowActionMenu(Vector2 mousePos)
        {
            Vector2 menuPos = new Vector2(mousePos.x + ((_actionMenu.GetComponent<RectTransform>().localScale.x + 2.5f) / 2), mousePos.y + ((_actionMenu.GetComponent<RectTransform>().localScale.y + 2f) / 2));

            _actionMenu.transform.position = menuPos;
            _actionMenu.SetActive(true);
            _isActionMenuActive = true;
        }

        public void SetPosition(Vector2 newPos, int x, int y)
        {
            transform.position = newPos;
            _indice.SetIndice(x, y);           
        }
        public void AddTileToRange(int x, int y)
        {
            Indice indice = new Indice(x, y);
            if (!_inRangeCoords.Contains(indice))
            {
                _inRangeCoords.Add(indice);
            }
        }

        public void ResetRange()
        {
            _inRangeCoords.Clear();
        }

        public void ResetAttackRange()
        {
            _enemyInRange.Clear();
        }

        public void SetActionMenu(GameObject actionMenu)
        {
            _actionMenu = GameObject.Instantiate(actionMenu);
        }

        public void SetMoveButton(GameObject button)
        {
            _moveButton = button; // GameObject.Instantiate(button);
        }

        public void Deselect()
        {
            _isSelected = true;
            SelectUnit();
            statsManager.IsActive(_isSelected);            
        }

        public void AddTileToAttackRange(int x, int y)
        {
            Indice indice = new Indice(x, y);

            if (!_enemyInRange.Contains(indice))
            {
                _enemyInRange.Add(indice);
            }
        }

        public Vector2 Position
        {
            get { return this.transform.position; }            
        }
        
        public float WalkRange
        {
            get { return _walkRange; }
            set { _walkRange = value; }
        }
        public List<Indice> InRangeCoordsList
        {
            get { return _inRangeCoords; }
        }

        public List<Indice> EnemyInRange
        {
            get { return _enemyInRange; }
        }

        public Indice Indice
        {
            get { return _indice; }
        }
        public UnitClass UnitClass
        {
            get { return _class; }
        }

        private MovementType movementType;
        private Faction faction;
        private Indice currentTileCoords;
        private Indice previousTileCoords;
        private List<Indice> inMovementRangeCoords;
        private List<Indice> inAttackRangeCoords;

        public Indice CurrentTileCoords { get { return currentTileCoords; } }
        public List<Indice> InMovementRangeCoords { get { return inMovementRangeCoords; } }
        public List<Indice> InAttackRangeCoords { get { return inAttackRangeCoords; } }
        public UnitBehaviour Behaviour { get { return behaviour; } set { behaviour = value; } }
        public bool FinishedTurn { get { return finishedTurn; } set { finishedTurn = value; } }
        public MovementType MovementType { get { return movementType; } set { movementType = value; } }
        public Faction Faction { get { return faction; } set { faction = value; } }

        public void AddTileToMovementRange(int x, int y)
        {
            Indice p = new Indice(x, y);

            if (!inMovementRangeCoords.Contains(p) && p != currentTileCoords)
            {
                inMovementRangeCoords.Add(p);
            }
        }

        public void SetMovementRange(List<Indice> range)
        {
            inMovementRangeCoords = range;
        }

        public void SetAttackRange(List<Indice> range)
        {
            inAttackRangeCoords = range;
        }

        public void ResetMovementRange()
        {
            inMovementRangeCoords.Clear();
        }

        ///----- new stuff
        public void AddTileToAttackRange(int x, int y)
        {
            Indice p = new Indice(x, y);

            if (!inAttackRangeCoords.Contains(p))
            {
                inAttackRangeCoords.Add(p);
            }
        }

        public void ResetAttackRange()
        {
            inAttackRangeCoords.Clear();
        }

        public void ReturnToPreviousCoords()
        {
            currentTileCoords = previousTileCoords;
        }

        public void MoveTo(Indice position)
        {
            previousTileCoords = currentTileCoords;
            currentTileCoords = position;
        }

        public static InteractionSimulationResults SimulateInteraction(Unit initiator, Unit defender)
        {
            InteractionType interactionType = InteractionType.Battle;
            int damageDealt = 0;
            int damageTaken = 0;
            int victory = 0;

            switch (Faction.GetRelationship(initiator.Faction, defender.Faction))
            {
                case FactionRelationship.Friendly:
                    interactionType = InteractionType.Healing;

                    damageDealt = 10;
                    break;
                case FactionRelationship.Neutral:
                    break;
                case FactionRelationship.Hostile:
                    interactionType = InteractionType.Battle;

                    damageDealt = initiator.attack - defender.defense;

                    if (defender.hp - damageDealt <= 0)
                    {
                        damageTaken = 0;
                        victory = 1;
                    }
                    else
                    {
                        damageTaken = defender.attack - initiator.defense;

                        if (initiator.hp - damageTaken <= 0)
                        {
                            victory = -1;
                        }
                        else
                        {
                            victory = 0;
                        }
                    }
                    break;
                default:
                    break;
            }

            return new InteractionSimulationResults(interactionType, damageDealt, damageTaken, victory);
        }

        public static void Interact(Unit initiator, Unit defender)
        {
            switch (Faction.GetRelationship(initiator.Faction, defender.Faction))
            {
                case FactionRelationship.Friendly:
                    break;
                case FactionRelationship.Neutral:
                    break;
                case FactionRelationship.Hostile:

                    defender.hp -= initiator.attack - defender.defense;

                    if (defender.hp <= 0) return;

                    initiator.hp -= defender.attack - initiator.defense;
                    break;
                default:
                    break;
            }
        }

    }
}
