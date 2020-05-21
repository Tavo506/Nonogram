using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Collections.Generic;
     /*
        Gustavos Algorithm Picross Nonogram, blockMoving -     Last Update 05-20-2020  Completed on 40.4%
        Authors:
        Gustavo Alvarado Orozco
        Gustavo Blanco Alfaro
     
        Curso:     Análisis de Algoritmos
        Instituto: Tecnológico de Costa Rica
        Educador:  Ywen Law
     */
public class Logic : MonoBehaviour
{
    /*
        Acá se declaran las variables que usaremos dentro de nuestro proyecto
        Matriz, corresponde a la matriz lógica de nuestro nonograma que almacena
        vacios y llenos, a su vez gameObject Cubitos es su homólogo dentro de la parte gráfica.
        
        X y Y se usaran para definir el tamaño de la matriz, y sus limites.
        UltimoAcomodo, será una variable global que estará guardadon la última posición donde se movió un bloque

        Declaramos los estados que podemos tener en el nonograma, en este caso creamos un tercero que no estamos utilizando
        en este algoritmo pero sí en otro.

        casilla corresponde a la variable que va a estar contenida dentro de cubitos, es decir
        la celda en sí

        Filas y columnas, no son más que eso, las pistas por fila y columnas que hemos leído del txt
    */
    int[,] matriz;
    GameObject[,] cubitos;
    int x, y;
    int ultimoAcomodo;

    public Sprite state1,   //Blanco
        state2, //Azul
        state3; //X
    private GameObject casilla;
    List<string[]> filas = new List<string[]>();
    List<string[]> columnas = new List<string[]>();



    // Start funcion default de Unity
    void Start()
    {
        //Acá sacamos todos los datos de txt y lo vamos leyendo / guardando
        string[] nonogram = System.IO.File.ReadAllLines(@"Assets\Nonogram.txt");

        x = int.Parse(nonogram[0].Split(',')[0]);
        y = int.Parse(nonogram[0].Split(',')[1].Replace(" ", ""));

        /*
         -1 = vacio
         0 = X
         1 = pintado
         */

        //Aca inicializamos la matriz con el tamaño leído
        matriz = new int[x, y];

        for (int n = 0; n < x; n++)
        {
            for (int m = 0; m < y; m++)
            {
                matriz[n, m] = -1;
            }
        }

        cubitos = new GameObject[x, y];

        
        
        //Ésta función se encarga de leer los datos después de la palabra columnas y se encarga de separarlas
        // y leerlas para posteriormente guardarlas donde corresponden
        int i = 2;
        while (nonogram[i] != "COLUMNAS")
        {
            string[] aux = nonogram[i].Replace(" ", "").Split(',');
            filas.Add(aux);
            i++;
        }

        i++;
        //Esta parte del código es homologo al pequeño bloque anterior pero trabaja sobre Filas
        while (i < nonogram.Length)
        {
            string[] aux = nonogram[i].Replace(" ", "").Split(',');
            columnas.Add(aux);
            i++;
        }

        StartCoroutine(NonogramStart());

    }

    //Creamos la funcion que nos permitirá ir contando e ir paso a paso.
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
                //Generamos la parte gráfica del nonograma
                casilla = new GameObject("Casilla " + j + ", " + i);
                casilla.transform.SetParent(this.transform);
                casilla.AddComponent<SpriteRenderer>();
                casilla.GetComponent<SpriteRenderer>().sprite = state1;
                casilla.transform.position = new Vector2(i * 0.33f, j * -0.33f);
                cubitos[j, i] = casilla;

            }
        }
    }


    //Funcion que nos facilitará cambiar los sprites cuando queramos pasar
    // de estado vacío a lleno
    void changeSprite(GameObject cuadro, Sprite estado)
    {

        cuadro.GetComponent<SpriteRenderer>().sprite = estado;

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

    //Esta funcion primero comprueba que la columna cumpla las pistas y de no ser así
    // jala bloques a para igualar la misma y en caso que después de jalar no haber podido igualar
    // las pistas de las columnas comienza a llamar el backtrack
    void NonogramPuntoSolve(int columna, int fila)
    {
        if (!VerificaColumnas(columna)) {
            acomoda(columna, fila);
        }
        if (VerificaColumnas(columna)) 
        {
            return;
        }
        else
        {
            //Aca va a empezar a backtrackear hasta que la columna esté bien acomodada
            int contador = 0;
            while (!VerificaColumnas(columna) && columna + 1 < y && fila + contador + 1 < x) { 
                backway(columna + 1 ,fila+contador);
                contador++;
            }
            
        }
        return;
    }

    //Funcion que imprime la matriz, no es referenciada
    void imprimeMat() {

        for (int i = 0; i < x; i++) {
            string fila = "";
            for (int j = 0; j < y; j++) {
                fila = fila + matriz[i, j] + ", ";
            }
            Debug.Log(fila);
        }
    
    }

    //Esta funcion se llama cuando no se puede igualar bloques con las pistas, recibe la columna anterior
    //busca lo que decidí llamara cabezas de bloques, si encuentra una que se caracteriza por:
    // tiene que ser un lleno y la celda a su derecha ser vacío.
    // una vez encontrado lo vuelve a mover con moverBloque
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

    //Esta función debería volver a poner el bloque de caracteres lo más a la izquierda posible
    // de y luego vuelve a llamar a la funcion NonogramPuntoSolve pero en este caso le dice que ignore la fila
    // en la que se hizo el ultimo acomodo, es decir normalmente la más alta, va a ignorarla, para
    // buscar algun bloque más inferior que igualmente cumpla con igualar la pista
    // en caso de no encontrar cabezas de bloques o las que hay no se pueden mover, el programa vuelve a moverse
    // de columna para repetir el proceso
    void moverBloque(int fila, int columna, int filaOrigen) 
    {
        int aux = buscarBloque(fila, columna);
        invPonerBloque(fila, dondePonerBack(fila, columna), aux);
        
        if (!VerificaColumnas(columna)) {

            NonogramPuntoSolve(columna, ultimoAcomodo +1);
        
        }else backway(columna + 1, filaOrigen);
    }


    //Esta funcion comienza a ser llamada desde la ultima columna, para ir cumpliendo las pistas de columnas
    // intenta jalar el primer bloque a disposición para ir igualando a las pistas y encaso de igualar una
    // da un salto de fila y comienza nuevamente con la siguiente pista.
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

    // Esta funcion retrocede en caso de no tener donde poner las pistas que no las deje tiradas
    // en caso de no poder poner un bloque que iguale la pista simplemente no pone la cadena
    // de la columna por lo que no lo pone y NonogramPuntoSolve debería decir que no hay solucion con los bloques que tenemos
    // y comenzar a llamar al backtraking
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

    //Esta funcion busca el primer bloque de llenos que se encuentre apartir de una columna y fila determinada
    // los fija en vacío y cuenta de que tamaño es el bloque
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

    //Usando la misma fila y columna que el anterior esta función recibe el tamaño del bloque y apartir de acá
    // comienza a llenar las celdas del mismo tamaño que el bloque contado
    void ponerBloque(int fila, int columna, int bloque)
    {
        for(int j = bloque-1; j >= 0; j--)
        {
            //Debug.Log(fila + ", " + columna);
            matriz[fila, columna-j] = 1;
            changeSprite(cubitos[fila, columna-j], state2);
        }
    }

    //Esta funcion es igual a la anterior pero coloca los bloques de izquiera a derecha
    void invPonerBloque(int fila, int columna, int bloque)
    {
        for (int j = columna; j < columna+bloque; j++)
        {
            //Debug.Log(fila + ", " + columna);
            matriz[fila, j] = 1;
            changeSprite(cubitos[fila, j], state2);
        }
    }

    //esta funcion retorna la columna más a la izquierda posible donde se podría insertar un bloque de celdas llenas
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

    // Verifica que las pistas de las filas sean iguales a las pistas de las filas
    //Se dejo de usar en este algoritmo porque siempre retornará true
    bool VerificaFilas(int i)
    {
        string recorredor = crearStringFila(i);

        if (recorredor.Replace(",", "") == string.Join("", filas[i]))
        {
            return true;
        }
        else return false;
    }

    //Verifica que las coulmnas estén bien con respecto a sus pistas
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

    //Funicon que cuenta las celdas llenas de una fila y las convierte en un string
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

    //Funicon que cuenta las celdas llenas de una columna y las convierte en un string
    // posterior mente se compara con el string que almacena las pistas
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

    //Pregunta si una columna esta vacía
    bool columnaVacia(int j)
    {
        for(int i = 0; i < x; i++)
        {
            if (matriz[i, j] != -1)
                return false;
        }
        return true;
    }

    //Nos imprime las pistas
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

    //Verfica con el algorimo de bloques que todas las verifica columnas sean correctos
    // de ser así significa que pudimos solucionar el nonograma
    bool resuelto()
    {
        for(int i = 0; i < y; i++)
        {
            if (!VerificaColumnas(i))
                return false;
        }
        return true;
    }


    //Funciones que nos permiten llamar de una u otra manera el programa.
    public void solveSinPausa()
    {
       resolverNonograma();
    }


    public void solveConPausa()
    {
        StartCoroutine(resolverNonograma(1f));
    }
}
