using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance { get; private set; }
    private MusicPlayer player;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            ChangeScene("Main Menu");
        }
    }

    public static void NextLevel()
    {
        
    }

    public static void ChangeScene(string name)
    {
        if(PlayerInput.Instance != null)
        PlayerInput.Instance.currentlySelecting = false;
        Instance.StartCoroutine(LoadYourAsyncScene(name, Instance.gameObject));
    }

    static IEnumerator LoadYourAsyncScene(string m_Scene, GameObject m_MyGameObject)
    {
        // Set the current Scene to be able to unload it later
        Scene currentScene = SceneManager.GetActiveScene();

        // The Application loads the Scene in the background at the same time as the current Scene.
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(m_Scene, LoadSceneMode.Additive);

        // Wait until the last operation fully loads to return anything
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        // Move the GameObject (you attach this in the Inspector) to the newly loaded Scene
        SceneManager.MoveGameObjectToScene(m_MyGameObject, SceneManager.GetSceneByName(m_Scene));
        // Unload the previous Scene
        SceneManager.UnloadSceneAsync(currentScene);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
