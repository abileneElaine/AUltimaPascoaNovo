using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;             // O jogador que a câmera segue
    public float smoothSpeed = 0.125f;   // Velocidade de suavização
    public Vector3 offset = new Vector3(0, 1, -10); // Distância da câmera
    public float lookAheadDistance = 2f; // Quanto a câmera olha à frente

    private float lookAheadDir;          // Direção atual (1 = direita, -1 = esquerda)
    private float lookAheadSpeed = 3f;   // Velocidade com que a câmera muda de lado
    private float lastTargetX;            // Guarda a posição anterior do player

    void Start()
    {
        if (target != null)
            lastTargetX = target.position.x;
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Calcula direção de movimento do jogador (direita ou esquerda)
        float deltaX = target.position.x - lastTargetX;
        lastTargetX = target.position.x;

        if (Mathf.Abs(deltaX) > 0.01f)
        {
            // Atualiza a direção do olhar com base no movimento
            lookAheadDir = Mathf.Sign(deltaX);
        }

        // Alisa a transição entre esquerda e direita (sem mudança brusca)
        float lookAheadX = Mathf.Lerp(0, lookAheadDir * lookAheadDistance, Time.deltaTime * lookAheadSpeed);

        // Posição desejada da câmera
        Vector3 desiredPosition = new Vector3(target.position.x + lookAheadX, target.position.y, 0) + offset;

        // Movimento suave da câmera até a posição desejada
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;
    }
}
