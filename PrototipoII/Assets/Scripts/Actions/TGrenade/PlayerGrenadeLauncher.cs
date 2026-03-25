using UnityEngine;
using UnityEngine.InputSystem;
/*
 *COMO FUNCIONA:
 * Primero clickamos en el suelo como al disparar , luego seleccionamos 1,2,3 y le damos a la G
 * TODO:
 * lanzamiento?
 */
public class PlayerGrenadeLauncher : MonoBehaviour
{
   [Header("Referencias")]
    [SerializeField] private Camera cam;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private ActionController actionController;
    [SerializeField] private GameObject grenadePrefab;

    [Header("Lanzamiento")]
    [SerializeField] private float aimPlaneHeight = 0f;
    [SerializeField] private float throwForce = 12f;
    [SerializeField] private float upwardForce = 4f;
    //objetivo
    [SerializeField] private GameObject targetMarker;

    [Header("Slots")]
    [SerializeField] private bool[] grenadeSlots = new bool[3] { true, true, true };
    [SerializeField] private int selectedSlot = -1;

    private Vector3 currentAimPoint;

    private void Update()
    {
        UpdateAimPoint();
        UpdateTargetMarker();
        HandleSlotInput();
        HandleThrowInput();
    }

    private void UpdateAimPoint()//actualiza el punto de mira basado en la posición del mouse
    {
        if (cam == null) return;

        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());//crea un rayo desde la camara hacia el punto del mouse
        Plane groundPlane = new Plane(Vector3.up, new Vector3(0f, aimPlaneHeight, 0f));//plano horizontal a la altura del player

        if (groundPlane.Raycast(ray, out float enter))
        {
            currentAimPoint = ray.GetPoint(enter);//actualiza el punto de mira al punto donde el rayo intersecta el plano
        }
    }

    private void UpdateTargetMarker()//actualiza la posición del marcador de objetivo y su visibilidad
    {
        if (targetMarker == null) return;

        bool shouldShow = selectedSlot >= 0 && grenadeSlots[selectedSlot];

        targetMarker.SetActive(shouldShow);

        if (shouldShow)
        {
            targetMarker.transform.position = currentAimPoint + Vector3.up * 0.05f;//eleva un poco el marcador para evitar z-fighting con el suelo
        }
    }
    private void HandleSlotInput()//maneja la entrada para seleccionar los slots de granada
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
            SelectSlot(0);

        if (Keyboard.current.digit2Key.wasPressedThisFrame)
            SelectSlot(1);

        if (Keyboard.current.digit3Key.wasPressedThisFrame)
            SelectSlot(2);
    }

    private void HandleThrowInput()
    {
        if (Keyboard.current.gKey.wasPressedThisFrame)
        {
            ThrowSelectedGrenade();
        }
    }

    public void SelectSlot(int slotIndex)//selecciona un slot de granada si es válido y tiene una granada disponible
    {
        if (slotIndex < 0 || slotIndex >= grenadeSlots.Length) return;
        if (!grenadeSlots[slotIndex]) return;

        selectedSlot = slotIndex;
        Debug.Log("Granada seleccionada en slot: " + (slotIndex + 1));
    }

    public void ThrowSelectedGrenade()
    {
        if (selectedSlot < 0) return;
        if (!grenadeSlots[selectedSlot]) return;
        if (grenadePrefab == null || throwPoint == null) return;

        if (actionController != null && !actionController.CanUseUpperBody())
            return;

        GameObject grenadeObj = Instantiate(
            grenadePrefab,
            throwPoint.position,
            Quaternion.identity
        );

        GrenadeProjectile grenade = grenadeObj.GetComponent<GrenadeProjectile>();//intenta obtener el componente GrenadeProjectile del objeto instanciado
        if (grenade == null) return;

        Vector3 direction = (currentAimPoint - throwPoint.position).normalized;//calcula la dirección del lanzamiento hacia el punto de mira
        Vector3 velocity = direction * throwForce + Vector3.up * upwardForce;//calcula la velocidad inicial

        grenade.Launch(velocity);

        grenadeSlots[selectedSlot] = false;
        selectedSlot = -1;
    }

    public bool HasGrenadeInSlot(int slotIndex)//verifica si un slot específico tiene una granada disponible
    {
        if (slotIndex < 0 || slotIndex >= grenadeSlots.Length) return false;
        return grenadeSlots[slotIndex];
    }

    public int GetSelectedSlot()
    {
        return selectedSlot;
    }

    public int GetSlotCount()
    {
        return grenadeSlots.Length;
    }

    public Vector3 GetAimPoint()
    {
        return currentAimPoint;
    }
}
