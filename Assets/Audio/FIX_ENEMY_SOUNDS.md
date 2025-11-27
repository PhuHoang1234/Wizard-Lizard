# 🔧 Fix Enemy Sounds - Quick Setup Guide

## The Problem
Player sounds work, but enemy sounds don't = Missing enemy sound setup in AudioManager!

## ✅ Quick Fix (5 steps)

### Step 1: Add Enemy Sound Diagnostic
1. Create empty GameObject, name it "EnemySoundTest"
2. Add `EnemySoundDiagnostic` script to it
3. Press Play

### Step 2: Test Enemy Sounds
**Press these keys in Play mode:**
- **"E"** = Test Enemy Alert
- **"R"** = Test Enemy Chase  
- **"Q"** = Test Enemy Patrol
- **"C"** = Test Player Captured
- **"F"** = Full enemy sound setup check

### Step 3: Check Console Output
Look for these messages:
- ✅ = Sound found and working
- ❌ = Problem found
- 💡 = Exact solution provided

### Step 4: Add Missing Enemy Sounds

**Most likely you need to add these to AudioManager:**

1. **Select AudioManager GameObject** in scene
2. **Expand "Enemy Sounds" array** in Inspector
3. **Add these 4 sounds** (increase Size to add slots):

| Slot | Name | Clip | Volume |
|------|------|------|--------|
| 0 | `EnemyAlert` | Drag growl/roar sound | 0.8 |
| 1 | `EnemyChase` | Drag chase/angry sound | 0.6 |
| 2 | `EnemyPatrol` | Drag footstep/ambient sound | 0.4 |
| 3 | `PlayerCaptured` | Drag dramatic/defeat sound | 1.0 |

### Step 5: Test Again
- Press **"E", "R", "Q", "C"** - should hear all sounds
- Walk near enemy to trigger alert
- Get caught to trigger capture sound

## 🎵 Temporary Fix (Use Existing Sounds)

If you don't have enemy sounds yet:

1. **Copy Player Sounds:**
   - Use PlayerFootstep for EnemyPatrol
   - Use PlayerFootstep for EnemyAlert (temporarily)
   - Use PlayerFootstep for EnemyChase (temporarily)
   - Use PlayerFootstep for PlayerCaptured (temporarily)

2. **This will make sounds work** while you find proper enemy sounds

## 🔍 What Each Sound Does

- **EnemyAlert**: Plays when enemy first spots you
- **EnemyChase**: Plays occasionally while enemy chases you  
- **EnemyPatrol**: Plays during enemy patrol (currently unused)
- **PlayerCaptured**: Plays when enemy catches you (dramatic!)

## 🎯 Expected Behavior

**When working correctly:**
1. Enemy spots you → Hear alert sound + console shows "👹 Enemy spotted player!"
2. Enemy chases you → Occasionally hear chase sound
3. Enemy catches you → Hear capture sound → Game over after 1.5 seconds

## 📱 Quick Sound Resources

**For testing, use any .mp3/.wav file:**
- Record yourself making monster sounds on phone
- Download from Freesound.org (search "monster growl")
- Use any existing audio file temporarily

**The diagnostic script will tell you exactly what's missing!**
