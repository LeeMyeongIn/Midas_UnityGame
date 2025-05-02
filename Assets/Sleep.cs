using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sleep : MonoBehaviour
{
    internal void DoSleep()
    {
        StartCoroutine(SleepRoutine());
    }
    IEnumerator SleepRoutine()
    {
        ScreenTint scrrenTint = GameManager.instance.screenTint;
        scrrenTint.Tint();
        yield return new WaitForSeconds(2f);



        scrrenTint.UnTint();
        yield return new WaitForSeconds(2f);

        yield return null;
    }
}
