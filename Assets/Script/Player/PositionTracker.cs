using System.Collections.Generic;
using UnityEngine;

public class PositionTracker : MonoBehaviour
{
    [Header("Configurações de Rastro")]
    [SerializeField] private float minDistanceToRecord = 0.1f; //Distância mínima para registrar uma nova posição
    [SerializeField] private int maxHistorySize = 500;

    private List<Vector3> positionHistory = new List<Vector3>();
    private Vector3 lastMoveDirection = Vector3.forward;

    //Apenas leitura
    public IReadOnlyList<Vector3> PositionHistory => positionHistory;
    public Vector3 LastMoveDirection => lastMoveDirection;

    private void Start()
    {
        //Grava a posição inicial
        //No primeiro FixedUpdate o rastro já se ajusta
        positionHistory.Add(transform.position);
    }

    private void FixedUpdate()
    {
        Vector3 currentPos = transform.position;
        Vector3 lastRecordedPos = positionHistory[positionHistory.Count - 1];

        //Calcula o deslocamento desde o último ponto registrado
        Vector3 displacement = currentPos - lastRecordedPos;
        displacement.y = 0f; //Ignora valor vertical

        if (displacement.magnitude >= minDistanceToRecord)
        {
            //Atualiza a direção para onde o player está olhando
            lastMoveDirection = displacement.normalized;

            positionHistory.Add(currentPos);

            //Limite em 500 para evitar consumo alto de memória RAM
            if (positionHistory.Count > maxHistorySize)
            {
                //Remove o ponto mais antigo
                positionHistory.RemoveAt(0);
            }
        }
    }
}