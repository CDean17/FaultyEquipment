using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISlideTransitionScript : MonoBehaviour
{
    private RectTransform rt;
    private Vector3 originalPos;
    public Vector3 teleportPos;
    public float smoothTime = 1f;
    private Vector3 refVel = Vector3.zero;
    // Start is called before the first frame update
    void Start()
    {
        rt = gameObject.GetComponent<RectTransform>();
        originalPos = rt.anchoredPosition;
        rt.anchoredPosition = teleportPos;
    }

    private void Update()
    {
        rt.anchoredPosition = Vector3.SmoothDamp(rt.anchoredPosition, originalPos, ref refVel, smoothTime);
    }

    private void OnEnable()
    {
        originalPos = rt.anchoredPosition;
        rt.anchoredPosition = teleportPos;
    }
}
