using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCs : MonoBehaviour
{
    [Header("Variaveis de controle do NPC")]
    public bool alive;
    public bool canStack;

    void Start()
    {
        //Inicia o NPC com a variavel live verdadeira
        alive = true;
    }

    void Update()
    {
        if (!alive)
        {
            //Se o NPC nao esta mais vivo entao em 1 segundo deixar o NPC hapto para ser Empilhado
            Invoke(nameof(PreparingToStack), 1f);
        }
    }

    private void PreparingToStack()
    {
        //Deixa o NPC hapto para ser empilhado ativando a variavel booleana canStack
        canStack = true;
    }
}
