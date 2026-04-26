using UnityEngine;

public class ExitHole : MonoBehaviour, IInteractable
{
    public Transform teleportDestination;
    public string exitID;

    void Start()
    {
        exitID ??= GlobalHelper.GenerateUniqueID(gameObject);
    }

    public bool CanInteract()
    {
        return teleportDestination != null;
    }

    public void Interact()
    {
        if (!CanInteract()) return;
        TeleportPlayer();
    }

    public void TeleportPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = teleportDestination.position;
        }
    }
}