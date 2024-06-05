using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingShot : MonoBehaviour
{
    public Transform pontoDeDisparo;
    public GameObject projetil;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Verifica se o jogador pressionou o bot�o de disparo
        {
            if (transform.localScale.x > 0) // Verifica se o jogador est� virado para a direita
            {
                // L�gica para disparar para a direita
                Disparar(1f); // Chama a fun��o de disparo passando 1 como dire��o
            }
            else // O jogador est� virado para a esquerda
            {
                // L�gica para disparar para a esquerda
                Disparar(-1f); // Chama a fun��o de disparo passando -1 como dire��o
            }
        }
    }

    void Disparar(float direcao)
    {
        GameObject novoProjetil = Instantiate(projetil, pontoDeDisparo.position, pontoDeDisparo.rotation); // Instancia um novo projetil
        novoProjetil.GetComponent<Rigidbody2D>().velocity = new Vector2(direcao * 10f, 0); // Aplica uma velocidade ao projetil na dire��o especificada
    }
}
