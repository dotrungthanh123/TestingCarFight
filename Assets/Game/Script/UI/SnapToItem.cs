using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SnapToItem : MonoBehaviour
{
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private RectTransform contentPanel;
    [SerializeField] private RectTransform sampleListItem;

    [SerializeField] private GridLayoutGroup LG;

    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private String itemName;

    [SerializeField] private float snapForce;
    private float snapSpeed;

    private bool isSnapped;

    private void Start() {
        isSnapped = false;
    }

    private void Update() {
        // The content panel starts at 0
        // everytime it moves to the left by 'sampleListItem.rect.width + HLG.spacing' amount
        // the next item is precisely fit into the center
        // 'contentPanel.localPosition.x / (sampleListItem.rect.width + HLG.spacing)' means how many times it has moved to the left of that amount
        // meaning how many items have been scrolled through
        // therefore giving the index of the next item in the scroll list
        
        // The negative sign is because moving to the left decrease the x value making it negative
        int currentItem = Mathf.RoundToInt(-contentPanel.localPosition.x / (sampleListItem.rect.width + LG.spacing.x));

        if (scrollRect.velocity.magnitude < 200 && !isSnapped) {
            scrollRect.velocity = Vector2.zero;

            snapSpeed += snapForce * Time.deltaTime;

            contentPanel.localPosition = new Vector3(
                Mathf.MoveTowards(contentPanel.localPosition.x, -currentItem * (sampleListItem.rect.width + LG.spacing.x), snapSpeed), 
                contentPanel.localPosition.y, 
                contentPanel.localPosition.z);

            if (contentPanel.localPosition.x == -currentItem * (sampleListItem.rect.width + LG.spacing.x)) {
                isSnapped = true;
            }
        }

        if (scrollRect.velocity.magnitude > 200) {
            isSnapped = false;
        }
    }
}
