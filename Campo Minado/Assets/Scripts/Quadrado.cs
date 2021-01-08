using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadrado : MonoBehaviour
{
    public int posX;
    public int posY;
    private int valor;
    public int Valor { get => valor; set => valor = value; }
    public Sprite padrao;
    public Sprite travado;
    private bool isOpen = false;
    public bool IsOpen { get => isOpen; }
    private bool isLocked = false;
    public bool IsLocked { get => isLocked; }
    public Sprite[] sprites;
    private new SpriteRenderer renderer;

    void Start() 
    {
        renderer = GetComponent<SpriteRenderer>();
        Valor = 0;
    }
    

    // Update is called once per frame
    void Update()
    {
        if(isOpen)
        {
            renderer.sprite = sprites[Valor];
            if(Valor == 10)
            {
                Mapa.perdeu = true;
            }
        }
        else if(!isLocked)
        {
            renderer.sprite = padrao;
        }
        else if(isLocked)
        {
            renderer.sprite = travado;
        }
    }

    public void Abrir()
    {
        if(!isLocked)
        {
            isOpen = true;
        }
    }

    public void Travar()
    {
        if(!isOpen)
        {
            isLocked = !isLocked;
        }
    }
}
