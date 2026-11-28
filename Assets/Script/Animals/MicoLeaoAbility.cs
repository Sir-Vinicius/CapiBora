using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicoLeaoAbility : MonoBehaviour
{
    [Header("Configurações da Habilidade")]
    [SerializeField] private float disableDuration = 5f; //Tempo em que as câmeras ficam desativadas
    [SerializeField] private float cooldownDuration = 8f; //Tempo de recarga até poder usar a habilidade novamente
    [SerializeField] private bool disableConeGameObject = true; //Também desativa/esconde o objeto ConeVision

    public bool IsOnCooldown => isOnCooldown;
    public float CooldownRemaining => Mathf.Max(0f, cooldownRemaining);

    private bool isOnCooldown = false;
    private float cooldownRemaining = 0f;
    private Coroutine abilityRoutine;

    private readonly List<SecurityCameraVision> affectedCameras = new List<SecurityCameraVision>();

    void Update()
    {
        if (isOnCooldown)
        {
            cooldownRemaining = Mathf.Max(0f, cooldownRemaining - Time.deltaTime);
        }
    }

    public void LaunchAbility(Vector3 direction)
    {
        if (isOnCooldown)
        {
            Debug.Log($"Habilidade do Mico-Leão em cooldown");
            return;
        }

        // O Mico-Leão permanece na fila (não altera tag nem desabilita AnimalFollower)
        DisableAllCameras();

        if (abilityRoutine != null)
        {
            StopCoroutine(abilityRoutine);
        }

        abilityRoutine = StartCoroutine(AbilityAndCooldownRoutine());
        Debug.Log("Habilidade do Mico-Leão ativada");
    }

    private IEnumerator AbilityAndCooldownRoutine()
    {
        isOnCooldown = true;

        float totalCooldown = Mathf.Max(cooldownDuration, disableDuration);
        cooldownRemaining = totalCooldown;

        if (disableDuration > 0f)
        {
            yield return new WaitForSeconds(disableDuration);
            EnableAllCameras();

            float remainingAfterCameras = Mathf.Max(0f, totalCooldown - disableDuration);
            if (remainingAfterCameras > 0f)
            {
                yield return new WaitForSeconds(remainingAfterCameras);
            }
        }
        else if (totalCooldown > 0f)
        {
            //Aguarda o cooldown para permitir novo uso
            yield return new WaitForSeconds(totalCooldown);
        }

        cooldownRemaining = 0f;
        isOnCooldown = false;
        abilityRoutine = null;

        Debug.Log("Habilidade do Mico-Leão pronta novamente!");
    }

    private void DisableAllCameras()
    {
        affectedCameras.Clear();

        SecurityCameraVision[] cameras = FindObjectsByType<SecurityCameraVision>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (SecurityCameraVision cameraVision in cameras)
        {
            if (cameraVision == null) continue;

            affectedCameras.Add(cameraVision);

            if (disableConeGameObject)
            {
                cameraVision.gameObject.SetActive(false);
            }
            else
            {
                cameraVision.enabled = false;
            }
        }
    }

    private void EnableAllCameras()
    {
        foreach (SecurityCameraVision cameraVision in affectedCameras)
        {
            if (cameraVision == null) continue;

            if (disableConeGameObject)
            {
                cameraVision.gameObject.SetActive(true);
            }

            cameraVision.enabled = true;
        }

        affectedCameras.Clear();
    }
}