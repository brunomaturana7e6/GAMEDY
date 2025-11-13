using UnityEngine;

public class MiniMapBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Transform car;
    [SerializeField] private EnterCar enterCar;
    private void LateUpdate()
    {
        Vector3 newPosition;
        if(enterCar.inCar)
            newPosition = car.position;
        else
            newPosition = _player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
