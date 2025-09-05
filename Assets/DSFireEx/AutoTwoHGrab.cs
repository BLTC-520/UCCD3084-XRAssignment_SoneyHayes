using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class AutoTwoHGrab : MonoBehaviour
{
    [Header("Main grab point (Handle)")]
    public XRGrabInteractable handleGrab;   // Handle 上的 XRGrabInteractable

    [Header("Auto grab point (Nozzle)")]
    public Transform autoGrabPointNozzle;   // Nozzle 的自动吸附点

    [Header("Left Hand Controller")]
    public Transform leftHand;   // XR Rig → LeftHand Controller 的 Transform

    private bool isHandleGrabbed = false;

    void OnEnable()
    {
        handleGrab.selectEntered.AddListener(OnHandleGrabbed);
        handleGrab.selectExited.AddListener(OnHandleReleased);
    }

    void OnDisable()
    {
        handleGrab.selectEntered.RemoveListener(OnHandleGrabbed);
        handleGrab.selectExited.RemoveListener(OnHandleReleased);
    }

    private void OnHandleGrabbed(SelectEnterEventArgs args)
    {
        isHandleGrabbed = true;
        // 一抓住就立刻把左手吸附到 Nozzle
        AttachLeftHandToNozzle();
    }

    private void OnHandleReleased(SelectExitEventArgs args)
    {
        isHandleGrabbed = false;
    }

    void Update()
    {
        if (isHandleGrabbed && leftHand != null && autoGrabPointNozzle != null)
        {
            // 持续保持左手跟随 Nozzle 吸附点
            leftHand.position = autoGrabPointNozzle.position;
            leftHand.rotation = autoGrabPointNozzle.rotation;
        }
    }

    private void AttachLeftHandToNozzle()
    {
        if (leftHand != null && autoGrabPointNozzle != null)
        {
            leftHand.position = autoGrabPointNozzle.position;
            leftHand.rotation = autoGrabPointNozzle.rotation;
        }
    }
}
