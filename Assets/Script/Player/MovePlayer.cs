using UnityEngine;
using UnityEngine.InputSystem;

public class MovePlayer : MonoBehaviour
{
    public Rigidbody meuRB;
    private Vector2 direcao;

    [SerializeField] private float vel = 5f;
    private Vector3 forward, right;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        meuRB = GetComponent<Rigidbody>();
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        
        right = Camera.main.transform.right;
        right.y = 0;
        right = Vector3.Normalize(right);
    }

    public void Mover(InputAction.CallbackContext context)
    {


        direcao = context.ReadValue<Vector2>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 movimento = new Vector3(direcao.x * right.x + direcao.y * forward.x, 0, direcao.x * right.z + direcao.y * forward.z);
        meuRB.MovePosition(meuRB.position + movimento * vel * Time.fixedDeltaTime);
    }
}
