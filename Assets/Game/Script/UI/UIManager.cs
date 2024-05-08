using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject menu;
    [SerializeField] private GameObject ingame;
    [SerializeField] private ScrollList scrollList;
    
    public void CloseShop() {
        shop.transform.DOMoveY(-1080, 1);
        scrollList.UnLoad();
    }

    public void OpenMenu() {
        menu.SetActive(true);
        menu.transform.DOMoveY(540, 1);
    }

    public void CloseMenu() {
        menu.transform.DOMoveY(1620, 1);
    }

    public void OpenShop() {
        shop.SetActive(true);
        shop.transform.DOMoveY(540, 1);
    }
    
    public void OpenInGame() {
        ingame.SetActive(true);
        ingame.transform.DOMoveY(540, 1);
    }

    public void CloseInGame() {
        ingame.transform.DOMoveY(1620, 1);
    }
}
