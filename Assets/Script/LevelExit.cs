using System;
using Unity.VisualScripting;
using UnityEngine;

public class LevelExit : MonoBehaviour
{
    [Header("Referências")]
    [SerializeField] private FollowerManager followerManager;

    [Header("Regras da Fase")]
    [SerializeField] private bool requiresMainAnimal = true; //Se a fase exige o animal principal para acabar

    //Events 
    public static event Action OnLevelCompleted;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Se a fase exige o animal principal
            if (requiresMainAnimal)
            {
                if (!HasMainAnimalRescued())
                {
                    Debug.Log("Você precisa resgatar o animal principal antes");
                    return; //Sai do método e impede o fim da fase
                }
            }

            CompleteLevel(other.gameObject);
        }
    }

    bool HasMainAnimalRescued()
    {
        foreach (Transform animalTransform in followerManager.ActiveQueue)
        {
            AnimalFollower follower = animalTransform.GetComponent<AnimalFollower>();

            if (follower != null && follower.IsMainAnimal)
            {
                return true;
            }
        }

        return false;
    }
    
    void CompleteLevel(GameObject player)
    {
        Debug.Log("Fase concluída");

        MovePlayer playerMovement = player.GetComponent<MovePlayer>();

        if (playerMovement != null)
        {
            playerMovement.enabled = false; //Desativa o script de movimento do player
        }

        //Avisa o UIManager para mostrar a tela de vitória
        OnLevelCompleted?.Invoke();

    }
}
