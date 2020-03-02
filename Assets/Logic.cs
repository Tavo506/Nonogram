using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logic : MonoBehaviour
{
    int[,] matriz;
    GameObject[,] cubitos;
    public Sprite state1, state2;
    private GameObject casilla;
    ArrayList filas = new ArrayList();
    ArrayList columnas = new ArrayList();

    // Start is called before the first frame update
    void Start()
    {
        string[] nonogram = System.IO.File.ReadAllLines(@"Assets\Nonogram.txt");

        int x = int.Parse(nonogram[0].Split(',')[0]);
        int y = int.Parse(nonogram[0].Split(',')[1].Replace(" ", ""));

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

        generarNonograma(x, y);

        changeSprite(cubitos[1, 1]);
    }

    void generarNonograma(int x, int y)
    {
        for(int i = 0; i < y; i++)
        {
            for(int j = 0; j < x; j++)
            {
                casilla = new GameObject("Casilla " + i + ", " + j);
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

}
