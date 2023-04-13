using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryOpener : MonoBehaviour
{
    public GameObject Story;
    
    public void OpenStory()
    {
        if(Story != null)
        {
            bool isActive = Story.activeSelf;
            Story.SetActive(!isActive);
        }
    }
}
