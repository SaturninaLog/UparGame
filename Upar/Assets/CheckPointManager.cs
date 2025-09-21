using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance;

    private Vector3 lastCheckpointPosition;
    private bool hasCheckpoint = false;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    public void SetStartPosition(Vector3 startPos)
    {
        if (!hasCheckpoint)
        {
            lastCheckpointPosition = startPos;
            hasCheckpoint = true;
        }
    }

    public void UpdateCheckpoint(int id, Vector3 pos)
    {
        lastCheckpointPosition = pos;
        hasCheckpoint = true;
        Debug.Log("Checkpoint actualizado: " + id);
    }

    public Vector3 GetRespawnPosition()
    {
        return lastCheckpointPosition;
    }
}
