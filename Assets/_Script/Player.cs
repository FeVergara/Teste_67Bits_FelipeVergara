using DG.Tweening;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    [Header("Variaveis para controlar os NPCs")]
    public Transform stackingSpace;
    private List<NPCs> npcs = new();
    private int NPCsStacked = -1;
    private int stackingCapacity;

    [Space(20)]

    [Header("Variaveis para controlar o Player")]
    public Color[] colors;
    public int money;
    private int levelPlayer;
    private bool levelUp;
    private float time;
    private Animator anim;
    [SerializeField]private int forcePunch;

    [Space(20)]

    [Header("Variaveis para UI")]
    public TextMeshProUGUI levelPlayerUI;
    public TextMeshProUGUI moneyPlayerUI;
    public TextMeshProUGUI stackingPlayerUI;


    void Start()
    {
        //Configuraçao inicial das variaveis de algumas variaveis de controle e da UI

        anim = this.GetComponent<Animator>();

        stackingCapacity = 2;
        levelPlayer = 1;
        money = 0;

        stackingPlayerUI.text = "StackingMax: " + (NPCsStacked + (int)1) + "/" + (stackingCapacity + (int)1).ToString();
        levelPlayerUI.text = "LevelPlayer: " + levelPlayer.ToString();
        moneyPlayerUI.text = "Money: " + money.ToString();
    }

    private void Update()
    {
        InertiaAnimation();

        LevelUpSystem();
    }



    private void LevelUpSystem()
    {
       if (levelUp)
       {
            //Se a booleana levelUp está ativada então adiciona o tempo do jogo a variavel time

            time += Time.deltaTime;

            if (time >= 2)
            {
                //Se a variavel time for maior que 2 então Aumentar o Nível se tiver 50 de dinheiro
                if (money >= 50)
                {
                    //Ao comprar o Level Up por 50 moedas -> Zerar a Variavel time, Diminuir o Dinheiro em 50, Aumentar o Nivel do Jogador, Aumentar a Capacidade de Empilhamento
                    time = 0;
                    money -= 50;
                    levelPlayer++;
                    stackingCapacity += 2;


                    //Troca a cor do Player para uma cor aleatoria dentro do Vetor de Cores
                    int numberColor = Random.Range(0, colors.Length - 1);
                    Color c = colors[numberColor];
                    GameObject.Find("Character_Man").GetComponent<Renderer>().material.color = c;

                    //Atualiza toda a UI
                    stackingPlayerUI.text = "StackingMax: " + (NPCsStacked + (int)1) + "/" + (stackingCapacity + (int)1).ToString();
                    levelPlayerUI.text = "LevelPlayer: " + levelPlayer.ToString();
                    moneyPlayerUI.text = "Money: " + money.ToString();
                }else
                {
                    //Se não tiver dinhiero suficiente para o LevelUp então apenas Zerar a variavel time
                    Debug.Log("Dinheiro insuficiente para passar de nível");
                    time = 0;
                }
            }
        }
    }




    //CONTROLE DE COLISÕES
    private void OnTriggerEnter(Collider other)
    {
        //CONTROLA AS COLISÕES COM OS NPCs
        if (other.gameObject.CompareTag("NPCs"))
        {
            if (other.gameObject.GetComponent<NPCs>().alive)
            {
                //Se o NPC está vivo faz Animação de Soco, configura o Ragdol do NPC para sofrer física e determina que o NPC morreu e pode ser empilhado.

                anim.SetBool("isPunching", true);
                Invoke(nameof(ControlAnimationPunch), 0.6f);

                other.gameObject.GetComponent<Rigidbody>().AddForce((other.gameObject.transform.position + this.transform.position) * forcePunch, ForceMode.Force);
                other.GetComponent<Rigidbody>().isKinematic = true;
                other.GetComponent<Animator>().enabled = false;

                other.gameObject.GetComponent<NPCs>().alive = false;
             

            }else if (other.gameObject.GetComponent<NPCs>().canStack) //EMPILHAR NPC
            {
                if (NPCsStacked < stackingCapacity)
                {
                    //Se o NPC pode ser empilhado e o Player tem capacidade para empilhar estes NPCs então ->
                    // -> Adicionar NPC no Vetor de NPCs empilhados, configurar o Ragdol do NPC para ficar empilhados, transforma o objeto do NPC para ser filho do Objeto player
                    
                    npcs.Add(other.gameObject.GetComponent<NPCs>());
                    NPCsStacked++;
                    other.GetComponent<Animator>().enabled = true;
                    other.GetComponent<Rigidbody>().isKinematic = false;
                    other.transform.parent = stackingSpace.transform;
                    other.GetComponent<BoxCollider>().enabled = false;

                    //Atualiza a UI da quantidade de NPCs empilhados
                    stackingPlayerUI.text = "StackingMax: " + (NPCsStacked + (int)1) + "/" + (stackingCapacity + (int)1).ToString();

                    //Se for o primeiro NPC então empilhar na posição do Objeto do Espaço de Empilhar com Rotação 0
                    if (NPCsStacked == 0)
                    {
                        other.gameObject.transform.localPosition = Vector3.zero;
                        other.gameObject.transform.localRotation = new(0, 0, 0, 0);

                        other.gameObject.transform.SetPositionAndRotation(stackingSpace.position, new(0, 0, 0, 0));
                    }
                    else//Se NÃO for o primeiro NPC então empilhar na posição do ultimo NPC empilhado + 0.5f na coordenada Y com Rotação 0
                    {
                        other.gameObject.transform.localPosition = new(npcs[NPCsStacked - 1].transform.position.x, npcs[NPCsStacked - 1].transform.position.y, npcs[NPCsStacked - 1].transform.position.z);
                        other.gameObject.transform.localRotation = new(0, 0, 0, 0);

                        other.gameObject.transform.SetPositionAndRotation(new(npcs[NPCsStacked - 1].transform.position.x, npcs[NPCsStacked - 1].transform.position.y + 0.5f, npcs[NPCsStacked - 1].transform.position.z), new(0, 0, 0, 0));
                    }
                }
            }
        }



        //VENDE OS NPCs
        if (other.gameObject.CompareTag("Store"))
        {
            //Verifica se há um jogador empilhado e se há jogadores no Vetor de npcs
            if (NPCsStacked >= 0)
            {
                if (npcs.Count > 0)
                {
                    for (int i = npcs.Count; i > 0; i--)
                    {
                        //Para cada NPC ele se move ate o centro da Loja
                        Tweener tween = npcs[i - 1].transform.DOMove(other.GetComponentInChildren<Transform>().position, 0.5f);
                        tween.onComplete += () =>
                        {
                            //Recebe os atributos da venda do NPC, mais Dinheiro, menos Jogador Empilhado, destroy o objeto do NPC, remove NPC do Vetor
                            money += 12;
                            NPCsStacked--;
                            Destroy(npcs[i].gameObject, 0.55f);
                            npcs.RemoveAt(i);

                            //Atualiza a UI da quantidade de player enmpilhados e do dinheiro
                            stackingPlayerUI.text = "StackingMax: " + (NPCsStacked + (int)1) + "/" + (stackingCapacity + (int)1).ToString();
                            moneyPlayerUI.text = "Money: " + money.ToString();
                        };
                    }
                }
            }
        }


        if (other.gameObject.CompareTag("LevelUp"))
        {
            //Ao entrar no Objeto com a Tag LevelUp ativa o bool levelup que ativa o Sistema de Level Up.
            levelUp = true;
        }
    }


    private void OnTriggerExit(Collider other)
    {
        //Ao sair do Objeto com a Tag LevelUp desativa o bool levelup que desativa o Sistema de Level Up, também reseta o tempo necessario para upar de nivel 
        if (other.gameObject.CompareTag("LevelUp"))
        {
            levelUp = false;
            time = 0;
        }
    }




    //CONTROLE DE ANIMAÇÕES

    //Controlador da Animação de Movimento
    public void ControlAnimationMoving(bool isMoving)
    {
        //Controla o Parametro isMoving que controla a Animação de Movimento
        anim.SetBool("isMoving", isMoving);

    }

    //Controlador da Animação de Soco
    private void ControlAnimationPunch()
    {
        //Desativa o Parametro IsPunching que controla a Animação de Soco
        anim.SetBool("isPunching", false);
    }


    //Animação de Inercia dos NPCs carregados nas costas
    public void InertiaAnimation()
    {
        //Se o numero de npcs for maior que 1 então inicia a Animação de Inercia
        if (npcs.Count > 1)
        {
            for (int i = 1; i < npcs.Count; i++)
            {
                //Atualiza a Posição do NPC empilhado para a mesma do NPC abaixo dele, com 0.25f de delay
                Tweener tween = npcs[i].transform.DOMove(new(npcs[i - 1].transform.position.x, npcs[i].transform.position.y, npcs[i - 1].transform.position.z), 0.25f);
                tween.onComplete += () =>
                {
                    return;
                };
            }
        }
    }
}