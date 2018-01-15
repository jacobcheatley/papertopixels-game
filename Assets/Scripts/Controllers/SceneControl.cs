using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneControl : MonoBehaviour
{
    private static SceneControl instance;

    private static string mainName = "Main";
    private static string lobbyName = "Lobby";
    private static string levelName = "LevelSelect";
    private static string endName = "EndGame";

    private static bool inLoad = false;

    void Start()
    {
        instance = this;
        instance.StartCoroutine(instance.Setup());
}
    
    public static void ToLevelSelect()
    {
        // Lobby should already be active scene
        instance.StartCoroutine(instance.LevelSelect_());
    }

    public static void ToGame(int id)
    {
        // Level Select should already be active scene
        instance.StartCoroutine(instance.Game_(id));
    }

    public static void ToEndGame()
    {
        instance.StartCoroutine(instance.EndGame_());
    }

    IEnumerator Setup()
    {
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(mainName));
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(levelName));
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName(endName));
    }

    IEnumerator LevelSelect_()
    {
        StartCoroutine(ToLevelExclusive(levelName));
        while (inLoad)
            yield return null;
    }

    IEnumerator Game_(int id)
    {
        StartCoroutine(ToLevelExclusive(mainName));
        while (inLoad)
            yield return null;
        LevelLoader.Create(id);
        InGameUI.Init();
    }

    IEnumerator EndGame_()
    {
        StartCoroutine(ToLevelExclusive(endName));
        while (inLoad)
            yield return null;
        EndGameUI.Init();
    }

    IEnumerator ToLevelExclusive(string name)
    {
        inLoad = true;
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
        AsyncOperation loading = SceneManager.LoadSceneAsync(name, LoadSceneMode.Additive);
        yield return loading;
        SceneManager.SetActiveScene(SceneManager.GetSceneByName(name));
        inLoad = false;
    }
}
