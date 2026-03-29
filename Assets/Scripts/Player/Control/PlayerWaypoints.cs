using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWaypoints : MonoBehaviour
{
    [SerializeField] MaskChange maskChange;
    [SerializeField] GameObject MapWindow;


    [SerializeField] Button[] teleportButton;
    [SerializeField] Transform[] TeleportPointer;

    [SerializeField] Transform[] savePointer;

    private void Start()
    {
        for (int i = 0; i < teleportButton.Length; i++)
        {
            //람다식으로 버튼에 함수 연결
            int index = i; //i로 바로 쓸 경우 오류
            teleportButton[index].onClick.AddListener(() => TeleportPlayer(TeleportPointer[index]));
        }
    }

    public void TeleportPlayer(Transform pointer)
    {
        maskChange.CurrentMask.transform.position = pointer.position;
        MapWindow.SetActive(false);
    }
}
