using UnityEngine;
using System.Collections;

public class LookAtKiller : MonoBehaviour
{
    [Header("Camera Target Direction")]
    public Transform lookTarget;
    public float totalDuration = 2f; // total time for look + return

    [Header("Player Settings")]
    public string playerTag = "Player";
    public Transform playerCamera;

    private bool activated = false;

    [Header("Curve Settings")]
    [Tooltip("Curve controlling rotation: 0=start, 0.5=target, 1=end. Ease-out first half, ease-in second half.")]
    public AnimationCurve smoothCurve = new AnimationCurve(
        new Keyframe(0f, 0f, 0f, 2f),     // fast start
        new Keyframe(0.5f, 0.5f, 0f, 0f), // slow at target
        new Keyframe(1f, 1f, 2f, 0f)      // slow start, fast finish
    );

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;
        if (!other.CompareTag(playerTag)) return;
        if (lookTarget == null || playerCamera == null) return;

        activated = true;
        StartCoroutine(LookSequence());
    }

    private IEnumerator LookSequence()
    {
        Quaternion startRot = playerCamera.rotation;
        Quaternion targetRot = Quaternion.LookRotation(lookTarget.position - playerCamera.position);

        float elapsed = 0f;

        while (elapsed < totalDuration)
        {
            float t = elapsed / totalDuration;
            float curveValue = smoothCurve.Evaluate(t);

            Quaternion currentRot;

            if (curveValue <= 0.5f)
            {
                // First half: rotate start → target
                float forwardT = curveValue * 2f;
                currentRot = Quaternion.Slerp(startRot, targetRot, forwardT);
            }
            else
            {
                // Second half: rotate target → start
                float backT = (curveValue - 0.5f) * 2f;
                currentRot = Quaternion.Slerp(targetRot, startRot, backT);
            }

            playerCamera.rotation = currentRot;

            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.rotation = startRot;
    }
}
