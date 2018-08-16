using UnityEngine.Networking;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour {

    [SerializeField] int maxHealth = 3;

    [SyncVar(hook = "OnHealthChanged")] int health;           //when value changes, all players locally and remotely will be reduced--only server knows---needs command


    Player player;
    

    void Awake()        //even local player does not know if local player(isLocalPlayer==false)
    {
        player = GetComponent<Player>();

    }
    [ServerCallback]        //method can only run on the server
    private void OnEnable()
    {
        health = maxHealth;
    }
    [ServerCallback]        //method can only run on the server
    private void Start()
    {
        health = maxHealth;
    }

    [Server]
    public bool TakeDamage()
    {
        bool died = false;

        if (health <= 0)
            return died;

        health--;
        died = health <= 0;

        RpcTakeDamage(died);

        return died;
    }

    [ClientRpc]
    void RpcTakeDamage(bool died)       //handle graphical effects
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.FlashDamageEffect();                
        }
        if (died)
            player.Die();
    }

    void OnHealthChanged(int value)             //because server handles damage and health, you may print the health you used to have due to delay
    {
        health = value;
        if(isLocalPlayer)
        {
            PlayerCanvas.canvas.SetHealth(value);
        }
    }
}
