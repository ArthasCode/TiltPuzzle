using UnityEngine;

public class TableController : MonoBehaviour
{
    [SerializeField]public float taxaInclinacao;
    [SerializeField]public Vector3 inclinacaoMax, inclinacaoMin;
    public Vector3 rotacaoCalc;


    void Start()
    {
        rotacaoCalc = Vector3.zero;
        transform.localEulerAngles = rotacaoCalc;
        
    }

    public void resetInc()
    {
        rotacaoCalc = Vector3.zero;
        transform.localEulerAngles = rotacaoCalc;
    }
    
}