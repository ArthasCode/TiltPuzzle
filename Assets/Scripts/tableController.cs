using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
public class tableController : MonoBehaviour
{
    [SerializeField]public float taxaInclinacao;
    [SerializeField]public Vector3 inclinacaoMax, inclinacaoMin;
    internal Vector3 rotacaoCalc;


    void Start()
    {
        rotacaoCalc = Vector3.zero;
        transform.localEulerAngles = rotacaoCalc;
        
    }

    void FixedUpdate()
    {
        Vector3 alterInclinacao = new Vector3();

        if (Input.GetKey(KeyCode.UpArrow))
        {
            alterInclinacao.x += taxaInclinacao * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            alterInclinacao.x -= taxaInclinacao * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            alterInclinacao.z += taxaInclinacao * Time.fixedDeltaTime;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            alterInclinacao.z -= taxaInclinacao * Time.fixedDeltaTime;
        }

        rotacaoCalc += alterInclinacao;

        rotacaoCalc = Vector3.Min(Vector3.Max(rotacaoCalc, inclinacaoMin),inclinacaoMax);

        transform.localEulerAngles = rotacaoCalc;
    }

    public void resetInc()
    {
        rotacaoCalc = Vector3.zero;
        transform.localEulerAngles = rotacaoCalc;
    }

    
    
}
