using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookingShot : MonoBehaviour
{
    public Transform pontoDeDisparo;
    public GameObject projetil;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // Verifica se o jogador pressionou o botão de disparo
        {
            if (transform.localScale.x > 0) // Verifica se o jogador está virado para a direita
            {
                // Lógica para disparar para a direita
                Disparar(1f); // Chama a função de disparo passando 1 como direção
            }
            else // O jogador está virado para a esquerda
            {
                // Lógica para disparar para a esquerda
                Disparar(-1f); // Chama a função de disparo passando -1 como direção
            }
        }
    }

    void Disparar(float direcao)
    {
        GameObject novoProjetil = Instantiate(projetil, pontoDeDisparo.position, pontoDeDisparo.rotation); // Instancia um novo projetil
        novoProjetil.GetComponent<Rigidbody2D>().velocity = new Vector2(direcao * 10f, 0); // Aplica uma velocidade ao projetil na direção especificada
    }
}
