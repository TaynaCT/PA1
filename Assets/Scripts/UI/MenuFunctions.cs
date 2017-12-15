using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;

namespace Assets.Scripts.Ui
{
    public class MenuFunctions : MonoBehaviour
    {

        public GameObject InfoPainel;

        private GameObject _infoPanel;

        private void Start()
        {
            // _infoPanel = Instantiate(_infoPanel);
            //_infoPanel.SetActive(false);
        }

        /// <summary>
        /// Método que recebe os controles de movimento do player
        /// </summary>
        public void Move()
        {
            Debug.Log("move BITCH!!!!");
        }

        /// <summary>
        /// Método que recebe os controles de ataque
        /// </summary>
        public void Attack()
        {
            Debug.Log("ATTACK!!!!");
        }

        /// <summary>
        /// Método q mostra as informações do player
        /// </summary>
        public void Info()
        {
            Debug.Log("info!!!!");
            _infoPanel.SetActive(true);
        }
    }
}