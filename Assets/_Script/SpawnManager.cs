using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [Header("Variaveis de controle do Spawn de NPC")]
    public GameObject NPC;
    public Transform SpawnPosition;

    public void SpawnNPC()
    {
        //Controlado por um bot�o -> Instancia um NPC na posi��o de Spawn com Rota��o 0
        Instantiate(NPC, SpawnPosition.position, new Quaternion(0, 0, 0, 0), SpawnPosition);
    }
}
