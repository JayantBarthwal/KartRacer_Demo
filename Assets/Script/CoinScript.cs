using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour
{
    public AnimationCurve scaleCurve;
    public void startLerp(GameObject magnet) {

        StartCoroutine(LerpNow(magnet.transform));

    }
    IEnumerator LerpNow(Transform player)//Only for magnet power
    {
        float ct = 0;
        float nt = 0;
        float tot = .5f;
        Vector3 startPos = transform.position;
        Vector3 startScale = transform.localScale;
        Vector3 midPoint = startPos;
        midPoint.y += 1f;

        Vector3 helper=midPoint;

        while (ct<tot)
        {
            ct += Time.deltaTime;
            nt = ct / tot;
            helper = Vector3.Lerp(midPoint, player.position, nt);
            transform.position = Vector3.Lerp(startPos, helper, nt);

            nt = scaleCurve.Evaluate(nt);
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, nt);
            yield return null;
        }

        Destroy(gameObject);
    }


}
