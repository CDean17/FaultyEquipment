using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{

    private Image slot1;
    private Image slot2;
    private Image slot3;
    private Image healthBar;
    public int UINum = 0;

    // Start is called before the first frame update
    void Start()
    {
        int playerNumber = GameObject.FindGameObjectsWithTag("Player").Length;

        if(playerNumber < UINum)
        {
            gameObject.SetActive(false);
        }

        slot1 = transform.Find("Slot1").GetComponent<Image>();
        slot2 = transform.Find("Slot2").GetComponent<Image>();
        slot3 = transform.Find("Slot3").GetComponent<Image>();
        healthBar = transform.Find("Healthbar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UIReset()
    {

    }

    public void UpdateHealthbar(float hDecimal)
    {
        healthBar.fillAmount = hDecimal;
    }
}
