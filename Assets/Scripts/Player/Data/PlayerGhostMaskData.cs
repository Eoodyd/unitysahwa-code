using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;




public partial class PlayerGhostMaskData : ScriptableObject
{
    [Header("����")]
    [Header("�ͽ�Ż ��ų ��Ÿ��")]
    public float cooldown = 20;
    [Header("ó�� Ÿ�� Ž�� ����")]
    public float detectRange=20;
    [Header("ó�� ���� ����")]
    public float viewAngle=100;
    [Header("ó�� ��ų ����")]
    public float skillRange=20;
}

public partial class PlayerGhostMaskData : ScriptableObject
{
    [Space(20)]
    [Header("���Ż ó��")]
    [Tooltip("������ ����ð�")]
    public float humanSetGhostWeaponTime;
    [Tooltip("���� ����� ����ð�")]
    public float humanSetOriginalWeaponTime;
    [Tooltip("�� ������� �ð�")]
    public float humanKillTargetTime;

    [Space(10)]
    [Tooltip("���� �̵�")]
    public SkillMoveStruct[] humanSkillMove;
    [Tooltip("�ൿ ����")]
    public RestrictStruct humanRestrict;
    [Tooltip("�ִϸ��̼� �ӵ�")]
    public animationSpeedStruct[] huamnHitGroundAnimationSpeed;
    public animationSpeedStruct[] humanSwingAnimationSpeed;

    [Space(10)]
    [Tooltip("�� ����Ʈ")]
    public EffectStruct humanCutEffect;
    [Tooltip("�� ����Ʈ")]
    public EffectStruct humanDomeEffect;
    
    [Space(10)]
    [Tooltip("��ų ����")]
    public SoundStruct humanHitGroundSound;
    public SoundStruct humanSwingSound;
    public SoundStruct humanAfterSwingSound;

    [Space(10)]
    [Header("ī�޶� ����ũ")]
    public CameraShakeStruct humanFinishSwingCameraShake;
    public CameraShakeStruct humanFinishHitGroundCameraShake;

    [Space(10)]
    [Header("Ÿ�ӽ�����")]
    public TimeScaleStruct humanFinishTimeScale;
}

public partial class PlayerGhostMaskData : ScriptableObject
{
    [Space(20)]
    [Header("����Ż ó��")]
    [Tooltip("������ ����ð�")]
    public float animalSetGhostWeaponTime;
    [Tooltip("���� ����� ����ð�")]
    public float animalSetOriginalWeaponTime;
    [Tooltip("�� ������� �ð�")]
    public float animalKillTargetTime;


    [Space(10)]
    [Tooltip("���� �̵�")]
    public SkillMoveStruct[] animalSkillMove;
    [Tooltip("�ൿ ����")]
    public RestrictStruct animalRestrict;
    [Tooltip("�ִϸ��̼� �ӵ�")]
    public animationSpeedStruct[] animalSweapAnimationSpeed;
    public animationSpeedStruct[] animalSwingAnimationSpeed;


    [Space(10)]
    [Tooltip("�� ����Ʈ")]
    public EffectStruct animalCutEffect;
    [Tooltip("�� ����Ʈ")]
    public EffectStruct animalDomeEffect;
    
    [Space(10)]
    [Tooltip("��ų ����")]
    public SoundStruct animalSweapSound;
    public SoundStruct animalSwingSound;
    public SoundStruct animalAfterSwingSound;
    
    [Space(10)]
    [Header("ī�޶� ����ũ")]
    public CameraShakeStruct animalFinishSweapCameraShake;
    public CameraShakeStruct animalFinishSwingCameraShake;

    [Space(10)]
    [Header("Ÿ�ӽ�����")]
    public TimeScaleStruct animalFinishTimeScale;

}

public partial class PlayerGhostMaskData : ScriptableObject
{
    #region �̱���
    //�������� �̱���: �̱��� ������Ʈ�� �����ϴ� ������ ������Ʈ�� ���ٸ� �������

    private const string SettingFileDirectory = "Assets/Resources";
    private const string SettingFilePath = "Assets/Resources/PlayerGhostMaskData.asset";
    //���ҽ��� �ǵ��� ���X, �������� ���Ǵ� ���� ū ���� ���ٰ� ��
    //Resources ������ ������ Ȯ��, ������ ����

    private static PlayerGhostMaskData instance;
    public static PlayerGhostMaskData Instance
    {
        get
        {
            if (instance != null) //instance�� �����Ѵٸ� ��������
            {
                return instance;
            }

            //���ٸ� 
            instance = Resources.Load<PlayerGhostMaskData>("PlayerGhostMaskData");

            //������ Ÿ�ӿ��� �ڵ����� �̸� �����ǵ���, ��Ÿ�ӿ� ������ �־����.
#if UNITY_EDITOR

            if (instance == null)
            {
                if (!AssetDatabase.IsValidFolder(SettingFileDirectory))//�ش� ������ ��ȿ����
                {
                    AssetDatabase.CreateFolder("Assets", "Resources"); //�ƴ϶�� Assets �Ʒ��� Resources ���� ����
                }

                //� ������ ������ �Ȱ����Դٸ� �ϵ��ϰ� ��������
                instance = AssetDatabase.LoadAssetAtPath<PlayerGhostMaskData>(SettingFilePath);

                if (instance == null) //�׷����� �Ȱ��������ٸ� ���ٴ� ��. ���� ������ֱ�
                {
                    instance = CreateInstance<PlayerGhostMaskData>(); //�̷��� �����ϸ� �޸𸮿��� ����. ���� �������� ������ �ȵ�
                    AssetDatabase.CreateAsset(instance, SettingFilePath); //��� ������ ������Ʈ�� ����Ƽ�������� ����, ������ �� �ְ�
                }

            }
#endif
            return instance;
        }
    }
    #endregion
}