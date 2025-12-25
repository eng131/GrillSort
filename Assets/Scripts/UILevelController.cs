using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private Image progressCircle;

    private float totalTime;
    private float remainingTime;
    private bool isRunning;

    // =========================
    // INIT TỪ LEVEL DATA
    // =========================
    public void Init(LevelDataSO levelData)
    {
        // Level number
        levelText.text = $"Level {levelData.levelIndex}";

        // Time
        totalTime = levelData.levelSeconds;
        remainingTime = levelData.levelSeconds;

        UpdateTimeUI();
        isRunning = true;
    }

    private void Update()
    {
        if (GameStateManager.Instance.CurrentState != GameState.Playing)
            return;

        if (!isRunning) return;

        remainingTime -= Time.deltaTime;
        remainingTime = Mathf.Max(remainingTime, 0);

        UpdateTimeUI();

        if (remainingTime <= 0)
        {
            isRunning = false;
            OnTimeOut();
        }
    }

    private void UpdateTimeUI()
    {
        int totalSeconds = Mathf.CeilToInt(remainingTime);

        int minutes = totalSeconds / 60;
        int seconds = totalSeconds % 60;

        timeText.text = $"{minutes:00}:{seconds:00}";

        if (progressCircle != null)
            progressCircle.fillAmount = remainingTime / totalTime;

        if (remainingTime <= 10f)
        {
            progressCircle.color = Color.red;
        }
        else
        {
            progressCircle.color = Color.green;
        }




    }


    private void OnTimeOut()
    {
        Debug.Log("⏰ Time Out!");
        
    }
}
