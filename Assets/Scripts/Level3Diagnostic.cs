using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3Diagnostic : MonoBehaviour
{
    void Start()
    {
        Debug.Log("🔍 === LEVEL 3 DIAGNOSTIC ===");
        
        // Check current scene
        Scene currentScene = SceneManager.GetActiveScene();
        Debug.Log($"📍 Current Scene: {currentScene.name} (Build Index: {currentScene.buildIndex})");
        
        // Check if we're in Level 3
        if (currentScene.name == "Level3")
        {
            Debug.Log("✅ We are in Level 3!");
            CheckLevel3Setup();
        }
        else
        {
            Debug.Log($"❌ We are NOT in Level 3, we're in: {currentScene.name}");
            CheckSceneBuildSettings();
        }
    }
    
    void CheckLevel3Setup()
    {
        Debug.Log("🔍 Checking Level 3 components...");
        
        // Check for player
        PlayerController3D player = FindObjectOfType<PlayerController3D>();
        Debug.Log($"🎮 Player found: {(player != null ? "✅" : "❌")}");
        
        // Check for boss
        BossWyvernSimpleAI boss = FindObjectOfType<BossWyvernSimpleAI>();
        Debug.Log($"🐲 Boss found: {(boss != null ? "✅" : "❌")}");
        
        // Check for boss intro trigger
        BossIntroTrigger introTrigger = FindObjectOfType<BossIntroTrigger>();
        Debug.Log($"🎬 Boss Intro Trigger found: {(introTrigger != null ? "✅" : "❌")}");
        
        // Check for AudioManager
        Debug.Log($"🎵 AudioManager found: {(AudioManager.Instance != null ? "✅" : "❌")}");
        
        // Check cameras
        Camera[] cameras = FindObjectsOfType<Camera>();
        Debug.Log($"📷 Cameras found: {cameras.Length}");
        
        foreach (Camera cam in cameras)
        {
            Debug.Log($"   📹 {cam.name} - Enabled: {cam.enabled}");
        }
    }
    
    void CheckSceneBuildSettings()
    {
        Debug.Log("🔍 Checking Build Settings...");
        
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        Debug.Log($"📋 Total scenes in build: {sceneCount}");
        
        for (int i = 0; i < sceneCount; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"   Scene {i}: {sceneName}");
        }
    }
    
    void Update()
    {
        // Press L key to try loading Level 3
        if (Input.GetKeyDown(KeyCode.L))
        {
            TryLoadLevel3();
        }
        
        // Press B key to go back to previous level
        if (Input.GetKeyDown(KeyCode.B))
        {
            GoToPreviousLevel();
        }
    }
    
    void TryLoadLevel3()
    {
        Debug.Log("🚀 Trying to load Level 3...");
        
        try
        {
            SceneManager.LoadScene("Level3");
            Debug.Log("✅ Level 3 load command sent");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to load Level 3: {e.Message}");
        }
    }
    
    void GoToPreviousLevel()
    {
        Debug.Log("⬅️ Going to previous level...");
        
        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int previousIndex = currentIndex - 1;
        
        if (previousIndex >= 0)
        {
            SceneManager.LoadScene(previousIndex);
        }
        else
        {
            Debug.Log("❌ No previous level to go to");
        }
    }
}
