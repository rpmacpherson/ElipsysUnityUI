using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncAdjust : MonoBehaviour
{
    [SerializeField]
    PlaybackVideoController playbackController;

    [SerializeField]
    TMPro.TMP_InputField syncInput;

    [SerializeField]
    float nudgeValue = 1 / 24f;

    private void Update()
    {
        syncInput.SetTextWithoutNotify(playbackController.syncTime.ToString());
    }

    public void NudgeForward()
    {
        playbackController.SetSyncTime(playbackController.syncTime+nudgeValue);
    }

    public void NudgeBack()
    {
        playbackController.SetSyncTime(playbackController.syncTime - nudgeValue);
    }
}
