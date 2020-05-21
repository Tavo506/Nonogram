using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;

public class Logic : MonoBehaviour
{
    int[,] matriz, copia;
    GameObject[,] cubitos;
    int x, y;
    int ultimoAcomodo;

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
        //StartCoroutine(resolverNonograma(0.2f));
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

    /*
     * Genera la solucion de las filas lo más a la izquierda posible
     */
    void crearSolucion()
    {
        for(int i = 0; i < x; i++)
        {
            int indice = 0;
            int contador = 0;
            for(int j = 0; j < y && indice < filas[i].Length; j++)
            {
                if (contador < int.Parse(filas[i][indice]))
                {
                    matriz[i, j] = 1;
                    changeSprite(cubitos[i, j], state2);
                    contador++;
                }
                else
                {
                    contador = 0;
                    indice++;
                }
            }
        }
    }

    /*
     * 
     * Funcion inicial al resolver el nonograma +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
     * 
     */
    IEnumerator resolverNonograma(float time)   //Con pausas
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        yield return new WaitForSeconds(time);

        crearSolucion();

        yield return new WaitForSeconds(time);

        for (int aux = y - 1; aux >= 0; aux--)
        {
            NonogramPuntoSolve(aux, 0);
            yield return new WaitForSeconds(time);
        }


        double tiempo = sw.Elapsed.TotalMilliseconds;
        Debug.Log("Tiempo de ejecución: " + tiempo + " milisegundos \n");

        if (resuelto())
            Debug.Log("Nonograma resuelto!!!");
        else
            Debug.Log("No se pudo resolver el nonograma :c");
    }

    void resolverNonograma()    //Sin pausas
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        crearSolucion();

        for (int aux = y - 1; aux >= 0; aux--)
        {
            NonogramPuntoSolve(aux, 0);
        }

        double tiempo = sw.Elapsed.TotalMilliseconds;
        Debug.Log("Tiempo de ejecución: " + tiempo + " milisegundos \n");

        if (resuelto())
            Debug.Log("Nonograma resuelto!!!");
        else
            Debug.Log("No se pudo resolver el nonograma :c");
    }
    /*
     * 
     * Esto es solo para encontrar más rápido la función y no buscar tanto :v +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
     */

    void NonogramPuntoSolve(int columna, int fila)
    {
        if (!VerificaColumnas(columna)) {
            copia = matriz;
            acomoda(columna, fila);
        }
        if (VerificaColumnas(columna)) 
        {
            return;
        }
        else
        {
            //matriz = copia;
            
            int contador = 0;
            while (!VerificaColumnas(columna) && columna + 1 < y && fila + contador + 1 < x) { 
                backway(columna + 1 ,fila+contador);
                contador++;
            }
            
        }
        return;
    }

    void imprimeMat() {

        for (int i = 0; i < x; i++) {
            string fila = "";
            for (int j = 0; j < y; j++) {
                fila = fila + matriz[i, j] + ", ";
            }
            Debug.Log(fila);
        }
    
    }

    void backway(int col, int fila) 
    {

        for (int i = fila; i < x; i++) 
        {
            if (col+1 < y && matriz[i, col] == 1 && matriz[i, col + 1] != 1) {

                moverBloque(i, col, fila);
                return;

            }
        }
        return;
    }

    void moverBloque(int fila, int columna, int filaOrigen) 
    {
        int aux = buscarBloque(fila, columna);
        invPonerBloque(fila, dondePonerBack(fila, columna), aux);
        
        if (!VerificaColumnas(columna)) {

            NonogramPuntoSolve(columna, ultimoAcomodo +1);
        
        }else backway(columna + 1, filaOrigen);
    }



    void acomoda(int columna, int inicio)
    {
        int cont = 0, indice = 0;

        for(int fila = inicio; fila < x && indice < columnas[columna].Length; fila++)
        {
            if(cont == int.Parse(columnas[columna][indice]))
            {
                indice++;
                cont = 0;
                if (VerificaColumnas(columna))
                    return;
            }

            else if(matriz[fila, columna] == -1 && (columna + 1 == y || matriz[fila, columna + 1] == -1))
            {
                int bloque = buscarBloque(fila, columna);
                ponerBloque(fila, columna, bloque);
                ultimoAcomodo = fila;
                cont++;
            }
            else if(matriz[fila, columna] == 1)
            {
                cont++;
            }
            else if(cont != 0)
            {
                fila = corrige(fila-1, columna);
                cont = 0;
            }
        }
    }

    int corrige(int fila, int columna)
    {
        while(matriz[fila, columna] == 1) {
            int aux = buscarBloque(fila, columna);
            invPonerBloque(fila, dondePonerBack(fila, columna), aux);

            if (fila - 1 >= 0)
                fila--;
            else
                break;
            
        }
        return fila+1;
    }

    int buscarBloque(int fila, int columna)
    {
        int cont = 0;
        bool encontrado = false;
        for(int j = columna; j >= 0; j--)
        {
            if (matriz[fila, j] == -1 && !encontrado)
                continue;
            else if(matriz[fila, j] == -1 && encontrado) { 
                return cont;
            }
            else if(matriz[fila, j] == 1)
            {
                encontrado = true;
                cont++;
                matriz[fila, j] = -1;
                changeSprite(cubitos[fila, j], state1);
            }
        }
        if (encontrado)
            return cont;
        return 0;
    }

    void ponerBloque(int fila, int columna, int bloque)
    {
        for(int j = bloque-1; j >= 0; j--)
        {
            //Debug.Log(fila + ", " + columna);
            matriz[fila, columna-j] = 1;
            changeSprite(cubitos[fila, columna-j], state2);
        }
    }

    void invPonerBloque(int fila, int columna, int bloque)
    {
        for (int j = columna; j < columna+bloque; j++)
        {
            //Debug.Log(fila + ", " + columna);
            matriz[fila, j] = 1;
            changeSprite(cubitos[fila, j], state2);
        }
    }

    int dondePonerBack(int fila, int columna)
    {
        int aux = columna;

            if (matriz[fila, aux] == -1) { while (aux > 0 && matriz[fila, aux] == -1) { aux--; } }
            if (matriz[fila, aux] == 1)  { while (aux > 0 && matriz[fila, aux] == 1)  { aux--; } }
            if (matriz[fila, aux] == -1) { while (aux > 0 && matriz[fila, aux] == -1) { aux--; } }

        if (aux == 0)
            return 0;
        return aux+1;
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

        if (columnaVacia(j)) 
            return "0";

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
            if (conta + 1 == 0 && n != 0)
                recorredor += n;
        }
        try
        {
            
            if (recorredor == "")
                return "0";
            if (recorredor.EndsWith(","))
                recorredor = recorredor.Remove(recorredor.Length - 1);
            return recorredor;
        }
        catch
        {
            Debug.Log("se murió");
            return "0";
        }
    }

    bool columnaVacia(int j)
    {
        for(int i = 0; i < x; i++)
        {
            if (matriz[i, j] != -1)
                return false;
        }
        return true;
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

    bool resuelto()
    {
        for(int i = 0; i < y; i++)
        {
            if (!VerificaColumnas(i))
                return false;
        }
        return true;
    }

    public void solveSinPausa()
    {
       resolverNonograma();
    }


    public void solveConPausa()
    {
        StartCoroutine(resolverNonograma(1f));
    }
}
