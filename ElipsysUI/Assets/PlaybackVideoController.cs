using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Video;

public class PlaybackVideoController : MonoBehaviour
{

    [SerializeField]
    VideoPlayer tobiiPlayer;
    [SerializeField]
    VideoPlayer externalPlayer;

    bool playing = false;
    [SerializeField]
    double playbackTime = 0;
    [SerializeField]
    public float syncTime = 0;

    [SerializeField]
    UnityEvent OnPlay;
    [SerializeField]
    UnityEvent OnPause;


    [SerializeField]
    TMPro.TMP_Text time_text;

    [SerializeField]
    Slider timeline_slider;

    [SerializeField]
    Vector2 time_bounds;

    [SerializeField]
    List<Vector2> shot_bounds;
    [SerializeField]
    List<GameObject> shot_pips;
    [SerializeField]
    GameObject shot_pip_prefab;

    bool initialized = false;

    bool seekDone = false;

    [SerializeField]
    int shot_index = -1;
    [SerializeField]
    bool loop;

    void SetBounds(int index = -1)
    {
        //If index is -1, reset to the whole video
        if (index == -1)
        {
            time_bounds = new Vector2(syncTime, tobiiPlayer.frameCount / tobiiPlayer.frameRate);
        }
        else time_bounds = shot_bounds[index];
    }




    [ContextMenu("Play")]
    public void Play()
    {
        if (!initialized)
        {
            time_bounds = new Vector2(syncTime, tobiiPlayer.frameCount / tobiiPlayer.frameRate);
            ResetVideo();
            
            initialized = true;
        }
        playing = true;
        tobiiPlayer.Play();
        externalPlayer.Play();
        OnPlay?.Invoke();
    }
    public void ResetVideo()
    {
        if (syncTime < 0)
        {
            tobiiPlayer.time = time_bounds[0]-syncTime;
            externalPlayer.time = time_bounds[0];
        }
        else
        {
            tobiiPlayer.time = time_bounds[0];
            externalPlayer.time = time_bounds[0]-syncTime;
        }
        tobiiPlayer.Play();
        externalPlayer.Play();
        playbackTime = 0;
        SetUI();

        //OnPause?.Invoke();

    }

    [ContextMenu("Pause")]
    public void Pause()
    {
        playing = false;
        tobiiPlayer.Pause();
        externalPlayer.Pause();
        OnPause?.Invoke();
        SetUI();
    }

    public void StepForward()
    {
        SetTime((float)playbackTime + (1 / 25f));
        //playing = false;
        SetUI();
    }
    public void StepBack()
    {
        Pause();
        SetUI();
    }
    public void StepToEnd()
    {
        Pause();
        SetUI();
    }
    // Start is called before the first frame update
    void Start()
    {
        tobiiPlayer.seekCompleted += OnSeekCompleted;
        externalPlayer.seekCompleted += OnSeekCompleted;
    }

    void OnSeekCompleted(VideoPlayer source)
    {
        //print("Seek Complete");
        //source.Play();
        //if (source == tobiiPlayer)
        //{
        //    seekDone = true;
        //}
    }





    public void SetTimeFromTimeline(float normalized_time)
    {
        SetTime(Mathf.Lerp(time_bounds[0], time_bounds[1], normalized_time));
    }

    public void SetTime(float new_time)
    {

        tobiiPlayer.time = Mathf.Clamp(new_time, time_bounds[0], time_bounds[1]);
        externalPlayer.time = tobiiPlayer.time - syncTime;

        playbackTime = tobiiPlayer.time;
        SetUI();

    }

    public void SetSyncTime(float new_sync)
    {
        syncTime = new_sync;
        if (syncTime < 0)
        {
            tobiiPlayer.time = 0;
            externalPlayer.time = Mathf.Abs(syncTime);
        }
        else
        {
            tobiiPlayer.time = Mathf.Abs(syncTime);
            externalPlayer.time = 0;
        }
        playing = false;
        tobiiPlayer.Play();
        externalPlayer.Play();
        OnPause?.Invoke();
    }

    void SetUI()
    {
        time_text.SetText(playbackTime.ToString("0000.00"));
        float sliderValue = Mathf.InverseLerp(time_bounds[0], time_bounds[1], (float)playbackTime);
        timeline_slider.SetValueWithoutNotify(sliderValue);

    }



    // Update is called once per frame
    void Update()
    {
        SetUI();


        if (playing)
        {
            if (tobiiPlayer.isPaused || playbackTime>time_bounds[1])
            {
                if (loop)
                {
                    ResetVideo();
                    Play();
                    SetUI();
                }
                else
                {
                    Pause();
                }
                
            }
            playbackTime = tobiiPlayer.time;
            SetUI();

        }
        else
        {
            if (tobiiPlayer.isPlaying)
            {
                tobiiPlayer.Pause();
            }
            if (externalPlayer.isPlaying)
            {
                externalPlayer.Pause();
            }
        }
    }
    private void LateUpdate()
    {

        //if (seekDone)
        //{
        //    playing = false;
        //    seekDone = false;
        //    SetUI();
        //    OnPause?.Invoke();
        //}

        //if (!playing)
        //{
        //    if (tobiiPlayer.isPlaying)
        //    {
        //        tobiiPlayer.Pause();
        //    }
        //    if (externalPlayer.isPlaying)
        //    {
        //        externalPlayer.Pause();
        //    }
        //}
    }
}
