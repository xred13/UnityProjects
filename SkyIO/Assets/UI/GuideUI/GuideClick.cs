using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideClick : MonoBehaviour {

    bool open = true;
    Button button;
 
    public Animator anim;
    // Use this for initialization
    void Start () {

        button = GetComponent<Button>();
        button.onClick.AddListener(() => Clicking());
    }

    // Update is called once per frame
    void Update()
    {

    }
    void Clicking()
    {
        anim.SetBool("Enter", open);
        open = !open;
    }
}
