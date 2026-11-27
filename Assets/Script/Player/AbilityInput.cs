using System;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.InputSystem;


public class AbilityInput : MonoBehaviour
{
    public event Action OnAbilityInputTriggered;
    public event Action<int> OnSelectSlotTriggered;

    //Tecla 1
    public void OnSelect1(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSelectSlotTriggered?.Invoke(0); //Índice 0 = 1º animal da fila
            Debug.Log("Botão 1 pressionado");
        }
    }

    //Tecla 2
    public void OnSelect2(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSelectSlotTriggered?.Invoke(1); //Índice 1 = 2º animal da fila
            Debug.Log("Botão 2 pressionado");
        }
    }

    //Tecla 3
    public void OnSelect3(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnSelectSlotTriggered?.Invoke(2); //Índice 2 = 3º animal da fila
            Debug.Log("Botão 3 pressionado");
        }
    }

    //Tecla "F"
    public void OnAbility(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            OnAbilityInputTriggered?.Invoke();
            Debug.Log("Habilidade pressionada");
        }
    }
}
