using UnityEngine;
using System.Collections.Generic;
using Vatio.Filters;
using Vuforia;

[RequireComponent(typeof(VuforiaBehaviour))]
public class SmoothCamera : MonoBehaviour {
    const float FILTER_ALPHA = 0.15f; // if ALPHA = 1 OR 0, no filter applies.

    public int smoothingFrames = 10;
    public bool smoothPosition = false;
    public bool smoothRotation = false;

    public bool filterPosition = false;
    public bool filterRotation = false;

    private VuforiaBehaviour qcarBehavior;

    private Quaternion rotation;
    private Vector3 position;

    private Queue<Quaternion> smoothingRotations;
    private Queue<Vector3> smoothingPositions;

    LowPassFilter<Vector3> cameraPositionFilter;
    LowPassFilter<Quaternion> cameraRotationFilter;

    public void OnInitialized()
    {
    }

    public void OnTrackablesUpdated()
    {
        // --------------------------------------------------------------------------
        // Position

        // Optional low pass filter
        if (filterPosition)
        {
            if (cameraPositionFilter != null)
            {
                position = cameraPositionFilter.Append(transform.position);
            }
            else
            {
                cameraPositionFilter = new LowPassFilter<Vector3>(FILTER_ALPHA, transform.position);
                position = transform.position;
            }

            Debug.Log("position delta = " + (transform.position.x - position.x) + ", " + (transform.position.y - position.y) + ", " + (transform.position.z - position.z));
        }
        else
        {
            position = transform.position;
        }

        // Optional smoothing
        if (smoothPosition)
        {
            if (smoothingPositions.Count >= smoothingFrames)
            {
                smoothingPositions.Dequeue();
            }
            smoothingPositions.Enqueue(position);

            Vector3 avgp = Vector3.zero;
            foreach (Vector3 singlePosition in smoothingPositions)
            {
                avgp += singlePosition;
            }
            position = avgp / smoothingPositions.Count;
        }

        //Debug.Log("position = " + position.x + ", " + position.y + ", " + position.z);

        // --------------------------------------------------------------------------
        // ROTATION

        // Optional low pass filter
        if (filterPosition)
        {
            if (cameraPositionFilter != null)
            {
                rotation = cameraRotationFilter.Append(transform.rotation);
            }
            else
            {
                cameraRotationFilter = new LowPassFilter<Quaternion>(FILTER_ALPHA, transform.rotation);
                rotation = transform.rotation;
            }
        }
        else
        {
            rotation = transform.rotation;
        }

        // Optional smoothing
        if (smoothRotation)
        {
            if (smoothingRotations.Count >= smoothingFrames)
            {
                smoothingRotations.Dequeue();
            }
            smoothingRotations.Enqueue(rotation);

            Vector4 avgr = Vector4.zero;
            foreach (Quaternion singleRotation in smoothingRotations) {
                Math3d.AverageQuaternion(ref avgr, singleRotation, smoothingRotations.Peek(), smoothingRotations.Count);
            }

            rotation = new Quaternion(avgr.x, avgr.y, avgr.z, avgr.w);
        }
    }

    // Use this for initialization
    void Start()
    {
        smoothingRotations = new Queue<Quaternion>(smoothingFrames);
        smoothingPositions = new Queue<Vector3>(smoothingFrames);
        qcarBehavior = GetComponent<VuforiaBehaviour>();

        VuforiaARController vuforia = VuforiaARController.Instance;
        //		qcarBehavior.RegisterVuforiaStartedCallback(OnInitialized);
        //		qcarBehavior.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);

        vuforia.RegisterVuforiaStartedCallback(OnInitialized);
        vuforia.RegisterTrackablesUpdatedCallback(OnTrackablesUpdated);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //Debug.Log("transform.position.x: " + transform.position.x + "smoothedPosition.x: " + smoothedPosition.x );
        //Debug.Log("transform.position.y: " + transform.position.y + "smoothedPosition.y: " + smoothedPosition.y );
        //Debug.Log("transform.position.z: " + transform.position.z + "smoothedPosition.z: " + smoothedPosition.z );

        if (filterPosition || smoothPosition)
        {
            transform.position = position;
        }
        if (filterRotation || smoothRotation)
        {
            transform.rotation = rotation;
        }
	}
}
 