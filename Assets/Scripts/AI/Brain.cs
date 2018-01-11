using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.Interface;
using Assets.Scripts.Managers;
using Assets.Scripts.Map;
using Assets.Scripts.Player;
using Assets.Scripts.SupportClasses;
using System.Linq;
using System.Threading;

namespace Assets.Scripts.AI
{
    public static class Brain
    {
        private static bool executeWithDelays = false;
        private static int delay = 500;

        public static bool ExecuteWithDelays { get { return executeWithDelays; } set { executeWithDelays = value; } }
        public static int Delay { get { return delay; } set { delay = value; } }

        public static void ProcessTurn(Matrix board, Faction faction)
        {
            // Get all the units belonging to the faction the AI is controlling.
            //List<UnitExtra> unitList = board.Units.Where(u => u.Faction == faction).Select(u => UnitExtra.FromUnit(u)).ToList();
            Dictionary<Unit, UnitExtra> unitList = board.Units.Where(u => u.Faction == faction && !u.FinishedTurn).ToDictionary(x => x, x => UnitExtra.FromUnit(x));

            Debug.Log("start");
            while (unitList.Count > 0)
            {
                Debug.Log("units left: " + unitList.Count);
                var current = unitList.First();

                foreach (var item in unitList)
                {
                    if (item.Value.Priority > current.Value.Priority)
                    {
                        current = item;
                    }
                }

                Debug.Log("priority calculated");

                Dictionary<Unit, InteractionSimulationResults> unitsInRange;

                Debug.Log("behaviour: " + current.Key.Behaviour.ToString());

                // Switch between unit behaviours.
                switch (current.Key.Behaviour)
                {
                    case UnitBehaviour.AttackerAgressive:
                    case UnitBehaviour.AttackerPassive:
                        {
                            board.CalculateUnitMovementRange(current.Key);
                            unitsInRange = board.UnitsInRange(current.Key).ToDictionary(k => k, v => new InteractionSimulationResults());

                            {
                                List<Unit> unitsToRemove = new List<Unit>();

                                foreach (var item in unitsInRange)
                                {
                                    if (!IsInteractable(current.Key, item.Key))
                                    {
                                        unitsToRemove.Add(item.Key);
                                    }
                                }

                                foreach (var item in unitsToRemove)
                                {
                                    unitsInRange.Remove(item);
                                }
                            }

                            // If there are units in range...
                            if (unitsInRange.Count > 0)
                            {
                                foreach (var key in unitsInRange.Keys.ToList())
                                {
                                    unitsInRange[key] = Unit.SimulateInteraction(current.Key, key);
                                }

                                Unit unitToAttack = unitsInRange.Aggregate((a, b) => BattleEvaluation(a.Value) < BattleEvaluation(b.Value) ? b : a).Key;

                                // To choose from where to attack, we verify which tile from where we can attack is further from the unit we want
                                // to attack, which is the longest shortest distance.
                                Indice target = board.TilesFromWhereUnitCanAttack(current.Key, unitToAttack.CurrentTileCoords).Aggregate((a, b) =>
                                    Dijkstra.GetShortestPathLength(board, unitToAttack.CurrentTileCoords, a, unitToAttack.MovementType) <
                                    Dijkstra.GetShortestPathLength(board, unitToAttack.CurrentTileCoords, b, unitToAttack.MovementType) ? b : a);

                                // Path to target.
                                List<Indice> path = Dijkstra.GetShortestPath(board, current.Key.CurrentTileCoords, target, current.Key.MovementType);

                                /* -------------------------------

                                Call to movement function goes here.
                                Replace with proper function.
                                
                                ------------------------------- */

                                board.MoveUnit(current.Key, target); // movimento

                                /* -------------------------------

                                Call to attack function goes here.
                                Attack variable is 'unitToAttack'

                                ------------------------------- */

                                Unit.Interact(current.Key, unitToAttack); //ataque

                                if (current.Key.UnityHP <= 0)
                                {
                                    board.RemoveUnit(current.Key);
                                }

                                if (unitToAttack.UnityHP <= 0)
                                {
                                    board.RemoveUnit(unitToAttack);
                                }

                                unitList.Remove(current.Key);
                            }
                            // If there are no units in range...
                            else
                            {
                                // And their behavior is passive, process waiting units, increasing cooldown.
                                // This state may change as other units move around, leading to a situation where they can attack.
                                // If they wait for too long, simply leave them in the same place.
                                if (current.Key.Behaviour == UnitBehaviour.AttackerPassive)
                                {
                                    if (current.Value.WaitTurns > 15)
                                    {
                                        unitList.Remove(current.Key);
                                    }
                                    else
                                    {
                                        current.Value.WaitTurns++;
                                    }
                                }
                                // And they if they are agressive, check if they can move.
                                else
                                {
                                    // If they can move, go towards the most favorable unit.
                                    if (current.Key.InMovementRangeCoords.Count > 0)
                                    {
                                        unitsInRange = board.Units.ToDictionary(k => k, v => new InteractionSimulationResults());

                                        {
                                            List<Unit> unitsToRemove = new List<Unit>();

                                            foreach (var item in unitsInRange)
                                            {
                                                if (!IsInteractable(current.Key, item.Key))
                                                {
                                                    unitsToRemove.Add(item.Key);
                                                }
                                            }

                                            foreach (var item in unitsToRemove)
                                            {
                                                unitsInRange.Remove(item);
                                            }
                                        }

                                        // If there are any units we can interact with...
                                        if (unitsInRange.Count > 0)
                                        {
                                            Unit unitToAttack = unitsInRange.Aggregate((a, b) => BattleEvaluation(a.Value) < BattleEvaluation(b.Value) ? b : a).Key;
                                            Debug.Log("unit to attack selected");
                                            Debug.Log(board);
                                            Debug.Log(current.Key.CurrentTileCoords.ToString());
                                            Debug.Log(unitToAttack.CurrentTileCoords.ToString());
                                            Debug.Log(current.Key.MovementType);
                                            List<Indice> path = Dijkstra.GetShortestPath(board, current.Key.CurrentTileCoords, unitToAttack.CurrentTileCoords, current.Key.MovementType);
                                            Debug.Log("path selected");
                                            Indice target = Dijkstra.GetLastTileInRange(board,
                                                path,
                                                current.Key.MovementType, current.Key.Movement);
                                            Debug.Log("target selected");
                                            /* -------------------------------

                                            Call to movement function goes here.
                                            Replace with proper function.

                                            ------------------------------- */

                                            board.MoveUnit(current.Key, target);
                                        }

                                        unitList.Remove(current.Key);
                                    }
                                    // If they can't, wait until they can or, if too long passes, stay.
                                    else
                                    {
                                        if (current.Value.WaitTurns > 15)
                                        {
                                            unitList.Remove(current.Key);
                                        }
                                        else
                                        {
                                            current.Value.WaitTurns++;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case UnitBehaviour.Defender:
                        {
                            board.CalculateUnitAttackRange(current.Key);
                            Debug.Log("range calculated");
                            unitsInRange = board.UnitsInTiles(current.Key.InAttackRangeCoords).ToDictionary(k => k, v => new InteractionSimulationResults()); ;
                            Debug.Log("units calculated: " + unitsInRange.Count);
                            if (unitsInRange.Count > 0)
                            {
                                Unit unitToAttack = unitsInRange.Aggregate((a, b) => BattleEvaluation(a.Value) < BattleEvaluation(b.Value) ? b : a).Key;

                                /* -------------------------------

                                Call to attack function goes here.

                                ------------------------------- */
                            }

                            unitList.Remove(current.Key);
                            break;
                        }
                    default:
                        unitList.Remove(current.Key);
                        break;
                }

                Debug.Log("end unit processing");

                if (executeWithDelays)
                {
                    Thread.Sleep(delay);
                }
            }
            Debug.Log("end");
        }

        private static int BattleEvaluation(InteractionSimulationResults results)
        {
            switch (results.InteractionType)
            {
                case InteractionType.Battle:
                    return results.Victory * 100 + results.DamageDealt - results.DamageTaken;
                case InteractionType.Healing:
                    return 100 + results.DamageDealt;
                default:
                    return 0;
            }
        }

        public static bool IsInteractable(Unit a, Unit b)
        {
            switch (a.UnitClass)
            {
                case UnitClass.Figher:
                    return Faction.GetRelationship(a.Faction, b.Faction) == FactionRelationship.Hostile;
                case UnitClass.Archer:
                    return Faction.GetRelationship(a.Faction, b.Faction) == FactionRelationship.Hostile;
                default:
                    return false;
            }
        }
    }
}