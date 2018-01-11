using Assets.Scripts.Player;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.AI;

namespace Assets.Scripts.Map
{
    public class Matrix
    {
        private int _width;
        private int _height;

        public HexagonTile[,] _matrix;

        private List<Unit> units;
        public List<Unit> Units { get { return units; } set { units = value; } }

        public Matrix(int _x, int _y)
        {
            _width = _x;
            _height = _y;
            _matrix = new HexagonTile[_width, _height];

            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    int rand = Random.Range(0, 6);
                    if (j % 2 == 0)
                    {
                        _matrix[i, j] = GameObject.Instantiate((GameObject)Resources.Load("HexagonSprite"), new Vector2(1.12f * i - 9, 0.95f * j - 4), Quaternion.identity).GetComponent<HexagonTile>();
                        _matrix[i, j].Indice.SetIndice(i, j);
                    }
                    else
                    {
                        _matrix[i, j] = GameObject.Instantiate((GameObject)Resources.Load("HexagonSprite"), new Vector2((1.12f * i - 8.44f), 0.95f * j - 4), Quaternion.identity).GetComponent<HexagonTile>();
                        _matrix[i, j].Indice.SetIndice(i, j);
                    }

                    if (rand <= 1)
                        _matrix[i, j].SetTileProperties(HexagonTile.TileType.Forest);
                    else if (rand == 2)
                        _matrix[i, j].SetTileProperties(HexagonTile.TileType.River);
                    else if (rand > 2 && rand <= 4)
                        _matrix[i, j].SetTileProperties(HexagonTile.TileType.Normal);
                    else if (rand < 6)
                        _matrix[i, j].SetTileProperties(HexagonTile.TileType.Road);
                }
            }
        }

        /// <summary>
        /// Retorna um determinada celula da matrix a partir do indereço do seu indice
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public HexagonTile GetMatrixCell(int x, int y)
        {
            return _matrix[x, y];
        }

        public int MatrixWidth
        {
            get { return _width; }
        }

        public int MatrixHeight
        {
            get { return _height; }
        }

        /// <summary>
        /// Retorna uma celula da matrix pela posição no espaço 
        /// </summary>
        /// <param name="tilePos"></param>
        /// <returns></returns>
        public HexagonTile FindTilebyPos(Vector2 tilePos)
        {
            for (int i = 0; i < _width; i++)
            {
                for (int j = 0; j < _height; j++)
                {
                    if ((Vector2)_matrix[i, j].transform.position == tilePos)
                        return _matrix[i, j];
                }
            }
            return null;
        }

        //public void CalculateUnitRange(Unit unit)
        //{           
        //    unit.ResetRange();

        //    if (unit.WalkRange > 0)
        //    {
        //        List<Indice> adjacents = GetAdjTile(unit.Indice.X, unit.Indice.Y);
        //        Debug.Log("tiles adjacentes" + adjacents.Count);

        //        for (int i = 0; i < adjacents.Count; i++)
        //        {
        //            float oldMovement = unit.WalkRange;
        //            float delta = 1 / _matrix[adjacents[i].X, adjacents[i].Y].WalkCost;
        //            unit.WalkRange -= delta;
        //            CalculateUnitRangeInternal(unit, adjacents[i].X, adjacents[i].Y);

        //            unit.WalkRange = oldMovement;
        //        }
        //    }
        //}

        //private void CalculateUnitRangeInternal(Unit unit, int x, int y)
        //{
        //    if (unit.WalkRange >= 0)
        //    {
        //        unit.AddTileToRange(x, y);
        //        List<Indice> adjacents = GetAdjTile(x, y);

        //        for (int i = 0; i < adjacents.Count; i++)
        //        {
        //            float oldMovement = unit.WalkRange;
        //            float delta = 1 / _matrix[adjacents[i].X, adjacents[i].Y].WalkCost;
        //            unit.WalkRange -= delta;
        //            CalculateUnitRangeInternal(unit, adjacents[i].X, adjacents[i].Y);

        //            unit.WalkRange = oldMovement;
        //        }
        //    }
        //}

        public void CalculateUnitMovementRange(Unit unit)
        {
            unit.SetMovementRange(Dijkstra.TilesWithinDistance(this, unit.CurrentTileCoords, unit.MovementType, unit.Movement));            
        }       

        //public void CalculateUnitAttackRange(Unit unit)
        //{
        //    unit.ResetAttackRange();
        //    List<Indice> adjacents = GetAdjTile(unit.Indice.X, unit.Indice.Y);
        //    List<Indice> inRangeList = new List<Indice>();
        //    List<Indice> notInRangeList = new List<Indice>();
        //    notInRangeList.Add(unit.Indice);

        //    for (int i = 0; i < adjacents.Count; i++)
        //    {
        //        CalculateUnitAttackRangeInternal(unit, adjacents[i].X, adjacents[i].Y, inRangeList, notInRangeList, 0);
        //    }

        //    inRangeList.RemoveAll(p => notInRangeList.Contains(p));
        //    for (int i = 0; i < inRangeList.Count; i++)
        //    {
        //        unit.AddTileToAttackRange(inRangeList[i].X, inRangeList[i].Y);
        //    }
        //}

        //private void CalculateUnitAttackRangeInternal(Unit unit, int x, int y, List<Indice> inRangeList, List<Indice> notInRangeList, int currentDistance)
        //{
        //    currentDistance++;
        //    List<Indice> adjacents = GetAdjTile(x, y);

        //    switch (unit.UnitClass)
        //    {
        //        case UnitClass.Figher:
        //            if (currentDistance > 1) return;

        //            inRangeList.Add(new Indice(x, y));

        //            for (int i = 0; i < adjacents.Count; i++)
        //            {
        //                CalculateUnitAttackRangeInternal(unit, adjacents[i].X, adjacents[i].Y, inRangeList, notInRangeList, currentDistance);
        //            }
        //            return;
        //        case UnitClass.Archer:
        //            if (currentDistance > 2) return;

        //            if (currentDistance < 2)
        //            {
        //                notInRangeList.Add(new Indice(x, y));
        //            }
        //            else
        //            {
        //                inRangeList.Add(new Indice(x, y));
        //            }

        //            for (int i = 0; i < adjacents.Count; i++)
        //            {
        //                CalculateUnitAttackRangeInternal(unit, adjacents[i].X, adjacents[i].Y, inRangeList, notInRangeList, currentDistance);
        //            }
        //            return;
        //        default:
        //            return;
        //    }
        //}

        public void CalculateUnitAttackRange(Unit unit)
        {
            unit.SetAttackRange(AttackRangeOfUnitAtPosition(unit, unit.CurrentTileCoords));
        }

        //    public List<Unit> UnitsInRange(Unit unit)
        //    {
        //        Unit modifiedUnit = unit;

        //        List<Indice> attackableTiles = new List<Indice>();
        //        int length = unit.InRangeCoordsList.Count;

        //        for (int i = 0; i < length; i++)
        //        {
        //            modifiedUnit.Indice.SetIndice(modifiedUnit.InRangeCoordsList[i].X, modifiedUnit.InRangeCoordsList[i].Y);
        //            CalculateUnitAttackRange(modifiedUnit);
        //            attackableTiles.AddRange(modifiedUnit.EnemyInRange);
        //        }

        //        length = attackableTiles.Count;

        //        List<Unit> unitsInRange = new List<Unit>();

        //        for (int i = 0; i < length; i++)
        //        {
        //            if (_matrix[attackableTiles[i].X, attackableTiles[i].Y].CurrentUnit != null)
        //            {
        //                unitsInRange.Add(_matrix[attackableTiles[i].X, attackableTiles[i].Y].CurrentUnit);
        //            }
        //        }

        //        return unitsInRange;
        //    }  

        public List<Unit> UnitsInRange(Unit unit)
        {
            List<Unit> unitsInRange = new List<Unit>();
            List<Indice> tilesInRange;

            for (int i = 0; i < units.Count; i++)
            {
                tilesInRange = AttackRangeOfUnitAtPosition(unit, units[i].CurrentTileCoords);

                foreach (var t in tilesInRange)
                {
                    bool result = Dijkstra.IsWithinDistance(this, unit.CurrentTileCoords, t, unit.MovementType, unit.Movement);

                    if (result)
                    {
                        unitsInRange.Add(units[i]);
                        break;
                    }
                }
            }

            return unitsInRange;
        }

        public List<Unit> UnitsInTiles(List<Indice> tiles)
        {
            List<Unit> unitsInTiles = new List<Unit>();

            for (int i = 0; i < units.Count; i++)
            {
                if (tiles.Contains(units[i].CurrentTileCoords))
                {
                    unitsInTiles.Add(units[i]);
                }
            }

            return unitsInTiles;
        }

        private List<Indice> AttackRangeOfUnitAtPosition(Unit unit, Indice position)
        {
            List<Indice> tiles;

            switch (unit.UnitClass)
            {
                case UnitClass.Figher:
                    tiles = Dijkstra.TilesWithinDistanceRange(this, position, MovementType.Attack, 1, 1);
                    break;
                case UnitClass.Archer:
                    tiles = Dijkstra.TilesWithinDistanceRange(this, position, MovementType.Attack, 2, 3);
                    break;
                default:
                    tiles = new List<Indice>();
                    break;
            }

            return tiles;
        }
        public List<Indice> TilesFromWhereUnitCanAttack(Unit unit, Indice target)
        {
            List<Indice> range = AttackRangeOfUnitAtPosition(unit, target);

            for (int i = 0; i < range.Count; i++)
            {
                if (!Dijkstra.IsWithinDistance(this, unit.CurrentTileCoords, range[i], unit.MovementType, unit.Movement) || !CanMoveTo(unit.CurrentTileCoords, range[i]))
                {
                    range.RemoveAt(i);
                    i--;
                }
            }

            return range;
        }

        public List<Indice> GetAdjTile(int x, int y)
        {
            //List<Indice> adjacents;
            List<Indice> adjacents = new List<Indice>(6);
            Indice p;

            if (y % 2 == 0)
            {
                //adjacents = new List<Indice>()
                //{
                //    new Indice(x - 1, y),
                //    new Indice(x - 1, y - 1),
                //    new Indice(x, y - 1),
                //    new Indice(x + 1, y),
                //    new Indice(x, y + 1),
                //    new Indice(x - 1, y + 1)
                //};
                p = new Indice(x - 1, y);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x - 1, y - 1);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x, y - 1);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x + 1, y);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x, y + 1);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x - 1, y + 1);
                if (IsInBoard(p))
                    adjacents.Add(p);
            }
            else
            {
                //adjacents = new List<Indice>()
                //{
                //    new Indice(x - 1, y),
                //    new Indice(x, y - 1),
                //    new Indice(x + 1, y - 1),
                //    new Indice(x + 1, y),
                //    new Indice(x + 1, y + 1),
                //    new Indice(x, y + 1)
                //};
                p = new Indice(x - 1, y);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x, y - 1);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x + 1, y - 1);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x + 1, y);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x + 1, y + 1);
                if (IsInBoard(p))
                    adjacents.Add(p);

                p = new Indice(x, y + 1);
                if (IsInBoard(p))
                    adjacents.Add(p);
            }

            //adjacents.RemoveAll(p => p.X < 0 || p.X > _width - 1 || p.Y < 0 || p.Y > _height - 1);

            return adjacents;
        }

        public bool IsInBoard(Indice p)
        {
            return p.X >= 0 && p.X <= _width - 1 && p.Y >= 0 && p.Y <= _height - 1;
        }

        public float GetWalkSpeed(Indice p, MovementType movementType)
        {
            switch (movementType)
            {
                case MovementType.Attack:
                    return 1;
                case MovementType.IgnoreAll:
                    return 1;
                case MovementType.Foot:
                    return _matrix[p.X, p.Y].WalkCost;
                default:
                    return 1;
            }
        }

        public void MoveUnit(Unit unit, Indice position)
        {
            if (IsInBoard(position))
            {
                unit.MoveTo(position);
            }
        }

        public bool CanMoveTo(Indice origin, Indice position)
        {
            if (position == origin)
            {
                return true;
            }
            
            for (int i = 0; i < units.Count; i++)
            {
                if (units[i].CurrentTileCoords == position)
                {
                    return false;
                }
            }
            return true;
        }

        public void RemoveUnit(Unit unit)
        {
            units.Remove(unit);
        }

        public static Vector2 GetVectorFromIndice(Indice indice)
        {
            if (indice.Y % 2 == 0) return new Vector2(1.12f * indice.X - 9, 0.95f * indice.Y - 4);
            else return new Vector2(1.12f * indice.X - 8.44f, 0.95f * indice.Y - 4);
        }
    }
}
