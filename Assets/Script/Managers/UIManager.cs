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

    [Header("Destaque do Slot Selecionado")]
    [SerializeField] private Color normalSlotColor = new Color(0.2f, 0.2f, 0.2f, 0.7f);
    [SerializeField] private Color selectedColor = new Color(0.85f, 0.15f, 0.15f, 1f);
    [SerializeField] private float selectedScale = 1.12f;

    private readonly List<GameObject> spawnedSlots = new List<GameObject>(); //Mantém as referências dos slots criados na ordem da fila
    private int currentSelectedIndex = -1;

    void OnEnable()
    {
        LevelExit.OnLevelCompleted += ShowVictoryScreen;
        FollowerManager.OnAnimalAddedToQueue += HandleAnimalAdded;
        FollowerManager.OnAnimalRemovedFromQueue += HandleAnimalRemoved;
        FollowerManager.OnSelectedSlotChanged += HandleSelectedSlotChanged;
    }

    void OnDisable()
    {
        LevelExit.OnLevelCompleted -= ShowVictoryScreen;
        FollowerManager.OnAnimalAddedToQueue -= HandleAnimalAdded;
        FollowerManager.OnAnimalRemovedFromQueue -= HandleAnimalRemoved;
        FollowerManager.OnSelectedSlotChanged -= HandleSelectedSlotChanged;
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

    void HandleAnimalAdded(AnimalFollower animal)
    {
        if (animal == null || queueContainer == null || slotPrefab == null) return;

        //Cria o slot visual dentro do QueueHUD
        GameObject newSlot = Instantiate(slotPrefab, queueContainer);
        spawnedSlots.Add(newSlot);

        //Procura o Image
        Image targetImage = newSlot.GetComponent<Image>();

        //Slot tiver um específico para o ícone (A image pai serve para destacar a seleção do slot)
        Transform iconChild = newSlot.transform.Find("Icon");
        if (iconChild != null)
        {
            Image childImage = iconChild.GetComponent<Image>();
            if (childImage != null)
            {
                targetImage = childImage;
            }
        }

        if (targetImage != null && animal.AnimalIcon != null)
        {
            targetImage.sprite = animal.AnimalIcon;
            targetImage.enabled = true;
        }

        RefreshSelectionVisuals();
    }

    void HandleAnimalRemoved(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= spawnedSlots.Count) return;

        GameObject slotToRemove = spawnedSlots[slotIndex];
        spawnedSlots.RemoveAt(slotIndex);

        if (slotToRemove != null)
        {
            Destroy(slotToRemove);
        }

        //Volta para seleção padrão (-1 -> vira 0 se ainda houver animais na fila)
        currentSelectedIndex = -1;
        RefreshSelectionVisuals();
    }

    void HandleSelectedSlotChanged(int newSelectedIndex)
    {
        currentSelectedIndex = newSelectedIndex;
        RefreshSelectionVisuals();
    }

    void RefreshSelectionVisuals()
    {
        //Se nenhum índice específico foi escolhido (-1) e há animais na fila, o 1º (índice 0) fica destacado por padrão
        int effectiveSelectedIndex = (currentSelectedIndex >= 0 && currentSelectedIndex < spawnedSlots.Count)
            ? currentSelectedIndex
            : (spawnedSlots.Count > 0 ? 0 : -1);

        for (int i = 0; i <spawnedSlots.Count; i++)
        {
            GameObject slot = spawnedSlots[i];
            if (slot == null) continue;

            bool isSelected = (i == effectiveSelectedIndex);

            //Muda a cor da imagem de fundo/borda do slot
            Image slotBackground = slot.GetComponent<Image>();
            if (slotBackground != null)
            {
                slotBackground.color = isSelected ? selectedColor : normalSlotColor;
            }

            //Aumenta um pouquinho a escala do slot selecionado para dar destaque
            float targetScale = isSelected ? selectedScale : 1f;
            slot.transform.localScale = new Vector3(targetScale, targetScale, 1f);
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
    
    public void NextLevel()
    {
        Time.timeScale = 1f;
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        currentSceneIndex++;
        SceneManager.LoadScene(currentSceneIndex);
    }
    
    public void Menu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
}
