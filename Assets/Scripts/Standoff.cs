using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;

public class Standoff : MonoBehaviour
{
    public System.Random rnd = new System.Random();
    // disable player control script while selecting upgrade
    public GameObject player;
    public PlayerState playerState;
    public GameObject fire;
    public GameObject next;
    public GameObject screenReady;
    public GameObject screenWin;
    public GameObject screenLose;
    public GameObject now;
    public LevelUpSystem levelUp;
    public AudioSource eagle;
    public AudioSource bang;
    public AudioSource glass;
    public AudioSource zap;
    public bool shot = false;
    public bool window = false;
    public bool win = false;

    void Start()
    {

    }

    void Update()
    {
        
    }

    async public void Ready()
    {
        player.GetComponent<playerControl>().enabled = false;
        shot = false;
        window = false;
        win = false;
        Time.timeScale = 0;
        screenReady.SetActive(true);
        fire.SetActive(true);
        await Task.Delay(rnd.Next(4000, 6000));
        if (!shot)
        {
            eagle.Play();
            now.SetActive(true);
            window = true;
            await Task.Delay(rnd.Next(250, 500));
            now.SetActive(false);
            window = false;
            if (!shot)
            {
                LoseScreen();
            }
        }
    }

    async public void Fire()
    {
        bang.Play();
        fire.SetActive(false);
        now.SetActive(false);
        shot = true;
        if (window)
        {
            WinScreen();
        }
        else
        {
            LoseScreen();
        }
    }

    async public void WinScreen()
    {
        glass.Play();
        fire.SetActive(false);
        screenWin.SetActive(true);
        screenReady.SetActive(false);
        win = true;
        await Task.Delay(2000);
        next.SetActive(true);
    }

    async public void LoseScreen()
    {
        zap.Play();
        fire.SetActive(false);
        screenLose.SetActive(true);
        screenReady.SetActive(false);
        await Task.Delay(2000);
        next.SetActive(true);
    }

    public void ResumeGame() {
        screenWin.SetActive(false);
        screenLose.SetActive(false);
        next.SetActive(false);
        Time.timeScale = 1;
        // enable player controls (shooting)
        player.GetComponent<playerControl>().enabled = true;
        if (win)
        {
            levelUp.ChooseReward();
        }
        else {
            playerState.TakeDamage(25);
        }
    }
}
