using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Buttons_MainMenus : MonoBehaviour {
    public Button Settings;
    public Button Audio;
    public Button Restore;
	// Use this for initialization
	void Start () {
        Settings.onClick.AddListener(OpenGuide);
	}

    // Update is called once per frame
    void Update()
    {

    }
    void OpenGuide()
    {
        SceneManager.LoadScene("Guide");
    }
}
