using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(PlayerSetup))]
public class Player : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;
    public bool isDead
    {
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField]
    private int maxHealth = 100;

    [SyncVar]
    private int currentHealth;

    [SerializeField]
    private Behaviour[] disableOnDeath;
    private bool[] wasEnabled;

    [SerializeField]
    private GameObject[] disableGameObjectsOnDeath;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject spawnEffect;

    private bool FirstSetup = true;

    public void SetupPlayer()
    {
        if (isLocalPlayer)
        {
            //Switch cameras
            switchCamera();
        }
        //player setup 
        CmdBroadCastNewPlayerSetup();
    }

    /*
     void Update()
     {  
         if (!isLocalPlayer)
         {
             return;
         }

         if (Input.GetKeyDown(KeyCode.K))
         {
             RpcTakeDamage(999999);
         }
     }
     */

    [Command]
    private void CmdBroadCastNewPlayerSetup()
    {
        RpcSetupPlayerOnAllClients();
    }

    [ClientRpc]
    private void RpcSetupPlayerOnAllClients()
    {
        if (FirstSetup)
        {
            wasEnabled = new bool[disableOnDeath.Length];
            for (int i = 0; i < wasEnabled.Length; i++)
            {
                wasEnabled[i] = disableOnDeath[i].enabled;
            }
            FirstSetup = false;
        }

        SetDefaults();
    }

    [ClientRpc]
    public void RpcTakeDamage (int _amount)
    {
        if (isDead)
            return;

        currentHealth -= _amount;

        Debug.Log(transform.name + " now has " + currentHealth + " health.");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        //disable components on death
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = false;
        }

        //disable GameObjects on death
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(false);
        }

        //disable collider on death
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = false;

        //spawn death effect
        GameObject _GFXIns = (GameObject) Instantiate(deathEffect, transform.position, Quaternion.identity);
        Destroy(_GFXIns, 3f);

        //Switch cameras
        if (isLocalPlayer)
        {
            GameManager.instance.SetSceneCameraActive(true);
            GetComponent<PlayerSetup>().playerUIInstance.SetActive(false);
        }

        //Disable Componenets
        Debug.Log(transform.name + " is DEAD!");

        //call respawn method
        StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTime);

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();
        transform.position = _spawnPoint.position;
        transform.rotation = _spawnPoint.rotation;

        yield return new WaitForSeconds(0.1f);

        SetupPlayer();

        Debug.Log(transform.name + " Respawned");
    }

    public void SetDefaults()
    {
        isDead = false;
        currentHealth = maxHealth;

        //Enable Components on Respawn
        for (int i = 0; i < disableOnDeath.Length; i++)
        {
            disableOnDeath[i].enabled = wasEnabled[i];
        }

        //Enable GameObjects on Respawn
        for (int i = 0; i < disableGameObjectsOnDeath.Length; i++)
        {
            disableGameObjectsOnDeath[i].SetActive(true);
        }
        //enable collider
        Collider _col = GetComponent<Collider>();
        if (_col != null)
            _col.enabled = true;

        //create spawn effect
        GameObject _GFXIns = (GameObject)Instantiate(spawnEffect, transform.position, Quaternion.identity);
        Destroy(_GFXIns, 3f);

    }

    private void switchCamera()
    {
        //Switch cameras
        GameManager.instance.SetSceneCameraActive(false);
        GetComponent<PlayerSetup>().playerUIInstance.SetActive(true);
    }

}
