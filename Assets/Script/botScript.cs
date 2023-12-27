using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class botScript : MonoBehaviour
{
    KartController kc;
    public GameObject mesh;
    private void Awake()
    {
        kc = GetComponent<KartController>();
        kc.isBot = true;
    }
    private void Start()
    {
        kc.localX = kc.kb.transform.localPosition.x;
        kc.v = 1;
        InvokeRepeating(nameof(randomForwardSpeed),4f,2);
    }

    void randomForwardSpeed() {//random horizontal steer
        kc.v = 1f;
        kc.h =  Random.Range(-.5f, .5f);
    }

    #region blink Effect

    public void StartBlinkEff() {
        if (transEnum != null) StopCoroutine(transEnum);
        transEnum = tEnum();
        StartCoroutine(transEnum);
    }
    public void StopBlinkEff() {
        if (transEnum != null) StopCoroutine(transEnum);
        mesh.SetActive(true);
    }


    IEnumerator transEnum;
    IEnumerator tEnum() {//hide unhide animation
        while (true)
        {
            mesh.SetActive(true);
            yield return new WaitForSeconds(.05f);
            mesh.SetActive(false);
            yield return new WaitForSeconds(.05f);
        }
    }

    #endregion
}
