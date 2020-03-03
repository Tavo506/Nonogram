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

        generarNonograma();

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
                casilla.transform.position = new Vector2(i * 0.30f, j * -0.30f);
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
        for(int i = 0; i < x; i++) //Rellena filas
        {
            if (filas[i].ToString().Split(',').Length == 1)
            {
                if(int.Parse(filas[i].ToString()) == y)
                {
                    for(int j = 0; j < y; j++)
                    {
                        changeSprite(cubitos[i, j]);
                        matriz[i, j] = 1;
                        Debug.Log(i + "," + j);
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
                    for (int j = 0; j < y; j++)
                    {
                        if(pistas.Contains(j)) {
                            matriz[i, j] = 0;
                        }
                        else
                        {
                            changeSprite(cubitos[i, j]);
                            matriz[i, j] = 1;
                            Debug.Log(i + "," + j);
                        }
                    }
                }
            }

        }
        for (int i = 0; i < y; i++) //Rellena columnas
        {
            if (columnas[i].ToString().Split(',').Length == 1)
            {
                if (int.Parse(columnas[i].ToString()) == x)
                {
                    for (int j = 0; j < x; j++)
                    {
                        changeSprite(cubitos[j, i]);
                        Debug.Log(i + " " + j);
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
