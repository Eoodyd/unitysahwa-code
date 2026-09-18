using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region ---[Enum]---------------------------------------------------------------------------------------------------------------------------------
#endregion

#region SkillCooldown

public enum SkillCooldown
{
    INKSHAPE,
    LEAPSTRIKE,
    FINISH,
    DASH
}
#endregion

#region MaskType
public enum MaskType
{
    HUMAN,
    ANIMAL,
    GHOST
}
#endregion

#region FunctionTarget
public enum FunctionTarget
{
    PLAYER,
    HUMAN_WEAPON,
    ANIMAL_LEFTHAND,
    ANIMAL_RIGHTHAND,
    ANIMAL_LEFTARM,
    ANIMAL_RIGHTARM,
    CURRENTENEMY,
}
#endregion

#region HitBox Type

public enum HitBoxType
{
    HUMAN_NORMALATTACK_FIRST,
    HUMAN_NORMALATTACK_SECOND,
    HUMAN_NORMALATTACK_THIRD,
    HUMAN_INKSHAPE,

    ANIMAL_NORMALATTACK_FIRST,
    ANIMAL_NORMALATTACK_SECOND,
    ANIMAL_NORMALATTACK_THIRD,
    ANIMAL_LEAPSTRIKE
}

#endregion

#region CameraShakeType
public enum CameraShakeType
{
    IMPULSE_RECOIL,
    IMPULSE_BUMP,
    IMPULSE_EXPOLOSION,
    IMPULSE_RUMBLE
}
#endregion

#region CameraReactionType
public enum CameraReactionType
{
    NOISE_6DSHAKE,
    NOISE_6DWOBBLE,
    HANDHELD_NORMAL_EXTREME,
    HANDHELD_NORMAL_MILD,
    HANDHELD_NORMAL_STRONG,
    HANDHELD_TELE_MILD,
    HANDHELD_TELE_STRONG,
    HANDHELD_WIDEANGLE_MILD,
    HANDHELD_WIDEANGLE_STRONG,
}
#endregion

#region TimeScaleApplyTarget
public enum TimeScaleApplyTarget
{
    GAME,
    VFX
}
#endregion

#region PlayerStateType
public enum PlayerStateType
{
    NONE,
    IDLE,
    WALK,
    HUMAN_NORMALATTACK,
    HUMAN_INKSHAPE,
    HUMAN_INKFLOOR,
    ANIMAL_NORMALATTACK,
    ANIMAL_LEAPSTRIKE,
    ANIMAL_ROAR,
    GHOST_FINISHSKILL,
    DASH,
    HIT,
    DEAD
}
#endregion

#region PlayerSubStateType
public enum PlayerSubStateType
{
    NONE,
    WALK_GROUND,
    WALK_WATER,
    DEAD_HPZERO,
    DEAD_FALL,
    HIT_DEFAULT,

    //���Ż
    HUMAN_FIRSTNORMALATTACK,
    HUMAN_SECONDNORMALATTACK,
    HUMAN_THIRDNORMALATTACK,

    //����Ż
    ANIMAL_FIRSTNORMALATTACK,
    ANIMAL_SECONDNORMALATTACK,
    ANIMAL_THIRDNORMALATTACK,
}
#endregion

#region RestrictionType
public enum PlayerRestrictionType
{
    ACT,
    MOVE,
    ROTATE
}
#endregion

#region MoveDirection
public enum MoveDirection
{
    FRONT,
    BACK,
    UP,
    DOWN,
}
#endregion

#region ---[Struct]------------------------------------------------------------------------------------------------------------------------------------
#endregion

#region PlayerBasicStat
[System.Serializable]
public struct PlayerBasicStatStruct
{
    [Header("��ų ����")]
    public float damage;
    public float cooldown;
    public float inkStack;
}


#endregion

#region FunctionStop
[System.Serializable]
public struct FunctionStopStruct
{
    public float waitTime;
    public float duration;
}
#endregion

#region Restrict
[System.Serializable]
public struct RestrictStruct
{
    [Header("��ų �ൿ ����")]

    public float actRestrictWaitTime;
    public float actRestrictDuration;

    public float moveRestrictWaitTime;
    public float moveRestrictDuration;

    public float rotateRestrictWaitTime;
    public float rotateRestrictDuration;
}
#endregion

#region animationSpeed
[System.Serializable]
public struct animationSpeedStruct
{
    [Range(0, 1)] public float startTime;
    [Range(0, 1)] public float endTime;
    public float animationSpeed;
    public animationSpeedStruct(float startTime = 0, float endTime = 1, float animationSpeed = 1)
    {
        this.startTime = 0;
        this.endTime = 1;
        this.animationSpeed = 1;
    }
}
#endregion

#region SkillMove
[System.Serializable]
public struct SkillMoveStruct
{
    public MoveDirection direction;
    public float moveSpeed;
    public float waitTime;
    public float duration;
}

#endregion

#region TimeScale
[System.Serializable]
public struct TimeScaleStruct
{
    public bool useFunction;
    [Range(0, 1)] public float timeScale;
    
    [Header("useFrame üũ: ������ ���������� ����, frame ���� �����ϱ�")]
    public bool useFrame;
    public float waitTimeFrames;
    public float durationFrames;

    [Space(10)]
    public float waitTimeSeconds;
    public float durationSeconds;

    public void Initialize(float tixxxmeScale, float waitTime, float duration)
    {

    }
}
#endregion

#region PlayerEffect
[System.Serializable]
public struct EffectStruct
{
    public bool useFunction;
    public float waitTime;
    public float duration;

    [Header("untilFinish: ����Ʈ ���������� ��� \nfollowPosition: �������� ��� ����ٴ�")]
    public bool untilFinish;

    [Space(10)]
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;

    public bool followPosition;
    public float followWaitTime;
    public float followDuration;
}
#endregion

#region PlayerHitBox
[System.Serializable]
public struct HitBoxStruct
{
    public bool useFunction;
    public bool showHitbox;
    public bool untilFinish;

    [Space(10)]
    public float waitTime;
    public float duration;

    [Space(10)]
    public HitBoxType hitBoxType;
}
#endregion

#region PlayerSound
[System.Serializable]
public struct SoundStruct
{
    //"����� ���� �����̳�" �����ֱ���
    public bool useFunction;
    public AudioClip[] audioClip;
    public bool loop;
    public float volume;
    public float pitch;
    public float waitTime;

    [Range(0, 1)] public float spatialBlend;
    public float MinDistance;
    public float MaxDistance;

    public bool untilFinish;
}

#endregion

#region CameraShake
[System.Serializable]
public struct CameraShakeStruct
{
    public bool useFunction;

    [Tooltip("cameraType: ����� ����Ǵ� ī�޶�")]
    public CameraType cameraType;
    [Tooltip("waitTime: waitTime ���Ŀ� ��� ����")]
    public float waitTime;
    
    [Space(10)]
    [Tooltip("shakeType: ī�޶� ����ũ ����")]
    public CameraShakeType shakeType;
    [Tooltip("imulseVelocty: ����ũ �ӵ�")]
    public Vector3 impulseVelocty;
    [Tooltip("impulseDuration: ����ũ �ð�")]
    public float impulseDuration;
    
    [Space(10)]
    [Header("reaction�� ����ũ�� ī�޶� �����ϴ� �� ���մϴ�")]
    [Tooltip("reactionType: ����ũ�� �����ϴ� Ÿ��")]
    public CameraReactionType reactionType;
    [Tooltip("amplitudeGain: ���� ���̸� ī�޶� ��鸲�� ������ ����")]
    public float amplitudeGain;
    [Tooltip("frequencyGain: ���� ���̸� ī�޶� ��鸲�� �ӵ��� ����")]
    public float frequencyGain;
    [Tooltip("reactionDuration: �����ϴ� �ð�")]
    public float reactionDuration; //ī�޶� ��� ���� duration�� ����
}
#endregion

#region CameraRecomposer
[System.Serializable]
public struct CameraRecomposerStruct
{
    public bool useFunction;
    [Tooltip("waitTime: waitTime ���Ŀ� ��� ����")]
    public float waitTime;
    [Tooltip("duration: waitTime�� ������ duratio ���� ��� ����")]
    public float duration;
    [Tooltip("���ϴ� ����")]
    public float zoomScale;
    [Tooltip("ī�޶� Ÿ���� ���󰡴� ����")]
    public float followAttachment;
    [Tooltip("ī�޶� Ÿ���� �ٶ󺸴� ����")]
    public float lookAtAttachment;
}
#endregion

#region CameraZoom
[System.Serializable]
public struct CameraZoomStruct
{
    [Header("isZoomInFirst üũ: Zoom In ��������")]
    public float isZoomInFirst;
    public float waitTime;
    public float firstDuration;
    public float secondDuration;

    [Header("ī�޶� Ÿ�� �����ϰ� positionOffset���� ��������")]
    public FunctionTarget cameraTarget;
    public float positionOffset;

    public void Initialize(float isZoomInFirst, float waitTime, float firstDuration, float secondDuration, FunctionTarget cameraTarget, float positionOffset)
    {
        this.isZoomInFirst = isZoomInFirst;
        this.waitTime = waitTime;
        this.firstDuration = firstDuration;
        this.secondDuration = secondDuration;

        this.cameraTarget = cameraTarget;
        this.positionOffset = positionOffset;
    }
}
#endregion

#region CameraPosition
[System.Serializable]
public struct CameraPositionOffeset
{
    public float cameraZoomWaitTime;
    public float cameraZoomDuration;
    public GameObject cmaeraZoomTarget;

    public void Initialize(float cameraZoomWaitTime, float cameraZoomDuration, GameObject cmaeraZoomTarget)
    {
        this.cameraZoomWaitTime = cameraZoomWaitTime;
        this.cameraZoomDuration = cameraZoomDuration;
        this.cmaeraZoomTarget = cmaeraZoomTarget;
    }
}
#endregion

#region PlayerSkillInput
[System.Serializable]
public struct PlayerSkillInputStruct
{
    public float storeWaitTime;
    public float storeDuration;
    public float executeWaitTime;
    public float executeDuration;
}
#endregion

#region PostProcessing-Vignette
[System.Serializable]
public struct VignetteStruct
{
    public bool useFunction;
    public Color color;
    public Vector2 center;
    [Range(0, 1)] public float intensity;
    [Range(0,1)] public float smoothness;
    public bool rounded;

    //�ݺ��ؼ� ����� ���ΰ�
    public bool isLooping;
    //��ȸ �ݺ��� ���ΰ�
    public int count;
    //�� �ֱ� Ŀ��
    public AnimationCurve oneTimeCurve;
    //�����ð�
    public float waitTime;
    //1ȸ ����ð�
    public float oneTimeDuration;
}


#endregion