using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthTick : MonoBehaviour
{
    public Sprite filledTick;
    public Sprite emptyTick;

    UnityEngine.UI.Image tickImage; 

    public void SetFilled(bool filled)
    {
        if (tickImage == null)
            tickImage = GetComponent<UnityEngine.UI.Image>();

        tickImage.sprite = filled ? filledTick : emptyTick;
    }
}
