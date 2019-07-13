/* Script para girar al personaje arrastrando con el dedo.
 * Agregado al gameobject Personaje
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateChar : MonoBehaviour {

    float rotSpeed = 300;

    private void OnMouseDrag() {
        float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad;

        transform.Rotate(Vector3.up, -rotX);
    }
}
