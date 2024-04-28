using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarItem : MonoBehaviour
{
    public RawImage image;
    public TextMeshProUGUI disabledCost, normalCost, highlightedCost;

    public void Init(Texture texture, int cost) {
        image.texture = texture;
        disabledCost.SetText(cost.ToString());
        normalCost.SetText(cost.ToString());
        highlightedCost.SetText(cost.ToString());
    }
}
