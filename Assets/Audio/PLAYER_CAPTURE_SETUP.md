# Player Capture Sound Effect Setup

## 🎵 What's Been Added

### 1. AudioManager Enhancement
- Added `PlayPlayerCaptured()` method
- Searches for a sound named "PlayerCaptured"

### 2. Enemy Capture Logic
- Enhanced `EnemyBase.cs` with audio feedback
- Added 1-second delay before scene change to let sound play
- Uses coroutine for smooth transition

### 3. Audio Testing
- Press **"8"** key to test player capture sound
- Added to AudioTester controls

## 🔧 Setup Required

### AudioManager Configuration
1. In Unity, select your AudioManager GameObject
2. In the Inspector, find the AudioManager component
3. Add a new sound to one of the sound arrays (recommend `enemySounds`)
4. Set the sound properties:
   - **Name**: `PlayerCaptured`
   - **AudioClip**: Drag your capture sound file here
   - **Volume**: 0.7-1.0 (should be loud/dramatic)
   - **Pitch**: 1.0 (or adjust for effect)

### Recommended Sound Types
- Dramatic "game over" sound
- Evil laugh or growl
- Scary/tension sound effect
- Monster roar
- Dark musical sting

### Free Sound Resources
**Search terms for capture sounds:**
- "game over dramatic"
- "monster attack"
- "evil laugh"
- "horror sting"
- "defeat sound"
- "caught sound effect"

**Websites:**
- Freesound.org
- Zapsplat.com  
- OpenGameArt.org

## 🎮 How It Works

1. **Player Contact**: When player touches enemy collider
2. **Sound Plays**: `PlayPlayerCaptured()` is called
3. **Delay**: 1-second wait for dramatic effect
4. **Scene Change**: Game over scene loads

## ⚡ Testing

- Press **"8"** during gameplay to test the sound
- Walk into an enemy to trigger the full sequence
- Sound should play before scene changes

## 🎯 Pro Tips

- Use a dramatic, attention-grabbing sound
- Consider fade-out effects for smooth transition
- Test volume levels - capture sound should be prominent
- Can be combined with screen effects later
