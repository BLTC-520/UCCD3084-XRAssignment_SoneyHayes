using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;

public class SmokeVR : MonoBehaviour
{
    [Header("Particle System")]
    public ParticleSystem smoke; // Nozzle 下的 Particle System

    [Header("Input Settings")]
    public Key keyToSpray = Key.F; // Inspector 可选键 (测试用)
    
    [Header("VR Controller Settings")]
    public XRNode controllerNode = XRNode.RightHand; // 右手控制器
    public InputFeatureUsage<bool> triggerButton = UnityEngine.XR.CommonUsages.triggerButton; // 扳机键
    
    [Header("Pin")]
    public PinLock pin; // 指向 PinLock 脚本

    private bool isSpraying = false;
    private UnityEngine.XR.InputDevice targetDevice;

    void Awake()
    {
        if (smoke != null)
            smoke.Stop();
    }

    void Update()
    {
        // 获取VR控制器设备
        if (!targetDevice.isValid)
            targetDevice = InputDevices.GetDeviceAtXRNode(controllerNode);

        bool pressed = false;

        // VR控制器输入检测
        if (targetDevice.isValid)
        {
            targetDevice.TryGetFeatureValue(triggerButton, out pressed);
        }
        
        // 键盘输入检测 (测试用)
        if (!pressed && Keyboard.current != null)
            pressed = Keyboard.current[keyToSpray].isPressed;

        // 只有 Pin 被拔出才能喷
        if (pin.isPulledOut)
        {
            if (pressed && !isSpraying)
                StartSpraying();
            else if (!pressed && isSpraying)
                StopSpraying();
        }
        else if (isSpraying)
        {
            // Pin 没拔出时停止喷烟
            StopSpraying();
        }
    }

    void StartSpraying()
    {
        if (smoke != null)
        {
            smoke.Play();
            isSpraying = true;
        }
    }

    void StopSpraying()
    {
        if (smoke != null)
        {
            smoke.Stop();
            isSpraying = false;
        }
    }
}
