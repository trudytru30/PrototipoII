using System;
using System.Collections.Generic;
using System.Numerics;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class AmmoVisual : MonoBehaviour
{
    [SerializeField] private Sprite ammoSprite;
    [SerializeField] private int ammoCount; // Replace later with actual player ammo count
    [SerializeField] private int maxCount; // Replace later with actual player ammo count

    private List<AmmoImage> ammoImageList;

    private void Awake()
    {
        ammoImageList = new List<AmmoImage>();
    }

    private void Start()
    {
        AddAmmo();
    }

    private void AddAmmo()
    {
        int xPos = -850;   
        for (int i = 0; i < ammoCount; i++)
        {
            CreateAmmoImage(new Vector2(xPos, -475));
            xPos += 20;
        }
    }
    
    private AmmoImage CreateAmmoImage(Vector2 position)
    {
        // Create GameObject
        GameObject ammoGameObject = new GameObject("Ammo", typeof(Image));
        
        // Set as child
        ammoGameObject.transform.parent = transform;
        ammoGameObject.transform.localPosition = Vector3.zero;

        // Set position
        ammoGameObject.GetComponent<RectTransform>().anchoredPosition = position;
        ammoGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(10, 10);
        
        // Set ammo sprite
        Image ammoImageHud = ammoGameObject.GetComponent<Image>();
        ammoImageHud.sprite = ammoSprite;
        
        // Add to list
        AmmoImage ammoImage = new AmmoImage(ammoImageHud);
        ammoImageList.Add(ammoImage);
        
        return ammoImage;
    }
    
    // Represents single ammo
    public class AmmoImage
    {
        private Image ammoImage;
        private GameObject ammoObject;

        public AmmoImage(Image ammoImage)
        {
            this.ammoImage = ammoImage;
        }

        public void SetVisible(bool isVisible)
        {
            ammoImage.enabled = isVisible;
        }
        
    }

    public void RemoveAmmo()
    {
        AmmoImage ammoImage = ammoImageList[ammoCount - 1];
        ammoImage.SetVisible(false);
       
        ammoCount--;
        
        if (ammoCount <= 0)
        {
            ammoCount = maxCount;
            
            for (int i = ammoCount - 1; i >= 0; i--)
            {
                ammoImage = ammoImageList[i];
                ammoImage.SetVisible(true);
            }
            
        }

    }

}
