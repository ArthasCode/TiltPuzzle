using UnityEngine;

public class cameraFollow : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Transform alvo;
    [SerializeField] private float velocidadeSuavizacao = 2f;

    [Header("Intensidade por eixo (pode inverter com valor negativo)")]
    [SerializeField] private float intensidadeX = -0.1f; // pitch — negativo inverte a direção
    [SerializeField] private float intensidadeZ = 0.15f;  // roll

    private Quaternion rotacaoBase;
    private Vector3 rotacaoInicialMesa;

    void Start()
    {
        rotacaoBase = transform.rotation;
        if (alvo != null)
            rotacaoInicialMesa = alvo.localEulerAngles;
    }

    void LateUpdate()
    {
        if (alvo == null) return;

        Vector3 deltaMesa = alvo.localEulerAngles - rotacaoInicialMesa;
        deltaMesa = NormalizarAngulos(deltaMesa);
        

        Quaternion offset = Quaternion.Euler(
            deltaMesa.x * intensidadeX,
            0f,
            deltaMesa.z * intensidadeZ
        );

        Quaternion rotacaoAlvo = rotacaoBase * offset;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            rotacaoAlvo,
            velocidadeSuavizacao * Time.deltaTime
        );
    }

    Vector3 NormalizarAngulos(Vector3 angulos)
    {
        angulos.x = (angulos.x > 180f) ? angulos.x - 360f : angulos.x;
        angulos.y = (angulos.y > 180f) ? angulos.y - 360f : angulos.y;
        angulos.z = (angulos.z > 180f) ? angulos.z - 360f : angulos.z;
        return angulos;
    }
}
