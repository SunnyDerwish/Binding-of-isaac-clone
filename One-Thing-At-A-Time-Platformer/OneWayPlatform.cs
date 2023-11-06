using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    public LayerMask playerLayer;
    public float disableTime = 0.2f;
    private BoxCollider2D platformCollider;
    private Rigidbody2D playerRigidbody;

    // Start is called before the first frame update
    void Start()
    {
        platformCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
      GameObject player = GameObject.FindGameObjectWithTag("Player");
      playerRigidbody = player.GetComponent<Rigidbody2D>();

      if (playerRigidbody.velocity.y > 0)
      {
        if (IsPlayerBelowPlatform(player.transform))
        {
            StartCoroutine(DisableColliderForTime(disableTime));
        }
      }   
    }

    bool IsPlayerBelowPlatform(Transform playerTransform)
    {
        return playerTransform.position.y < transform.position.y;
    }
    
    System.Collections.IEnumerator DisableColliderForTime(float time)
    {
        platformCollider.enabled = false;
        yield return new WaitForSeconds(time);
        platformCollider.enabled = true;
    }
}
