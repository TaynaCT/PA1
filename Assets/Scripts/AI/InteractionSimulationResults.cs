using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.AI
{
    public struct InteractionSimulationResults
    {
        public InteractionType InteractionType;
        public int DamageDealt;
        public int DamageTaken;
        public int Victory;

        public InteractionSimulationResults(InteractionType interactionType, int damageDealt, int damageTaken, int victory)
        {
            InteractionType = interactionType;
            DamageDealt = damageDealt;
            DamageTaken = damageTaken;
            Victory = victory;
        }
    }
}
