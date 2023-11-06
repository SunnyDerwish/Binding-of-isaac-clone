using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float speed = 10.0f; // Speed at which the bullet moves

    private Vector2 moveDirection;

    void Start()
    {
        // Assuming the bullet's local up direction is its forward direction
        moveDirection = transform.up;
    }

    void Update()
    {
        MoveBullet();
    }

    void MoveBullet()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }
}
