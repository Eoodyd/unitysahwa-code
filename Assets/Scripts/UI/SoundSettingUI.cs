using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class SoundSettingUI : MonoBehaviour
{
    private SaveManager saveManager;

    [SerializeField] private AudioMixer enemyAudioMixer;
    [SerializeField] private AudioMixer playerAudioMixer;
    [SerializeField] private AudioMixer envAudioMixer;

    [SerializeField] private Slider masterAudioSlider;
    [SerializeField] private Slider BGMAudioSlider;
    [SerializeField] private Slider enemySFXSlider;
    [SerializeField] private Slider playerSFXSlider;

    private void Awake()
    {
        //float �Ű����� 1�� ��������.
        masterAudioSlider.onValueChanged.AddListener(SetMasterVolume);
        BGMAudioSlider.onValueChanged.AddListener(SetBGM);
        enemySFXSlider.onValueChanged.AddListener(SetEnemySFX);
        playerSFXSlider.onValueChanged.AddListener(SetPlayerSFX);
    }

    private void Start()
    {
        saveManager = SaveManager.instance;
        LoadVolumeData();
    }

    #region SetVolume
    public void SetMasterVolume(float value)
    {//�����̴� ���� �ٷ� �޾Ƽ� ���
        enemyAudioMixer.SetFloat("Master", value);
        playerAudioMixer.SetFloat("Master", value);
        envAudioMixer.SetFloat("Master", value);
    }
    public void SetBGM(float value)
    {//�����̴� ���� �ٷ� �޾Ƽ� ���

        envAudioMixer.SetFloat("BGM", value);
        envAudioMixer.SetFloat("SFX", value);
    }
    public void SetEnemySFX(float value)
    {//�����̴� ���� �ٷ� �޾Ƽ� ���

        enemyAudioMixer.SetFloat("SFX", value);
    }

    public void SetPlayerSFX(float value)
    {//�����̴� ���� �ٷ� �޾Ƽ� ���

        playerAudioMixer.SetFloat("SFX", value);
    }

    #endregion

    public void MuteAudio()
    {
        SetMasterVolume(0);
    }

    public void SaveVolumeData()
    {//���� â���� ������ �������� ������ ���

        saveManager.ChangeVolumeSetting(masterAudioSlider.value, BGMAudioSlider.value, enemySFXSlider.value, playerSFXSlider.value);
        saveManager.SaveSoundData();
    }

    public void LoadVolumeData()
    {//saveManager ��ũ��Ʈ�� ������ġ �ҷ�����(������ ���� �ҷ����� ����)

        //����� ���� �纯ȯ �� 0~1���� ����� �� �� 
        SetMasterVolume(saveManager.masterVolume);
        SetBGM(saveManager.BGMVolume);

        SetEnemySFX(saveManager.enemySFXVolume);
        SetPlayerSFX(saveManager.playerSFXVolume);

        //�纯ȯ �� UI �����̴� �� ����.
        masterAudioSlider.value = saveManager.masterVolume;
        BGMAudioSlider.value = saveManager.BGMVolume;
        enemySFXSlider.value = saveManager.enemySFXVolume;
        playerSFXSlider.value = saveManager.playerSFXVolume;
    }
}
