using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "newWeapon", menuName = "ScriptableObjects/WeaponScriptableObject", order = 1)]
public class WeaponScriptableObjects : ScriptableObject
{
    public string weaponName;
    public GameObject weaponObj;
    public GameObject weaponPickupObj;
    public Sprite weaponImage;


}
