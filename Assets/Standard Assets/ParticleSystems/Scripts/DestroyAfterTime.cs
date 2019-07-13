using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour {

	public float destroyTime = 2.0f;

	// Use this for initialization
	void Start () {
		//Despues del tiempo establecido, ejecuta el metodo die
		Invoke ("Die", destroyTime);
	}

	//Destruye el propio objeto
	void Die () {
		Destroy (gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
