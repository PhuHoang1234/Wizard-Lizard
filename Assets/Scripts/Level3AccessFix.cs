using UnityEngine;
using UnityEngine.SceneManagement;

public class Level3AccessFix : MonoBehaviour
{
    [Header("Testing Controls")]
    public KeyCode testKey = KeyCode.T;
    public KeyCode forceLevel3Key = KeyCode.Alpha3;
    
    void Start()
    {
        Debug.Log("🔧 Level 3 Access Fix loaded");
        Debug.Log($"Press {testKey} to test level progression");
        Debug.Log($"Press {forceLevel3Key} to force load Level 3");
    }
    
    void Update()
    {
        if (Input.GetKeyDown(testKey))
        {
            TestLevelProgression();
        }
        
        if (Input.GetKeyDown(forceLevel3Key))
        {
            ForceLoadLevel3();
        }
    }
    
    void TestLevelProgression()
    {
        Debug.Log("🧪 Testing level progression...");
        
        Scene currentScene = SceneManager.GetActiveScene();
        int currentIndex = currentScene.buildIndex;
        int nextIndex = currentIndex + 1;
        
        Debug.Log($"Current: {currentScene.name} (Index: {currentIndex})");
        Debug.Log($"Next would be: Index {nextIndex}");
        
        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
            string nextSceneName = System.IO.Path.GetFileNameWithoutExtension(nextScenePath);
            Debug.Log($"✅ Next level exists: {nextSceneName}");
            
            if (nextSceneName == "Level3")
            {
                Debug.Log("🐲 Next level is Level 3!");
            }
        }
        else
        {
            Debug.Log("❌ No next level in build settings");
        }
    }
    
    void ForceLoadLevel3()
    {
        Debug.Log("🚀 Force loading Level 3...");
        
        // Stop audio properly
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.DisableFootsteps();
        }
        
        Time.timeScale = 1f;
        
        try
        {
            SceneManager.LoadScene("Level3");
            Debug.Log("✅ Level 3 load command sent");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Failed to load Level 3: {e.Message}");
            
            // Try by index instead
            TryLoadLevel3ByIndex();
        }
    }
    
    void TryLoadLevel3ByIndex()
    {
        Debug.Log("🔍 Trying to find Level 3 by index...");
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            
            if (sceneName == "Level3")
            {
                Debug.Log($"✅ Found Level 3 at index {i}, loading...");
                SceneManager.LoadScene(i);
                return;
            }
        }
        
        Debug.LogError("❌ Level 3 not found in build settings!");
        ShowBuildSettingsInfo();
    }
    
    void ShowBuildSettingsInfo()
    {
        Debug.Log("📋 Current Build Settings:");
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            Debug.Log($"   Index {i}: {sceneName}");
        }
        
        Debug.Log("💡 If Level 3 is missing, add it to Build Settings:");
        Debug.Log("   File → Build Settings → Add Open Scenes (with Level3 open)");
    }
}
