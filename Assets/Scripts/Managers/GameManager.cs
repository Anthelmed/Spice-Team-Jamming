using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System;
public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        title,
        overworld,
        battle,
        pause
    }

    [SerializeField] string testBattleMapName;
    [SerializeField] UIPanel startScreen;
    [SerializeField] UIPanel pauseScreen;
    [SerializeField] UIPanel loadingScreen;
    [SerializeField] Transform panelParent;
    [SerializeField] GameObject mapGraphics;
    UIPanel[] allPanels;
  
    GameState currentGameState;
    String currentBattleScene;

    public static GameManager instance;

    float pauseCoolDown;

    public Action<bool> loadingScreenVisibilityEvent = delegate { };
    public Action<bool> startScreenVisibilityEvent = delegate { };
    public Action<bool> pauseScreenVisibilityEvent = delegate { };


    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this.gameObject);
        else
        {
            instance = this;

        }

        HideAllPanels();
        startScreen.Show();
    }

    public void StartGame()
    {
        if (currentGameState == GameState.title)
        {
            HideAllPanels();
            TransitionToState(GameState.overworld);
        }
    }
    
     void TransitionToState(GameState newState)
    {
        GameState tmpInitialState = currentGameState;
        OnStateExit(tmpInitialState, newState);
        currentGameState = newState;
        OnStateEnter(newState, tmpInitialState);
    }
    public void OnStateExit(GameState state, GameState toState)
    {
        switch (state)
        {
            case GameState.title:
                break;
            case GameState.overworld:
                break;
            case GameState.battle:
                break;
            case GameState.pause:
                {
                    pauseScreen.Hide();
                }
                break;
            default:
                break;
        }
    }
    public void OnStateEnter(GameState state, GameState fromState)
    {
        switch (state)
        {
            case GameState.title:
                break;
            case GameState.overworld:
                break;
            case GameState.battle:
                break;
            case GameState.pause:
                {
                    pauseScreen.Show();
                    //switch input map
                }
                break;
            default:
                break;
        }
    }

    private void Update()
    {
        switch (currentGameState)
        {
            case GameState.title:
                {
                    //on press any key start gameplay
                }
                break;
            case GameState.overworld:
                {
                    //on click tile load scene
                }
                break;
            case GameState.battle:
                {
                    // if (pausePressed)
                  //  TransitionToState(GameState.pause);
                }
                break;
            case GameState.pause:
                {
                  //   if (pausePressed)
                  //   TransitionToState(GameState.battle);
                  //switch input map
                }
                break;
            default:
                break;
        }
    }


    public void LoadBattleMap(string sceneName)
    {
        if (currentGameState == GameState.battle) return;

        Scene scene = SceneManager.GetSceneByName(sceneName);
        print("scene index" + scene.buildIndex);
        StartCoroutine(LoadScene(sceneName));

    }

    public void ReturnToOverworld()
    {
        if (currentGameState == GameState.battle && currentBattleScene != null)
        {
            SceneManager.UnloadSceneAsync(currentBattleScene);
            TransitionToState(GameState.battle);
            mapGraphics.SetActive(true); /// do this in the map script. just the graphics should get turned off
        }
    }

    IEnumerator LoadScene(String sceneToLoad)
    {
        foreach (var panel in allPanels) panel.Hide();
        loadingScreen.Show();
         AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneToLoad, LoadSceneMode.Additive);

        while (!asyncLoad.isDone)
        {
            print("loading scene");
            yield return null;
        }
        currentBattleScene = sceneToLoad;

        yield return new WaitForSeconds(1);

        mapGraphics.SetActive(false); /// do this in the map script. just the graphics should get turned off

        TransitionToState(GameState.battle);
        loadingScreen.Hide();
        
    }


    private void OnValidate()
    {
        allPanels = panelParent.GetComponentsInChildren<UIPanel>();
    }
    [ContextMenu(" hide all panels")]
    public void HideAllPanels()
    {
        foreach (var panel in allPanels) panel.Hide();
    }

    public void TogglePause()// for testing only
    {

        switch (currentGameState)
        {

            case GameState.battle:
                {
                    TransitionToState(GameState.pause);
                }
                break;
            case GameState.pause:
                {
                    TransitionToState(GameState.battle);
                }
                break;
        }
    }
    /// <summary>
    /// //debbuggin stuff below here
    /// </summary>
    public void LoadTestScene()
    {
        LoadBattleMap(testBattleMapName);
    }





}


