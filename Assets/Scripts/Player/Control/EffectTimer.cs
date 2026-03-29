//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class SkillTimer : MonoBehaviour
//{
//    public enum SkillEffect
//    {
//        INKSHAPE_HITBOX,
//        INKFLOOR_PROJECTILE_EFFECT,
//        INKFLOOR_HITBOX
//    }

//    [Header("사용처")]
//    [SerializeField] SkillEffect skillEffect;

//    private void OnEnable()
//    {
//        StartTimer();
//    }

//    public void StartTimer()
//    {
//        if (skillEffect == SkillEffect.INKFLOOR_PROJECTILE_EFFECT)
//        {
//            StartCoroutine(InkFloorEffect());
//        }
        
//        if (skillEffect == SkillEffect.INKFLOOR_HITBOX)
//        {
//            StartCoroutine(InkFloorHitBox());
//        }
        
//        if (skillEffect == SkillEffect.INKSHAPE_HITBOX)
//        {
//            StartCoroutine(InkShapeHitBox());
//        }
//    }

//    IEnumerator InkFloorEffect()
//    {
//        float startTime = Time.time;
//        while(true)
//        {
//            if (Time.time > startTime + PlayerHumanMaskData.Instance.inkFloorEffectDuration)
//            {
//                this.gameObject.SetActive(false);
//                break;
//            }

//            yield return null;
//        }
//    }

//    IEnumerator InkFloorHitBox()
//    {
//        float startTime = Time.time;
//        int hitCount = 0;

//        CapsuleCollider hitBoxCollider = this.gameObject.GetComponent<CapsuleCollider>();

//        while (true)
//        {
//            if (Time.time > startTime + PlayerHumanMaskData.Instance.inkFloorHitBoxDuration)
//            {
//                this.gameObject.SetActive(false);
//                break;
//            }
//            //간격대로 카운트
            
//            yield return null;

//            if (hitCount >= PlayerHumanMaskData.Instance.inkFloorHitCount)
//            {
//                continue;
//            }

//            if (Time.time >= startTime + PlayerHumanMaskData.Instance.inkFloorHitInterval * hitCount)
//            {
//                hitBoxCollider.enabled = false;

//                yield return null;

//                hitBoxCollider.enabled = true;
//                hitCount++;
//            }
            
//        }
//    }

//    IEnumerator InkShapeHitBox()
//    {
//        float startTime = Time.time;
//        int hitCount = 0;

//        MeshCollider hitBoxCollider = this.gameObject.GetComponentInChildren<MeshCollider>();
//        while (true)
//        {
//            if (Time.time > startTime + PlayerHumanMaskData.Instance.inkShapeHitBoxDuration)
//            {
//                this.gameObject.SetActive(false);
//                break;
//            }

//            yield return null;

//            if (hitCount >= PlayerHumanMaskData.Instance.inkShapeHitCount)
//            {
//                continue;
//            }

//            if (Time.time >= startTime + PlayerHumanMaskData.Instance.inkFloorHitInterval * hitCount)
//            {
//                hitBoxCollider.enabled = false;

//                yield return null;

//                hitBoxCollider.enabled = true;
//                hitCount++;
//            }

//        }
//    }



//}
