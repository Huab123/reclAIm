using UnityEngine;
using Unity.Cinemachine;

public class MapTransition : MonoBehaviour
{
	[SerializeField] private PolygonCollider2D mapBoundry;
	private CinemachineConfiner2D confiner;
	[SerializeField] private Direction direction;
	[SerializeField] private float additivePos = 2f;

	private enum Direction {Up, Down, Left, Right}

	private void Awake() {
		confiner = FindFirstObjectByType<CinemachineConfiner2D>();
	}	

	private void OnTriggerEnter2D(Collider2D collision) {
		if (collision.gameObject.CompareTag("Player")) {
			confiner.BoundingShape2D = mapBoundry;
			UpdatePlayerPosition(collision.gameObject);

			MapController_Manual.Instance?.HighlightArea(mapBoundry.name);
			MapController_Dynamic.Instance?.UpdateCurrentArea(mapBoundry.name);
		}
	}

	private void UpdatePlayerPosition(GameObject player) {
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
