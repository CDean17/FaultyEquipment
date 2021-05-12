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

    public void UpdateSlot(int slotNum, Sprite img)
    {
        switch (slotNum)
        {
            case 0:
                slot1.sprite = img;
                slot1.color = Color.white;
                break;
            case 1:
                slot2.sprite = img;
                slot2.color = Color.white;
                break;
            case 3:
                slot3.sprite = img;
                slot3.color = Color.white;
                break;
        }
    }

    public void ClearSlot(int slotNum)
    {
        slot1.color = Color.clear;
    }
}
