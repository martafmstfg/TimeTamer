using UnityEngine;
using System.Collections;
using RotaryHeart.Lib.SerializableDictionary;

public class DiccionarioTexturas : MonoBehaviour {

    public TextureDictionary texturas;

    public Texture GetTextura (string nombre) {
        return texturas[nombre];
    }

}

//Clase auxiliar para crear un diccionario que se pueda editar desde el inspector
[System.Serializable]
public class TextureDictionary : SerializableDictionaryBase<string, Texture> {
}