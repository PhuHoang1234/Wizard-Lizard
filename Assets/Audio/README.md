# Audio Assets Directory

## Structure:
```
Assets/Audio/
├── Music/
│   └── (Background music files - .mp3, .wav, .ogg)
├── SFX/
│   ├── Player/
│   │   ├── footstep.wav
│   │   ├── run.wav
│   │   └── hide.wav
│   ├── Enemy/
│   │   ├── alert.wav
│   │   ├── chase.wav
│   │   └── patrol.wav
│   └── Environment/
│       ├── key_pickup.wav
│       ├── treasure_pickup.wav
│       └── door_open.wav
```

## Recommended Audio Sources:

### Free Audio Resources:
1. **Freesound.org** - High quality sound effects
2. **OpenGameArt.org** - Game-ready audio assets  
3. **Zapsplat.com** - Professional sound library (free with registration)
4. **BBC Sound Effects** - High quality library
5. **Unity Asset Store** - Free audio packages

### Suggested Search Terms:
- Player: "footstep stone", "running medieval", "spell cast", "stealth hide"
- Enemy: "monster growl", "alert sound", "chase music", "guard patrol"  
- Environment: "key pickup", "treasure chest", "door creak", "dungeon ambiance"
- Music: "fantasy ambient", "dungeon background", "medieval atmosphere"

### Audio Settings in Unity:
- **Format**: Compressed in Memory for SFX, Streaming for Music
- **Load Type**: Decompress On Load for short SFX, Streaming for Music
- **Compression**: Vorbis for most files, PCM for short, critical sounds
