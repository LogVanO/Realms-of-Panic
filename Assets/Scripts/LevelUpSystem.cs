using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpSystem : MonoBehaviour
{
    [SerializeField]
    List<Upgrade> upgrades;

    [SerializeField]
    GameObject overlay;

    Button[] buttons = new Button[3];
    TextMeshProUGUI[] names = new TextMeshProUGUI[3];
    TextMeshProUGUI[] descriptions = new TextMeshProUGUI[3];
    Image[] images = new Image[3];
    Upgrade[] currentUpgrades = new Upgrade[3];

    public AudioSource Yahoo;

    // disable player control script while selecting upgrade
    public GameObject player;

    void Start()
    {
        Component[] buttonList = GetComponentsInChildren<Button>(true);
        buttons[0] = (Button)buttonList[0];
        buttons[1] = (Button)buttonList[1];
        buttons[2] = (Button)buttonList[2];

        Component[] text1 = buttons[0].GetComponentsInChildren<TextMeshProUGUI>(true);
        names[0] = (TextMeshProUGUI)text1[0];
        descriptions[0] = (TextMeshProUGUI)text1[1];

        Component[] text2 = buttons[1].GetComponentsInChildren<TextMeshProUGUI>(true);
        names[1] = (TextMeshProUGUI)text2[0];
        descriptions[1] = (TextMeshProUGUI)text2[1];

        Component[] text3 = buttons[2].GetComponentsInChildren<TextMeshProUGUI>(true);
        names[2] = (TextMeshProUGUI)text3[0];
        descriptions[2] = (TextMeshProUGUI)text3[1];

        images[0] = buttons[0].GetComponentsInChildren<Image>()[1];
        images[1] = buttons[1].GetComponentsInChildren<Image>()[1];
        images[2] = buttons[2].GetComponentsInChildren<Image>()[1];
    }

    void Update()
    {
        
    }

    public void ChooseReward() {
        // disable player control (shooting) while selecting upgrade
        player.GetComponent<playerControl>().enabled = false;

        Time.timeScale = 0;

        // remove upgrade functions from the buttons from the previous level-up
        for(int i = 0; i < 3; i++) {
            buttons[i].gameObject.SetActive(true);

            if(currentUpgrades[i] != null){
                upgrades.Add(currentUpgrades[i]);
                buttons[i].onClick.RemoveListener(currentUpgrades[i].PerfomUpgrade);
                currentUpgrades[i] = null;
            }
        }

        // Remove maxed out upgrades from the list of available upgrades
        /*foreach (Upgrade u in upgrades) {
            if(u.maxed)
                upgrades.Remove(u);
        }*/
        List<Upgrade> marked = new List<Upgrade>();
        for (int u = 0; u < upgrades.Count; u++) {
            if(upgrades[u].maxed)
                marked.Add(upgrades[u]);
        }

        foreach(Upgrade u in marked) {
            upgrades.Remove(u);
        }

        // get three unique upgrades 
        for(int i = 0; i < 3; i++) {
            if(upgrades.Count > 0) {
                currentUpgrades[i] = upgrades[Random.Range(0, upgrades.Count)];
                upgrades.Remove(currentUpgrades[i]);
                buttons[i].onClick.AddListener(currentUpgrades[i].PerfomUpgrade);
                names[i].text = currentUpgrades[i].name;
                descriptions[i].text = currentUpgrades[i].desc;
                images[i].sprite = currentUpgrades[i].image;

            }
            else {
                buttons[i].gameObject.SetActive(false);
            }
        }
        
        overlay.SetActive(true);
    }
    

    public void ResumeGame() {
        overlay.SetActive(false);
        Time.timeScale = 1;
        Yahoo.Play();
        // enable player controls (shooting)
        player.GetComponent<playerControl>().enabled = true;
    }

}
