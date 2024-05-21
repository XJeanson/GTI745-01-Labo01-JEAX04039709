using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GhostTransform
{
    public Vector3 position;
    public Quaternion rotation;

    public GhostTransform(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
}

public class GhostManager : MonoBehaviour
{
    public Transform kart;
    public Transform ghost;

    public bool recording;
    public bool playing;

    public float playbackSpeed = 1.0f; // Add playback speed variable

    private List<GhostTransform> recordedGhostTransforms = new List<GhostTransform>();
    private GhostTransform lastRecordedGhostTransform;
    private float bestLapTime = float.MaxValue;
    private List<GhostTransform> bestRecordedGhostTransforms = new List<GhostTransform>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (recording)
        {
            if (kart.position != lastRecordedGhostTransform.position || kart.rotation != lastRecordedGhostTransform.rotation)
            {
                var newGhostTransform = new GhostTransform(kart);
                recordedGhostTransforms.Add(newGhostTransform);

                lastRecordedGhostTransform = newGhostTransform;

                //Debug.Log("recorded list length: " + recordedGhostTransforms.Count);
            }
        }

        if (playing)
        {
            Play();
            playing = false;
        }
    }

    void Play()
    {
        ghost.gameObject.SetActive(true);
        StartCoroutine(StartGhost());
        playing = false;
    }

    IEnumerator StartGhost()
    {
        for (int i = 0; i < bestRecordedGhostTransforms.Count; i++)
        {
            ghost.position = bestRecordedGhostTransforms[i].position;
            ghost.rotation = bestRecordedGhostTransforms[i].rotation;

            // Calculate the wait time based on playback speed
            float waitTime = Time.fixedDeltaTime / playbackSpeed;
            yield return new WaitForSeconds(waitTime);
        }
    }

    public void SaveLap(float lapTime)
    {


        //Debug.Log("lapTime : " + lapTime + " ... bestLapTime : " + bestLapTime);
        if (lapTime < bestLapTime)
        {
            bestLapTime = lapTime;
            bestRecordedGhostTransforms = new List<GhostTransform>(recordedGhostTransforms);
            recordedGhostTransforms = new List<GhostTransform>();
        }
    }
}
