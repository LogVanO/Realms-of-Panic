using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyState : MonoBehaviour
{
    public EnemyData data;

    public int hp = 100;
    int lastHp;
    public int damage = 10;
    public int speed = 2;
    public float hitDelay = 1;
    public float maxHitDelay = 1;

    // enemy rigidbody
    Rigidbody2D enemyBody;
    // enemy sprite
    SpriteRenderer enemySprite;
    // enemy death position
    Vector3 deathPos;

    // xp prefab
    public GameObject xpPrefab;

    void Awake() {
        SetEnemyValues();
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyBody = gameObject.GetComponentInParent<Rigidbody2D>();
        enemySprite = gameObject.GetComponentInParent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hitDelay < maxHitDelay) {
            hitDelay += Time.deltaTime;
        }

        // check for damage taken
        if (hp < lastHp) {
            if(hp > 0) {
                StartCoroutine(HitIndication());
            } else {
                StartCoroutine(DeathSequence());
            }
        }

        // save last hp value for next update
        lastHp = hp;
    }

    IEnumerator HitIndication() {
        // set enemy sprite colour to red then back to normal
        enemySprite.color = new Color32(255, 100, 100, 255);

        yield return new WaitForSeconds(0.5f);

        enemySprite.color = Color.white;
    }

    // Play animation of player dying, then put up death screen
    IEnumerator DeathSequence() {
        // stop damage before object is destroyed
        damage = 0;
        // set enemy sprite colour to red and wait 0.1 seconds
        enemySprite.color = new Color32(255, 100, 100, 255);
        yield return new WaitForSeconds(0.15f);

        // get enemy position for dropping xp
        deathPos = new Vector3(enemyBody.position.x, enemyBody.position.y, 0);
        
        // place xp in deathpos (behind enemy)
        Instantiate(xpPrefab, deathPos, Quaternion.identity);
        yield return null;

        // maybe death effect (blood splatter sprite?)

        // destroy enemy object
        Destroy(gameObject.transform.parent.gameObject);

        yield return null;
    }

    private void SetEnemyValues() {
        hp = data.hp;
        gameObject.GetComponentInParent<EnemyBehavior>().speed = data.speed;
        damage = data.damage;
    }
}
