using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour , IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [Header("Vari�veis do Joystick")]
    public Image joystickBackground;
    public Image joystick;
    private float radiusJoystick;
    private bool touch;

    [Space(20)]

    [Header("Vari�veis de Movimenta��o")]
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
            //Se o jogador esta tocando no Joystick ent�o movimentar ele com a variavel vectorMovement
            player.transform.Translate(vectorMovement, Space.World);    
        }
    }


    //CONTROLA O JOYSTICK E O MOVIMENTO
    void ControlJoystickAndMovement(Vector2 vecTouch)
    {
        //Define a posi��o do Joystick
        Vector2 vectorControlJoystick = new(vecTouch.x - joystickBackground.rectTransform.position.x, vecTouch.y - joystickBackground.rectTransform.position.y); // Vetor de controle do Joystick -> definido pela posi��o do toque - a posi��o do background;
        vectorControlJoystick = Vector2.ClampMagnitude(vectorControlJoystick, radiusJoystick); // ClampMagnitude do Vetor de controle do Joystick com o Raio do Joystick;
        joystick.rectTransform.transform.localPosition = vectorControlJoystick; // Define o Joystick para a posi��o do Vetor de Controle do Joystick;
        
        //Define a movimenta��o e rota��o do Player
        vectorMovement = new Vector3((vectorControlJoystick.x / 100) * speed * Time.deltaTime, 0f, (vectorControlJoystick.y / 100) * speed * Time.deltaTime); // Define a Movimenta��o do Jogador a partir do Vetor de Controle do Joystick;
        player.transform.eulerAngles = new Vector3(0f, Mathf.Atan2(vectorControlJoystick.normalized.x, vectorControlJoystick.normalized.y) * Mathf.Rad2Deg, 0f); // Define a Rota��o do Jogador a partir do Vetor de controle do Joystick em graus;
    }





    //INTERFACE PARA CONTROLAR O TOUCH
    public void OnDrag(PointerEventData eventData)
    {
        //Enquanto arrastando o Joystick passar informa��o da posi��o do Joysick, definir que esta tocando na tela e que o Player est� se movendo

        ControlJoystickAndMovement(eventData.position);

        touch = true;

        isMoving = true;
        player.ControlAnimationMoving(isMoving);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Enquando estiver clicando no Joystick passar informa��o da posi��o do Joysick, definir que esta tocando na tela e que o Player est� se movendo

        ControlJoystickAndMovement(eventData.position);

        touch = true;
        player.ControlAnimationMoving(isMoving);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        //Quando parar de clicar ou arrastar o Joystick zerar a posi��o do Joystick, definir que n�o esta tocando na tela e que o Player N�O est� se movendo

        joystick.rectTransform.transform.localPosition = Vector3.zero;

        touch = false;

        isMoving = false;
        player.ControlAnimationMoving(isMoving);
    }
}