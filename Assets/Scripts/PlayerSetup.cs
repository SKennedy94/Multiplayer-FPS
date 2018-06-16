using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField]
    Behaviour[] componentsToDisable;

    [SerializeField]
    string remoteLayerName = "RemotePlayer";

    [SerializeField]
    string dontDrawLayerName = "DontDraw";
    [SerializeField]
    GameObject playerGraphics;

    [SerializeField]
    GameObject playerUIPrefab;

    [HideInInspector]
    public GameObject playerUIInstance;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        } else
        {
            //Disable player graphics for local player
            Utility.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //create PlayerUI for local player
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            //configure PlayerUI
            PlayerUI ui = playerUIInstance.GetComponent<PlayerUI>();

            if (ui == null)
            {
                Debug.LogError("no player ui component on playerui prefab");
            }
            ui.SetPlayer(GetComponent<Player>());

            GetComponent<Player>().SetupPlayer();
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player _player = GetComponent<Player>();

        GameManager.RegisterPlayer(_netID, _player);
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }

    //when player is destroyed
    private void OnDisable()
    {
        //destory player UI
        Destroy (playerUIInstance);

        if(isLocalPlayer)
        //Re-enable the scene camera
        GameManager.instance.SetSceneCameraActive(true);

        GameManager.UnRegisterPlayer(transform.name);
    }

}
