using UnityEngine;
using UnityEngine.UI;

public class GrenadeUI : MonoBehaviour
{
    [SerializeField] private PlayerGrenadeLauncher grenadeLauncher;

    [Header("Referencias UI")]
    [SerializeField] private Image[] slotIcons;
    [SerializeField] private Image[] selectionBorders;

    [Header("Colores")]
    [SerializeField] private Color availableColor = Color.white;
    [SerializeField] private Color emptyColor = Color.black;

    private void Update()
    {
        if (grenadeLauncher == null) return;

        int selected = grenadeLauncher.GetSelectedSlot();

        for (int i = 0; i < slotIcons.Length; i++)
        {
            bool hasGrenade = grenadeLauncher.HasGrenadeInSlot(i);

            if (slotIcons[i] != null)
            {
                slotIcons[i].color = hasGrenade ? availableColor : emptyColor;
            }

            if (selectionBorders != null && i < selectionBorders.Length && selectionBorders[i] != null)
            {
                selectionBorders[i].enabled = (selected == i && hasGrenade);
            }
        }
    }
}
