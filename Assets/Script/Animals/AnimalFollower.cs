using MoreMountains.Tools;
using System;
using UnityEngine;

public class AnimalFollower : MonoBehaviour
{
    [Header("Configuração de Fase")]
    [SerializeField] private bool isMainAnimal = false; //Marcar True no Inspector se for o animal obrigatório da fase
    public bool IsMainAnimal => isMainAnimal;

    [Header("Ícone da UI")]
    [SerializeField] private Sprite animalIcon;
    public Sprite AnimalIcon => animalIcon;

    private Transform targetToFollow;
    private PositionTracker positionTracker;
    private int historyIndex;
    [SerializeField] private float followSpeed = 5f;

    private bool isFollowing = false;

    private Animator animator;
    private MovePlayer playerMovement;

    private int inputXHash = Animator.StringToHash("inputX");
    private int inputYHash = Animator.StringToHash("inputY");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerMovement = FindAnyObjectByType<MovePlayer>();
    }

    //Chama esse método no FollowerManager e configura a posição do animal na fila, o líder que ele deve seguir e a referência do player para acessar o histórico de posições
    public void StartFollowing(Transform leader, PositionTracker posTracker, int indexInQueue)
    {
        targetToFollow = leader;
        positionTracker = posTracker;
        historyIndex = indexInQueue; //Define qual ponto do histórico ele deve seguir
        isFollowing = true;
    }

    void FixedUpdate()
    {
        //Se não estiver seguindo, para a FixedUpdate
        if (!isFollowing || targetToFollow == null || positionTracker == null) return;

        //Verifica se o histórico da capivara já tem pontos suficientes gravados
        if (positionTracker.PositionHistory.Count > historyIndex)
        {
            //Calcula qual índice exato do histórico pertence a este animal
            int targetIndex = positionTracker.PositionHistory.Count - 1 - historyIndex;

            if (targetIndex >= 0 && targetIndex < positionTracker.PositionHistory.Count)
            {
                Vector3 targetPos = positionTracker.PositionHistory[targetIndex];

                //Move o animal até a posição gravada no passado
                Vector2 direcaoInput = (targetPos - transform.position).normalized;
                animator.SetFloat(inputXHash, playerMovement.direcaoInput.x);
                animator.SetFloat(inputYHash, playerMovement.direcaoInput.y);

                transform.position = Vector3.MoveTowards(transform.position, targetPos, followSpeed * Time.fixedDeltaTime);
                
            }
        }

    }
}
