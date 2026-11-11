using UnityEngine;

public class HoleFall : MonoBehaviour
{
    public bool BallEntered { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball"))
        {
            BallEntered = true;
        }
    }

    public void ResetTrigger()
    {
        BallEntered = false;
    }
}