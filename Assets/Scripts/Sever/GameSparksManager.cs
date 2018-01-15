using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using GameSparks.Core;
using GameSparks.RT;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.Server
{
    public class GameSparksManager : MonoBehaviour
    {
        private static GameSparksManager _instance;
        public static GameSparksManager Instance()
        {
            if (_instance != null)
            {
                return _instance;
            }
            Debug.Log("GSM | GameSparksManager Not Initialized");
            return null;
        }
        

        public GameSparksRTUnity GameSparksRtUnity { get; set; }
        public RTSessionInfo RtSessionInfo { get; set; }

        public delegate void AuthenticationCallback(AuthenticationResponse authenticationResponse);
        public delegate void RegistrationCallback(RegistrationResponse registrationResponse);

        public void AuthenticateUser(
            string userName, 
            string password, 
            RegistrationCallback registrationCallback,
            AuthenticationCallback authenticationCallback)
        {
            new RegistrationRequest().
                SetUserName(userName).
                SetPassword(password).
                SetDisplayName(userName).
                Send((registrationResposnse) =>
                {
                    if (!registrationResposnse.HasErrors)
                    {
                        Debug.Log("Registration Successful");
                        registrationCallback(registrationResposnse);
                    }
                    else
                    {
                        if (registrationResposnse.NewPlayer == false)
                        {
                            new AuthenticationRequest().SetUserName(userName).SetPassword(password).Send(
                                (authenticationResponse) =>
                                {
                                    if (!authenticationResponse.HasErrors)
                                    {
                                        Debug.Log("Authentication Successful");
                                        authenticationCallback(authenticationResponse);
                                    }
                                    else
                                    {
                                        Debug.Log("Authentication Error " +
                                                  authenticationResponse.Errors.JSON);
                                    }
                                });
                        }
                        else
                        {
                            Debug.Log("Authentication Error " +
                                      registrationResposnse.Errors.JSON);
                        }
                    }
                });
        }

        public void FindPlayers()
        {
            Debug.Log("Attempting Matchmaking");
            new MatchmakingRequest().
                SetMatchShortCode("TOP_SHOOTER_MATCH")
                .SetSkill(0)
                .Send((matchmakingResponse) =>
                {
                    if (matchmakingResponse.HasErrors)
                    {
                        Debug.Log("Matchmaking Error " +
                                  matchmakingResponse.Errors.JSON);
                    }
                });
        }

        public void StartNewRtSession(RTSessionInfo rtSessionInfo)
        {
            Debug.Log("Creating New RT Session Instance");
            RtSessionInfo = rtSessionInfo;
            GameSparksRtUnity = gameObject.AddComponent<GameSparksRTUnity>();


            GSRequestData requestData = new GSRequestData()
                .AddNumber("port", (double)rtSessionInfo.PortId)
                .AddString("host", rtSessionInfo.HostUrl)
                .AddString("accessToken", rtSessionInfo.AccessToken);

            Debug.Log((double)rtSessionInfo.PortId);
            Debug.Log(rtSessionInfo.HostUrl);
            Debug.Log(rtSessionInfo.AccessToken);

            FindMatchResponse findMatchResponse = new FindMatchResponse(requestData);

            GameSparksRtUnity.Configure(findMatchResponse, 
                (peerId) => {OnPlayerConnected(peerId);},
                (peerId) => { OnPlayerDisconnected(peerId); },
                (ready) => {OnRtReady(ready);},
                (packet) => {OnPacketReceived(packet);});
            GameSparksRtUnity.Connect();
        }

        private void OnPlayerConnected(int peerId)
        {
            Debug.Log("Player Connected " + peerId);
        }

        private void OnPlayerDisconnected(int peerId)
        {
            Debug.Log("Player Disconnected " + peerId);
        }

        private void OnRtReady(bool isReady)
        {
            if (isReady)
            {
                Debug.Log("RT Sessions Connected");
                //SceneManager.LoadScene(1);
            }
        }

        private void OnPacketReceived(RTPacket rtPacket)
        {
            if (GameController.Instance() != null)
            {
                GameController.Instance().PacketReceived(rtPacket.PacketSize);
            }
            else
            {
                return;
            }

            switch (rtPacket.OpCode)
            {
                case 2:
                    GameController.Instance().UpdateOpponentUnit(rtPacket);
                    break;                
                         
            }
        }

        void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
