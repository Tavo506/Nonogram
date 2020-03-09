﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    int[,] matriz;
    GameObject[,] cubitos;
    int x, y;

    bool resuelto = false;
    public Sprite state1, state2, state3;
    private GameObject casilla;
    ArrayList filas = new ArrayList();
    ArrayList columnas = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        string[] nonogram = System.IO.File.ReadAllLines(@"Assets\Nonogram.txt");

        x = int.Parse(nonogram[0].Split(',')[0]);
        y = int.Parse(nonogram[0].Split(',')[1].Replace(" ", ""));

        

        matriz = new int[x, y];

        for(int n = 0; n < x; n++)
        {
            for(int m = 0; m < y; m++)
            {
                matriz[n, m] = -1;
            }
        }

        cubitos = new GameObject[x, y];

        //Debug.Log("DIMENSIONES: " + x + ", " + y);

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
        /*
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
        */
        generarNonograma();
        transform.position = new Vector2(-x/7.2f, y/7.2f);
        resolverNonograma();
    }

    void generarNonograma()
    {
        for(int i = 0; i < y; i++)
        {
            for(int j = 0; j < x; j++)
            {
                casilla = new GameObject("Casilla " + j + ", " + i);
                casilla.transform.SetParent(this.transform);
                casilla.AddComponent<SpriteRenderer>();
                casilla.GetComponent<SpriteRenderer>().sprite = state1;
                casilla.transform.position = new Vector2(i * 0.33f, j * -0.33f);
                cubitos[j, i] = casilla;
            }
        }
    }

    int cuantoshayfila(int x){
        int n=0;
        for(int i=0; i!=y; i++)
        {
            //Debug.Log(matriz[x,i]);
            if (matriz[x,i] == 1)
            {
                n++;
            }
        }
        //Debug.Log(n);
        return n;

    }

    int cuantoshaycolumna(int z)
    {
        int n = 0;
        for (int i = 0; i != x; i++)
        {
            if (matriz[i, z] == 1)
            {
                n++;
            }
        }
        //Debug.Log(n);
        return n;

    }

    void changeSprite(GameObject cuadro, Sprite estado)
    {

        cuadro.GetComponent<SpriteRenderer>().sprite = estado;
    }


    public void resolverNonograma()
    {

        revisarPistasNulas();

        //Filas----------------------------------------------------
        for (int i = 0; i < x; i++) //Rellena filas
        {
            if (filas[i].ToString() == "0") continue;   //Si es una pista vacia se ignora

            if (filas[i].ToString().Split(',').Length == 1) //Si es una sola pista...
            {
                int pista = int.Parse(filas[i].ToString());
                if (pista == y)         //Si cubre toda la fila
                {
                    for (int j = 0; j < y; j++)
                    {
                        changeSprite(cubitos[i, j], state2);
                        matriz[i, j] = 1;
                        
                        //Debug.Log(i + "," + j);
                    }
                    filas[i] = "0";
                }
                else if (y / 2 < pista) //Si hay parte fija en el medio
                {
                    for (int n = 1, m = y - 2; n < m; n++, m--)
                    {
                        //Debug.Log(i + "," + n);
                        if (n + pista >= x)
                        {
                            changeSprite(cubitos[i, n], state2);
                            changeSprite(cubitos[i, m], state2);
                            matriz[i, n] = 1;
                            matriz[i, m] = 1;
                        }
                    }
                }

            }

            else {  //Si es mas de una pista...
                int total = 0;
                ArrayList pistas = new ArrayList();
                foreach (string num in filas[i].ToString().Replace(" ", "").Split(',')) //Suma las pistas para verificar si cubriran toda la fila
                {
                    total += int.Parse(num);
                    pistas.Add(int.Parse(num));
                }
                if (total + pistas.Count - 1 == y)   //Si cubre toda la fila con espacios vacios pero fijos lo completa
                {
                    for (int j = 0, k = 0; j < y; j++, k++)
                    {
                        if (pistas.Count != 0 && pistas[0].ToString() == k.ToString()) {    //Encuentra el espacio y lo salta
                            matriz[i, j] = 0;
                            changeSprite(cubitos[i, j], state1);
                            k = -1;
                            pistas.RemoveAt(0);
                            changeSprite(cubitos[i, j], state3);
                        }
                        else
                        {
                            changeSprite(cubitos[i, j], state2);
                            matriz[i, j] = 1;
                            //Debug.Log(i + "," + j);
                        }
                    }
                    filas[i] = "0";
                }

            }

        }

        //Columnas--------------------------------------------------------
        for (int i = 0; i < y; i++) //Rellena columnas
        {
            if (columnas[i].ToString() == "0") continue;     //Si es una pista vacia se ignora

            if (columnas[i].ToString().Split(',').Length == 1)  //Si es una sola pista...
            {
                int pista = int.Parse(columnas[i].ToString());
                if (pista == x)  //Si cubre toda la columna
                {
                    for (int j = 0; j < x; j++)
                    {
                        changeSprite(cubitos[j, i], state2);
                        matriz[j, i] = 1;
                        //Debug.Log(i + " " + j);
                    }
                    columnas[i] = "0";
                }
                else if (x / 2 < pista) {   //Si hay parte fija en el medio
                    for (int n = 1, m = x - 2; n < m; n++, m--)
                    {
                        if (n + pista >= y)
                        {
                            changeSprite(cubitos[n, i], state2);
                            changeSprite(cubitos[m, i], state2);
                            matriz[n, i] = 1;
                            matriz[m, i] = 1;
                        }
                    }
                }
            }

            else{    //Si es mas de una pista
                int total = 0;
                ArrayList pistas = new ArrayList();
                foreach (string num in columnas[i].ToString().Replace(" ", "").Split(','))  //Suma las pistas para verificar si cubriran toda la fila
                {
                    total += int.Parse(num);
                    pistas.Add(int.Parse(num));
                }
                if (total + pistas.Count - 1 == y)    //Si cubre toda la columna con espacios vacios pero fijos lo completa
                {
                    for (int j = 0, k = 0; j < y; j++, k++)
                    {
                        if (pistas.Count != 0 && pistas[0].ToString() == k.ToString())  //Encuentra el espacio y lo salta
                        {
                            matriz[j, i] = 0;
                            k = -1;
                            pistas.RemoveAt(0);
                            changeSprite(cubitos[j, i], state3);
                        }
                        else
                        {
                            changeSprite(cubitos[j, i], state2);
                            matriz[j, i] = 1;
                            //Debug.Log(i + "," + j);
                        }
                    }
                    columnas[i] = "0";
                }
            }
        }

        verificarCompletos();
        completarBordesFilas();
        completarBordesColumnas();
        verificarCompletos();
        verificarNulos();
        revisar();

        // while (!resuelto)
        {
            //foreach(string fila in filas)
            {
                
            }
        }
        
        Debug.Log("Nonograma Resuelto");
    }

    void completarBordesFilas() //Revisa si se pueden completar pistas que ya inicien en un extremo
    {
        for(int i = 0; i < y; i++)
        {
            string[] col = columnas[i].ToString().Split(',');
            if (matriz[0,i] == 1)
            {
                if (int.Parse(col[0]) > 1)
                {
                    rellenaColumnas(1, i, int.Parse(col[0]), 1);
                }
            }
            if(matriz[x-1, i] == 1)
            {
                if (int.Parse(col[col.Length-1]) > 1)
                {
                    rellenaColumnas(x-int.Parse(col[col.Length-1]), i, x-1, 1);
                }
            }
        }
    }

    void rellenaColumnas(int inicio, int colum, int lim, int x) //Rellena la matriz en cierto intervalo con 1 o 0 en la columna
    {
        for(int i = inicio; i < lim; i++)
        {
            if (matriz[i, colum] == -1)
            {
                matriz[i, colum] = x;
                if(x == 1)
                    changeSprite(cubitos[i, colum], state2);
                else if (x == 0)
                    changeSprite(cubitos[i, colum], state3);
            }
        }
    }

    void completarBordesColumnas()  //Completa a partir de los bordes filas que empiezan en los bordes
    {
        for (int i = 0; i < x; i++)
        {
            string[] fil = filas[i].ToString().Split(',');
            if (matriz[i, 0] == 1)
            {
                if (int.Parse(fil[0]) > 1)
                {
                    rellenaFilas(1, i, int.Parse(fil[0]), 1);
                }
            }
            if (matriz[i, y-1] == 1)
            {
                if (int.Parse(fil[fil.Length - 1]) > 1)
                {
                    rellenaFilas(x - int.Parse(fil[fil.Length - 1]), i, y - 1, 1);
                }
            }
        }
    }
    void rellenaFilas(int inicio, int fila, int lim, int x) //Rellena la matriz en cierto intervalo con 1 o 0 en la fila
    {
        for (int i = inicio; i < lim; i++)
        {
            if (matriz[fila, i] == -1)
            {
                matriz[fila, i] = x;
                if(x == 1)
                    changeSprite(cubitos[fila, i], state2);
                else if(x == 0)
                    changeSprite(cubitos[fila, i], state3);
            }
        }
    }

    void revisarPistasNulas()   //Revisa si hay pistas nulas y pone en 0 toda la fila o columna
    {
        for(int i = 0; i < x; i++)
        {
            if(filas[i].ToString() == "0")
            {
                rellenaColumnas(0, i, y, 0);
            }
            if(columnas[i].ToString() == "0")
            {
                rellenaFilas(0, i, x, 0);
            }
        }
    }

    void verificarCompletos()   //Recorre la matriz para ver si las pistas han sido completadas y eliminarlas
    {
        for (int conta_filas = 0; conta_filas < x - 1; conta_filas++)
        {
            //if (filas[conta_filas].ToString() != "0")
            {
                int total = 0;
                foreach (string num in filas[conta_filas].ToString().Replace(" ", "").Split(','))
                {
                    total += int.Parse(num);
                }

                Debug.Log(total + "-");
                Debug.Log(cuantoshayfila(conta_filas));
                if (total == cuantoshayfila(conta_filas))
                {
                    filas[conta_filas] = "0";
                    rellenaFilas(0, conta_filas, y, 0);
                }
            }
        }

        for (int conta_columnas = 0; conta_columnas < y - 1; conta_columnas++)
        {
            if (columnas[conta_columnas].ToString() != "0")
            {
                int total = 0;
                foreach (string num in columnas[conta_columnas].ToString().Replace(" ", "").Split(','))
                {
                    total += int.Parse(num);
                }

                if (total == cuantoshaycolumna(conta_columnas))
                {
                    columnas[conta_columnas] = "0";
                    rellenaColumnas(0, conta_columnas, x, 0);
                }
            }
        }
    }


    void verificarNulos()
    {

    }

    void revisar()
    {

    }
}
