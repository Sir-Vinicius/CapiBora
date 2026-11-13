using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject victoryScreen;

    void OnEnable()
    {
        LevelExit.OnLevelCompleted += ShowVictoryScreen;
    }

    void OnDisable()
    {
        LevelExit.OnLevelCompleted -= ShowVictoryScreen;
    }

    void Start()
    {
        if (victoryScreen != null) victoryScreen.SetActive(false);
        else Debug.LogWarning("Coloque a tela de vitória no Inspector do UIManager");
    }

    private void ShowVictoryScreen()
    {
        if (victoryScreen != null) victoryScreen.SetActive(true);
    }

    public void RestartLevel()
    {
        //Garante que o tempo volte ao normal caso você use Time.timeScale = 0 em algum pause
        Time.timeScale = 1f;

        //Pega o índice da cena atual e recarrega ela do zero
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
