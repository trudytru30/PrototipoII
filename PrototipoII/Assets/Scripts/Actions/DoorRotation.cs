using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DoorRotation : MonoBehaviour
{

    public Transform jugador;
    public string idLlave; 
    public float distanciaParaAbrir = 2.0f;
    public float rotacion = -90f;
    public float velocidad = 2f;

    private bool abierta = false;
    private bool enMovimiento = false;

    void Update()
    {
        if (jugador == null) return;


        if (!string.IsNullOrEmpty(idLlave))
        {
            GameObject llave = GameObject.Find(idLlave);
            if (llave != null && llave.activeSelf) return;
        }
        if (Vector3.Distance(transform.position, jugador.position) < distanciaParaAbrir && !abierta && !enMovimiento)
        {
            StartCoroutine(AbrirPuerta());
        }
    }

    IEnumerator AbrirPuerta()
    {
        enMovimiento = true;
        Quaternion rotacionInicial = transform.rotation;
        Quaternion rotacionFinal = Quaternion.Euler(0, rotacion, 0) * rotacionInicial;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * velocidad;
            transform.rotation = Quaternion.Lerp(rotacionInicial, rotacionFinal, t);
            yield return null;
        }
        abierta = true;
        enMovimiento = false;
    }
}