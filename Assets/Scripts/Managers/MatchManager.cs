using GameSparks.Api.Messages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Assets.Scripts.SupportClasses;
using System.Linq;
using Assets.Scripts.Player;
using GameSparks.RT;
using Assets.Scripts.Map;

namespace Assets.Scripts.Managers
{
    public class MatchManager : Singleton<MatchManager>
    {
        public UnityEvent MatchStarted;
        public UnityEvent MatchTurnTaken;
        public UnityEvent MatchWon;
        public UnityEvent MatchLost;
        public Matrix MatchMap;

        private string _matchID;

        private void Start()
        {
            ChallengeStartedMessage.Listener += OnChallengeStarted;
        }

        public bool IsMatchActive { get; private set; }
        public string Faction01PlayerName { get; private set; }
        public string Faction01PlayerId { get; private set; }
        public string Faction02PlayerName { get; private set; }
        public string Faction02PlayerId { get; private set; }
        public string CurrentPlayerName { get; private set; }

        /// <summary>
        /// indentifica a facção do player atual
        /// </summary>
        public FactionName CurrentPlayerFaction
        {
            get
            {
                if (IsMatchActive)
                {
                    return CurrentPlayerName == Faction01PlayerName ? FactionName.CenturyWarriors : FactionName.NewNation;
                }
                else return FactionName.None;
            }
        }

        //public FactionNames[] PlayerUnits { get; private set; }

        private void OnChallengeStarted(ChallengeStartedMessage message)
        {
            IsMatchActive = true;
            _matchID = message.Challenge.ChallengeId;
            Faction01PlayerName = message.Challenge.Challenger.Name;
            Faction01PlayerId = message.Challenge.Challenger.Id;
            Faction02PlayerName = message.Challenge.Challenged.First().Name;
            Faction02PlayerId = message.Challenge.Challenged.First().Id;
            CurrentPlayerName = message.Challenge.NextPlayer == Faction01PlayerId ? Faction01PlayerName : Faction02PlayerName;
            //PlayerUnits = message.Challenge.ScriptData.GetIntList("PlayerUnits").Cast<PieceType>
            MatchStarted.Invoke();
        }

        private void OnChallengeTurnTaken(ChallengeTurnTakenMessage message)
        {
            CurrentPlayerName = message.Challenge.NextPlayer == Faction01PlayerId ? Faction01PlayerName : Faction02PlayerName;
            ///Fields = message.Challenge.ScriptData.GetIntList("fields").Cast<PieceType>().ToArray();
            MatchTurnTaken.Invoke();
        }

        private void OnChallengeWon(ChallengeWonMessage message)
        {
            IsMatchActive = false;
            MatchWon.Invoke();
        }

        private void OnChallengeLost(ChallengeLostMessage message)
        {
            IsMatchActive = false;
            MatchLost.Invoke();
        }       

    }
}