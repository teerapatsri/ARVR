using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Leap;
using Leap.Unity;
using PDollarGestureRecognizer;

public class Demo : MonoBehaviour
{

    public GameObject shot;

    LeapProvider provider;

    public Finger indexFinger;

    public Transform gestureOnScreenPrefab;

    private List<Gesture> trainingSet = new List<Gesture>();

    private List<Point> points = new List<Point>();
    private int strokeId = -1;

    private Vector3 virtualKeyPosition = Vector2.zero;
    private Rect drawArea;

    private RuntimePlatform platform;
    private int vertexCount = 0;

    private List<LineRenderer> gestureLinesRenderer = new List<LineRenderer>();
    private LineRenderer currentGestureLineRenderer;

    //GUI
    private string message;
    private bool recognized;
    private bool isRecording;
    private string newGestureName = "";

    public AudioSource m_ExplosionAudio;
    public AudioSource audio;

    public GameController gameController;
    public float createPosZ;

    public ParticleSystem m_ExplosionParticles;

    void Start()
    {

        provider = FindObjectOfType<LeapProvider>() as LeapProvider;
        platform = Application.platform;
        drawArea = new Rect(0, 0, Screen.width - Screen.width / 3, Screen.height);

        ////Load pre-made gestures
        //TextAsset[] gesturesXml = Resources.LoadAll<TextAsset>("GestureSet/10-stylus-MEDIUM/");
        //foreach (TextAsset gestureXml in gesturesXml)
        //    trainingSet.Add(GestureIO.ReadGestureFromXML(gestureXml.text));

        //Load user custom gestures
        string[] filePaths = Directory.GetFiles(Application.persistentDataPath, "*.xml");
        foreach (string filePath in filePaths)
            trainingSet.Add(GestureIO.ReadGestureFromFile(filePath));

        isRecording = false;

        print(Application.persistentDataPath);
    }

    void Update()
    {

        Frame frame = provider.CurrentFrame;
        foreach (Hand hand in frame.Hands)
        {
            if (hand.IsRight)
            {
                indexFinger = hand.Fingers[(int)Finger.FingerType.TYPE_INDEX];
                //Debug.Log (indexFinger.TipPosition);
            }
        }
        //Debug.Log ("");
        if (isRecording)
        {
            virtualKeyPosition = new Vector3(indexFinger.TipPosition.x * 2000+400, indexFinger.TipPosition.y * 1000 + 200);
        }
        //if (drawArea.Contains(virtualKeyPosition)) {
        //indexFinger.TipVelocity.Magnitude > 0.1
        if (!isRecording && indexFinger.IsExtended)
        {
            isRecording = true;

            ++strokeId;

            Transform tmpGesture = Instantiate(gestureOnScreenPrefab, transform.position, transform.rotation) as Transform;
            currentGestureLineRenderer = tmpGesture.GetComponent<LineRenderer>();

            gestureLinesRenderer.Add(currentGestureLineRenderer);

            vertexCount = 0;
        }
        if (isRecording && !indexFinger.IsExtended)

        {
            isRecording = false;
            recognized = true;
            if (strokeId >= 0 && vertexCount >=2)
            {
                message = findSymbol();
                createMagic(message);
            }
            if (recognized)
            {
                recognized = false;
                strokeId = -1;

                points.Clear();

                foreach (LineRenderer lineRenderer in gestureLinesRenderer)
                {

                    lineRenderer.numPositions = 0;

                    Destroy(lineRenderer.gameObject);
                }

                gestureLinesRenderer.Clear();
            }
        }
        //Debug.Log (indexFinger.TipVelocity.Magnitude);
        if (isRecording)
        {
            points.Add(new Point(virtualKeyPosition.x, -virtualKeyPosition.y, strokeId));
            //Debug.Log (indexFinger.TipPosition.x * 5000 + " "  + indexFinger.TipPosition.y * 2000);
            if (currentGestureLineRenderer != null)
            {
                currentGestureLineRenderer.SetVertexCount(++vertexCount);
                currentGestureLineRenderer.SetPosition(vertexCount - 1, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, 10)));
            }
        }
        //}
    }

    void OnXGUI()
    {

        GUI.Box(drawArea, "Draw Area");

        GUI.Label(new Rect(10, Screen.height - 40, 500, 50), message);

        if (GUI.Button(new Rect(Screen.width - 300, 10, 100, 30), "aosidjasd"))
        {
            Debug.Log("asdoij");
        }

        if (GUI.Button(new Rect(Screen.width - 100, 10, 100, 30), "Recognize"))
        {

            //			recognized = true;
            //
            //			Gesture candidate = new Gesture(points.ToArray());
            //			Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
            //			
            //			message = gestureResult.GestureClass + " " + gestureResult.Score;
            //
            //			Debug.Log (gestureResult.GestureClass);
            //recognized = true;
            //message = findSymbol();
            //createMagic(message);

        }

        GUI.Label(new Rect(Screen.width - 200, 150, 70, 30), "Add as: ");
        newGestureName = GUI.TextField(new Rect(Screen.width - 150, 150, 100, 30), newGestureName);


        //If have point cloud(s) and a name is filled
        if (GUI.Button(new Rect(Screen.width - 50, 150, 50, 30), "Add") && points.Count > 0 && newGestureName != "")
        {

            string fileName = String.Format("{0}/{1}-{2}.xml", Application.persistentDataPath, newGestureName, DateTime.Now.ToFileTime());

#if !UNITY_WEBPLAYER
            GestureIO.WriteGesture(points.ToArray(), newGestureName, fileName);
#endif

            trainingSet.Add(new Gesture(points.ToArray(), newGestureName));

            newGestureName = "";
        }
    }

    string findSymbol()
    {
        string message = "null";
        recognized = true;
        Gesture candidate = new Gesture(points.ToArray());
        Result gestureResult = PointCloudRecognizer.Classify(candidate, trainingSet.ToArray());
        //		message = gestureResult.GestureClass + " " + gestureResult.Score;
        message = gestureResult.GestureClass;
        return message;
    }

    void createMagic(string magicName)
    {
        Debug.Log(magicName + "===");

        float cx = 0, cy = 0;
        for (int i = 0; i < points.ToArray().Length; i++)
        {
            cx += points[i].X;
            cy += points[i].Y;
        }

        Point middlePivot = new Point(cx / points.ToArray().Length, cy / points.ToArray().Length, 0);

        //		Vector3 middle = new Vector3 (middlePivot.X, middlePivot.Y, 0f);
        Vector3 middle = new Vector3(0f, 1f, -10f);
        switch (magicName)
        {
            case "O":
                Debug.Log("circle");
                //Instantiate (atk1, middle, transform.rotation);
                //			Instantiate (atk1, transform.position, transform.rotation);
                //var mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, createPosZ);
                ////mousePos.z = 0;
                //var screenMousePos = Camera.main.ScreenToWorldPoint(virtualKeyPosition);
                //print(mousePos+"\t"+screenMousePos);

                Instantiate(shot, Camera.main.ScreenToWorldPoint(new Vector3(virtualKeyPosition.x, virtualKeyPosition.y, createPosZ)), Quaternion.Euler(new Vector3(0, 0, 0)));
                audio.Play();
                break;
            case "five point star":
                Debug.Log("star");
                //Instantiate (atk2, transform.position, transform.rotation);

                // Play the particle system.
                Instantiate(m_ExplosionParticles, new Vector3(0, 0, 3), Quaternion.Euler(new Vector3(0, 0, 0)));

                // Play the explosion sound effect.
                m_ExplosionAudio.Play();

                Bomb();
                break;
            default:
                Debug.Log("null");
                break;
        }

    }
    GameObject[] gameObjects;

    void Bomb()
    {

        gameObjects = GameObject.FindGameObjectsWithTag("Enemy");

        for (var i = 0; i < gameObjects.Length; i++)
        {
            if (gameObjects[i].transform.position.z < 17)
            {
                gameController.AddScore(1);
                Destroy(gameObjects[i]);
            }
        }
    }
}
