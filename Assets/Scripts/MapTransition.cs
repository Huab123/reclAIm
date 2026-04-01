using UnityEngine;
using Unity.Cinemachine;
using Unity.VisualScripting;
using System.Threading.Tasks;

public class MapTransition : MonoBehaviour
{
	[SerializeField] private PolygonCollider2D mapBoundry;
	private CinemachineConfiner2D confiner;
	[SerializeField] private Direction direction;
	[SerializeField] private Transform teleportTargetPosition;
	[SerializeField] private float additivePos = 2f;

	private enum Direction {Up, Down, Left, Right, Teleport}

	private void Awake() {
		confiner = FindFirstObjectByType<CinemachineConfiner2D>();
	}	

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			FadeTransition(collision.gameObject);

			MapController_Manual.Instance?.HighlightArea(mapBoundry.name);
			MapController_Dynamic.Instance?.UpdateCurrentArea(mapBoundry.name);
		}
	}

	async void FadeTransition(GameObject player)
	{
		var playerMovement = player.GetComponent<PlayerMovement>();
		var rb = player.GetComponent<Rigidbody2D>();
		if (rb != null) rb.linearVelocity = Vector2.zero;
		if (playerMovement != null) playerMovement.enabled = false;

		await ScreenFader.Instance.FadeOut();
		confiner.BoundingShape2D = mapBoundry;
		UpdatePlayerPosition(player);
		await Task.Delay(500);
		await ScreenFader.Instance.FadeIn();

		if (playerMovement != null) playerMovement.enabled = true;
	}

	private void UpdatePlayerPosition(GameObject player) {
		if (direction == Direction.Teleport) {
			player.transform.position = teleportTargetPosition.position;
			return;
		}
		Vector3 newPos = player.transform.position;

		switch (direction) {
			case Direction.Up:
				newPos.y += additivePos;
				break;
			case Direction.Down:
				newPos.y -= additivePos;
				break;
			case Direction.Left:
				newPos.x -= additivePos;
				break;
			case Direction.Right:
				newPos.x += additivePos;
				break;
		}

		player.transform.position = newPos;
	}
}
