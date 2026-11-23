using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Screens")]
    [SerializeField] private GameObject victoryScreen;

    [Header("HUD da Fila de Animais")]
    [SerializeField] private Transform queueContainer;
    [SerializeField] private GameObject slotPrefab;

    //Mantém as referências dos slots criados na ordem da fila
    private readonly List<GameObject> spawnedSlots = new List<GameObject>();

    void OnEnable()
    {
        LevelExit.OnLevelCompleted += ShowVictoryScreen;
        FollowerManager.OnAnimalAddedToQueue += HandleAnimalIcon;
    }

    void OnDisable()
    {
        LevelExit.OnLevelCompleted -= ShowVictoryScreen;
        FollowerManager.OnAnimalAddedToQueue -= HandleAnimalIcon;
    }

    void Start()
    {
        if (victoryScreen != null) victoryScreen.SetActive(false);
        else Debug.LogWarning("Coloque a tela de vitória no Inspector do UIManager");
    }

    void ShowVictoryScreen()
    {
        if (victoryScreen != null) victoryScreen.SetActive(true);
    }

    void HandleAnimalIcon(AnimalFollower animal)
    {
        if (animal == null || queueContainer == null || slotPrefab == null) return;

        //Cria o slot visual dentro do QueueHUD
        GameObject newSlot = Instantiate(slotPrefab, queueContainer);
        spawnedSlots.Add(newSlot);

        //Procura o Image
        Image targetImage = newSlot.GetComponent<Image>();

        //Se o slot tiver um específico para o ícone
        //Script aqui

        if (targetImage != null && animal.AnimalIcon != null)
        {
            targetImage.sprite = animal.AnimalIcon;
            targetImage.enabled = true;
        }
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
