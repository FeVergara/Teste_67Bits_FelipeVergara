using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Variaveis de controle do Spawn de NPC")]
    public GameObject NPC;
    public Transform SpawnPosition;

    public void SpawnNPC()
    {
        //Controlado por um botão -> Instancia um NPC na posição de Spawn com Rotação 0
        Instantiate(NPC, SpawnPosition.position, new Quaternion(0, 0, 0, 0), SpawnPosition);
    }
}
