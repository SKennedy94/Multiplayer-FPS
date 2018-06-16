using UnityEngine;

public class PlayerUI : MonoBehaviour {

    [SerializeField]
    RectTransform thrusterFuelFill;

    [SerializeField]
    RectTransform healthBarFill;

    [SerializeField]
    GameObject pauseMenu;

    private Player player;
    private PlayerController controller;

    public void SetPlayer(Player _player)
    {
        player = _player;
        controller = player.GetComponent<PlayerController>();
    }

    private void Start()
    {
        PauseMenu.IsOn = false;
    }

    private void Update()
    {
        SetFuelAmount(controller.GetThrusterFuelAmount());
        SetHealthAmount(player.GetHealthPct());
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }

    void SetFuelAmount(float _amount)
    {
        thrusterFuelFill.localScale = new Vector3(1f, _amount, 1f);
    }

    void SetHealthAmount(float _amount)
    {
        healthBarFill.localScale = new Vector3(1f, _amount, 1f);
    }
}
