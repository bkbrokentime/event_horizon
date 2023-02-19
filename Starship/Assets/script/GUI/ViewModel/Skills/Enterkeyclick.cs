using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ViewModel.Skills;

public class Enterkeyclick : MonoBehaviour
{
    // Start is called before the first frame update

    public GameObject window;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            window.GetComponent<SkillTree>().UnlockButtonClicked();
        }
    }
}
