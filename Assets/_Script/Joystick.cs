using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Variáveis do Joystick")]
    public Image joystickBackground;
    public Image joystick;
    private float radiusJoystick;
    private bool touch;

    [Space(20)]

    [Header("Variáveis de Movimentação")]
    public Player player;
    public float speed;
    private Vector3 vectorMovement;
    private bool isMoving;


    void Start()
    {
        //Configura o raio do Joystick para ser metade do tamanho do background
        radiusJoystick = ((int)joystickBackground.rectTransform.rect.width)/2;
    }

    void Update()
    {
        if (touch)
        {
            //Se o jogador esta tocando no Joystick então movimentar ele com a variavel vectorMovement
            player.transform.Translate(vectorMovement, Space.World);    
        }
    }


    //CONTROLA O JOYSTICK E O MOVIMENTO
    void ControlJoystickAndMovement(Vector2 vecTouch)
    {
        //Define a posição do Joystick
        Vector2 vectorControlJoystick = new(vecTouch.x - joystickBackground.rectTransform.position.x, vecTouch.y - joystickBackground.rectTransform.position.y); // Vetor de controle do Joystick -> definido pela posição do toque - a posição do background;
        vectorControlJoystick = Vector2.ClampMagnitude(vectorControlJoystick, radiusJoystick); // ClampMagnitude do Vetor de controle do Joystick com o Raio do Joystick;
        joystick.rectTransform.transform.localPosition = vectorControlJoystick; // Define o Joystick para a posição do Vetor de Controle do Joystick;
        
        //Define a movimentação e rotação do Player
        vectorMovement = new Vector3((vectorControlJoystick.x / 100) * speed * Time.deltaTime, 0f, (vectorControlJoystick.y / 100) * speed * Time.deltaTime); // Define a Movimentação do Jogador a partir do Vetor de Controle do Joystick;
        player.transform.eulerAngles = new Vector3(0f, Mathf.Atan2(vectorControlJoystick.normalized.x, vectorControlJoystick.normalized.y) * Mathf.Rad2Deg, 0f); // Define a Rotação do Jogador a partir do Vetor de controle do Joystick em graus;
    }





    //INTERFACE PARA CONTROLAR O TOUCH
    public void OnDrag(PointerEventData eventData)
    {
        //Enquanto arrastando o Joystick passar informação da posição do Joysick, definir que esta tocando na tela e que o Player está se movendo

        ControlJoystickAndMovement(eventData.position);

        touch = true;

        isMoving = true;
        player.ControlAnimationMoving(isMoving);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Enquando estiver clicando no Joystick passar informação da posição do Joysick, definir que esta tocando na tela e que o Player está se movendo

        ControlJoystickAndMovement(eventData.position);

        touch = true;
        player.ControlAnimationMoving(isMoving);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Quando parar de clicar ou arrastar o Joystick zerar a posição do Joystick, definir que não esta tocando na tela e que o Player NÃO está se movendo

        joystick.rectTransform.transform.localPosition = Vector3.zero;

        touch = false;

        isMoving = false;
        player.ControlAnimationMoving(isMoving);
    }
}