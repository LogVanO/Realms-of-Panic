using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class playerControl : MonoBehaviour
{
    // player RigidBody2D
    Rigidbody2D playerBody;
    //Rigidbody2D cam;
    public Camera cam;

    // player movement
    [SerializeField]
    float moveSpeed = 4;
    float inputLR;
    float inputUD;

    bool facingRight = true;

    Animator animator;
    SpriteRenderer spriteRenderer;

    // player weapon controls
    public Weapon weapon;
    Vector2 shotDirection;

    // ui text
    public Canvas ui;
    TextMeshProUGUI speedText, speedTextShadow;

    // Start is called before the first frame update
    void Start()
    {
        playerBody = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        weapon = gameObject.GetComponent<Weapon>();
        // get ui elements to update
        speedTextShadow = ui.GetComponentsInChildren<TextMeshProUGUI>()[6];
        speedText = ui.GetComponentsInChildren<TextMeshProUGUI>()[7];
    }

    // Update is called once per frame
    void Update()
    {
        // get keyboard directional input
        inputLR = Input.GetAxisRaw("Horizontal");
        inputUD = Input.GetAxisRaw("Vertical");

        // track player with camera
        cam.transform.position = new Vector3(transform.position.x, transform.position.y, cam.transform.position.z);

        // detect mouse click for shooting weapon
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown("space")) {
            shotDirection = new Vector2(Input.mousePosition.x - Screen.width/2, Input.mousePosition.y - Screen.height/2);
            weapon.shoot(shotDirection);
        }
    }

    void FixedUpdate()
    {
        if (inputLR != 0 || inputUD != 0)
        {
            // slow movement if diagonal
            if (inputLR != 0 && inputUD != 0)
            {
                inputLR *= 1/Mathf.Sqrt(2);
                inputUD *= 1/Mathf.Sqrt(2);
            }
            // move player
            playerBody.velocity = new Vector2(inputLR*moveSpeed, inputUD*moveSpeed);

            animator.SetBool("isWalking", true);

            // face player the proper direction
            if((inputLR < 0 && facingRight) || (inputLR > 0 && !facingRight)) {
                facingRight = !facingRight;
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
        }
        else
        {
            // stop moving
            playerBody.velocity = new Vector2(0, 0);
            animator.SetBool("isWalking", false);
        }
    }

    public void UpgradeMoveSpeed(float scale) {
        moveSpeed *= scale;
        // update ui
        speedTextShadow.text = "Speed: "+(moveSpeed/4).ToString("0.00")+"x";
        speedText.text = "Speed: "+(moveSpeed/4).ToString("0.00")+"x";
    }
}
