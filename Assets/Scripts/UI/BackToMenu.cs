using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class BackToMenu : MonoBehaviour {

	// Use this for initialization
	public void Back()
    {        
        SceneManager.LoadScene("StartMenu");
    }
}
