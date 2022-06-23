using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class ShapeManager : MonoBehaviour
{
    [SerializeField] private Shape[] _shapes;
    [SerializeField] private GameObject _finishedText;

    public  void BeginTest()
    {
        // _finishedText.SetActive(false);
        // await _shapes[0].GetComponent<Shape>().RotateForSeconds(1 + 1 * 0);

        // var tasks = new List<Task>();

        // for (int i = 1; i < _shapes.Length; i++)
        // {
        //     tasks.Add(_shapes[i].GetComponent<Shape>().RotateForSeconds(1 + 1 * i));
        // }

        // await Task.WhenAll(tasks);

        // _finishedText.SetActive(true);
        var randomNumber =  GetRandomNumber().Result;
        print(randomNumber);
    }

    async Task<int> GetRandomNumber() {
        var random = Random.Range(1000, 3000);
        await Task.Delay(random);
        return random;
    }
}
