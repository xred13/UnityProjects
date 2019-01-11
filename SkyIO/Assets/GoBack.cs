using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GoBack : MonoBehaviour {

    bool played = false;
    Animator anim;
	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}

    // Update is called once per frame
    void Update()
    {
        if (played && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
        {
            GoToMainMenu();
        }
        if (Input.GetMouseButtonUp(0))
        {
            PlayAnimation();            
        }
    }
    void PlayAnimation()
    {
        anim.SetBool("Enter",false);
        played = true;
    }
    void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenuUI");
    }
}
