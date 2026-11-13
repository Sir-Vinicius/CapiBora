using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class FollowerManager : MonoBehaviour
{
    [SerializeField] private MovePlayer playerMovement;
    [SerializeField] private List<Transform> activeQueue = new List<Transform>(); //Lista para armazenar a ordem exata dos animais na fila
    public IReadOnlyList<Transform> ActiveQueue => activeQueue; //Apenas leitura para outros scripts

    [Header("Espaçamento da Fila")]
    [SerializeField] private int historySpacing = 15; //Quantidade de pontos de histórico de distância

    void Start()
    {
        if (playerMovement == null) Debug.LogWarning("Não achou MovePlayer");
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
        activeQueue.Add(animalTransform); //Coloca no final da lista

        AnimalFollower follower = animalTransform.GetComponent<AnimalFollower>();

        if (follower == null) return;

        Transform leader;

        if (activeQueue.Count == 1) leader = playerMovement.transform; //Se for o primeiro, o lider dele é a capivara (segue ela)
        else leader = activeQueue[activeQueue.Count - 2]; //Se já houver outros, ele segue o animal anterior na fila

        int queueIndex = activeQueue.Count * historySpacing; //Aplica o Spacing

        //Ativa o comportamento de seguimento no script do animal
        follower.StartFollowing(leader, playerMovement, queueIndex);
    }
}
