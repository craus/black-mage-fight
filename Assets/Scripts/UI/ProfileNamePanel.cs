﻿using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ProfileNamePanel : MonoBehaviour
{   
    public Text defaultName;
    public InputField inputField;

    string GetName() {
        if (inputField.text != "") {
            return inputField.text;       
        }
        return defaultName.text;
    }

    public void Go() {
        GameManager.instance.gameState.CurrentProfile.name = GetName();
        GameManager.instance.Save();
        GameManager.instance.UpdateState();

    }

}
