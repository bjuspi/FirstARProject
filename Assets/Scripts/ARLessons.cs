using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

public class ARLessons : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text stateText;

    [SerializeField]
    private TMP_Text planeText;

    [SerializeField]
    private TMP_Text pointText;

    [SerializeField]
    private ARSession ARSession_;

    private ARPlaneManager ARPlaneManager;
    private ARPointCloudManager ARPointCloudManager;

    GameObject ARSessionOrigin;

    // Create Vector3 list to store the Points of Point Clouds to be able to count them
    static List<Vector3> s_Vertices = new List<Vector3>();
    static List<ARPlane> activePlanes = new List<ARPlane>();

    void Awake()
    {
        ARSessionOrigin = GameObject.FindWithTag("ARSessionOrigin");

        ARPlaneManager = ARSessionOrigin.GetComponent<ARPlaneManager>();
        ARPointCloudManager = ARSessionOrigin.GetComponent<ARPointCloudManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void OnEnable()
    {
        //ARSession State
        ARSession.stateChanged += OnSystemStateChanged;
        StartCoroutine(ARSupportStatusCheck());

        // Planes count
        ARPlaneManager.planesChanged += OnPlanesChanged;

        // Points of Point Clouds count
        ARPointCloudManager.pointCloudsChanged += OnPointCloudChanged;
    }

    void OnDisable()
    {
        //ARSession State
        ARSession.stateChanged -= OnSystemStateChanged;

        // Planes count
        ARPlaneManager.planesChanged -= OnPlanesChanged;

        // Points of Point Clouds count
        ARPointCloudManager.pointCloudsChanged -= OnPointCloudChanged;
    }

    // ARSession State Callbacks (Using Switch Case statement)
    public void OnSystemStateChanged(ARSessionStateChangedEventArgs args)
    {
        switch (ARSession.state)
        {
            case ARSessionState.None:
                stateText.text = "" + ARSession.state;
                break;
            case ARSessionState.CheckingAvailability:
                Debug.Log("AR Session - Checking for availability");
                stateText.text = "" + ARSession.state;
                break;
            case ARSessionState.Unsupported:
                Debug.Log("AR Session - Not supported by this handheld device");
                stateText.text = "" + ARSession.state;
                break;
            case ARSessionState.NeedsInstall:
                ARSession_.enabled = true;
                Debug.Log("AR Session - Not installed on this handheld device");
                stateText.text = "" + ARSession.state;
                break;
            case ARSessionState.Installing:
                ARSession_.enabled = true;
                Debug.Log("AR Session - Installing AR support software on this handheld device");
                stateText.text = "" + ARSession.state;
                break;
            case ARSessionState.Ready:
                ARSession_.enabled = true;
                Debug.Log("AR Session - Supported by this handheld device and ready");
                stateText.text = "" + ARSession.state;
                break;
            case ARSessionState.SessionInitializing:
                ARSession_.enabled = true;
                Debug.Log("AR Session is initializing...");
                stateText.text = "" + ARSession.state;
                break;
            case ARSessionState.SessionTracking:
                Debug.Log("AR Session is tracking...");
                stateText.text = "" + ARSession.state;
                break;
            default:
                Debug.Log("AR Session - ERROR - No switch case found");
                stateText.text = "Unknown AR State";
                break;
        }
    }

    // PLANES COUNT - Planes Callbacks when ADDED, REMOVED or UPDATED events occur (calling separate functions for each operation)
    void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        OnPlanesAdded(args.added);
        OnPlanesUpdated(args.updated);
        OnPlanesRemoved(args.removed);
    }

    void OnPlanesAdded(List<ARPlane> addedPlanes)
    {
        foreach (var plane in addedPlanes)
        {
            // Check for plane merge when new planes added and if this occurs, disable one plane
            if (plane.subsumedBy)
            {
                plane.gameObject.SetActive(false);
            }
        }
        planeText.text = "" + ARPlaneManager.trackables.count;
    }

    void OnPlanesUpdated(List<ARPlane> updatedPlanes)
    {
        foreach (var plane in updatedPlanes)
        {
            // Check for plane merge when updating planes and if this occurs, disable one plane
            if (plane.subsumedBy)
            {
                plane.gameObject.SetActive(false);
            }
        }
        planeText.text = "" + ARPlaneManager.trackables.count;
    }

    void OnPlanesRemoved(List<ARPlane> removedPlanes)
    {
        foreach (var plane in removedPlanes)
        {
            planeText.text = "" + ARPlaneManager.trackables.count;
        }
    }

    // POINTS COUNT - Points of Point Clouds Callback when UPDATED event occurs
    void OnPointCloudChanged(ARPointCloudChangedEventArgs args)
    {
        OnPointCloudUpdated(args.updated);
    }

    void OnPointCloudUpdated(List<ARPointCloud> pointClouds)
    {
        foreach (var pointcloud in ARPointCloudManager.trackables)
        {
            var points = s_Vertices;
            points.Clear();
            foreach (var point in pointcloud.positions)
                s_Vertices.Add(point);
            pointText.text = "" + points.Count;
        }
    }


    // Detect ARSession state and display it at runtime
    IEnumerator ARSupportStatusCheck()
    {
        Debug.Log("Current State: " + ARSession.state);
        stateText.text = "" + ARSession.state;

        if ((ARSession.state == ARSessionState.None) || (ARSession.state == ARSessionState.CheckingAvailability))
        {
            Debug.Log("Checking AR Availability on this handheld device...");
            yield return ARSession.CheckAvailability();
        }
    }
}

