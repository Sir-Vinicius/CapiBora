using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    [Header("Spawn")]
    [SerializeField] private Transform spawnPoint;

    public Rigidbody meuRB;
    private Vector2 direcaoInput;

    [SerializeField] private float vel = 5f;
    private Vector3 forward, right;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meuRB = GetComponent<Rigidbody>();

        if (spawnPoint != null)
        {
            meuRB.position = spawnPoint.position;
            transform.position = spawnPoint.position;
        }
        else Debug.LogWarning("SpawnPoint não atribuído no Inspector do MovePlayer");


        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        
        right = Camera.main.transform.right;
        right.y = 0;
        right = Vector3.Normalize(right);
    }

    public void Mover(InputAction.CallbackContext context)
    {
        direcaoInput = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movimento = new Vector3(direcaoInput.x * right.x + direcaoInput.y * forward.x, 0, direcaoInput.x * right.z + direcaoInput.y * forward.z);
        Vector3 novaPosicao = meuRB.position + movimento * vel * Time.fixedDeltaTime;
        meuRB.MovePosition(novaPosicao);
    }
}
