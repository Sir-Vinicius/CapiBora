using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class CageController : MonoBehaviour
{
    [Header("Configurações da Jaula")]
    [SerializeField] private bool startsOpen = false;
    [SerializeField] private float openInteractionTime = 1.5f;

    [Header("Pontos de Teleporte")]
    [SerializeField] private Transform interiorPoint;
    [SerializeField] private Transform exteriorPoint;
    [SerializeField] private float formationSpacing = 0.6f;

    [Header("Referências")]
    [SerializeField] private FollowerManager followerManager;

    [SerializeField] private MMF_Player unlockSound;
    

    [Header("UI")]
    [SerializeField] private CageUI cageUI;

    private PlayerInput playerInput;
    private InputAction interactAction;
    private MovePlayer playerMovement;

    private bool playerInside;
    private bool interactHeld;
    private float interactionTimer;

    public bool IsOpen { get; private set; }

    private void Awake()
    {
        IsOpen = startsOpen;
        
    }

    private void Update()
    {
        // Só conta o tempo enquanto a jaula estiver trancada
        // e o jogador estiver segurando E.
        if (IsOpen || !interactHeld || playerInput == null)
            return;

        interactionTimer += Time.deltaTime;
        cageUI.SetProgress(interactionTimer / openInteractionTime);

        if (interactionTimer >= openInteractionTime)
        {
            OpenCage();
            interactHeld = false;
        }
    }

    public void OpenCage()
    {
        if (IsOpen) return;

        IsOpen = true;
        interactionTimer = 0f;
        cageUI.SetProgress(0f);

        Debug.Log($"{name}: Jaula aberta!");
        unlockSound.PlayFeedbacks();
    }

    public void LockCage()
    {
        if (!IsOpen || playerInside) return;

        IsOpen = false;

        Debug.Log($"{name}: Jaula trancada!");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Algo entrou na área de interação: " + other.name);

        PlayerInput detectedInput =
            other.GetComponentInParent<PlayerInput>();

        if (detectedInput == null)
        {
            Debug.LogWarning("O objeto detectado não possui PlayerInput!");
            return;
        }

        cageUI.Show(transform);

        Debug.Log("Jogador detectado pela jaula!");

        if (playerInput != null && playerInput != detectedInput)
            return;

        playerInput = detectedInput;
        playerMovement = playerInput.GetComponent<MovePlayer>();

        interactAction =
            playerInput.actions.FindAction("Interact", false);

        if (interactAction == null)
        {
            Debug.LogWarning("Action Interact não encontrada!");
            return;
        }

        interactAction.started += OnInteractStarted;
        interactAction.canceled += OnInteractCanceled;

        Debug.Log("Action Interact conectada à jaula!");
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerInput detectedInput =
            other.GetComponentInParent<PlayerInput>();

        if (detectedInput == null || detectedInput != playerInput)
            return;

        UnsubscribeFromInput();
        cageUI.Hide();
    }

    private void OnInteractStarted(InputAction.CallbackContext context)
    {
        if (!IsOpen)
        {
            cageUI.SetProgress(0f);
            interactionTimer = 0f;
            interactHeld = true;
            return;
        }

        ToggleGroupPosition();
    }

    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        cageUI.SetProgress(0f);
        interactHeld = false;
        interactionTimer = 0f;
    }

    private void ToggleGroupPosition()
    {
        if (playerInput == null) return;

        Transform destination;

        if (playerInside)
        {
            destination = exteriorPoint;
            playerInside = false;
        }
        else
        {
            destination = interiorPoint;
            playerInside = true;
        }

        if (destination == null)
        {
            Debug.LogWarning("Ponto de teleporte não configurado!");
            return;
        }

        // Teleporta a capivara
        MoveCharacter(playerInput.transform, destination.position);

        // Teleporta os animais que estão seguindo a capivara
        if (followerManager != null)
        {
            var animals = followerManager.ActiveQueue;

            for (int i = 0; i < animals.Count; i++)
            {
                Transform animal = animals[i];

                if (animal == null) continue;

                int column = i % 3;
                int row = i / 3;

                float horizontalOffset =
                    (column - 1) * formationSpacing;

                float depthOffset =
                    (row + 1) * formationSpacing;

                Vector3 offset =
                    destination.right * horizontalOffset -
                    destination.forward * depthOffset;

                MoveCharacter(
                    animal,
                    destination.position + offset
                );
            }
        }

        // Atualiza o histórico para evitar que os animais
        // tentem seguir posições anteriores ao teleporte.
        if (playerMovement != null)
            playerMovement.ResetPositionHistory();

        Physics.SyncTransforms();

        Debug.Log(playerInside
            ? "Grupo entrou na jaula!"
            : "Grupo saiu da jaula!");
    }

    private void MoveCharacter(Transform character, Vector3 position)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 30f, 0f);

        Rigidbody rb = character.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.position = position;
            rb.rotation = targetRotation;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else
        {
            character.SetPositionAndRotation(position, targetRotation);
        }
    }

    private void UnsubscribeFromInput()
    {
        if (interactAction != null)
        {
            interactAction.started -= OnInteractStarted;
            interactAction.canceled -= OnInteractCanceled;
        }

        interactAction = null;
        playerInput = null;
        playerMovement = null;

        interactHeld = false;
        interactionTimer = 0f;
    }

    private void OnDestroy()
    {
        UnsubscribeFromInput();
    }
}
