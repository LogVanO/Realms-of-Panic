using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerState : MonoBehaviour
{
    public int hp = 200;
    public int maxHp = 200;
    public int level = 1;
    public int xp = 0;
    public int maxXp = 50;
    public AudioSource Ouch;
    public AudioSource Ding;
    public AudioSource Game_music;
    public AudioSource Standoff_music;

    [SerializeField]
    float maxXPModifier = 1.2f;

    SpriteRenderer playerSprite;
    Rigidbody2D playerBody;
    Animator playerAnimator;
    public playerControl controlScript;
    public Weapon weapon;

    // death variables
    bool dead;
    Vector3 deathPos;
    public GameObject gameOverScreen;
    public LevelUpSystem levelUp;
    public Standoff standoff;


    // ui text and health bar
    public Canvas ui;
    TextMeshProUGUI healthText, healthTextShadow;
    TextMeshProUGUI levelText, levelTextShadow;
    TextMeshProUGUI xpText, xpTextShadow;
    LineRenderer healthBar, healthBarBase;
    Vector3 barStart, barEnd;


    // Start is called before the first frame update
    void Start()
    {
        playerSprite = gameObject.GetComponentInParent<SpriteRenderer>();
        playerBody = gameObject.GetComponentInParent<Rigidbody2D>();
        playerAnimator = gameObject.GetComponentInParent<Animator>();
        levelUp = FindObjectOfType<LevelUpSystem>();
        weapon = gameObject.GetComponent<Weapon>();
        dead = false;
        level = 1;

        // get ui elements to update
        healthTextShadow = ui.GetComponentsInChildren<TextMeshProUGUI>()[0];
        healthText = ui.GetComponentsInChildren<TextMeshProUGUI>()[1];
        levelTextShadow = ui.GetComponentsInChildren<TextMeshProUGUI>()[2];
        levelText = ui.GetComponentsInChildren<TextMeshProUGUI>()[3];
        xpTextShadow = ui.GetComponentsInChildren<TextMeshProUGUI>()[4];
        xpText = ui.GetComponentsInChildren<TextMeshProUGUI>()[5];
        healthBarBase = ui.GetComponentsInChildren<LineRenderer>()[2];
        healthBar = ui.GetComponentsInChildren<LineRenderer>()[3];
    }

    // Update is called once per frame
    void Update()
    {
        // move health bar with player
        // update health bar position
        barStart = new Vector3(Camera.main.transform.position.x-0.4f, Camera.main.transform.position.y-0.7f, -1);
        barEnd = new Vector3(Camera.main.transform.position.x+0.4f, Camera.main.transform.position.y-0.7f, -1);
        // place health bar base
        healthBarBase.SetPosition(0, barStart);
        healthBarBase.SetPosition(1, barEnd);
        // update positions for health bar
        barStart.y += 0.02f;
        barStart.z = -2;
        barEnd.y += 0.02f;
        barEnd.z = -2;
        // position reload bar as a fraction of cooldown time remaining
        healthBar.SetPosition(0, barStart);
        healthBar.SetPosition(1, Vector3.Lerp(barStart, barEnd, (float)hp/maxHp));
        
        // update level based on xp
        if (xp >= maxXp) {
            levelUp.ChooseReward();
            level++;
            xp = 0;
            maxXp  = (int)(maxXPModifier * maxXp);
            // update ui
            levelTextShadow.text = "Level: "+level;
            levelText.text = "Level: "+level;
            xpTextShadow.text = "XP: "+xp+"/"+maxXp;
            xpText.text = "XP: "+xp+"/"+maxXp;
        }

        // animate player falling when dead
        if (dead) {
            if (playerBody.rotation < 90) {
                playerBody.transform.RotateAround(deathPos, Vector3.forward, 90*Time.deltaTime);
            }
        }
    }

    // What happens when player takes damage
    public void TakeDamage(int damage) {
        hp -= damage;
        if (hp < 0)
            hp = 0;
        if(hp > 0) {
            StartCoroutine(HitIndication()); // see below
        } else if (!dead) { // only trigger death sequence once
            StartCoroutine(DeathSequence());
        }
        // update ui
        healthTextShadow.text = "Health:\n"+hp+"/"+maxHp;
        healthText.text = "Health:\n"+hp+"/"+maxHp;
    }

    public void Heal(float amount) {
        hp = (int)(amount + hp);
        if(hp > maxHp)
            hp = maxHp;
        // update ui
        healthTextShadow.text = "Health:\n"+hp+"/"+maxHp;
        healthText.text = "Health:\n"+hp+"/"+maxHp;
    }

    public void UpgradeMaxHp(float amount) {
        maxHp = (int)(amount * maxHp);
        // update ui
        healthTextShadow.text = "Health:\n"+hp+"/"+maxHp;
        healthText.text = "Health:\n"+hp+"/"+maxHp;
    }

    // Damages player when touching an enemy
    void OnCollisionStay2D(Collision2D other) {
        GameObject collided = other.gameObject;
        if(collided.tag != "Enemy")
            return;

        EnemyState enemy = collided.GetComponentInChildren<EnemyState>();

        if(enemy != null && enemy.hitDelay >= enemy.maxHitDelay) {
            if (enemy.damage > 0) { // enemy can have 0 damage when out of health
                TakeDamage(enemy.damage);
                enemy.hitDelay = 0;
            }
        }
    }

    // pick up xp on collision enter
    void OnTriggerEnter2D(Collider2D other) {
        GameObject collided = other.gameObject;
        if((collided.tag != "XP") && (collided.tag != "Portal"))
            return;

        if(collided.tag == "XP")
        {
            Ding.Play();
            xp += 10;
            // update ui
            xpTextShadow.text = "XP: "+xp+"/"+maxXp;
            xpText.text = "XP: "+xp+"/"+maxXp;
        
            Destroy(collided);
        }

        if (collided.tag == "Portal")
        {
            Standoff_music.Play();
            Destroy(collided);
            standoff.Ready();
        }
    }

    // indication that the player is being damaged.
    IEnumerator HitIndication() {
        // play sound
        Ouch.Play();
        playerSprite.color = new Color32(255, 100, 100, 255);

        yield return new WaitForSeconds(0.25f);

        if (hp > 0) {
            // return to normal color
            playerSprite.color = Color.white;
        } else {
            // death color
            playerSprite.color = new Color32(123, 123, 123, 255);
        }
    }

    // Play animation of player dying, then put up death screen
    IEnumerator DeathSequence() {
        // play sound
        Ouch.Play();
        // get position at player's feet for rotation axis
        deathPos = new Vector3(playerBody.position.x, playerBody.transform.position.y - 0.5f, 0);
        // trigger falling animation in update()
        dead = true;
        // disable animator
        playerAnimator.enabled = false;
        // disable player controls
        controlScript.enabled = false;
        // stop player body from being pushed by enemies
        playerBody.constraints = RigidbodyConstraints2D.FreezeAll;
        // disable health bar
        healthBarBase.enabled = false;
        healthBar.enabled = false;

        Game_music.Stop();
        // show game over screen
        gameOverScreen.SetActive(true);

        yield return null;
    }
}
