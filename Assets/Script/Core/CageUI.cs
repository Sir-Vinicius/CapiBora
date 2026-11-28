using UnityEngine;
using UnityEngine.UI;

public class CageUI : MonoBehaviour
{

    [Header("Referências")]
    [SerializeField] private Image progressRing;
    [SerializeField] private float heightOffset = 2f;
    [SerializeField] private Camera targetCamera;

    private Transform currentCage;

    private Canvas canvas;

    private void Awake()
    {
        canvas = GetComponent<Canvas>();

        if (targetCamera == null)
            targetCamera = Camera.main;

        canvas.enabled = false;
        progressRing.fillAmount = 0f;
    }

    private void LateUpdate()
    {
        if (currentCage == null)
            return;

        // Mantém o indicador acima da jaula.
        transform.position =
            currentCage.position + Vector3.up * heightOffset;

        // Mantém o Canvas orientado para a câmera.
        if (targetCamera != null)
            transform.rotation = targetCamera.transform.rotation;
    }

    public void Show(Transform cage)
    {
        currentCage = cage;
        progressRing.fillAmount = 0f;
        canvas.enabled = true;
    }

    public void Hide()
    {
        currentCage = null;
        progressRing.fillAmount = 0f;
        canvas.enabled = false;
    }

    public void SetProgress(float progress)
    {
        progressRing.fillAmount = Mathf.Clamp01(progress);
    }
}
