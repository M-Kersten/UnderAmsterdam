using Fusion;
using Fusion.XR.Host.Rig;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class SettingsUI : MonoBehaviour
{
    [SerializeField] private AudioMixer volumeMixer;
    [SerializeField] private UISlider[] allSliders;
    [SerializeField] private UISlider masterSlider;
    [SerializeField] private Material[] leftHandButtonMaterials;
    [SerializeField] private GameObject leftHandButton;
    private Renderer leftHandButtonRenderer;
    private NetworkObject myNetworkObject;

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
            slider.HandlePosition(masterSlider.handle.transform.localPosition.x);
        }
    }

    public void GetNetworkObj(NetworkObject givenNetworkObject)
    {
        myNetworkObject = givenNetworkObject;
    }

    public void LeftHanded()
    {
        if (myNetworkObject != null && myNetworkObject == Gamemanager.Instance.networkData.GetComponent<NetworkObject>().InputAuthority)
            Gamemanager.Instance.networkData.RPC_SwitchHands();
        else
            MainMenuHands.Instance.SwitchWatch();

        Gamemanager.Instance.localData.SwitchUI();

        Gamemanager.Instance.localData.leftHanded = !Gamemanager.Instance.localData.leftHanded;
        ButtonColour(Gamemanager.Instance.localData.leftHanded);
    }
    private void ButtonColour(bool lefthanded)
    {
        if(lefthanded)
            leftHandButtonRenderer.material = leftHandButtonMaterials[1];
        else
            leftHandButtonRenderer.material = leftHandButtonMaterials[0];
    }
}