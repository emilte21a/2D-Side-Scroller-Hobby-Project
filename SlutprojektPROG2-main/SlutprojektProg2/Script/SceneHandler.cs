public class SceneHandler
{
    public enum CurrentSceneState
    {
        Start,
        Game,
        Gameover
    }

    public CurrentSceneState currentScene;

    public SceneHandler()
    {
        currentScene = CurrentSceneState.Start;
    }

    public void ChangeScene(CurrentSceneState currentScene1)
    {
        currentScene = currentScene1;
    }
}