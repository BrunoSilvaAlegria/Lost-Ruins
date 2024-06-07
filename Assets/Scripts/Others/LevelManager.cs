using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private Player     playerPrefab;
    [SerializeField] private Checkpoint activeCheckpoint;

    Player  player;

    static LevelManager _Instance = null;
    public static LevelManager Instance => _Instance;

    private void Awake()
    {
        if (_Instance == null)
        {
            _Instance = this;
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
}
