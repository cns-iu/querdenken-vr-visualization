using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Log : MonoBehaviour
{
    public GameObject headset;
    public GameObject controllerLeft;
    public GameObject controllerRight;
    public float logInterval = .1f;
    public bool isDataLogOn = false;
    private float elapsedTime;
    private string dateTimeAtStart;
    private bool isRunning = false;
    private bool isExperimentRunning = true;

    private void OnEndOfSessionStopLogging()
    {
        isExperimentRunning = false;
    }

    private void Start()
    {
        //get start time/date for file name
        dateTimeAtStart = GiveDateTime();

        //set up CSV file if logging is on
        if (isDataLogOn)
        {
            SetupCSV();
        }
    }

    private void Update()
    {
        UpdateData();

        if (isDataLogOn && isExperimentRunning)
        {
            if (!isRunning) StartCoroutine(LogMessage());
        }
    }

    public void UpdateData()
    {
        elapsedTime += Time.deltaTime;
    }

    public IEnumerator LogMessage()
    {
        AddRecord(GetPath());
        isRunning = true;
        yield return new WaitForSeconds(logInterval);
        isRunning = false;
    }

    public void AddRecord(string filepath)
    {
        using (StreamWriter file = new StreamWriter(filepath, true))
        {
            file.WriteLine(
           FormatMessage()
            );
        }
    }

    private string FormatMessage()
    {
        return
            //general info
            +elapsedTime + ","

            //position
            + GetPosition(headset) + ","
            + GetPosition(controllerLeft) + ","
            + GetPosition(controllerRight) + ","

            //rotation
            + GetRotation(headset) + ","
            + GetRotation(controllerLeft) + ","
            + GetRotation(controllerRight) + ",";
    }

    private string GetPosition(GameObject gameObject)
    {
        string result =
            gameObject.transform.position.x + ","
            + gameObject.transform.position.y + ","
            + gameObject.transform.position.z;
        return result;
    }

    private string GetRotation(GameObject gameObject)
    {
        string result =
            gameObject.transform.rotation.eulerAngles.x + ","
            + gameObject.transform.rotation.eulerAngles.y + ","
            + gameObject.transform.rotation.eulerAngles.z + ","
            + UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).x + ","
            + UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).y + ","
            + UnityEditor.TransformUtils.GetInspectorRotation(gameObject.transform).z;

        return result;
    }

    public void SetupCSV()
    {
        using (StreamWriter file = new StreamWriter(GetPath(), true))

        {
            file.WriteLine(
           //general info
           "elapsedTime" + ","
           //position
           + "headsetPosX" + ","
           + "headsetPosY" + ","
           + "headsetPosZ" + ","
           + "ControllerLeftPosX" + ","
           + "ControllerLeftPosY" + ","
           + "ControllerLeftPosZ" + ","
           + "ControllerRightPosX" + ","
           + "ControllerRightPosY" + ","
           + "ControllerRightPosZ" + ","
           //rotation
           + "headsetRotEulerX" + ","
           + "headsetRotEulerY" + ","
           + "headsetRotEulerZ" + ","
           + "headsetRotInspectorX" + ","
           + "headsetRotInspectorY" + ","
           + "headsetRotInspectorZ" + ","
           + "controllerLeftRotEulerX" + ","
           + "controllerLeftRotEulerY" + ","
           + "controllerLeftRotEulerZ" + ","
           + "controllerLeftRotInspectorX" + ","
           + "controllerLeftRotInspectorY" + ","
           + "controllerLefttRotInspectorZ" + ","
           + "controllerRightRotEulerX" + ","
           + "controllerRightRotEulerY" + ","
           + "controllerRightRotEulerZ" + ","
           + "controllerRightRotInspectorX" + ","
           + "controllerRightRotInspectorY" + ","
           + "controllerRighttRotInspectorZ" + ","

           );
        }
    }

    private string GetPath()
    {
        return Application.dataPath + "/Data/TEDxDemo/TEDxDemo_" + dateTimeAtStart + ".csv";
    }

    private string GiveDateTime()
    {
        dateTimeAtStart = System.DateTime.Now.ToString();
        dateTimeAtStart = dateTimeAtStart.Replace(':', '_');
        dateTimeAtStart = dateTimeAtStart.Replace('/', '.');
        return dateTimeAtStart;
    }
}
