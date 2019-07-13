/*Script para que el fondo este en continuo movimiento.*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollFondo : MonoBehaviour {

    float velocidad = -0.8f;
    Vector3 posInicial;

	// Use this for initialization
	void Start () {
        posInicial = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        float pos = Mathf.Repeat(Time.time * velocidad, 12.3f);
        transform.position = posInicial + Vector3.up * pos;
	}
}
