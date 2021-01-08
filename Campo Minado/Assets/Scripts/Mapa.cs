using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour
{
    public int largura;
    public int altura;
    public int numeroDeBombas;
    public GameObject quadrado;

    private float xQuad, yQuad;
    private GameObject[,] campo;
    private bool bombPlanted = false;
    private bool hasValue = false;
    private int valorBomba = 10;
    private int areaLimpa = 1;
    private List<GameObject> candidatos;

    public static bool perdeu = false, ganhou = false;
    private bool stopGame;

    // Start is called before the first frame update
    void Start()
    {

        stopGame = false;
        perdeu = false;
        ganhou = false;

        xQuad = quadrado.transform.position.x;
        yQuad = quadrado.transform.position.y;
        if (altura < 10)
        {
            altura = 10;
        }

        if (largura < 10)
        {
            largura = 10;
        }

        campo = new GameObject[largura, altura];

        if (numeroDeBombas > (altura * largura - 10))
        {
            numeroDeBombas = (altura * largura - 10);
        }

        if (numeroDeBombas < Mathf.FloorToInt(altura * largura * 0.1f))
        {
            numeroDeBombas = Mathf.FloorToInt(altura * largura * 0.1f);
        }

        candidatos = new List<GameObject>();

        MakeMap();
    }

    // Update is called once per frame
    void Update()
    {
        if (!stopGame)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null)
                {
                    if (!bombPlanted)
                    {
                        PlantBomb(hit.transform.gameObject);
                    }

                    if (!hasValue)
                    {
                        ColocarValores();
                    }
                    Expand(hit.transform.gameObject);

                    VerificarVitoria();

                }
            }
            else if (Input.GetMouseButtonDown(1))
            {
                Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);
                if (hit.collider != null)
                {
                    hit.transform.gameObject.GetComponent<Quadrado>().Travar();
                }
            }

            if (perdeu)
            {
                stopGame = true;
                SceneController.GoToScene("Menu");
            }
            else if (ganhou)
            {
                Debug.Log("GANHOU!!!");
                stopGame = true;
                SceneController.GoToScene("Menu");
            }
        }
    }

    void VerificarVitoria()
    {
        int quantFechados = 0;
        for (int i = 0; i < campo.GetLength(0); i++)
        {
            for (int j = 0; j < campo.GetLength(1); j++)
            {
                if (!campo[i, j].GetComponent<Quadrado>().IsOpen)
                {
                    quantFechados++;
                }
            }
        }
        if (quantFechados == numeroDeBombas)
        {
            ganhou = true;
        }
    }

    void MakeMap()
    {
        for (int linha = 0; linha < largura; linha++)
        {
            for (int coluna = 0; coluna < altura; coluna++)
            {
                campo[linha, coluna] = Instantiate(quadrado, new Vector3(xQuad + linha, yQuad - coluna, 0), Quaternion.identity);
                campo[linha, coluna].GetComponent<Quadrado>().posX = linha;
                campo[linha, coluna].GetComponent<Quadrado>().posY = coluna;
            }
        }
    }

    void PlantBomb(GameObject aEvitar)
    {
        bombPlanted = true;
        // Debug.Log(aEvitar.GetComponent<Quadrado>().posX);
        // Debug.Log(aEvitar.GetComponent<Quadrado>().posY);
        for (int i = 0; i < numeroDeBombas; i++)
        {
            int x = Random.Range(0, largura);
            int y = Random.Range(0, altura);
            //Debug.Log("Iteração: " + i + "\nX = " + x + "/ Y = " + y);
            while ((Mathf.Abs(campo[x, y].GetComponent<Quadrado>().posX - aEvitar.GetComponent<Quadrado>().posX) <= areaLimpa &&
                    Mathf.Abs(campo[x, y].GetComponent<Quadrado>().posY - aEvitar.GetComponent<Quadrado>().posY) <= areaLimpa) ||
                    campo[x, y].GetComponent<Quadrado>().Valor == valorBomba)
            {
                x = Random.Range(0, largura);
                y = Random.Range(0, altura);
            }

            campo[x, y].GetComponent<Quadrado>().Valor = valorBomba;
        }
    }

    void ColocarValores()
    {
        hasValue = true;
        for (int i = 0; i < campo.GetLength(0); i++)
        {
            for (int j = 0; j < campo.GetLength(1); j++)
            {
                if (campo[i, j].GetComponent<Quadrado>().Valor != valorBomba)
                {
                    ChecarVizinhos(campo[i, j]);
                }
            }
        }
    }

    void ChecarVizinhos(GameObject casa)
    {
        int x = casa.GetComponent<Quadrado>().posX;
        int y = casa.GetComponent<Quadrado>().posY;
        int valorCasa = casa.GetComponent<Quadrado>().Valor;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i < 0 || j < 0 || i >= campo.GetLength(0) || j >= campo.GetLength(1) || (i == x && j == y))
                {
                    continue;
                }

                int valorVizinho = campo[i, j].GetComponent<Quadrado>().Valor;
                if (valorVizinho == valorBomba)
                {
                    valorCasa++;
                }
            }
        }
        casa.GetComponent<Quadrado>().Valor = valorCasa;
    }

    void Expand(GameObject casaInicial)
    {
        candidatos.Add(casaInicial);
        Expand(candidatos, 0);
    }

    void Expand(List<GameObject> candidatos, int index)
    {
        if (index >= candidatos.Count)
        {
            return;
        }
        candidatos[index].GetComponent<Quadrado>().Abrir();
        if (candidatos[index].GetComponent<Quadrado>().Valor == 0)
        {
            AddCandidatos(candidatos[index]);
        }
        Expand(candidatos, index + 1);
    }

    void AddCandidatos(GameObject casaInicial)
    {
        int x = casaInicial.GetComponent<Quadrado>().posX;
        int y = casaInicial.GetComponent<Quadrado>().posY;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                if (i < 0 || i >= campo.GetLength(0) || j < 0 || j >= campo.GetLength(1))
                {
                    continue;
                }

                if (!(campo[i, j].GetComponent<Quadrado>().IsOpen || campo[i, j].GetComponent<Quadrado>().IsLocked))
                {
                    candidatos.Add(campo[i, j]);
                }
            }
        }
    }
}
