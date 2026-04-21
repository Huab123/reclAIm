using UnityEngine;

public class combatSwitcher : MonoBehaviour
{
    [SerializeField] private PolygonCollider2D Trigger;

    public static bool CombatActive = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision is not BoxCollider2D) return;

        var playerObject = collision.attachedRigidbody?.gameObject ?? collision.transform.root.gameObject;
        if (playerObject.CompareTag("Player")) {
            //switch combat mode
            CombatActive = !CombatActive;
        }
    }
}
