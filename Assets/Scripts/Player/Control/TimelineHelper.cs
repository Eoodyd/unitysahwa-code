using Cinemachine.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class TimelineHelper : MonoBehaviour
{
    public static TimelineHelper instance;

    #region ��ũ��Ʈ
    [SerializeField] private MaskChange maskChange;
    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerSound playerSound;

    [SerializeField] private MenuUI menuUI;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private UIEffect cameraUIEffect;
    [SerializeField] private MouseSettingUI mouseSettingUI;
    #endregion

    #region ����
    [Space(20)]
    [Header("����")]
    private bool isTutorialScene;

    public GameObject startNewGameCamera;

    [SerializeField] private GameObject tutorialStartSceneCamera;
    [SerializeField] private GameObject tutorialStartSceneTimeline;
    private PlayableDirector tutorialStartlayableDirector;

    [SerializeField] private GameObject tutorialStartSceneSkipButton;
    [SerializeField] private GameObject[] tutorialStartSceneElement;

    [SerializeField] private GameObject playerGameStartPosition;
    [SerializeField] private GameObject playerTutorialStartPosition;
    #endregion

    [SerializeField] private PlayableDirector[] timelines;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this.gameObject);
    }

    private void Start()
    {
        //�ش� ��ũ��Ʈ�� ��ȹ���� Ÿ�Ӷ��� ��ý�Ʈ�� ���� ������.
        //���и��� ���� tutorial ���� �� ���� ���� ������ ���еǰ� �Ǿ���.

        //ù��° �� + ��ŸƮ�޴� Ȱ��ȭ + ���θ޴� �÷��� �ƴ� ��
        if (SceneManager.GetActiveScene().buildIndex == 0 && menuUI.MainMenu.activeSelf && !SceneSwitcher.instance.IsNotPlayingMainMenu)
        {
            isTutorialScene = true;
            DisablePlayerControl(true);
            maskChange.HumanMask.transform.position = playerGameStartPosition.transform.position;
            maskChange.HumanMask.transform.rotation = playerGameStartPosition.transform.rotation;

            if (tutorialStartSceneSkipButton != null) tutorialStartSceneSkipButton.SetActive(false);
            if (tutorialStartSceneTimeline != null) tutorialStartlayableDirector = tutorialStartSceneTimeline.GetComponent<PlayableDirector>();
        }

        else
        {
            isTutorialScene = false;
            menuUI.MainMenu.SetActive(false);
            DisablePlayerControl(false);

            //���θ޴� �ǳʶٱ� ���� ó��
            SceneSwitcher.instance.IsNotPlayingMainMenu = false;
        }

        if (startNewGameCamera != null)
        {
            startNewGameCamera.SetActive(isTutorialScene);
        }
        else
        {
            Debug.LogWarning("startNewGameCamera == null");
        }
    }

    #region Ʃ�丮�� ��
    public void StartTutorialTimeline()
    {
        StartCoroutine(CoStartTutorialTimeline());
    }

    public IEnumerator CoStartTutorialTimeline()
    {
        FadeOutScreen(2);
        maskChange.HumanMask.transform.position = playerTutorialStartPosition.transform.position;
        maskChange.HumanMask.transform.rotation = Quaternion.Euler(0, 90, 0);

        yield return new WaitForSeconds(1f);
        
        menuUI.MainMenu.SetActive(false);
        menuUI.NewGameWindow.SetActive(false);
        menuUI.StoryImageWindow.SetActive(false);
        MenuUI.instance.ActivateLetterBox(false);
        yield return new WaitForSeconds(.5f);

        FadeInScreen(.5f);
        tutorialStartlayableDirector.Play();
    }

    public IEnumerator CoSkipTutorialStartScene()
    {
        FadeOutScreen(2);

        yield return new WaitForSeconds(.5f);

        for (int i = 0; i < tutorialStartSceneElement.Length; i++)
        {
            if (tutorialStartSceneElement[i].activeSelf)
            {
                tutorialStartSceneElement[i].SetActive(false);
            }
        }

        //Ŀ�� ����
        if (PlatformSwitcher.instance.IsPCPlatform)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }

        tutorialStartSceneCamera.SetActive(false);
        tutorialStartSceneTimeline.SetActive(false);
        tutorialStartSceneSkipButton.SetActive(false);

        //������ �ٶ󺸵���
        yield return new WaitForSeconds(1f);

        FadeInScreen(2);
        DisablePlayerControl(false);
    }

    public void SkipTutorialStartScene()
    {
        StartCoroutine(CoSkipTutorialStartScene());
    }
    #endregion

    public bool IsTimelinePlaying()
    {
        bool isTimelinePlaying = false;

        for (int i = 0; i < timelines.Length; i++)
        {
            if (timelines[i] == null)
            {
                Debug.LogWarning($"timelines[{i}] == null");
                continue;
            }

            if (timelines[i].state == PlayState.Playing)
            {
                isTimelinePlaying = true;
            }
        }

        return isTimelinePlaying;
    }

    public void DisablePlayerControl(bool disableControl)
    {
        menuUI.DisablePlayerControl(disableControl);

        //Ÿ�Ӷ��� ���ȿ��� ����޴���밡��
        menuUI.CanShowPauseMenu(true);

        if (disableControl)
        {//������ �� ����
            playerSound.StopLoopingAudio();
            playerState.ChangePlayerState(PlayerStateType.NONE);
            playerState.ChangePlayerSubState(PlayerSubStateType.NONE);

            mouseSettingUI.SetXAxisValue(0);
            mouseSettingUI.SetYAxisValue(0);

            if (!PlatformSwitcher.instance.IsPCPlatform)
            {
                menuUI.InputHUD.SetActive(false);
            }
        }
        else
        {//������ �� ����
            mouseSettingUI.LoadMouseData();
            maskChange.HumanRigidbody.isKinematic = false;

            //Ÿ�Ӷ��� ���� ���� �ΰ��� ������ ��
            if (IsTimelinePlaying())
            {
                if (PlatformSwitcher.instance.IsPCPlatform)
                {
                    Cursor.visible = false;
                    Cursor.lockState = CursorLockMode.Locked;
                }
                else
                {
                    Cursor.visible = true;
                    Cursor.lockState = CursorLockMode.Confined;
                }
            }

            if (!PlatformSwitcher.instance.IsPCPlatform)
            {
                menuUI.InputHUD.SetActive(true);
            }
        }
    }

    public void FadeInScreen(float timeRate)
    {
        //���� ȭ������ ��ȯ
        cameraUIEffect.FadeInScreen(timeRate);
    }

    public void FadeOutScreen(float timeRate)
    {
        //�˰� ��ȯ
        cameraUIEffect.FadeOutScreen(timeRate);
    }
}