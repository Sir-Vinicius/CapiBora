using System;
using Unity.VisualScripting;
using UnityEngine;

public class TatuBolaAbility : MonoBehaviour
{
    [Header("Configurações")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float maxDistance = 8f;
    [SerializeField] private float maxDuration = 2f;

    private Vector3 rollDirection;
    private bool isUsingAbility = false;
    private float distanceTraveled = 0f;
    private float timer = 0f;
    private string originalTag;

    void Awake()
    {
        originalTag = gameObject.tag;
    }

    public void LaunchAbility(Vector3 direction)
    {
        rollDirection = direction.sqrMagnitude > 0.01f ? direction : Vector3.forward;
        rollDirection.y = 0f;
        rollDirection.Normalize();

        isUsingAbility = true;
        distanceTraveled = 0f;
        timer = 0f;

        //Evita que o tatu seja adicionado na fila novamente no lançamento
        gameObject.tag = "Untagged";

        AnimalFollower animalFollower = GetComponent<AnimalFollower>();
        if (animalFollower != null)
        {
            animalFollower.enabled = false;
        }

        Debug.Log("Habilidade do Tatu foi usada");
    }

    void FixedUpdate()
    {
        if (!isUsingAbility) return;

        float step = speed * Time.fixedDeltaTime;
        transform.position += rollDirection * step;

        distanceTraveled += step;
        timer += Time.fixedDeltaTime;

        if (distanceTraveled >= maxDistance || timer >= maxDuration)
        {
            StopAbility();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isUsingAbility) return;

        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Tatu colidiu com um obstáculo e parou");
            StopAbility();
        }
    }

    void StopAbility()
    {
        isUsingAbility = false;

        if (!string.IsNullOrEmpty(originalTag))
        {
            //Volta pra tag original
            gameObject.tag = originalTag;
        }
    }

    public void OnRejoinedQueue()
    {
        isUsingAbility = false;
        distanceTraveled = 0f;
        timer = 0f;

        if (!string.IsNullOrEmpty(originalTag))
        {
            gameObject.tag = originalTag;
        }

        AnimalFollower animalFollower = GetComponent<AnimalFollower>();
        if (animalFollower != null)
        {
            animalFollower.enabled = true;
        }
    }
}
