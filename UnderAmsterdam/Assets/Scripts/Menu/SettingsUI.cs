using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeMixer;
    private bool leftHandedMode;
    [SerializeField] private UISlider[] allSliders;
    [SerializeField] private UISlider masterSlider;
    [SerializeField] private Material[] leftHandButtonMaterials;
    [SerializeField] private GameObject leftHandButton;
    private Renderer leftHandButtonRenderer;

    private void Start()
    {
        leftHandButtonRenderer = leftHandButton.GetComponent<Renderer>();
    }

    public void SetVolume(string sliderType, float sliderValue)
    {
        volumeMixer.SetFloat(sliderType, Mathf.Log10(sliderValue) * 20);
    }

    public void MasterVolume()
    {
        foreach(UISlider slider in allSliders)
        {
            slider.handle.transform.localPosition = new Vector3(masterSlider.handle.transform.localPosition.x, slider.handle.transform.localPosition.y, slider.handle.transform.localPosition.z);
            slider.handlePosition(masterSlider.handle.transform.localPosition.x);
        }
    }

    public void LeftHanded()
    {
        if(SceneManager.GetActiveScene().name != "A1Menu") Gamemanager.Instance.networkData.RPC_SwitchHands();
        if (leftHandedMode)
        {
            leftHandButtonRenderer.material = leftHandButtonMaterials[0];
            leftHandedMode = false;
        } else
        {
            leftHandButtonRenderer.material = leftHandButtonMaterials[1];
            leftHandedMode = true;
        }
    }
}
