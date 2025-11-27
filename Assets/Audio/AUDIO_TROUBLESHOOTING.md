# 🔧 Audio Troubleshooting Guide

## 🧪 Step-by-Step Debug Process

### 1. **ADD THE NEW DIAGNOSTIC SCRIPT**
1. In Unity, create an empty GameObject
2. Name it "AudioDiagnostic"
3. Add the `AudioDiagnosticAdvanced` script to it

### 2. **RUN COMPREHENSIVE TEST**
**Press these keys in Play mode:**
- **"D"** = Run detailed diagnostic
- **"T"** = Test PlayerCaptured sound directly
- **"V"** = Check volume and audio system
- **"8"** = Test via AudioTester (if you still have it)

### 3. **CHECK CONSOLE OUTPUT**
Look for these messages in Unity Console:
- ✅ = Good/Working
- ❌ = Problem found
- 💡 = Solution provided

### 4. **COMMON ISSUES & FIXES**

#### **Issue 1: AudioClip Not Assigned**
**Symptoms:** 
```
❌ PlayerCaptured sound found but AudioClip is NULL!
```
**Fix:**
1. Select AudioManager in scene
2. Expand Enemy Sounds array
3. Find "PlayerCaptured" entry
4. Drag an audio file to the "Clip" field

#### **Issue 2: Wrong Sound Name**
**Symptoms:**
```
❌ No sound named 'PlayerCaptured' found!
```
**Fix:**
1. In AudioManager, find your capture sound
2. Make sure "Name" field is EXACTLY: `PlayerCaptured` (case-sensitive)

#### **Issue 3: No AudioListener**
**Symptoms:**
```
❌ No AudioListener found!
```
**Fix:**
1. Select your Main Camera
2. Add "Audio Listener" component if missing

#### **Issue 4: Volume Too Low**
**Symptoms:** Sound plays but very quiet
**Fix:**
1. In AudioManager, set PlayerCaptured volume to 1.0
2. Check Unity's master volume (should be 1.0)
3. Check your computer's volume

#### **Issue 5: AudioManager Instance Null**
**Symptoms:**
```
❌ AudioManager.Instance is NULL!
```
**Fix:**
1. Make sure you have AudioManager GameObject in scene
2. Make sure it has AudioManager script attached
3. Make sure script is enabled

### 5. **MANUAL TEST**

If diagnostics pass but still no sound:

1. **Test with existing sounds first:**
   - Try pressing "1" for footsteps
   - Try pressing "2" for running
   - If these don't work, the whole audio system has issues

2. **Check Unity Audio Settings:**
   - Edit > Project Settings > Audio
   - Make sure "Disable Unity Audio" is NOT checked
   - Set Master Volume to 1.0

3. **Check Computer Audio:**
   - Make sure your speakers/headphones work
   - Test with other applications
   - Check volume mixer

### 6. **EMERGENCY FIXES**

#### **Quick Fix 1: Use Existing Sound**
If you can't get PlayerCaptured working:
1. Find a working sound (like PlayerFootstep)
2. Copy its settings to PlayerCaptured
3. Use the same audio clip temporarily

#### **Quick Fix 2: Test with Simple Sound**
1. Record yourself saying "captured" on your phone
2. Save as .wav or .mp3
3. Import to Unity and use that

#### **Quick Fix 3: Create AudioSource Manually**
```csharp
// Add this test to CaptureTest.cs
AudioSource testSource = gameObject.AddComponent<AudioSource>();
testSource.clip = yourAudioClip;
testSource.volume = 1.0f;
testSource.Play();
```

## 🎯 Expected Working Flow

When everything works correctly:

1. Press "T" → Hear capture sound immediately
2. Press "D" → See all ✅ in console
3. Walk into enemy → Hear capture sound → Game over

## 📞 Debug Checklist

- [ ] AudioManager GameObject exists in scene
- [ ] AudioManager script attached and enabled  
- [ ] Enemy Sounds array has PlayerCaptured entry
- [ ] PlayerCaptured has audio clip assigned
- [ ] PlayerCaptured name is exactly "PlayerCaptured"
- [ ] Volume is set to 0.8-1.0
- [ ] AudioListener exists (usually on Main Camera)
- [ ] Computer audio works with other apps
- [ ] Unity audio is not disabled

**Run the diagnostic script first - it will tell you exactly what's wrong!**
