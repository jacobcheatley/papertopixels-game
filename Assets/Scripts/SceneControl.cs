using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private static SceneControl instance;

    private static string mainName = "Main";
    private static string lobbyName = "Lobby";

    private static bool inLevelLoad = false;

    void Start()
    {
        instance = this;
        instance.StartCoroutine(instance.Setup());
}
    
    public static void LoadGame()
    {
        instance.StartCoroutine(instance.LoadGame_());

        // Lobby should already be active scene
    }

    IEnumerator Setup()
    {
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(mainName));
    }

    IEnumerator LoadGame_()
    {
        StartCoroutine(ToLevelExclusive(mainName));
        while (inLevelLoad)
            yield return null;
        LevelLoader.Create(0); // TODO: Select correct ID
    }

    IEnumerator ToLevelExclusive(string name)
    {
        inLevelLoad = true;
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        AsyncOperation loading = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        yield return loading;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        inLevelLoad = false;
    }
}
