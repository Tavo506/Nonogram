using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{

    ArrayList filas = new ArrayList();
    ArrayList columnas = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        string[] nonogram = System.IO.File.ReadAllLines(@"D:\Programas\Unity\Juegos\Nonogram\Assets\Nonogram.txt");

        int x = int.Parse(nonogram[0].Split(',')[0]);
        int y = int.Parse(nonogram[0].Split(',')[1].Replace(" ", ""));

        int[,] matriz = new int[x, y];

        Debug.Log("DIMENSIONES: " + x + ", " + y);

        int i = 2;

        while(nonogram[i] != "COLUMNAS")
        {
            filas.Add(nonogram[i]);
            i++;
        }

        i++;

        while(i < nonogram.Length){
            columnas.Add(nonogram[i]);
            i++;
        }

        Debug.Log("FILAS");

        foreach (string fila in filas)
        {

            Debug.Log("\t" + fila);
        }

        Debug.Log("COLUMNAS");

        foreach (string columna in columnas)
        {

            Debug.Log("\t" + columna);
        }
    }

}
