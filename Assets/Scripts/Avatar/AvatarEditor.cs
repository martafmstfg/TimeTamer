/* Script que muestra y oculta ciertas partes del avatar para cambiar su aspecto
 * segun las prendas que se seleccionen en la interfaz. 
 * Tambien carga las texturas correspondientes a las prendas elegidas
 * y contiene los metodos para cambiar los colores.
 * Agregado al gameobject AvatarEditor
 */

using UnityEngine;
using Zenject;

public class AvatarEditor : MonoBehaviour {

    //Prendas
    [Header("Prendas")]
    public GameObject camiseta, camisa, jersey;
    public GameObject largo, corto, falda1, falda2;
    public GameObject bajos, altos;

    //Partes del cuerpo
    [Header("Partes del cuerpo")]
    public GameObject cabeza;
    public GameObject pelo;
    public GameObject ojos, pupila;
    public GameObject brazos, cuerpo, cintura, piernas, pies;
    
    //Dependencias
    private DiccionarioTexturas diccionarioTexturas;
    private AvatarFileManager avatarFile;

    private string[] nuevosColores = new string[3]; //Guarda los colores utilizados actualmente

    [Inject]
    public void Init(DiccionarioTexturas dt, AvatarFileManager af) {
        diccionarioTexturas = dt;
        avatarFile = af;
    }

    //Metodo para cambiar la prenda de la parte de arriba del cuerpo
    //Recibe el tipo de prenda que se va a mostrar y el nombre de su textura
    public void MostrarArriba(string categoria, string textura) {

        //Buscar la textura en el diccionario por el nombre
        Texture tex = diccionarioTexturas.GetTextura(textura);

        if (categoria.Equals("camiseta")) {

            if (camiseta.activeSelf == false) {
                //Mostrar la parte de arriba indicada
                camisa.SetActive(false);
                jersey.SetActive(false);
                camiseta.SetActive(true);

                //Mostrar brazos
                brazos.SetActive(true);
                //Ocultar cuerpo
                cuerpo.SetActive(false);
            }
            
            //Cambiar la textura del material
            camiseta.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }
        else if (categoria.Equals("camisa")) {
            if (camisa.activeSelf == false) {
                //Mostrar la parte de arriba indicada
                camiseta.SetActive(false);
                jersey.SetActive(false);
                camisa.SetActive(true);

                //Ocultar brazos y cuerpo
                brazos.SetActive(false);
                cuerpo.SetActive(false);
            }

            //Cambiar la textura del material
            camisa.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }
        else if (categoria.Equals("jersey")) {
            if (jersey.activeSelf == false) {
                //Mostrar la parte de arriba indicada
                camisa.SetActive(false);
                camiseta.SetActive(false);
                jersey.SetActive(true);

                //Ocultar brazos y cuerpo
                brazos.SetActive(false);
                cuerpo.SetActive(false);
            }

            //Cambiar la textura del material
            jersey.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }

        //Guarda en memoria la textura del avatar para la parte de arriba
        avatarFile.GuardarTextura(0, textura);
    }

    //Metodo para cambiar la prenda de la parte de abajo del cuerpo
    //Recibe el tipo de prenda que se va a mostrar y el nombre de su textura
    public void MostrarAbajo(string categoria, string textura) {

        //Buscar la textura en el diccionario por el nombre
        Texture tex = diccionarioTexturas.GetTextura(textura);

        if (categoria.Equals("largo")) {

            if (largo.activeSelf == false) {
                //Mostrar la parte de abajo indicada
                corto.SetActive(false);
                falda1.SetActive(false);
                falda2.SetActive(false);
                largo.SetActive(true);

                //Ocultar piernas y cintura
                piernas.SetActive(false);
                cintura.SetActive(false);
            }

            //Cambiar la textura del material
            largo.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }
        else if (categoria.Equals("corto")) {
            if (corto.activeSelf == false) {
                //Mostrar la parte de abajo indicada
                largo.SetActive(false);
                falda1.SetActive(false);
                falda2.SetActive(false);
                corto.SetActive(true);

                //Mostrar piernas 
                piernas.SetActive(true);
            }

            //Cambiar la textura del material
            corto.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }
        else if (categoria.Equals("falda1")) {
            if (falda1.activeSelf == false) {
                //Mostrar la parte de abajo indicada
                largo.SetActive(false);
                corto.SetActive(false);
                falda2.SetActive(false);
                falda1.SetActive(true);

                //Mostrar piernas
                piernas.SetActive(true);
            }

            //Cambiar la textura del material
            falda1.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }
        else if (categoria.Equals("falda2")) {
            if (falda2.activeSelf == false) {
                //Mostrar la parte de abajo indicada
                largo.SetActive(false);
                corto.SetActive(false);
                falda1.SetActive(false);
                falda2.SetActive(true);

                //Mostrar piernas y cintura
                piernas.SetActive(true);
                cintura.SetActive(true);
            }

            //Cambiar la textura del material
            falda2.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }

        //Guarda en memoria la textura del avatar para la parte de abajo
        avatarFile.GuardarTextura(1, textura);
    }

    //Metodo para cambiar los zapatos
    //Recibe el tipo de zapatos que se van a mostrar y el nombre de su textura
    public void MostrarZapatos(string categoria, string textura) {

        //Buscar la textura en el diccionario por el nombre
        Texture tex = diccionarioTexturas.GetTextura(textura);

        if (categoria.Equals("bajos")) {

            if (bajos.activeSelf == false) {
                //Mostrar el modelo de zapatos indicado
                altos.SetActive(false);
                bajos.SetActive(true);

                //Mostrar pies
                pies.SetActive(true);
            }

            //Cambiar la textura del material
            bajos.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);

        }
        else if (categoria.Equals("altos")) {
            if (altos.activeSelf == false) {
                //Mostrar el modelo de zapatos indicado
                bajos.SetActive(false);
                altos.SetActive(true);

                //Ocultar pies
                pies.SetActive(false);
            }

            //Cambiar la textura del material
            altos.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);
        }

        //Guarda en memoria la textura del avatar para los zapatos
        avatarFile.GuardarTextura(2, textura);
    }

    //Metodo para cambiar el tipo de pelo
    //Recibe el nombre del modelo que hay que cargar
    public void MostrarPelo (string mesh) {

        if(mesh.Equals("calvo")) {
            pelo.SetActive(false);
        } else {
            //Cargar mesh
            GameObject peinado = Resources.Load("Peinados/" + mesh) as GameObject;
            Mesh nuevaMesh = peinado.GetComponentInChildren<MeshFilter>().sharedMesh;

            //Asignarla al objeto pelo
            pelo.GetComponent<MeshFilter>().sharedMesh = nuevaMesh;

            pelo.SetActive(true); //Asegurar que el objeto Pelo es visible
            avatarFile.GuardarPeinado(mesh); //Guardar en memoria el tipo de pelo activo
        }
    }

    //Metodo para cambiar el tipo de ojos
    public void MostrarOjos(string ojoBase, string color) {

        //Buscar y asignar la textura del ojo base
        Texture tex = diccionarioTexturas.GetTextura(ojoBase);
        ojos.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);

        //Buscar y asignar la textura de la pupila
        tex = diccionarioTexturas.GetTextura(color);
        pupila.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", tex);

        //Guardar las texturas en memoria
        avatarFile.GuardarTextura(3, ojoBase);
        avatarFile.GuardarTextura(4, color);
    }

    //Metodo para cambiar el color del material del elemento indicado
    public void CambiarColor (int i, Color color) {
        //Piel
        if (i == 0) {
            cabeza.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);
        }
        //Ojos
        else if (i == 1) {
            pupila.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);
        }
        //Pelo
        else if (i == 2) {
            pelo.GetComponent<Renderer>().sharedMaterial.SetColor("_Color", color);            
        }

        //Guardar el color en forma de string rgb
        nuevosColores[i] = ColorUtility.ToHtmlStringRGB(color);
    }

    //Metodo para guardar los colores actuales en memoria
    public void ConfirmarColores () {
        for (int i=0; i<nuevosColores.Length; i++) {
            if (nuevosColores[i] != null) {
                avatarFile.GuardarColor(i, nuevosColores[i]);
            }
        }
    }

    //Metodo para restaurar los colores anteriores si se oculta el selector de color
    // sin haber confirmado los colores
    public void RestaurarColores (bool t) {
        if(!t) avatarFile.CargarColores();
    }
}

