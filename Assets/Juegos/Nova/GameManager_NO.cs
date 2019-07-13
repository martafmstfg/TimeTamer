/* Script que gestiona la logica del juego: generacion, movimiento y fusion de estrellas.
 * Agregado al GameObject GameManager.
 */

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameManager_NO : MonoBehaviour {

    //Lista de celdas (inyectada)
    public List<Celda> listaCeldas;
    //Matriz
    public Celda[,] celdas = new Celda[4, 4];
    //Lista de celdas vacias
    public List<Celda> celdasVacias;

    //DEPENDENCIAS UI
    //Canvas
    [Inject(Id = "canvasTransform")]
    public RectTransform canvas;
    //Puntuacion
    private int puntos;
    [Inject(Id = "puntosTxt")]
    private Text puntosTxt;
    //Tiempo
    private float segundosTotales = 60.0f * 3.0f;
    [Inject(Id = "tiempoTxt")]
    private Text tiempoTxt;

    //Audio (inyectado)
    private static AudioSource audioSourceFX;
    public AudioClip[] sonidos;

    private bool gameOver = false;

    [Inject]
    public void Init(List<Celda> listaCeldas, AudioSource audioSource) {
        this.listaCeldas = listaCeldas;
        audioSourceFX = audioSource;
    }

    // Use this for initialization
    void Start () {

        //Ordenar la lista de celdas (la inyeccion no asegura el orden)
        listaCeldas.Sort((c1, c2) => c1.id.CompareTo(c2.id));

        //Vacia las celdas, las mete en la matriz y en la lista de celdas vacias
		foreach (Celda c in listaCeldas) {
            c.TipoEstrella = 0;

            celdas[c.fila, c.columna] = c;

            celdasVacias.Add(c); 
        }

        //Genera dos estrellas al comienzo del juego
        GenerarEstrella();
        GenerarEstrella();
	}

    //Actualiza en la UI el tiempo de la partida en forma de cuenta atras
    void FixedUpdate() {

        if(!gameOver) {
            //Calcular el tiempo restante utilizando el tiempo que ha pasado  
            // desde que se cargo la escena
            float t = segundosTotales - Time.timeSinceLevelLoad; 

            //Calcular minutos y segundos, y generar el string MM:SS
            int min = ((int)t / 60);
            string minutos;
            if (min < 10) {
                minutos = "0" + min.ToString();
            }
            else {
                minutos = min.ToString();
            }

            int sec = ((int)t % 60);
            string segundos;
            if (sec < 10) {
                segundos = "0" + sec.ToString();
            }
            else {
                segundos = sec.ToString();
            }

            //Actualizar el tiempo en la interfaz
            tiempoTxt.text = minutos + ":" + segundos;

            //Cuando el tiempo restante llega a 0, se acaba la partida
            if (t <= 0) {
                FinJuego();
            }
        }        
    }

    /* El InputManager llama a este metodo con el id de la direccion en la que se ha deslizado el dedo
    * Este metodo recorre la lista de celdas (empezando por un extremo u otro, dependiendo de la direccion
    * en la que se haya deslizado el dedo) para comprobar si se puede fusionar o mover a la celda adyacente
    */
    public void Mover (int dir) {
        int idActual, idAdyacente;
        bool fusionada, continuar;
        bool movimiento = false;

        //Impedir el movimiento una vez ha terminado el juego
        if(!gameOver) {
            switch (dir) {
                //Arriba
                case 0:
                    //Comienza desde la celda 4 porque las 4 primeras no se pueden mover hacia arriba
                    for (int i = 4; i < listaCeldas.Count; i++) {
                        fusionada = false;
                        continuar = true;

                        //Si la celda actual no esta vacia
                        if (listaCeldas[i].TipoEstrella != 0) {
                            idActual = i;
                            idAdyacente = listaCeldas[idActual].idArriba;

                            //Mientras tenga una celda justo encima y se pueda continuar
                            while (idAdyacente != -1 && continuar) {
                                movimiento = true;

                                //Si la casilla adyacente tiene una estrella del mismo tipo
                                // y si aun no se ha fusionado en este turno
                                if (!fusionada && idAdyacente != -1 &&
                                    listaCeldas[idActual].TipoEstrella == listaCeldas[idAdyacente].TipoEstrella) {
                                    Fusionar(idActual, idAdyacente); //Fusionar las estrellas
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido hacia arriba
                                    idAdyacente = listaCeldas[idActual].idArriba; //Tras haberse movido, busca a su nueva adyacente por arriba
                                    fusionada = true;
                                }

                                //Despues de fusionar, intenta mover a la celda superior
                                // si no esta en la fila de arriba y si la celda esta vacia
                                if (idAdyacente != -1 && listaCeldas[idAdyacente].TipoEstrella == 0) {
                                    Intercambiar(idActual, idAdyacente); //Se puede mover a esa celda vacia
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido hacia arriba
                                    idAdyacente = listaCeldas[idActual].idArriba; //Busca a su nueva adyacente
                                }
                                else {
                                    //Si no se puede mover porque la adyacente esta ocupada (o no hay adyacente) se sale del bucle
                                    continuar = false;
                                }

                            }
                        }
                    }

                    break;

                //Abajo
                case 1:
                    //Empieza por la tercera fila porque son las ultimas que tienen adyacente debajo
                    for (int i = 11; i >= 0; i--) {
                        fusionada = false;
                        continuar = true;

                        //Si la celda actual no esta vacia
                        if (listaCeldas[i].TipoEstrella != 0) {
                            idActual = i;
                            idAdyacente = listaCeldas[idActual].idAbajo;

                            //Mientras tenga una celda justo debajo y se pueda continuar
                            while (idAdyacente != -1 && continuar) {
                                movimiento = true;

                                //Si la casilla adyacente tiene una estrella del mismo tipo
                                // y si aun no se ha fusionado en este turno
                                if (!fusionada && idAdyacente != -1 &&
                                    listaCeldas[idActual].TipoEstrella == listaCeldas[idAdyacente].TipoEstrella) {
                                    Fusionar(idActual, idAdyacente); //Fusiona las estrellas
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido hacia abajo
                                    idAdyacente = listaCeldas[idActual].idAbajo; //Tras moverse, busca su nueva adyacente
                                    fusionada = true;
                                }

                                //Despues de fusionar, intenta mover a la celda inferior
                                // si no esta en la fila de abajo y si la celda esta vacia
                                if (idAdyacente != -1 && listaCeldas[idAdyacente].TipoEstrella == 0) {
                                    Intercambiar(idActual, idAdyacente); //Se puede mover a esa celda vacia
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido hacia abajo
                                    idAdyacente = listaCeldas[idActual].idAbajo; // Tras moverse, busca su nueva adyacente
                                }
                                else {
                                    //Si no se puede mover porque la adyacente esta ocupada (o no hay adyacente) se sale del bucle
                                    continuar = false;
                                }

                            }
                        }
                    }

                    break;

                //Izquierda
                case 2:                    
                    for (int i = 0; i < listaCeldas.Count; i++) {
                        fusionada = false;
                        continuar = true;

                        //Si la celda actual no esta vacia y no esta en la columna 0
                        if (listaCeldas[i].columna != 0 && listaCeldas[i].TipoEstrella != 0) {
                            idActual = i;
                            idAdyacente = listaCeldas[idActual].idIzquierda;

                            //Mientras tenga una celda justo a su izquierda y se pueda continuar        
                            while (idAdyacente != -1 && continuar) {
                                movimiento = true;

                                //Si la casilla adyacente tiene una estrella del mismo tipo
                                // y si aun no se ha fusionado en este turno
                                if (!fusionada && idAdyacente != -1 &&
                                    listaCeldas[idActual].TipoEstrella == listaCeldas[idAdyacente].TipoEstrella) {
                                    Fusionar(idActual, idAdyacente); //Fusionar estrellas
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido a la izquierda
                                    idAdyacente = listaCeldas[idActual].idIzquierda; //Tras moverse, busca su nueva adyacente izquierda
                                    fusionada = true;
                                }

                                //Despues de fusionar, intenta mover a la celda izquierda
                                // si no esta en la columna 0 y si la celda esta vacia
                                if (idAdyacente != -1 && listaCeldas[idAdyacente].TipoEstrella == 0) {
                                    Intercambiar(idActual, idAdyacente); //Puede moverse a la casilla vacia
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido a la izquierda
                                    idAdyacente = listaCeldas[idActual].idIzquierda; //Tras moverse, busca su nueva adyacente izquierda
                                }
                                else {
                                    //Si no se puede mover porque la adyacente esta ocupada (o no hay adyacente) se sale del bucle
                                    continuar = false;
                                }

                            }
                        }
                    }

                    break;

                //Derecha
                case 3:
                    //Empieza desde la penultima porque es la ultima que tiene una celda adyacente a la derecha
                    for (int i = listaCeldas.Count - 2; i >= 0; i--) {
                        fusionada = false;
                        continuar = true;

                        //Si la celda actual no esta vacia y no esta en la columna 3
                        if (listaCeldas[i].columna != 3 && listaCeldas[i].TipoEstrella != 0) {
                            idActual = i;
                            idAdyacente = listaCeldas[idActual].idDerecha;

                            //Mientras tenga una celda justo a su derecha y se pueda continuar        
                            while (idAdyacente != -1 && continuar) {
                                movimiento = true;

                                //Si la casilla adyacente tiene una estrella del mismo tipo
                                // y si aun no se ha fusionado en este turno
                                if (!fusionada && idAdyacente != -1 &&
                                    listaCeldas[idActual].TipoEstrella == listaCeldas[idAdyacente].TipoEstrella) {
                                    Fusionar(idActual, idAdyacente); //Fusionar estrellas
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido a la derecha
                                    idAdyacente = listaCeldas[idActual].idDerecha; //Tras moverse busca su nueva adyacente derecha
                                    fusionada = true;
                                }

                                //Despues de fusionar, intenta mover a la celda derecha
                                // si no esta en la columna 3 y si la celda esta vacia
                                if (idAdyacente != -1 && listaCeldas[idAdyacente].TipoEstrella == 0) {
                                    Intercambiar(idActual, idAdyacente); //Se puede mover a la casilla vacia
                                    idActual = idAdyacente; //La estrella con la que se estaba operando se ha movido a la izquierda
                                    idAdyacente = listaCeldas[idActual].idDerecha; //Tras moverse busca su nueva adyacente derecha
                                }
                                else {
                                    //Si no se puede mover porque la adyacente esta ocupada (o no hay adyacente) se sale del bucle
                                    continuar = false;
                                }

                            }
                        }
                    }

                    break;
            }

            //Si se ha conseguido hacer un movimiento, se genera una nueva estrella
            if (movimiento) GenerarEstrella();
        }        
    }

    //Generar una estrella de tipo 1 o 2 en una casilla vacia aleatoria
    public void GenerarEstrella () {
        //Comprobar que quedan casillas vacias
        if(celdasVacias.Count > 0) {
            int id = Random.Range(0, celdasVacias.Count);
            int rand = Random.Range(0, 10);
            int tipo = (rand == 0) ? 2 : 1;            
            celdasVacias[id].TipoEstrella = tipo;
            celdasVacias[id].TriggerAnimacion("Generar");
            celdasVacias.RemoveAt(id); //Eliminar la celda de la lista de celdas vacias
        } 
        //Si no quedan casillas vacias, es el fin del juego
        else {
            FinJuego();
        }
    }

    //Una estrella se mueve de su celda a una celda adyacente que esta vacia
    //La celda ocupada por esa estrella pasa entonces a estar vacia
    public void Intercambiar (int idActual, int idAdyacente) {
        //La adyacente (anteriormente vacia) ahora tiene una estrella del tipo de la celda actual
        listaCeldas[idAdyacente].TipoEstrella = listaCeldas[idActual].TipoEstrella;
        //Poner el tipo de una estrella a 0 equivale a vaciar su celda
        listaCeldas[idActual].TipoEstrella = 0;

        //La adyacente (anteriormente vacia) ahora esta ocupada --> Quitar de la lista
        celdasVacias.Remove(listaCeldas[idAdyacente]);
        //La actual (anteriormente ocupada) ahora esta vacia --> Añadir a la lista
        celdasVacias.Add(listaCeldas[idActual]);
    }

    //Dos celdas del mismo nivel se fusionan en una de un nivel mayor
    public void Fusionar(int idActual, int idAdyacente) {
        int nuevoTipo = listaCeldas[idActual].TipoEstrella + 1; //Calcula el nuevo nivel/tipo de estrella
        //De momento no se puede pasar del nivel 7
        if (nuevoTipo < 8) {
            //La celda adyacente es la que va a contenet la nueva estrella
            listaCeldas[idAdyacente].TipoEstrella = nuevoTipo;
            listaCeldas[idAdyacente].TriggerAnimacion("Fusionar");
            //La celda actual es la que se va a vaciar
            listaCeldas[idActual].TipoEstrella = 0;
            PlaySound(0);
            SumarPuntos(nuevoTipo);

            //La adyacente (ocupada) sigue ocupada --> No estaba en la lista
            //La actual (anteriormente ocupada) ahora esta vacia --> Añadir a la lista
            celdasVacias.Add(listaCeldas[idActual]);
        }        
    }

    //Incrementa los puntos y actualiza la interfaz
    private void SumarPuntos (int p) {
        puntos += p;
        puntosTxt.text = puntos.ToString();
    }

    //Al terminar la partida, se muestra el pop-up con los puntos totales conseguidos
    public void FinJuego () {
        gameOver = true;
        StopMusic();

        var PopUpJuego = Instantiate(Resources.Load("Prefabs/PopUpJuego_Puntos") as GameObject, canvas);

        var PPPanel = PopUpJuego.transform.Find("PanelPopUp");
        PPPanel.transform.Find("TituloPopUp").GetComponent<Text>().text = "¡Fin del juego!";

        //Info del popup            
        PPPanel.transform.Find("TotalPopUp").GetComponent<Text>().text = puntos.ToString();

        //Record
        int recordActual = PlayerPrefs.GetInt("RecordNova", 0);
        if (puntos > recordActual) {
            PlayerPrefs.SetInt("RecordNova", puntos);
            PPPanel.transform.Find("RecordPopUp").GetComponent<Text>().text = "¡Nuevo récord!";

            //Al superar el record, comprobar si tambien se ha alcanzado el logro
            //Guardar nuevo numero de puntos
            PlayerPrefs.SetFloat("PuntosNova", puntos);
            //Comprobar si se ha completado el logro nova (id = 7)
            LogrosManager.ComprobarLogroCompletado(7);
        }
        else {
            PPPanel.transform.Find("RecordPopUp").GetComponent<Text>().text = "Record: " + recordActual;
        }
    }

    //AUDIO
    //Parar musica de fondo
    public void StopMusic() {
        GameObject.Find("Musica").GetComponent<AudioSource>().Stop();
    }

    //Reproducir el sonido cuyo indice se recibe como argumento
    public void PlaySound(int i) {
        audioSourceFX.PlayOneShot(sonidos[i], 1.5f);
    }
}
