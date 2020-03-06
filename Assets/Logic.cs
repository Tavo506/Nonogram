using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    int[,] matriz;
    GameObject[,] cubitos;
    int x, y;

    bool resuelto = false;
    public Sprite state1, state2;
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
        cubitos = new GameObject[x, y];

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

    void changeSprite(GameObject cuadro)
    {
        cuadro.GetComponent<SpriteRenderer>().sprite = state2;
    }


    public void resolverNonograma()
    {
        //Filas----------------------------------------------------
        for(int i = 0; i < x; i++) //Rellena filas
        {
            if (filas[i].ToString().Split(',').Length == 1)
            {
                int pista = int.Parse(filas[i].ToString());
                if (pista == y) //Si cubre toda la fila
                {
                    for(int j = 0; j < y; j++)
                    {
                        changeSprite(cubitos[i, j]);
                        matriz[i, j] = 1;
                        //Debug.Log(i + "," + j);
                    }
                }
                else if (y / 2 < pista) //Si hay parte fija en el medio
                {
                    for (int n = 1, m = y - 2; n < m; n++, m--)
                    {
                        Debug.Log(i + "," + n);
                        if (n + pista >= x)
                        {
                            changeSprite(cubitos[i, n]);
                            changeSprite(cubitos[i, m]);
                            matriz[i, n] = 1;
                            matriz[i, m] = 1;
                        }
                    }
                }

            }
            else{
                int total = 0;
                ArrayList pistas = new ArrayList();
                foreach(string num in filas[i].ToString().Replace(" ", "").Split(','))
                {
                    total += int.Parse(num);
                    pistas.Add(int.Parse(num));
                }
                if(total + (filas[i].ToString().Split(',').Length - 1) == y)
                {
                    for (int j = 0, k = 0; j < y; j++, k++)
                    {
                        if(pistas.Count != 0 && pistas[0].ToString() == k.ToString()) {
                            matriz[i, j] = 0;
                            k = -1;
                            pistas.RemoveAt(0);
                        }
                        else
                        {
                            changeSprite(cubitos[i, j]);
                            matriz[i, j] = 1;
                            //Debug.Log(i + "," + j);
                        }
                    }
                }

            }

        }

        //Columnas--------------------------------------------------------
        for (int i = 0; i < y; i++) //Rellena columnas
        {
            if (columnas[i].ToString().Split(',').Length == 1)
            {
                int pista = int.Parse(columnas[i].ToString());
                if (pista == x)
                {
                    for (int j = 0; j < x; j++)
                    {
                        changeSprite(cubitos[j, i]);
                        matriz[j, i] = 0;
                        //Debug.Log(i + " " + j);
                    }
                }
                else if (x/2 < pista) { 
                    for(int n = 1, m = x-2; n < m; n++, m--)
                    {
                        if(n + pista >= y)
                        {
                            changeSprite(cubitos[n, i]);
                            changeSprite(cubitos[m, i]);
                            matriz[n, i] = 1;
                            matriz[m, i] = 1;
                        }
                    }
                }
            }
            else
            {
                int total = 0;
                ArrayList pistas = new ArrayList();
                foreach (string num in columnas[i].ToString().Replace(" ", "").Split(','))
                {
                    total += int.Parse(num);
                    pistas.Add(int.Parse(num));
                }
                if (total + (columnas[i].ToString().Split(',').Length - 1) == y)
                {
                    for (int j = 0, k = 0; j < y; j++, k++)
                    {
                        if (pistas.Count != 0 && pistas[0].ToString() == k.ToString())
                        {
                            matriz[j, i] = 0;
                            k = -1;
                            pistas.RemoveAt(0);
                        }
                        else
                        {
                            changeSprite(cubitos[j, i]);
                            matriz[j, i] = 1;
                            //Debug.Log(i + "," + j);
                        }
                    }
                }
            }
        }
        // while (!resuelto)
        {
            //foreach(string fila in filas)
            {
                
            }
        }

        Debug.Log("Nonograma Resuelto");
    }

}
