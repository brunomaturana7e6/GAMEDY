using UnityEngine;

public class MiniMapBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _player;
    private void LateUpdate()
    {
        Vector3 newPosition = _player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
