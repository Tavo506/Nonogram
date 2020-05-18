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
        StartCoroutine(resolverNonograma());
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

        cuadro.transform.name += " - " + estado.name;
    }


    IEnumerator resolverNonograma()

    {

        revisarPistasNulas();

        //Filas----------------------------------------------------
        for (int i = 0; i < x; i++) //Rellena filas
        {
            yield return new WaitForSeconds(0.1f);
            if (filas[i][0] == "0") continue;   //Si es una pista vacia se ignora

            if (filas[i].Length == 1) //Si es una sola pista...
            {
                int pista = int.Parse(filas[i][0]);
                if (pista == y)         //Si cubre toda la fila
                {
                    for (int j = 0; j < y; j++)
                    {

                        changeSprite(cubitos[i, j], state2);
                        matriz[i, j] = 1;

                        //Debug.Log(i + "," + j);
                    }
                    filas[i][0] = "0";
                }
                else if (y / 2 < pista) //Si hay parte fija en el medio
                {
                    for (int n = 1, m = y - 2; n <= m; n++, m--)
                    {
                        //Debug.Log(i + "," + n);
                        if (n + pista >= y)
                        {
                            changeSprite(cubitos[i, n], state2);
                            changeSprite(cubitos[i, m], state2);
                            matriz[i, n] = 1;
                            matriz[i, m] = 1;
                        }
                    }
                }

            }

            else
            {  //Si es mas de una pista...
                int total = 0;
                ArrayList pistas = new ArrayList();
                foreach (string num in filas[i]) //Suma las pistas para verificar si cubriran toda la fila
                {
                    total += int.Parse(num);
                    pistas.Add(int.Parse(num));
                }
                if (total + pistas.Count - 1 == y)   //Si cubre toda la fila con espacios vacios pero fijos lo completa
                {
                    for (int j = 0, k = 0; j < y; j++, k++)
                    {
                        if (pistas.Count != 0 && pistas[0].ToString() == k.ToString())
                        {    //Encuentra el espacio y lo salta
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
                    filas[i][0] = "0";
                }
                else
                {
                    foreach (int pista in pistas)
                    {
                        if (y / 2 < pista) //Si hay parte fija en el medio
                        {
                            for (int n = 1, m = y - 2; n <= m; n++, m--)
                            {
                                //Debug.Log(i + "," + n);
                                if (n + pista >= y)
                                {
                                    changeSprite(cubitos[i, n], state2);
                                    changeSprite(cubitos[i, m], state2);
                                    matriz[i, n] = 1;
                                    matriz[i, m] = 1;
                                }
                            }
                        }
                    }
                }

            }

        }

        //Columnas--------------------------------------------------------
        for (int i = 0; i < y; i++) //Rellena columnas
        {
            yield return new WaitForSeconds(0.1f);
            if (columnas[i][0] == "0") continue;     //Si es una pista vacia se ignora

            if (columnas[i].Length == 1)  //Si es una sola pista...
            {
                int pista = int.Parse(columnas[i][0]);
                if (pista == x)  //Si cubre toda la columna
                {
                    for (int j = 0; j < x; j++)
                    {
                        changeSprite(cubitos[j, i], state2);
                        matriz[j, i] = 1;
                        //Debug.Log(i + " " + j);
                    }
                    columnas[i][0] = "0";
                }
                else if (x / 2 < pista)
                {   //Si hay parte fija en el medio
                    for (int n = 1, m = x - 2; n <= m; n++, m--)
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

            else
            {    //Si es mas de una pista
                int total = 0;
                ArrayList pistas = new ArrayList();
                foreach (string num in columnas[i])  //Suma las pistas para verificar si cubriran toda la fila
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
                    columnas[i][0] = "0";
                }
                else
                {
                    foreach (int pista in pistas)
                    {
                        if (x / 2 < pista) //Si hay parte fija en el medio
                        {
                            for (int n = 1, m = x - 2; n <= m; n++, m--)
                            {
                                if (n + pista >= x)
                                {
                                    changeSprite(cubitos[n, i], state2);
                                    changeSprite(cubitos[m, i], state2);
                                    matriz[n, i] = 1;
                                    matriz[m, i] = 1;
                                }
                            }
                        }
                    }
                }
            }
        }

        completarBordesFilas();
        completarBordesColumnas();

        VerificaCompletos();



        yield return new WaitForSeconds(0.2f);

        Debug.Log("Nonograma Resuelto");
    }

    void rellenaColumnas(int inicio, int colum, int lim, int x) //Rellena la matriz en cierto intervalo con 1 o 0 en la columna
    {
        for (int i = inicio; i < lim; i++)
        {
            if (matriz[i, colum] == -1)
            {
                matriz[i, colum] = x;
                if (x == 1)
                    changeSprite(cubitos[i, colum], state2);
                else if (x == 0)
                    changeSprite(cubitos[i, colum], state3);
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
                if (x == 1)
                    changeSprite(cubitos[fila, i], state2);
                else if (x == 0)
                    changeSprite(cubitos[fila, i], state3);
            }
        }
    }

    void completarBordesFilas() //Revisa si se pueden completar pistas que ya inicien en un extremo
    {
        for (int i = 0; i < y; i++)
        {
            string[] col = columnas[i];
            if (matriz[0, i] == 1)
            {
                if (int.Parse(col[0]) > 1)
                {
                    rellenaColumnas(1, i, int.Parse(col[0]), 1);
                }
            }
            if (matriz[x - 1, i] == 1)
            {
                if (int.Parse(col[col.Length - 1]) > 1)
                {
                    rellenaColumnas(x - int.Parse(col[col.Length - 1]), i, x - 1, 1);
                }
            }
        }
    }

    void completarBordesColumnas()  //Completa a partir de los bordes filas que empiezan en los bordes
    {
        for (int i = 0; i < x; i++)
        {
            string[] fil = filas[i];
            if (matriz[i, 0] == 1)
            {
                if (int.Parse(fil[0]) > 1)
                {
                    rellenaFilas(1, i, int.Parse(fil[0]), 1);
                }
            }
            if (matriz[i, y - 1] == 1)
            {
                if (int.Parse(fil[fil.Length - 1]) > 1)
                {
                    rellenaFilas(x - int.Parse(fil[fil.Length - 1]), i, y - 1, 1);
                }
            }
        }
    }

    void revisarPistasNulas()   //Revisa si hay pistas nulas y pone en 0 toda la fila o columna
    {
        for (int i = 0; i < x; i++)
        {
            if (filas[i][0] == "0")
            {
                rellenaFilas(0, i, y, 0);
            }
            if (columnas[i][0] == "0")
            {
                rellenaColumnas(0, i, x, 0);
            }
        }
    }

    void VerificaCompletos()
    {
        for(int i = 0; i < x; i++)
        {
            if(filas[i][0] != "0")
                VerificaFilas(i);

            if(columnas[i][0] != "0")
                VerificaColumnas(i);
        }
    }

    void VerificaFilas(int i)
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

        if(recorredor.Replace(",","") == string.Join("",filas[i]))
        {
            setFilasVacios(i);
        }
    }

    void VerificaColumnas(int j)
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
        
        if (recorredor.Replace(",", "") == string.Join("", columnas[j]))
        {
            setColumnasVacios(j);
        }
    }

    void setFilasVacios(int i)
    {
        for(int j = 0; j < y; j++)
        {
            if(matriz[i,j] == -1)
            {
                changeSprite(cubitos[i, j], state3);
                matriz[i, j] = 0;
            }
        }
    }

    void setColumnasVacios(int j)
    {
        for (int i = 0; i < y; i++)
        {
            if (matriz[i, j] == -1)
            {
                changeSprite(cubitos[i, j], state3);
                matriz[i, j] = 0;
            }
        }
    }


    public void verPistas()
    {
        Debug.Log("FILAS");
        for (int i = 0; i < x; i++)
        {
            string f = i + ": " + string.Join(", ", filas[i]);
            Debug.Log(f);
        }

        Debug.Log("COLUMNAS");
        for (int i = 0; i < y; i++)
        {
            string c = i + ": " + string.Join(", ", columnas[i]);
            Debug.Log(c);
        }
    }

}
