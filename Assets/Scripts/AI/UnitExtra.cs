using Assets.Scripts.Player;
using Assets.Scripts.SupportClasses;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public class UnitExtra
    {
        private float priority;
        private List<Unit> inRange;
        private int waitTurns;

        public float Priority { get { return priority; } set { priority = value; } }
        public List<Unit> InRange { get { return inRange; } set { inRange = value; } }
        public int WaitTurns { get { return waitTurns; } set { waitTurns = value; } }

        public static UnitExtra FromUnit(Unit u)
        {
            return new UnitExtra
            {
                priority = u.WalkRange * (u.Attack + u.Defense + u.UnityHP),
                inRange = new List<Unit>(),
                waitTurns = 0
            };
        }
    }
}