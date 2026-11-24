using System;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    [Header("Referências do Player")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private PositionTracker positionTracker;
    [SerializeField] private AbilityInput abilityInput;


    [Header("Configurações da Fila")]
    [SerializeField] private List<Transform> activeQueue = new List<Transform>(); //Lista para armazenar a ordem exata dos animais na fila
    [SerializeField] private int historySpacing = 15; //Quantidade de pontos de histórico de distância

    public IReadOnlyList<Transform> ActiveQueue => activeQueue; //Apenas leitura para outros scripts
    private int selectedIndex = -1;

    //Events
    public static event Action<AnimalFollower> OnAnimalAddedToQueue;
    public static event Action<int> OnAnimalRemovedFromQueue;
    public static event Action<int> OnSelectedSlotChanged;

    void Start()
    {
        if (positionTracker == null) Debug.LogWarning("Não achou PositionTracker");
        if (abilityInput == null) Debug.LogWarning("Não achou AbilityInput");
    }

    void OnEnable()
    {
        if (abilityInput != null)
        {
            abilityInput.OnAbilityInputTriggered += TryUseSelectedAnimalAbility;
            abilityInput.OnSelectSlotTriggered += HandleAnimalSelection;
        }
    }

    private void OnDisable()
    {
        if (abilityInput != null)
        {
            abilityInput.OnAbilityInputTriggered -= TryUseSelectedAnimalAbility;
            abilityInput.OnSelectSlotTriggered -= HandleAnimalSelection;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Animal"))
        {
            Transform animalTransform = other.transform;

            //Garante que o animal já não está na lista
            if (!activeQueue.Contains(animalTransform))
            {
                //Chama o método que coloca na fila
                AddAnimalToQueue(animalTransform);
            }
        }
    }

    //Adiciona e organiza o animal na fila indiana
    public void AddAnimalToQueue(Transform animalTransform)
    {
        if (animalTransform == null || positionTracker == null) return;
        
        //Se for Tatu reseta a habilidade e reabilita o FollowerManager
        TatuBolaAbility tatu = animalTransform.GetComponent<TatuBolaAbility>();
        if (tatu != null) tatu.OnRejoinedQueue();

        AnimalFollower follower = animalTransform.GetComponent<AnimalFollower>();
        if (follower == null) return;

        //Coloca no final da lista
        activeQueue.Add(animalTransform); 

        //Para garantir que está habilitado
        follower.enabled = true;

        Transform leader;
        if (activeQueue.Count == 1) leader = positionTracker.transform; //Se for o primeiro, o lider dele é a capivara (segue ela)
        else leader = activeQueue[activeQueue.Count - 2]; //Se já houver outros, ele segue o animal anterior na fila

        int queueIndex = activeQueue.Count * historySpacing; //Aplica o Spacing

        //Ativa o comportamento de seguimento no script do animal
        follower.StartFollowing(leader, positionTracker, queueIndex);

        //Avisa o UI Manager para colocar o Ícone na tela
        OnAnimalAddedToQueue?.Invoke(follower);
    }

    void HandleAnimalSelection (int slotIndex)
    {
        if (slotIndex < activeQueue.Count)
        {
            selectedIndex = slotIndex;
            OnSelectedSlotChanged?.Invoke(selectedIndex); //Avisa UIManager
            Debug.Log($"Animal na posição {selectedIndex + 1} selecionado!");
        }
        else
        {
            Debug.Log("Não há animal nessa posição da fila");
        }
    }

    void TryUseSelectedAnimalAbility()
    {
        if (activeQueue.Count == 0) return;

        int targetIndex = (selectedIndex >= 0 && selectedIndex < activeQueue.Count) ? selectedIndex : 0;
        Transform chosenAnimalTransform = activeQueue[targetIndex];

        TatuBolaAbility tatuBolaAbility = chosenAnimalTransform.GetComponent<TatuBolaAbility>();
        if (tatuBolaAbility != null)
        {
            activeQueue.RemoveAt(targetIndex); //Remove o animal da fila
            OnAnimalRemovedFromQueue?.Invoke(targetIndex); //Avisa o UIManager para remover o ícone da tela

            Vector3 playerFacingDir = positionTracker.LastMoveDirection;
            tatuBolaAbility.LaunchAbility(playerFacingDir); //Lança a habilidade do animal selecionado

            selectedIndex = -1; //Reseta o índice selecionado
            OnSelectedSlotChanged?.Invoke(selectedIndex); // Avisa a UI que não há mais seleção ativa
            ReorganizeQueue(); //Reorganiza a fila após a remoção do animal
        }
    }

    void ReorganizeQueue()
    {
        for (int i = 0; i < activeQueue.Count; i++)
        {
            AnimalFollower animalFollower = activeQueue[i].GetComponent<AnimalFollower>();
            if (animalFollower != null)
            {
                Transform newLeader;
                if  (i == 0)
                {
                    newLeader = playerTransform; //O primeiro animal segue o player
                }
                else
                {
                    newLeader = activeQueue[i -1]; //Os demais seguem o animal anterior na fila
                }

                int newQueueIndex = (i + 1) * historySpacing; //Atualiza o índice de espaçamento
                animalFollower.StartFollowing(newLeader, positionTracker, newQueueIndex); 
            }
        }
    }
}
