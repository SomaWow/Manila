using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PersonalInterface : MonoBehaviour {
    private Image headPortrait;
    private Text txt_UserName;
    private Text txt_Win;
    private Text txt_Lose;
    private Button btn_StartGame;

    private void Awake()
    {
        EventCenter.AddListener(EventType.BackPersonalInterface, BackPersonalInterface);
        if (Models.GameModel.userDto.IconName == "default")
        {
            gameObject.SetActive(false);
            EventCenter.Broadcast(EventType.ChooseHeadIcon);
            return;
        }

        Init();
    }
    private void OnDestroy()
    {
        EventCenter.RemoveListener(EventType.BackPersonalInterface, BackPersonalInterface);
    }
    private void Init()
    {

        headPortrait = transform.Find("headPortrait").GetComponent<Image>();
        txt_UserName = transform.Find("UserName").GetComponent<Text>();
        txt_Win = transform.Find("Win").GetComponent<Text>();
        txt_Lose = transform.Find("Lose").GetComponent<Text>();
        btn_StartGame = transform.Find("btn_StartGame").GetComponent<Button>();

        txt_UserName.text = Models.GameModel.userDto.UserName;
        txt_Win.text = "获胜： "+ Models.GameModel.userDto.Win.ToString();
        txt_Lose.text = "失败： "+ Models.GameModel.userDto.Lose.ToString();
        headPortrait.sprite = ResourcesManager.GetSprite(Models.GameModel.userDto.IconName);

        btn_StartGame.onClick.AddListener(()=> {
            EnterRoom();
        });
        
    }

    private void BackPersonalInterface()
    {
        gameObject.SetActive(true);
        Init();
    }

    private void EnterRoom()
    {
        SceneManager.LoadScene("3.Main");
    }

}
