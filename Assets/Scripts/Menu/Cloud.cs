using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public int speed;

    void Update()
    {
        GetComponent<RectTransform>().anchoredPosition = GetComponent<RectTransform>().anchoredPosition + Vector2.right * Time.deltaTime * speed;
        if (GetComponent<RectTransform>().anchoredPosition.x >= 1500) GetComponent<RectTransform>().anchoredPosition = new Vector2(-2340, 0);
    }
}
