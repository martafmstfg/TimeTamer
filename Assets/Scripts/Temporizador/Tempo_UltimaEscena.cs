/* Clase auxiliar estatica que almacena los datos de una sesion del temporizador
 * (minutos y monedas obtenidas) para poder repetirla directamente.
 */ 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tempo_UltimaEscena  {

    private static string escenaAnterior = "";
    private static float minutos = 0;
    private static int monedas = 0;

    public static string GetEscenaAnterior () {
        return escenaAnterior;
    }

    public static void SetEscenaAnterior (string escena) {
        escenaAnterior = escena;
    }

    public static float GetMinutos() {
        return minutos;
    }

    public static void SetMinutos (float min) {
        minutos = min;
    }

    public static int GetMonedas() {
        return monedas;
    }

    public static void SetMonedas(int m) {
        monedas = m;
    }

}
