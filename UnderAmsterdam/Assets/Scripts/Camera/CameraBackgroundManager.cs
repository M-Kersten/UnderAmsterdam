using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Camera))]
public class CameraBackgroundManager : MonoBehaviour
{
    private Camera _camera;

    [SerializeField] 
    private SceneCameraClearFlags[] cameraClearFlags;

    private void Awake()
    {
        _camera = GetComponent<Camera>();
    }

    private void Start()
    {
        SubscribeToEvents();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents() => SceneManager.activeSceneChanged += OnSceneChanged;
    private void UnsubscribeFromEvents() => SceneManager.activeSceneChanged -= OnSceneChanged;

    private void OnSceneChanged(Scene previousScene, Scene currentScene)
    {
        UpdateActiveClearFlags(currentScene.name);
    }

    private void UpdateActiveClearFlags(string sceneName)
    {
        if (_camera == null)
            _camera = GetComponent<Camera>();
        
        Debug.Log("updating clear flags and shadow distance");
        
        var setting = GetClearFlagsByScene(sceneName);
        
        _camera.clearFlags = setting.ClearFlags;
        
        var urp = (UniversalRenderPipelineAsset)GraphicsSettings.currentRenderPipeline;
        QualitySettings.shadowDistance = setting.ShadowDistance;
        urp.shadowDistance = setting.ShadowDistance;
    }

    private SceneCameraClearFlags GetClearFlagsByScene(string sceneName)
    {
        return cameraClearFlags.SingleOrDefault(x => x.Scene == sceneName);
    }
    
    
}
