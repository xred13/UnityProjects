using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UI_MainMenu : MonoBehaviour {

    public GameObject Upgrade;
    private Animator upgrade_Animator;

    public GameObject mainPanel;
    private Animator mainPanel_Animator;

    // Use this for initialization
    void Start() {
        upgrade_Animator = Upgrade.GetComponent<Animator>();
        mainPanel_Animator = mainPanel.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(mainPanel_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !mainPanel_Animator.IsInTransition(0))
        {
            StartUpgradeAnim();
        }
    }
    void StartUpgradeAnim()
    {
        upgrade_Animator.enabled = true;
    }
    void StopUpgradeAnim()
    {
        upgrade_Animator.enabled = false;
    }
    
}
