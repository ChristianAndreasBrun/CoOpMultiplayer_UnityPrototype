using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Pun.Demo.SlotRacer.Utils;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyControl : MonoBehaviourPunCallbacks, IPunObservable
{
    [Header("Values")]
    public float moveSpeed = 3f;
    public int maxHealth = 100;
    public int damage = 10;

    [Header("Components")]
    public Image healthFill;
    [SerializeField] private Animator anim;
    public NavMeshAgent agent;

    int currentHealth;
    Transform target;


    private void Start()
    {
        currentHealth = maxHealth;
        healthFill.fillAmount = 1f;

        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        SelectRandomTarget();
    }

    private void Update()
    {
        if (PhotonNetwork.IsMasterClient && target !=null)
        {
            // Mover el enemigo hacia el jugador
            agent.SetDestination(target.position);

            // Actualizar la animación del enemigo
            anim.SetBool("enemyMoving", agent.velocity.magnitude > 0.1f);
        }
    }

    void SelectRandomTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        if (players.Length > 0)
        {
            target = players[Random.Range(0, players.Length)].transform;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerControl player = other.GetComponent<PlayerControl>();
            if (player != null)
            {
                player.RPC_GetDamage(damage);
            }
        }
    }

    public void SetEnemyDamage(GameObject objective, int damage)
    {
        objective.GetComponent<PhotonView>().RPC(nameof(RPC_EnemyTakeDamage), RpcTarget.Others, damage);
    }

    [PunRPC]
    public void RPC_EnemyTakeDamage(int damage)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            currentHealth -= damage;
            if (currentHealth <= 0)
            {
                PhotonNetwork.Destroy(gameObject);
            }
        }
        
        float healthPercent = (float)currentHealth / maxHealth;
        healthFill.fillAmount = healthPercent;
    }


    public void SetAnimator(Animator anim)
    {
        this.anim = anim;
    }

    public void SetValueAnimator(string propertyName, bool value)
    {
        if (anim != null) anim.SetBool(propertyName, value);
    }

    public bool GetBoolAnimator(string propertyName)
    {
        if (anim != null) return anim.GetBool(propertyName);
        return false;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // ENVIO
            stream.SendNext(currentHealth);
            stream.SendNext(healthFill.fillAmount);
            stream.SendNext(GetBoolAnimator("enemyMoving"));
        }
        else
        {
            // RECIBO
            currentHealth = (int)stream.ReceiveNext();
            healthFill.fillAmount = (float)stream.ReceiveNext();
            bool enemyMovingAnim = (bool)stream.ReceiveNext();
            SetValueAnimator("enemyMoving", enemyMovingAnim);
        }
    }
}

