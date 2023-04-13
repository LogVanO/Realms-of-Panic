using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehavior : MonoBehaviour
{
    public float speed;
    
    // If the animation has flipped directions
    private bool flipped = false;
    private GameObject player;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D body;

    void Start()
    {
        body = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        MoveTowardsPlayer();
    }

    private void MoveTowardsPlayer() {
        body.velocity = (player.transform.position - transform.position).normalized * speed;

        // Flip sprite to look towards player
        if((transform.position.x > player.transform.position.x && !flipped)
                || transform.position.x < player.transform.position.x && flipped) {

            flipped = !flipped;
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }

    }
}
 