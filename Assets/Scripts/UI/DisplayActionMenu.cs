using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.Player;

public class DisplayActionMenu : MonoBehaviour
{
    public GameObject ActionMenuCanvas;
    //public GameObject MoveButton;
    //public GameObject AttackButton;
    //public GameObject InfoButton;

    private GameObject _menuCanvas;
    private bool _isActive;
 
    void Start()
    {
        _menuCanvas = Instantiate(ActionMenuCanvas);
        _menuCanvas.SetActive(false);
        _isActive = false;      
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {            
            Vector2 rayPos = new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y);
            RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.zero, 0f);

            if (hit.transform != null && hit.transform.CompareTag("Unit"))
            {
                if (!_isActive)//garante que só um menu é criado
                {                    
                    ShowActionMenu(rayPos);
                    Debug.Log("IT'S A HIT!!!!!");
                    
                }
            }
            //deveria fazer o menu desaparecer caso o player clicasse em algo q não fosse a unidade            
            else
            {
                if (_isActive)
                {
                   _menuCanvas.SetActive(false);
                    _isActive = false;
                }
                Debug.Log("NOT HITING !!!");
            }
        }

        if (Input.GetMouseButtonDown(0))
        {

        }
    }
    
    public void ShowActionMenu(Vector2 mousePos)
    {
        Vector2 menuPos = new Vector2(mousePos.x + ((_menuCanvas.GetComponent<RectTransform>().localScale.x + 2.5f ) / 2), mousePos.y + ((_menuCanvas.GetComponent<RectTransform>().localScale.y +2f) / 2));
        
        Debug.Log("MousePos -> " + mousePos + " menuPos ->" + menuPos);
        _menuCanvas.transform.position = menuPos;
        _menuCanvas.SetActive(true);
        _isActive = true;
    }
}
