using UnityEngine;
using System.Collections.Generic;
using Vuforia;

[RequireComponent(typeof (VuforiaBehaviour))]
public class SmoothCamera : MonoBehaviour {

    const float FILTER_DEFAULT_ALPHA = 0.25f; // if ALPHA = 1 OR 0, no filter applies.

    public int smoothingFrames = 10;
    public bool smoothPosition = false;
    public bool smoothRotation = false;
	private Queue<Quaternion> smoothingRotations;
    private Queue<Vector3> smoothingPositions;

    public float filterAlpha = FILTER_DEFAULT_ALPHA;
    public bool filterPosition = false;
    private bool positionFilterPrimed = false;

	private VuforiaBehaviour qcarBehavior;

	private Quaternion rotation;
    private Vector3 position;

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
            // Low-pass filter algorithm
            // for i from 1 to n
            //   output[i] := output[i - 1] + ALPHA * (input[i] - output[i])

            if (!positionFilterPrimed)
            {
                position.x = transform.position.x;
                position.y = transform.position.y;
                position.z = transform.position.z;
                positionFilterPrimed = true;
            }
            else
            {
                Debug.Log("position delta = " + (transform.position.x - position.x) + ", " + (transform.position.y - position.y) + ", " + (transform.position.z - position.z));

                position.x = position.x + filterAlpha * (transform.position.x - position.x);
                position.y = position.y + filterAlpha * (transform.position.y - position.y);
                position.z = position.z + filterAlpha * (transform.position.z - position.z);
            }
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

        Debug.Log("position = " + position.x + ", " + position.y + ", " + position.z);

        // --------------------------------------------------------------------------
        // ROTATION

        // Optional smoothing
        if (smoothRotation)
        {
            if (smoothingRotations.Count >= smoothingFrames)
            {
                smoothingRotations.Dequeue();
            }
            smoothingRotations.Enqueue(transform.rotation);

   		    Vector4 avgr = Vector4.zero;
    		foreach (Quaternion singleRotation in smoothingRotations) {
    			Math3d.AverageQuaternion(ref avgr, singleRotation, smoothingRotations.Peek(), smoothingRotations.Count);
    		}

            rotation = new Quaternion(avgr.x, avgr.y, avgr.z, avgr.w);
        }
        else
        {
            rotation = transform.rotation;
        }
	}

	// Use this for initialization
	void Start ()
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
	void LateUpdate ()
    {
        //Debug.Log("transform.position.x: " + transform.position.x + "smoothedPosition.x: " + smoothedPosition.x );
        //Debug.Log("transform.position.y: " + transform.position.y + "smoothedPosition.y: " + smoothedPosition.y );
        //Debug.Log("transform.position.z: " + transform.position.z + "smoothedPosition.z: " + smoothedPosition.z );

        transform.rotation = rotation;
        transform.position = position;
	}
}
 