using MoreMountains.Tools;
using System;
using UnityEngine;

public class AnimalFollower : MonoBehaviour
{
    [Header("Configuração de Fase")]
    [SerializeField] private bool isMainAnimal = false; //Marcar True no Inspector se for o animal obrigatório da fase
    public bool IsMainAnimal => isMainAnimal;

    private Transform targetToFollow;
    private MovePlayer playerMovement;
    private int historyIndex;
    [SerializeField] private float followSpeed = 5f;

    private bool isFollowing = false;


    //Chama esse método no FollowerManager e configura a posição do animal na fila, o líder que ele deve seguir e a referência do player para acessar o histórico de posições
    public void StartFollowing(Transform leader, MovePlayer playerRef, int indexInQueue)
    {
        targetToFollow = leader;
        playerMovement = playerRef;
        historyIndex = indexInQueue; //Define qual ponto do histórico ele deve seguir
        isFollowing = true;
    }

    void FixedUpdate()
    {
        //Se não estiver seguindo, para a FixedUpdate
        if (!isFollowing || targetToFollow == null || playerMovement == null) return;

        //Verifica se o histórico da capivara já tem pontos suficientes gravados
        if (playerMovement.PositionHistory.Count > historyIndex)
        {
            //Calcula qual índice exato do histórico pertence a este animal
            int targetIndex = playerMovement.PositionHistory.Count - 1 - historyIndex;

            if (targetIndex >= 0 && targetIndex < playerMovement.PositionHistory.Count)
            {
                Vector3 targetPos = playerMovement.PositionHistory[targetIndex];

                //Move o animal até a posição gravada no passado
                transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.fixedDeltaTime);
            }
        }

    }
}
