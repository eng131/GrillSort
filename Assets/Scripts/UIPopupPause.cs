using UnityEngine;

public class UIPopupPause : MonoBehaviour
{
    [SerializeField] private GameObject panelPause;

    private void Start()
    {
        panelPause.SetActive(false);
    }

    // ====================
    // OPEN / CLOSE
    // ====================
    public void Open()
    {
        panelPause.SetActive(true);
        GameStateManager.Instance.SetState(GameState.Paused);
    }

    public void Close()
    {
        panelPause.SetActive(false);
        GameStateManager.Instance.SetState(GameState.Playing);
    }

    // ====================
    // BUTTON EVENTS
    // ====================
    public void OnContinue()
    {
        Close();
    }

    public void OnRestart()
    {
        GameStateManager.Instance.SetState(GameState.Playing);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }

    public void OnHome()
    {
        // Ví d?
        //UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}
