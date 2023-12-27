using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class KartBody : MonoBehaviour
{
    KartController kc;
    [HideInInspector]public bool right,left;
    public float extraSpeed_multiplier = 5f;
    float targetExtraHandeling = 0;
    public AnimationCurve boosterCurve;
    public AnimationCurve shakeCurve;

    public float boosterMag=10f;
    public CinemachineVirtualCamera cvc;
    Cinemachine3rdPersonFollow camFollow;
    CinemachineBasicMultiChannelPerlin perlinShake;
    public ParticleSystem speedEff;
    public Collider magnetCollider;
    bool isMagnetOn = false;
    bool isBoosterOn = false;
    public GameObject floatingCoinObj;
    public botScript bot;
    public GameObject magnetPos;
    private void Start()
    {
        kc = GetComponentInParent<KartController>();
        camFollow = cvc.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        perlinShake = cvc.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    private void Update()
    {
        float localPos = transform.localPosition.x / kc.clampX;
        if (right)
            kc.extraSpeed = localPos * extraSpeed_multiplier;//extra speed for corner
        else if (left)
            kc.extraSpeed = localPos * -extraSpeed_multiplier;
        else
            kc.extraSpeed = 0f;

        //centrifugal force on curves
        kc.extraHandeling = Mathf.Lerp(kc.extraHandeling, targetExtraHandeling*(kc.crrSpeed/kc.maxSpeed), Time.deltaTime * 5f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("CurveR"))
        {
            right = true;
            targetExtraHandeling = -kc.finalSpeed/4;
        }
        if (other.CompareTag("CurveL"))
        {
            left = true;
            targetExtraHandeling = kc.finalSpeed / 4;

        }
        if (other.CompareTag("Booster"))
        {
            if (!kc.isBot) {//bots can not shake camera
                perlinShake.m_AmplitudeGain = 1f;
                StartCoroutine(AmplituteDown());
                speedEff.Play();
            }
            other.gameObject.SetActive(false);
            other.transform.parent.Find("BurstEff").GetComponent<ParticleSystem>().Play();
            StartCoroutine(activateObj(other.gameObject, 3f));

            if (BoosterForceEnum != null) StopCoroutine(BoosterForceEnum);
            BoosterForceEnum = boosterEnum();
            StartCoroutine(BoosterForceEnum);
        }
        if (other.CompareTag("Coin"))
        {
            if(!kc.isBot)GameManager.instance.AddCoins(1);
            if (isMagnetOn)
            {
                other.gameObject.SetActive(false);
                GameObject tempCoin= Instantiate(floatingCoinObj, other.transform.position, other.transform.rotation);
                tempCoin.SetActive(true);
                tempCoin.GetComponent<CoinScript>().startLerp(magnetPos);
                StartCoroutine(activateObj(other.gameObject, 3f));
            }
            else {
                other.gameObject.SetActive(false);
                other.transform.parent.Find("BurstEff").GetComponent<ParticleSystem>().Play();
                StartCoroutine(activateObj(other.gameObject, 3f));
            }
           
        }
        if (other.CompareTag("Magnet"))
        {
            magnetCollider.enabled = true;
            isMagnetOn = true;
            other.gameObject.SetActive(false);
            other.transform.parent.Find("BurstEff").GetComponent<ParticleSystem>().Play();
            Invoke(nameof(MagnetPowerOff), 7f);
            transform.Find("MagnetAnim").gameObject.SetActive(true);
            StartCoroutine(activateObj(other.gameObject, 3f));
        }

        if (other.CompareTag("Finish"))
        {
            if (kc.totalLaps == kc.crrLap)
            {
                GameManager.instance.yourPosition += 1;
                if (!kc.isBot) kc.RaceOverFn(GameManager.instance.yourPosition);
            }
            else
                kc.crrLap += 1;
        }

        if (other.CompareTag("Player"))
        {
            bot.StartBlinkEff();
        }
    }
   
    void MagnetPowerOff() {
        isMagnetOn = false;
        magnetCollider.enabled = false;
        transform.Find("MagnetAnim").gameObject.SetActive(false);
    }
    IEnumerator activateObj(GameObject obj,float waitTime) {//activate coin and booster once again for next lap
        yield return new WaitForSeconds(waitTime);
        obj.SetActive(true);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("CurveR"))
        {
            right = false;
            targetExtraHandeling = 0;
        }

        if (other.CompareTag("CurveL"))
        {
            left = false;
            targetExtraHandeling = 0;
        }

        if (other.CompareTag("Player"))
            bot.StopBlinkEff();
    }

    #region booster effect
    float handling;
    IEnumerator BoosterForceEnum;
    IEnumerator boosterEnum()
    {
        isBoosterOn = true;
        handling = kc.handling;
        float ct = 0;
        float nt = 0;
        float tot = 3f;
        while (ct < tot)
        {
            ct += Time.deltaTime;
            nt = ct / tot;
            kc.handling = nt * handling;

            nt = boosterCurve.Evaluate(nt);
            kc.boosterForce = nt * boosterMag;

            if (!kc.isBot) camFollow.CameraDistance = 2 + nt * 3;

            yield return null;
        }
        kc.handling = handling;
        isBoosterOn = false;
    }

    IEnumerator AmplituteDown()//camera shake effect
    {
        float ct = 0;
        float nt = 0;
        float tot = 1f;
        while (ct < tot)
        {
            ct += Time.deltaTime;
            nt = ct / tot;
            nt = shakeCurve.Evaluate(nt);
            perlinShake.m_AmplitudeGain = Mathf.Lerp(1, 0, nt);
            yield return null;
        }
    }
    #endregion

}
