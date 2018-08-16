
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;

[System.Serializable]
public class ToggleEvent : UnityEvent<bool> { }
 


public class Player : NetworkBehaviour {
    [SyncVar (hook = "OnNameChanged")] public string playerName;
    [SyncVar (hook = "OnColorChanged")] public Color playerColor;


    //[SerializeField] UnityEvent onSharedEnabled;        //shows in inspector but does not toggle
    [SerializeField] ToggleEvent onToggleShared;
    [SerializeField] ToggleEvent onToggleLocal;
    [SerializeField] ToggleEvent onToggleRemote;
    [SerializeField] float respawnTime = 5f;

    GameObject mainCamera;
    NetworkAnimator anim;       //can acess reg animator frm here

    void Start()
    {
        anim=GetComponent<NetworkAnimator>();
        mainCamera = Camera.main.gameObject;        //find scenes main cam

        EnablePlayer();     //at start, client enables player
    }

    void Update()
    {
        if (!isLocalPlayer)
        { return; }
        anim.animator.SetFloat("Speed", Input.GetAxis("Vertical"));
        anim.animator.SetFloat("Strafe", Input.GetAxis("Horizontal"));
    }

    void DisablePlayer()
    {
        if (isLocalPlayer)
        {
           PlayerCanvas.canvas.HideReticule();
           mainCamera.SetActive(true);
        }

        onToggleShared.Invoke(false);

        if (isLocalPlayer)
        {
            onToggleLocal.Invoke(false);
        }
        else
            onToggleRemote.Invoke(false);
    }
    void EnablePlayer()
    {
        if (isLocalPlayer)
        {
            PlayerCanvas.canvas.Initialize();       //clears game tet too
            mainCamera.SetActive(false);
        }


        onToggleShared.Invoke(true);

        if (isLocalPlayer)
            onToggleLocal.Invoke(true);
        else
            onToggleRemote.Invoke(true);
    }

    public void Die()
    {
        if(isLocalPlayer)
        {
            PlayerCanvas.canvas.WriteGameStatusText("You died");
            PlayerCanvas.canvas.PlayDeathAudio();

            anim.SetTrigger("Died");       //talks to network
        }
        DisablePlayer();

        Invoke("Respawn", respawnTime);
    }
    void Respawn()
    {
        if(isLocalPlayer)
        {
            Transform spawn = NetworkManager.singleton.GetStartPosition();      //find a spawn point 
            transform.position = spawn.position;
            transform.rotation = spawn.rotation;

            anim.SetTrigger("Restart");
        }
        EnablePlayer();
    }

    private void OnNameChanged(string value)
    {
        playerName = value;
        gameObject.name = playerName;
        //set text
        GetComponentInChildren<Text>(true).text = playerName;           //(true)finds it even if disabled
    }
    void OnColorChanged (Color value)
    {
        playerColor = value;
        GetComponentInChildren<RendererToggler>().ChangeColor(playerColor);
    }

}
