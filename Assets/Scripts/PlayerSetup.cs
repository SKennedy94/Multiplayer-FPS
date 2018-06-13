using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
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
    private GameObject playerUIInstance;

    Camera sceneCamera;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        } else
        {
            //disable scene camera if local player
            sceneCamera = Camera.main;
            if(sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(false);
            }

            //Disable player graphics for local player
            Utility.SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayerName));

            //create PlayerUI for local player
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;
        }

        GetComponent<Player>().Setup();
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

        //Re-enable the scene camera
        if (sceneCamera != null)
        {
            sceneCamera.gameObject.SetActive(true);
        }

        GameManager.UnRegisterPlayer(transform.name);
    }

}
