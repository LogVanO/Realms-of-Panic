using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Gun : MonoBehaviour, Weapon
{
    public int damage = 100;
    public float cooldownMax = 1;
    public float cooldown;
    public int piercing = 0;
    public AudioSource Bang;
    public AudioSource Click;

    public Material shotPathMaterial;

    // ui text and reload bar
    public Canvas ui;
    TextMeshProUGUI damageText, damageTextShadow;
    TextMeshProUGUI piercingText, piercingTextShadow;
    TextMeshProUGUI cooldownText, cooldownTextShadow;
    LineRenderer reloadBar, reloadBarBase;
    Vector3 barStart, barEnd;

    void Start() {
        cooldown = 0;
        // get ui elements to update
        damageTextShadow = ui.GetComponentsInChildren<TextMeshProUGUI>()[8];
        damageText = ui.GetComponentsInChildren<TextMeshProUGUI>()[9];
        piercingTextShadow = ui.GetComponentsInChildren<TextMeshProUGUI>()[10];
        piercingText = ui.GetComponentsInChildren<TextMeshProUGUI>()[11];
        cooldownTextShadow = ui.GetComponentsInChildren<TextMeshProUGUI>()[12];
        cooldownText = ui.GetComponentsInChildren<TextMeshProUGUI>()[13];
        reloadBarBase = ui.GetComponentsInChildren<LineRenderer>()[0];
        reloadBar = ui.GetComponentsInChildren<LineRenderer>()[1];
        reloadBarBase.enabled = false;
        reloadBar.enabled = false;
    }

    void Update() {
        // update cooldown
        if (cooldown > 0) {
            cooldown -= Time.deltaTime;
            // update cooldown bar
            barStart = new Vector3(Camera.main.transform.position.x-0.4f, Camera.main.transform.position.y-0.82f, -1);
            barEnd = new Vector3(Camera.main.transform.position.x+0.4f, Camera.main.transform.position.y-0.82f, -1);
            // place reload bar base
            reloadBarBase.SetPosition(0, barStart);
            reloadBarBase.SetPosition(1, barEnd);
            // update positions for reload bar
            barStart.y += 0.02f;
            barStart.z = -2;
            barEnd.y += 0.02f;
            barEnd.z = -2;
            // position reload bar as a fraction of cooldown time remaining
            reloadBar.SetPosition(0, barStart);
            reloadBar.SetPosition(1, Vector3.Lerp(barStart, barEnd, 1-cooldown/cooldownMax));
            // enable reload bar
            reloadBarBase.enabled = true;
            reloadBar.enabled = true;
        } else {
            // hide reload bar
            reloadBarBase.enabled = false;
            reloadBar.enabled = false;
        }
    }

    public void shoot(Vector2 shotDirection) {
        StartCoroutine(ShootWeapon(shotDirection));
    }

    public void UpgradeDamage(float scale) {
        damage = (int)((float)damage * scale);
        // update ui
        damageTextShadow.text = "Damage: "+damage;
        damageText.text = "Damage: "+damage;
    }

    public void UpgradeCooldown(float scale) {
        cooldownMax *= scale;
        // update ui
        cooldownTextShadow.text = "Cooldown: "+cooldownMax.ToString("0.00")+"s";
        cooldownText.text = "Cooldown: "+cooldownMax.ToString("0.00")+"s";
    }

    public void UpgradePiercing(float amount) {
        piercing += (int)amount;
        // update ui
        piercingTextShadow.text = "Piercing: "+piercing;
        piercingText.text = "Piercing: "+piercing;
    }

    IEnumerator ShootWeapon(Vector2 direction) {
        // only shoot if cooldown == 0
        if (cooldown <= 0) {
            // reset cooldown
            cooldown = cooldownMax;
            Bang.Play();

            int pierced = piercing;
            int peircingDamage = damage;

            // set start point slightly below center of player body
            Vector2 startPoint = new Vector2(transform.position.x, transform.position.y-0.1f);
            // scale direction to length of 15
            float scalar = (Mathf.Sqrt((direction.x*direction.x + direction.y*direction.y)/225));
            Vector2 endPoint = new Vector2(direction.x/scalar, direction.y/scalar);
            // animate bullet
            StartCoroutine(animateBullet(startPoint, endPoint));

            // get all enemeies in path from player in direction of mouse click
            RaycastHit2D[] hits = Physics2D.RaycastAll(startPoint, endPoint);

            // damage eah enemy hit
            foreach (RaycastHit2D hit in hits) {
                if (hit.transform.gameObject.tag == "Enemy" && pierced >= 0) {
                    // get enemy state
                    EnemyState enemy = hit.transform.gameObject.GetComponentInChildren<EnemyState>();

                    // damage enemy
                    enemy.hp -= peircingDamage;

                    // after each enemy hit, modify damege based on peircing
                    peircingDamage -= 10;
                    pierced--;
                    yield return null;
                }
            }
        }
        else
        {
            Click.Play();
        }
    }

    IEnumerator animateBullet(Vector2 startPoint, Vector2 endPoint) {

        // create line renderer for bullet path
        LineRenderer bulletPath = gameObject.AddComponent<LineRenderer>();
        bulletPath.material = shotPathMaterial;
        bulletPath.startColor = Color.yellow;
        bulletPath.endColor = Color.black;
        bulletPath.startWidth = 0.06f;
        bulletPath.endWidth = 0.06f;
        bulletPath.positionCount = 2;
        bulletPath.useWorldSpace = true;    

        // animate bullet path
        Vector3 Pos1 = new Vector3(startPoint.x, startPoint.y, -1);
        Vector3 Pos2 = new Vector3(startPoint.x + endPoint.x*0.1f, startPoint.y + endPoint.y*0.1f, -1);
        Vector3 endPos1 = new Vector3(startPoint.x + endPoint.x, startPoint.y + endPoint.y, -1);
        Vector3 endPos2 = new Vector3(startPoint.x + endPoint.x + endPoint.x*0.1f, startPoint.y + endPoint.y + endPoint.y*0.1f, -1);
        for (float shotTime = 0; shotTime < 0.15f; shotTime += Time.deltaTime) {
            bulletPath.SetPosition(0, Vector3.Lerp(Pos1, endPos1, shotTime/0.15f));
            bulletPath.SetPosition(1, Vector3.Lerp(Pos2, endPos2, shotTime/0.15f));
            yield return null;
        }
        // destroy line renderer
        Destroy(bulletPath);
        yield return null;
    }

}
