using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public GameObject LoadPanel;
    public Slider LoadingBar;

    public void LoadLevel(string sceneName)
    {
        StartCoroutine(LoadScene(sceneName));
    }
    
    IEnumerator LoadScene(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        LoadPanel.SetActive(true);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / .9f);
            //animação de espera enquanto a cena é carregada
            LoadingBar.value = progress;

            yield return null;
        }
    }
}
