using UnityEngine;

public class levelManager : MonoBehaviour
{
    public static levelManager Instance;

    [SerializeField]private GameObject[] levels;
    [SerializeField]private float yInitial = 1f;
    [SerializeField]private GameObject ball;
    [SerializeField]private Transform camera;
    [SerializeField]private float yInitialCam;

    private int levelNow = 0;
    private Rigidbody rb;

    void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        rb = ball.GetComponent<Rigidbody>();
        yInitialCam = camera.position.y;
        for (int i = 0; i < levels.Length; i++)
        {
            DefineLevelState(levels[i], i == levelNow);
        }
    }

    public void NextLevel()
    {
        int next = levelNow + 1;
        if(next >= levels.Length)
        {
            Debug.Log("The tower has been beaten!");
            return;
        }

        DefineLevelState(levels[levelNow], false);
        DefineLevelState(levels[next], true);

        levelNow = next;
        RespawnBall();

        tableController table = levels[next].GetComponentInChildren<tableController>();
        if(table != null)
        {
            table.resetInc();
        }

        Vector3 posCamera = camera.position;
        posCamera.y = yInitialCam - (10f * levelNow);
        camera.position = posCamera;
    }

    void DefineLevelState(GameObject level, bool active)
    {
        Renderer[] renderers = level.GetComponentsInChildren<Renderer>(true);
        foreach(Renderer r in renderers) r.enabled = active;

        Collider[] colliders = level.GetComponentsInChildren<Collider>(true);
        foreach (Collider c in colliders) c.enabled = active;
    }

    void RespawnBall()
    {
        float newY = yInitial - (10f * levelNow);
        Vector3 newPos = new Vector3(0f, newY, 0f);
        rb.position = newPos;
    }

    // Update is called once per frame
    
}
