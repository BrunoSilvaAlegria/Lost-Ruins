using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player playerPrefab;
    [SerializeField] private Checkpoint activeCheckpoint;

    Player player;

    static LevelManager _instance = null;
    public static LevelManager instance => _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (activeCheckpoint != null)
        {
            activeCheckpoint.EnableCheckpoint(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (player == null)
        {
            player = FindObjectOfType<Player>();
            if (player == null)
            {
                player = Instantiate(playerPrefab, activeCheckpoint.transform.position, activeCheckpoint.transform.rotation);

                ObjectFollow objectFollow = FindObjectOfType<ObjectFollow>();
                if (objectFollow.objectToFollow == null)
                {
                    objectFollow.objectToFollow = player.transform;
                }
            }
        }
    }

    public void SetCheckpoint(Checkpoint checkpoint)
    {
        if (activeCheckpoint != null)
        {
            activeCheckpoint.EnableCheckpoint(false);
        }
        activeCheckpoint = checkpoint;
        activeCheckpoint.EnableCheckpoint(true);
    }

    public void NextLevel()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
