using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;

public class Logic : MonoBehaviour
{
    int[,] matriz;
    GameObject[,] cubitos;
    int x, y;

    bool resuelto = false;
    public Sprite state1,   //Blanco
        state2, //Azul
        state3; //X
    private GameObject casilla;
    List<string[]> filas = new List<string[]>();
    List<string[]> columnas = new List<string[]>();



    // Start is called before the first frame update
    void Start()
    {
        string[] nonogram = System.IO.File.ReadAllLines(@"Assets\Nonogram.txt");

        x = int.Parse(nonogram[0].Split(',')[0]);
        y = int.Parse(nonogram[0].Split(',')[1].Replace(" ", ""));

        /*
         -1 = vacio
         0 = X
         1 = pintado
         */


        matriz = new int[x, y];

        for (int n = 0; n < x; n++)
        {
            for (int m = 0; m < y; m++)
            {
                matriz[n, m] = -1;
            }
        }

        cubitos = new GameObject[x, y];

        //Debug.Log("DIMENSIONES: " + x + ", " + y);

        int i = 2;
        while (nonogram[i] != "COLUMNAS")
        {
            string[] aux = nonogram[i].Replace(" ", "").Split(',');
            filas.Add(aux);
            i++;
        }

        i++;

        while (i < nonogram.Length)
        {
            string[] aux = nonogram[i].Replace(" ", "").Split(',');
            columnas.Add(aux);
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

        StartCoroutine(NonogramStart());

    }


    IEnumerator NonogramStart()
    {

        //yield return new WaitForSeconds(1f);
        generarNonograma();
        transform.position = new Vector2(-x / 7.2f, y / 7.2f);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(resolverNonograma(0.2f));
    }

    void generarNonograma()
    {
        for (int i = 0; i < y; i++)
        {
            for (int j = 0; j < x; j++)
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


    void changeSprite(GameObject cuadro, Sprite estado)
    {

        cuadro.GetComponent<SpriteRenderer>().sprite = estado;

        //cuadro.transform.name += " - " + estado.name;
    }

    void crearSolucion()
    {
        for(int i = 0; i < x; i++)
        {
            int indice = 0;
            for(int j = 0; j < y; j++)
            {

            }
        }
    }

    IEnumerator resolverNonograma(float time)
    {

        crearSolucion();

        yield return new WaitForSeconds(time);

    }


    bool VerificaFilas(int i)
    {
        string recorredor = crearStringFila(i);

        if (recorredor.Replace(",", "") == string.Join("", filas[i]))
        {
            return true;
        }
        else return false;
    }

    bool VerificaColumnas(int j)
    {
        string recorredor = crearStringColumna(j);

        if (recorredor.Replace(",", "") == string.Join("", columnas[j]))
        {
            return true;
        }
        else
            return false;
    }

    string crearStringFila(int i)
    {
        string recorredor = "";
        int n = 0;
        for (int conta = 0; conta < x; conta++)
        {
            if (matriz[i, conta] == 1 || matriz[i, conta] == 2)
            {
                n++;
            }
            if ((matriz[i, conta] == 0 || matriz[i, conta] == -1 || conta + 1 == x) && n != 0)
            {
                recorredor += n + ",";
                n = 0;
            }
        }
        if (recorredor.EndsWith(","))
            recorredor = recorredor.Remove(recorredor.Length-1);
        return recorredor;
    }

    string crearStringColumna(int j)
    {
        string recorredor = "";
        int n = 0;
        for (int conta = 0; conta < y; conta++)
        {
            if (matriz[conta, j] == 1 || matriz[conta, j] == 2)
            {
                n++;
            }
            if ((matriz[conta, j] == 0 || matriz[conta, j] == -1 || conta + 1 == y) && n != 0)
            {
                recorredor += n + ",";
                n = 0;
            }
        }
        if (recorredor.EndsWith(","))
            recorredor = recorredor.Remove(recorredor.Length - 1);
        return recorredor;
    }


    public void verPistas()
    {
        Debug.Log("FILAS");
        for (int i = 0; i < x; i++)
        {
            string f = i+1 + ": " + string.Join(", ", filas[i]);
            Debug.Log(f);
        }

        Debug.Log("COLUMNAS");
        for (int i = 0; i < y; i++)
        {
            string c = i+1 + ": " + string.Join(", ", columnas[i]);
            Debug.Log(c);
        }
    }
}
