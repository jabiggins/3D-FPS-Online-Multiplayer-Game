using UnityEngine;
using UnityEngine.Networking;

public class PlayerShooting : NetworkBehaviour {

    [SerializeField] float shotCooldown = 0.3f;
    [SerializeField] Transform firePosition;
    //[SerializeField] ShotEffectsManager shotEffects;  if you want to add particle systems

    [SyncVar (hook ="OnScoreChanged")] int score;

    float ellaspedTime;
    bool canShoot;

    private void Start()        //happens when player spawns into a level(know if ocal player
    {
        //shotEffects.Intialize();
        if (isLocalPlayer)
            canShoot = true;
    }

    [ServerCallback]                //only server executes, clients dont even see it
    private void OnEnable()
    {
        score = 0;
    }

    private void Update()
    {
        if (!canShoot)
            return;

        ellaspedTime += Time.deltaTime;
        if(Input.GetButtonDown("Fire1")&& ellaspedTime>shotCooldown)
        {
            ellaspedTime = 0;
            //cmd
            CmdFireShot(firePosition.position, firePosition.forward);
            
        }
    }
    [Command]       //commands run on server for security(client tells server to do something
    void CmdFireShot(Vector3 origin, Vector3 direction)
    {
        Debug.Log("Bang");
        RaycastHit hit;
        Ray ray = new Ray(origin, direction);
        Debug.DrawRay(ray.origin, ray.direction * 3f, Color.red, 1f);

        bool result = Physics.Raycast(ray, out hit, 50f);

        if(result)
        {
            //health script
            PlayerHealth enemy = hit.transform.GetComponent<PlayerHealth>();

            if(enemy != null)
            {
                bool wasKillshot = enemy.TakeDamage();

                if (wasKillshot)
                    score++;
            }
        }

        RpcProcessingShotEffects(result, hit.point);
    }
    [ClientRpc]     //server tells all of clients to do something
    void RpcProcessingShotEffects(bool playImpact, Vector3 point)
    {
        //shotEffects.PlayShotEffects();
        if (playImpact)
        {
            //shotEffecs.PlayImpactEffect(point);
            Debug.Log("IMPACT");
        }
            
    }
        
    void OnScoreChanged(int value)
    {
        score = value;
        if (isLocalPlayer)
            PlayerCanvas.canvas.SetKills(value);
    }

}
