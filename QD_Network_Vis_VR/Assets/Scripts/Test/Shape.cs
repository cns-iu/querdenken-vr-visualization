using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class Shape : MonoBehaviour
{
    // public IEnumerator RotateForSeconds(float duration)
    // {
    //     float elapsedTime = 0f;
    //     while (elapsedTime <= duration)
    //     {
    //         this.transform.Rotate(0f, 150f * Time.deltaTime, 0f);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }
    // }

    public async Task RotateForSeconds(float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime <= duration)
        {
            this.transform.Rotate(0f, 360f * Time.deltaTime, 0f);
            elapsedTime += Time.deltaTime;
            await Task.Yield();
        }
    }
}
