using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGame : MonoBehaviour
{
    bool started = false;
    float t;
    public GameObject title;
    CanvasGroup titleCanvasGroup;
    public GameObject tutorial;
    CanvasGroup tutorialCanvasGroup;
    public AudioSource yahoo;
    

    // enable player gun script once game is started
    public GameObject player;
    // enable stats ui once game is started
    public GameObject stats;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 0;
        Camera.main.orthographicSize = 0.0001f;
        t = 0.0001f;
        titleCanvasGroup = title.GetComponent<CanvasGroup>();
        tutorialCanvasGroup = tutorial.GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        if (started) {
            if (t < 1) {
                Camera.main.orthographicSize = Mathf.Lerp(0.0001f, 5.5f, t/1);
                titleCanvasGroup.alpha = Mathf.Lerp(1, 0, t/1);
                t += Time.unscaledDeltaTime;
            } else if (t >= 1) {
                started = false; // stop from running this if statement again
                Camera.main.orthographicSize = 5.5f;
                Time.timeScale = 1;
                title.SetActive(false);
                stats.SetActive(true);
                player.GetComponent<Gun>().enabled = true;
                StartCoroutine(showTutorial());
            }
        }
    }

    IEnumerator showTutorial() {
        tutorial.SetActive(true);
        // show tutorial for 9 seconds
        yield return new WaitForSeconds(9);
        // fade away (1 seconds)
        for (float i = 0; i < 1; i += Time.deltaTime) {
            tutorialCanvasGroup.alpha = Mathf.Lerp(1, 0, i/2);
            yield return null;
        }
        tutorialCanvasGroup.alpha = 0;
        yield return null;
        tutorial.SetActive(false);
        // disable this script
        this.enabled = false;
    }

    public void startGame() {
        yahoo.Play();
        started = true;
    }
}
