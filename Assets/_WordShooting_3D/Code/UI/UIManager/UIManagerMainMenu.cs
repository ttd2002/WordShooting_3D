using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManagerMainMenu : SingletonAbstract<UIManagerMainMenu>
{
    [SerializeField] private UIRoomsManager rooms;
    public UIRoomsManager Rooms => rooms;
    [SerializeField] private Transform roomDetail;
    public Transform RoomDetail => roomDetail;
    [SerializeField] private Transform loginMenu;
    public Transform LoginMenu => loginMenu;
    [SerializeField] private Transform mainMenu;
    public Transform MainMenu => mainMenu;
    [SerializeField] private PanelManager panelManager;
    public PanelManager PanelManager => panelManager;
    [SerializeField] private Transform loading;
    public Transform Loading => loading;

    public void OpenRoomDetail()
    {
        this.rooms.gameObject.SetActive(false);
        this.roomDetail.gameObject.SetActive(true);
    }
    public void CloseRoomDetail()
    {
        this.rooms.gameObject.SetActive(true);
        this.roomDetail.gameObject.SetActive(false);
    }
    public void OpenMainMenu()
    {
        this.panelManager.CloseCurrent();
        this.mainMenu.gameObject.SetActive(true);
        this.panelManager.OpenPanel(mainMenu.GetComponent<Animator>());
    }
    public void SetTitleRoomDetail(string sessionName)
    {
        Text roomTitle = roomDetail.transform.Find("RoomDetailTitle").Find("TitleLabel").GetComponent<Text>();
        roomTitle.text = sessionName;
    }

    public void EnableStartButton(bool isEnabled)
    {
        Button startButton = roomDetail.transform.Find("VerticalGroup").Find("HorizontalGroup").Find("StartButton").GetComponent<Button>();
        startButton.interactable = isEnabled;
    }
    protected override void LoadComponents()
    {
        base.LoadComponents();
        this.LoadRooms();
        this.LoadRoomDetail();
        this.LoadMainMenu();
        this.LoadLoginMenu();
        this.LoadPanelManager();
        this.LoadLoadingCircle();
    }

    protected virtual void LoadRooms()
    {
        if (this.rooms != null) return;
        this.rooms = transform.parent.Find("Rooms").GetComponent<UIRoomsManager>();
        Debug.Log(transform.name + ": LoadRooms", gameObject);
    }
    protected virtual void LoadRoomDetail()
    {
        if (this.roomDetail != null) return;
        this.roomDetail = transform.parent.Find("RoomDetail");
        Debug.Log(transform.name + ": LoadRoomDetail", gameObject);
    }
    protected virtual void LoadLoginMenu()
    {
        if (this.loginMenu != null) return;
        this.loginMenu = transform.parent.Find("LoginMenu");
        Debug.Log(transform.name + ": LoadLoginMenu", gameObject);
    }
    protected virtual void LoadMainMenu()
    {
        if (this.mainMenu != null) return;
        this.mainMenu = transform.parent.Find("MainMenu");
        Debug.Log(transform.name + ": LoadMainMenu", gameObject);
    }
    protected virtual void LoadPanelManager()
    {
        if (this.panelManager != null) return;
        this.panelManager = transform.GetComponent<PanelManager>();
        Debug.Log(transform.name + ": LoadPanelManager", gameObject);
    }
    protected virtual void LoadLoadingCircle()
    {
        if (this.loading != null) return;
        this.loading = transform.parent.Find("LoadingCircle");
        Debug.Log(transform.name + ": LoadLoadingCircle", gameObject);
    }
}
