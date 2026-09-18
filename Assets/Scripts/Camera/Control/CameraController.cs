using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum CameraType
{
    DEFAULT,
    LOCKON,
    FINISHSKILL
}

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    #region �ܺ�
    [SerializeField] private PlayerController playerController;
    [SerializeField] private SaveManager saveManager;

    private MaskChange maskChange;
    private Player player;

    [SerializeField] private PlayerState playerState;
    [SerializeField] private MenuUI menuUI;

    //������
    private PlayerCommonData commonData;
    private CameraData cameraData;
    #endregion
        
    #region ����
    [SerializeField] private Camera mainCamera;
    public Camera MainCamera
    {
        get
        { return mainCamera; }

        private set 
        {
            mainCamera = value; 
        }
    }

    [SerializeField] private CinemachineFreeLook defaultCamera;
    public CinemachineFreeLook DefaultCamera
    {
        get
        {
            return defaultCamera;
        }
    }

    [SerializeField] private CinemachineVirtualCamera lockOnCamera;
    private CinemachineTransposer lockOnCameraTransposer;
    private CinemachineGroupComposer lockOnCameraGroupComposer;
    [SerializeField] private CinemachineVirtualCamera finishSkillCamera;
    
    [SerializeField] private Camera terrainLoadCamera;
    public Camera TerrainLoadCamera
    {
        get
        {
            return terrainLoadCamera;
        }
    }
    #endregion

    [SerializeField] private bool isLockOnTarget;
    [SerializeField] private bool isCurrentEnemyDead;

    #region Camera Move
    [SerializeField] private VariableJoystick joystick;
    #endregion

    #region �ָ�
    public Collider visibleTarget { get; private set; } //���̴� Ÿ��
    public bool isTargetDetected { get; private set; } //�ֺ��� �����Ǵ� Ÿ��
    public bool isTargetWithMaxStack { get; private set; } //�ֺ��� ��ũ Ǯ������ Ÿ���� �ִ���

    //���� Ÿ��
    private Collider currentTarget;
    public Collider CurrentTarget { get { return currentTarget; } }
    
    //ī�޶� �ٶ󺸴� Ÿ���� ��ġ
    private Transform headTransform;
    private Transform targetTransform;

    [SerializeField] private Image targetMarker;

    [SerializeField] CinemachineTargetGroup targetGroup;

    [SerializeField] private Material outlineMaterial;
    private Material[] targetMaterials;
    private GameObject outlineTarget;

    #endregion

    private void Awake()
    {
        #region �̱���
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);
        #endregion

        AdjustResolution();
        Application.targetFrameRate = 60;
        
        lockOnCamera.gameObject.SetActive(true);
    }

    void Start()
    {
        CameraInitialSet();
    }
    private void Update()
    {
        if (playerState.playerCurrentState == PlayerStateType.GHOST_FINISHSKILL)
        {
            return;
        }

        //Ÿ�� �׽� Ž��
        DetectTargetAlways();
        CheckCurrentTargetState();
        ControlTargetMarker();
        CheckOutlineTarget();
        CameraHorizontalSwipe();
    }

    private void CameraInitialSet()
    {
        #region �ܺ�
        maskChange = playerController.maskChange;
        player = playerController.player;
        
        commonData = PlayerCommonData.Instance;
        cameraData = CameraData.Instance;
        #endregion

        #region ī�޶�
        lockOnCameraTransposer = lockOnCamera.GetCinemachineComponent<CinemachineTransposer>();
        lockOnCameraGroupComposer = lockOnCamera.GetCinemachineComponent<CinemachineGroupComposer>();

        defaultCamera.gameObject.SetActive(true);
        lockOnCamera.gameObject.SetActive(false);
        #endregion

        #region ���� ��Ŀ
        targetMarker.gameObject.SetActive(true);
        targetMarker.color = cameraData.detectedTargetMarkerColor;
        #endregion

        //���콺
        if (menuUI.MainMenu.activeSelf)
        {
            defaultCamera.m_XAxis.m_MaxSpeed = 0;
            defaultCamera.m_YAxis.m_MaxSpeed = 0;
        }
        else
        {
            defaultCamera.m_XAxis.m_MaxSpeed = saveManager.mouseSpeedWithXAxis;
            defaultCamera.m_YAxis.m_MaxSpeed = saveManager.mouseSpeedWithYAxis;
        }

        //ī�޶� �ʱ⿡ ��ġ�� ����
        StartCoroutine(HoldDefaultCameraValue());
    }
    private void AdjustResolution()
    {
        int targetWidth; // Target horizontal resolution
        int targetHeight; // Target vertical resolution
#if UNITY_ANDROID
        targetWidth = 1280;
        targetHeight = 720;
#else
        targetWidth = 1920;
        targetHeight = 1080;
#endif

        // Calculate the device's aspect ratio
        float deviceAspectRatio = (float)Screen.width / Screen.height;

        // Calculate the target pixel count
        int targetPixelCount = targetWidth * targetHeight;

        // Compute adjusted resolution to match target pixel count
        float adjustedHeight = Mathf.Sqrt(targetPixelCount / deviceAspectRatio);
        float adjustedWidth = adjustedHeight * deviceAspectRatio;

        // Log the adjusted resolution for debugging
        Debug.Log($"Adjusted Resolution: {adjustedWidth}x{adjustedHeight}");

        // Set the screen resolution (fullscreen mode)
        Screen.SetResolution((int)adjustedWidth, (int)adjustedHeight, true);
    }
    public void SetPCPlatform(bool isSet)
    {
        if (isSet)
        {
            //����Ʈ ī�޶� ȸ��������
            defaultCamera.m_YAxis.m_InputAxisName = "Mouse Y";
            defaultCamera.m_XAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            defaultCamera.m_YAxis.m_InputAxisName = "";
            defaultCamera.m_XAxis.m_InputAxisName = "";
        }
    }
    public void ChangeCamera(CameraType cameraType)
    {
        if (cameraType == CameraType.LOCKON)
        {
            defaultCamera.gameObject.SetActive(false);
            lockOnCamera.gameObject.SetActive(true);
            finishSkillCamera.gameObject.SetActive(false);

            StartCoroutine(HoldLockOnCameraValue());

        }
        else if(cameraType == CameraType.FINISHSKILL)
        {
            defaultCamera.gameObject.SetActive(false);
            lockOnCamera.gameObject.SetActive(false);
            finishSkillCamera.gameObject.SetActive(true);
        }
        else
        {
            defaultCamera.gameObject.SetActive(true);
            lockOnCamera.gameObject.SetActive(false);
            finishSkillCamera.gameObject.SetActive(false);

            if (PlatformSwitcher.instance.IsPCPlatform == false)
            {
                StartCoroutine(HoldDefaultCameraValue());
            }
        }
    }
    public IEnumerator HoldDefaultCameraValue()
    {
        bool isBossScene = (SceneManager.GetActiveScene().buildIndex == 4);
        var defaultCameraTopRig = defaultCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>(); //Top Rig
        var defaultCameraMiddleRig = defaultCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>(); //Middle Rig
        var defaultCameraBottomRig = defaultCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>(); //Bottom Rig
        
        float time = 0;

        while(true)
        {
            time += Time.deltaTime;

            if (!defaultCamera.gameObject.activeSelf)
            {
                break;
            }

            if (isBossScene)
            {
                defaultCamera.m_Orbits[0].m_Height = 10;
                defaultCamera.m_Orbits[0].m_Radius = 15;
                defaultCameraTopRig.m_TrackedObjectOffset.y = 4.7f;

                defaultCamera.m_Orbits[1].m_Height = 5;
                defaultCamera.m_Orbits[1].m_Radius = 15;
                defaultCameraMiddleRig.m_TrackedObjectOffset.y = 2.5f;

                defaultCamera.m_Orbits[2].m_Height = 1;
                defaultCamera.m_Orbits[2].m_Radius = 11;
                defaultCameraBottomRig.m_TrackedObjectOffset.y = 1.4f;  
            }
            else
            {
                defaultCamera.m_YAxis.Value = 0.5f;
            }


            if (time >= 2)
            {
                break;
            }

            yield return null; 
        }
    }
    private IEnumerator HoldLockOnCameraValue()
    {
        bool isBossScene = (SceneManager.GetActiveScene().buildIndex == 4);

        float time = 0;

        Vector3 lockOnCameraFollowOffset = new Vector3(0, 2.04f, -8.57f);
        Vector3 lockOnCameraTrackedObjectOffset = new Vector3(0, 1.08f, 0f);

        Vector3 lockOnCameraFollowOffset_4 = new Vector3(0, 4.73f, -10.34f);
        Vector3 lockOnCameraTrackedObjectOffset_4 = new Vector3(0, 2.2f, 0f);

        while (true)
        {
            time += Time.deltaTime;

            if (!lockOnCamera.gameObject.activeSelf)
            {
                break;
            }

            if (isBossScene)
            {
                lockOnCameraGroupComposer.m_TrackedObjectOffset = lockOnCameraTrackedObjectOffset_4;
                lockOnCameraTransposer.m_FollowOffset = lockOnCameraFollowOffset_4;
            }
            else
            {
                lockOnCameraGroupComposer.m_TrackedObjectOffset = lockOnCameraTrackedObjectOffset;
                lockOnCameraTransposer.m_FollowOffset = lockOnCameraFollowOffset;
            }


            if (time >= 2)
            {
                break;
            }

            yield return null;
        }
    }

    public void SetMouseSpeed(bool isXAxis, float value)
    {
        if (isXAxis)
        {
            defaultCamera.m_XAxis.m_MaxSpeed = value;
        }
        else
        {
            defaultCamera.m_YAxis.m_MaxSpeed = value;
        }
    }
    public float GetMouseSpeed(bool isXAxis)
    {
        if (isXAxis)
        {
            return defaultCamera.m_XAxis.m_MaxSpeed;
        }
        else
        {
            return defaultCamera.m_YAxis.m_MaxSpeed;
        }
    }

    public void SetBossRoomCamera()
    {
        defaultCamera.m_Orbits[0].m_Height = 10;
        defaultCamera.m_Orbits[0].m_Radius = 15;
        defaultCamera.GetRig(0).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = 4.7f;

        defaultCamera.m_Orbits[1].m_Height = 5;
        defaultCamera.m_Orbits[1].m_Radius = 15;
        defaultCamera.GetRig(1).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = 2.5f;

        defaultCamera.m_Orbits[2].m_Height = 1;
        defaultCamera.m_Orbits[2].m_Radius = 11;
        defaultCamera.GetRig(2).GetCinemachineComponent<CinemachineComposer>().m_TrackedObjectOffset.y = 1.4f;
    }

    #region �ָ�
    private void DetectTargetAlways()
    {
        visibleTarget = null;

        if (playerState.playerCurrentState == PlayerStateType.DEAD)
        {
            return;
        }

        Collider[] colliders
            = Physics.OverlapSphere(maskChange.CurrentMask.transform.position, CameraData.Instance.detectRange, CameraData.Instance.enemyLayer);

        //�ֺ��� ���̾��ų� �÷��̾� ����� ��ȯ
        if (colliders.Length == 0 || colliders == null)
        {
            headTransform = null;
            isTargetDetected = false;
            return;
        }
        else
        {
            UIEffect.instance.ShowPlayerHUDFadeEffect();
            isTargetDetected = true;
        }

        #region ���ǿ� �´� Ÿ�� ����
        //����1: �þ߰� / idDead / �Ÿ�
        float smallestAngle = Mathf.Infinity;
       
        float smallestDistance = cameraData.distanceWithCloseTarget;
        bool isEnemyInRange = false;
        
        isTargetWithMaxStack = false;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.GetComponent<Enemy>() == null) continue; //���� ������ �н�
            if (colliders[i].gameObject.GetComponent<Enemy>().isDead) continue; //���� ������ �н�

            Vector3 directionTowardTarget = colliders[i].transform.position - MainCamera.transform.position;
            directionTowardTarget.y = 0;
            Vector3 cameraForward = player.transform.position - MainCamera.transform.position;
            cameraForward.y = 0;
            float angleWithTarget = Vector3.Angle(directionTowardTarget, cameraForward);

            Vector3 targetPosition = colliders[i].gameObject.transform.position;
            targetPosition.y = 0;
            Vector3 characterPosition = maskChange.CurrentMask.transform.position;
            characterPosition.y = 0;
            float distanceWithTarget = Vector3.Distance(characterPosition, targetPosition);

            //���� ������ ����� �н�
            if (angleWithTarget > cameraData.maximumAngleWithTarget) continue;
            //���� �Ÿ��� ����� �н�
            if (distanceWithTarget > cameraData.maximumDistanceWithTarget ) continue;

            #region Activate HUD
            if (colliders[i].GetComponent<CalliSystem>())
            {
                if (colliders[i].GetComponent<CalliSystem>().IsPaintOverMax())
                {
                    isTargetWithMaxStack = true;

                    if(SceneManager.GetActiveScene().buildIndex == 0)
                    {
                        if (colliders[i].GetComponent<EnemyMino>())
                        {
                            Invoke("InvokeFinishGuide", 1.2f);
                        }
                    }
                }
            }
            #endregion

            //������ ���� ���� ������Ʈ ����
            if (distanceWithTarget < smallestDistance)
            {
                smallestDistance = distanceWithTarget;
                visibleTarget = colliders[i];

                isEnemyInRange = true;
            }

            //����� ���� �ִٸ� �켱����
            if (isEnemyInRange) continue;

            if (angleWithTarget < smallestAngle)
            {
                //������ ���� ���� ������Ʈ ����
                smallestAngle = angleWithTarget;
                visibleTarget = colliders[i];
            }

            //����Ÿ���� ���ٸ� Ž���� Ÿ�� �Ӹ���ġ ����(��Ŀǥ�ø� ����) 
            if (currentTarget == null)
            {
                headTransform = visibleTarget.transform.Find("HeadPosition");
            }
        }
        #endregion
    }
   
    private void CheckOutlineTarget()
    {
        //visibleTarget�� ������ outline ����
        if (visibleTarget == null)
        {
            ClearOutline(); 
            return;
        }

        //visibleTarget�� �ְ� outlineTarget�� ������ outline �׸���
        else if((visibleTarget != null) && (outlineTarget == null))
        {
            DrawOutlineOnTarget();
            return;
        }

        //visibleTarget�� �ְ� outlineTarget�̶� ������ ��ȯ
        else if ((visibleTarget != null) && (visibleTarget.gameObject == outlineTarget.transform.parent.gameObject))
        {
            return;
        }

        //visibleTarget�� �ְ� outlineTarget�̶� �ٸ��� outline ����
        else if ((visibleTarget != null) && (visibleTarget.gameObject != outlineTarget.transform.parent.gameObject))
        {
            ClearOutline();
            return;
        }
    }
    private void DrawOutlineOnTarget()
    {
        //Ž���� Ÿ���� �ְ� Ž������ ���� �ִٸ� 
        if (visibleTarget.GetComponent<BrokenObject>() || visibleTarget.GetComponent<WisuSuppressionController>())
        {
            for (int i = 0; i < visibleTarget.transform.childCount; i++)
            {
                var childObject = visibleTarget.transform.GetChild(i);

                if (childObject.gameObject.CompareTag("OutlineTarget"))
                {
                    //outlineTarget�� target����
                    outlineTarget = childObject.gameObject;

                    //target�� ���� materials �ϴ� ����
                    var targetMeshRenderer = outlineTarget.GetComponent<MeshRenderer>();
                    targetMaterials = targetMeshRenderer.materials;

                    //outlineMaterial �ϳ� �߰����ֱ�
                    Material[] newMaterials = new Material[targetMaterials.Length + 1];
                    for (int j = 0; j < targetMaterials.Length; j++)
                    {
                        newMaterials[j] = targetMaterials[j];
                    }
                    newMaterials[newMaterials.Length - 1] = outlineMaterial;
                    targetMeshRenderer.materials = newMaterials;
                }
            }
        }
    }
    private void ClearOutline()
    {
        if (outlineTarget == null)
        {
            return;
        }

        var targetMeshRenderer = outlineTarget.GetComponent<MeshRenderer>();
        targetMeshRenderer.materials = targetMaterials;

        targetMaterials = null;
        outlineTarget = null;
    }
    private void InvokeFinishGuide()
    {
        PlayGuide.instance.ShowTutorialUI(TutorialState.FINISHATTACK);
    }
    private void CheckCurrentTargetState()
    {
        //���� ��� Ȱ�� ����
        if (isLockOnTarget)
        {
            //���� �׾��� �� �ָ� �ڵ� �̵�
            if (currentTarget == null || 
                currentTarget.IsDestroyed() || 
                (currentTarget.gameObject.TryGetComponent<Enemy>(out Enemy enemy) && enemy.isDead) ||
                (enemy.hpBarCount <= 0))
            {
                //�ֺ��� ���� ������ �ڵ� Ÿ��
                if (visibleTarget)
                {
                    targetGroup.RemoveMember(targetTransform);

                    isLockOnTarget = true;
                    currentTarget = visibleTarget;
                    visibleTarget = null;

                    headTransform = currentTarget.transform.Find("HeadPosition");

                    if (headTransform == null)
                    {
                        targetTransform = currentTarget.transform;

                    }
                    else
                    {
                        targetTransform = headTransform;
                    }

                    targetGroup.AddMember(targetTransform, 1f, 1);

                }
                else
                {
                    DeactivateLockOn();
                }
            }
            else if ( Vector3.Distance(maskChange.CurrentMask.transform.position, currentTarget.gameObject.transform.position) > cameraData.detectRange)
            {
                DeactivateLockOn();
            }
        }
    }
    public void DeactivateLockOn()
    {
        targetGroup.RemoveMember(targetTransform);

        maskChange.CurrentAnimator.SetBool("isFocused", false);

        isLockOnTarget = false;
        currentTarget = null;
        targetTransform = null;
        headTransform = null;

        ChangeCamera(CameraType.DEFAULT);

        return;
    }
    private void ControlTargetMarker()
    {
        if (currentTarget)
        {
            if (!targetTransform.GetComponent<CapsuleCollider>())
            {
                return;
            }
            //currentTarget�� ���� �� ��Ŀ�� ������, ������
            targetMarker.gameObject.SetActive(true);

            Vector3 targetPosition;
            if (headTransform)
            {
                targetPosition = targetTransform.position ;
            }
            else
            {
                CapsuleCollider targetCollider = targetTransform.GetComponent<CapsuleCollider>();
                float targetHeight = targetCollider.transform.localScale.y * (targetCollider.center.y + targetCollider.height * 0.5f);

                targetPosition = new Vector3(targetTransform.position.x, targetTransform.position.y + targetHeight, targetTransform.position.z) + cameraData.targetMarkerOffset;
            }
            Vector3 targetScreenPoint = MainCamera.WorldToScreenPoint(targetPosition);
            targetMarker.transform.position = targetScreenPoint;

            targetMarker.color = CameraData.Instance.LockOnTargetMarkerColor;
        }
        else if (visibleTarget)
        {
            if (!visibleTarget.GetComponent<CapsuleCollider>())
            {
                return;
            }

            targetMarker.gameObject.SetActive(true);
            Vector3 targetPosition;
            
            if (headTransform)
            {
                targetPosition = headTransform.position;
            }
            else
            {
                CapsuleCollider targetCollider = visibleTarget.GetComponent<CapsuleCollider>();
                    
                float targetHeight = targetCollider.transform.localScale.y * (targetCollider.center.y + targetCollider.height * 0.5f);

                targetPosition = 
                    new Vector3(visibleTarget.transform.position.x, visibleTarget.transform.position.y + targetHeight, visibleTarget.transform.position.z) + cameraData.targetMarkerOffset;
            }


            Vector3 targetScreenPoint = MainCamera.WorldToScreenPoint(targetPosition);
            targetMarker.transform.position = targetScreenPoint;

            targetMarker.color = CameraData.Instance.detectedTargetMarkerColor;
        }
        else
        {
            targetMarker.gameObject.SetActive(false);
        }
    }
    public void LockOnTarget()
    {
        //���� ���¿��� �ѹ� �� ��ɽ���� ��� ��Ȱ��ȭ
        if (isLockOnTarget)
        {
            DeactivateLockOn();

            return;
        }

        //detectedTarget�� ������ currentTarget����
        if (visibleTarget)
        {
            isLockOnTarget = true;
            currentTarget = visibleTarget;
            visibleTarget = null;

            maskChange.CurrentAnimator.SetBool("isFocused", true);

            targetTransform = currentTarget.transform.Find("HeadPosition");

            //(��������)���½� Ÿ�ٸ�ĿȰ��ȭ >> ���� ������Ʈ
            targetMarker.gameObject.transform.localScale = cameraData.targetMarkerScale;


            if (targetTransform == null)
            {
                targetTransform = currentTarget.transform;
            }
            
            targetGroup.AddMember(targetTransform, 1f, 1);

            ChangeCamera(CameraType.LOCKON);
        }
    }
    #endregion
    public void CameraHorizontalSwipe()
    {
        if (joystick.Horizontal == 0) return;

        defaultCamera.m_XAxis.Value += joystick.Horizontal * cameraData.cameraHorizontalSwipeRate;
    }
}