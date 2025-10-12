using QFSW.QC;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Singleton instance
    public static GameController Instance { get; private set; }

    public GameObject MainPlayer;
    public Ragdoll defaultRagdoll;
    public Ground ground;

    [Command("@a")]
    static Vector2 PlayerPos()
    {
        return Instance.MainPlayer.transform.position;
    }

    void Awake()
    {
        // Singleton setup: Ensure only one instance exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }

        Logger.logs += Debug.Log;
        Logger.errors += Debug.LogError;
        Logger.warnings += Debug.LogWarning;
        Logger.Log("Initialized Logs", gameObject);

        LoadResources();
        ground = FindFirstObjectByType<Ground>();
    }

    void LoadResources()
    {
        defaultRagdoll = Resources.Load<Ragdoll>("Prefabs/defaultRagdoll");
    }
}
