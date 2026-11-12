using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    public Rigidbody meuRB;
    private Vector2 direcaoInput;

    [SerializeField] private float vel = 5f;
    private Vector3 forward, right;

    [Header("Configurações para Fila")]
    [SerializeField] private List<Vector3> positionHistory = new List<Vector3>(); //Armazena posições recentes para os animais seguirem
    [SerializeField] private float minDistanceToRecord = 0.1f; //Distância mínima para registrar uma nova posição

    //Apenas para Leitura em outros scripts
    public IReadOnlyList<Vector3> PositionHistory => positionHistory;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meuRB = GetComponent<Rigidbody>();
        
        //Salva a posição inicial
        positionHistory.Add(meuRB.position);

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

        //Só grava o histórico se o player estiver se movendo
        if (direcaoInput.sqrMagnitude > 0.01f)
        {
            //Verifica se o player andou o suficiente para gravar um novo ponto no rastro
            if (Vector3.Distance(positionHistory[positionHistory.Count - 1], meuRB.position) >= minDistanceToRecord)
            {
                positionHistory.Add(meuRB.position);

                //Limite em 500 para evitar consumo alto de memória RAM
                if (positionHistory.Count > 500)
                {
                    //Remove o ponto mais antigo
                    positionHistory.RemoveAt(0);
                }
            }
        }
    }
}
