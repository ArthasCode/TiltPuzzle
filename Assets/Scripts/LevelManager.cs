using System;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField]public GameObject[] levels;
    [SerializeField]private float yInitial;
    [SerializeField]private GameObject ball;
    [SerializeField]private Transform camera;
    [SerializeField]private float yInitialCam;

    public int levelNow {get; set;} = 0;
    private Vector3 ActualLevelPosition;

    void Start()
    {
        yInitialCam = camera.position.y;

        yInitial = levels[0].transform.position.y + 0.1f;
    }

    public void SetInitialLevel(Vector3 newPos)
    {
        ActualLevelPosition = levels[levelNow].transform.GetChild(0).transform.position;
        for (int i = 0; i < levels.Length; i++)
            DefineLevelState(levels[i], i == levelNow);
        RespawnBall(newPos);
    }
    public bool NextLevel(Vector3 newPos)
    {
        int next = levelNow + 1;
        if(next >= levels.Length)
        {
            Debug.Log("The tower has been beaten!");
            return false;
        }

        DefineLevelState(levels[levelNow], false);
        DefineLevelState(levels[next], true);

        levelNow = next;
        ActualLevelPosition = levels[levelNow].transform.GetChild(0).transform.position;
        RespawnBall(newPos);

        TableController table = levels[next].GetComponentInChildren<TableController>();
        if(table != null)
        {
            table.resetInc();
        }

        return true;
    }

    void DefineLevelState(GameObject level, bool active)
    {
        Renderer[] renderers = level.GetComponentsInChildren<Renderer>(true);
        foreach(Renderer r in renderers) r.enabled = active;

        Collider[] colliders = level.GetComponentsInChildren<Collider>(true);
        foreach (Collider c in colliders) c.enabled = active;
    }

    public void RespawnBall(Vector3 newPos)
    {   
        float newY = yInitial - MathF.Abs(10f * levelNow);
        
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        newPos.y = newY;
        ball.transform.position = newPos;

        Vector3 posCamera = camera.position;
        posCamera.y = ActualLevelPosition.y + 20f;
        camera.position = posCamera;
    }
    
}