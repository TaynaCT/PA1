﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSparks.Api.Messages;
using GameSparks.Api.Responses;
using GameSparks.Core;
using UnityEngine.UI;
using GameSparks.Api.Requests;
using UnityEngine.SceneManagement;
using Assets.Scripts.SupportClasses;
using Assets.Scripts.Server;
using System;

namespace Assets.Scripts.Managers
{
    public class LobbyManager : MonoBehaviour
    {
        public InputField UsernameInput;
        public InputField PasswordInput;
        public GameObject LoginButton;
        public GameObject StartGameButton;
        public GameObject RegisterButton;

        RTSessionInfo _rtSessionInfo;
        private void Start()
        {
            GS.GameSparksAvailable += (isAvailable) =>
            {
                if (isAvailable)
                {
                    Debug.Log("GameSparks Connected.");
                }
                else
                {
                    Debug.Log("GameSparks Disconnected.");
                }
            };

            StartGameButton.gameObject.SetActive(false);
            LoginButton.GetComponent<Button>().onClick.AddListener(Login);
            RegisterButton.GetComponent<Button>().onClick.AddListener(Register);
            StartGameButton.GetComponent<Button>().onClick.AddListener(StartGame);
            MatchNotFoundMessage.Listener += OnMatchNotFound;
            MatchFoundMessage.Listener += OnMatchFound;
            ChallengeStartedMessage.Listener += OnChallengeStarted;
        }

        private void OnMatchFound(MatchFoundMessage obj)
        {
            Debug.Log("OnMatchFound");
            _rtSessionInfo = new RTSessionInfo(obj);
            GameSparksManager.Instance().StartNewRtSession(_rtSessionInfo);
            //SceneManager.LoadScene("GamePlay");
        }

        private void OnDestroy()
        {
            ChallengeStartedMessage.Listener -= OnChallengeStarted;
        }

        private void Login()
        {
            Debug.Log("OnLogin!!!!!");
            //BlockInput();

            Debug.Log(UsernameInput.text);
 
            Debug.Log(PasswordInput.text);

            Debug.Log(GameSparksManager.Instance());

            GameSparksManager.Instance().AuthenticateUser(
                UsernameInput.text,
                PasswordInput.text,
                OnRegistrationSucess,
                OnAuthentication);

            Debug.Log("OnRegistSuccess!!!!!");

            LoginButton.gameObject.SetActive(false);
            RegisterButton.gameObject.SetActive(false);
            UsernameInput.gameObject.SetActive(false);
            PasswordInput.gameObject.SetActive(false);
            StartGameButton.gameObject.SetActive(true);
            UnblockInput();
        }


        /// <summary>
        /// Inicia a cena do jogo
        /// </summary>
        private void StartGame()
        {           
            Debug.Log("StartGame");           
            GameSparksManager.Instance().FindPlayers();
            
            
            
        }

        private void Register()
        {
            BlockInput();
            
            RegistrationRequest request = new RegistrationRequest();
            request.SetUserName(UsernameInput.text);
            request.SetDisplayName(UsernameInput.text);
            request.SetPassword(PasswordInput.text);
            request.Send(OnRegistrationSucess, OnRegistrationError);
            Debug.Log("OnRegistSuccess!!!!!");
        }

        private void OnLoginSuccess(AuthenticationResponse response)
        {
            Debug.Log("OnLoginSuccess!!!!!");
            LoginButton.gameObject.SetActive(false);
            RegisterButton.gameObject.SetActive(false);
            UsernameInput.gameObject.SetActive(false);
            PasswordInput.gameObject.SetActive(false);
            StartGameButton.gameObject.SetActive(true);
        }
        private void OnAuthentication(AuthenticationResponse response)
        {
            Debug.Log("User ID: " + response.UserId);
            Debug.Log("User Authenticated.");
        }
        private void OnLoginError(AuthenticationResponse response)
        {
            UnblockInput();
            Debug.Log(response.Errors.JSON.ToString());
        }

        private void OnRegistrationSucess(RegistrationResponse response)
        {
            Debug.Log("OnRegistSuccess!!!!!");
            Debug.Log("User ID: " + response.UserId);
            Debug.Log("User Authenticated.");
            Login();
        }

        private void OnRegistrationError(RegistrationResponse response)
        {
            UnblockInput();
            Debug.Log(response.Errors.JSON.ToString());
        }

        private void OnMatchmakingSuccess(MatchmakingResponse response)
        {
            //LoadingManager.Instance.LoadNextScene();
            //SceneManager.LoadScene("GamePlay");            
        }

        private void OnMatchmakingError(MatchmakingResponse response)
        {
            UnblockInput();
            Debug.Log(response.Errors.JSON.ToString());
        }

        private void OnMatchNotFound(MatchNotFoundMessage message)
        {
            Debug.Log("No Match Found!");
            UnblockInput();
        }

        private void OnChallengeStarted(ChallengeStartedMessage message)
        {
            //LoadingManager.Instance.LoadNextScene();
            this.gameObject.SetActive(false);
        }

        //Block/unblock são funções feitas para evitar requests adicionais
        private void BlockInput()
        {
            StartGameButton.GetComponent<Button>().interactable = false;
            LoginButton.GetComponent<Button>().interactable = false;
            RegisterButton.GetComponent<Button>().interactable = false;
        }

        private void UnblockInput()
        {
            StartGameButton.GetComponent<Button>().interactable = true;
            LoginButton.GetComponent<Button>().interactable = true;
            RegisterButton.GetComponent<Button>().interactable = true;
        }
    }
}