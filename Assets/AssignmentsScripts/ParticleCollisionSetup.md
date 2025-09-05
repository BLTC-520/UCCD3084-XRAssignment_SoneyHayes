# Particle Collision Setup Guide
## Fire Detection + Fire Extinguisher System

This guide shows how to set up particle collision for both fire detection and fire extinguishing using the same underlying system.

## 🔥 System Overview

Your system uses **identical particle collision technology** for two purposes:

### Fire Extinguishing (Existing)
```
Spray Particles → Fire Objects → OnParticleCollision() → Suppress Fire
```

### Fire Detection (New Integration)  
```
Fire Particles → Detector → OnParticleCollision() → Trigger Alarms
```

## ⚙️ Unity Editor Setup

### Step 1: Enable Fire Particle Collision

**For ChemicalContainer fire particles:**
1. In scene, find your ChemicalContainer GameObject
2. Locate the **fireParticles** ParticleSystem (usually child object)
3. In Inspector, find **Collision** module
4. ✅ Check **"Collision"** to enable
5. Set **Type** to **"World"**
6. ✅ Check **"Send Collision Messages"**
7. Set **Quality** to "Medium" or "High" for better detection

**For FlammableObject fire particles:**
1. Repeat above steps for FlammableObject fire ParticleSystem
2. Ensure all fire particle systems have collision enabled

### Step 2: Position Fire Detector

**Create Detector:**
1. Create empty GameObject named "FireDetector"
2. Add `FireDetector.cs` script
3. Position on ceiling or wall where fire particles can reach
4. Script automatically adds SphereCollider (trigger)

**Configure Detection Radius:**
1. Adjust `Detection Radius` to cover fire particle spread area
2. Use Scene view gizmo to visualize detection range
3. Yellow wireframe sphere shows detection area

### Step 3: Setup Fire Alarms

**For each of your 3 Fire Alarms:**
1. Create GameObject (e.g., "FireAlarm_01")
2. Add `FireAlarm.cs` script
3. Add `AudioSource` component
4. Assign alarm sound clips to `Alarm Clips` array
5. Add `Light` component for flashing effect
6. Configure colors and flash speed

**Connect to Detector:**
1. Select FireDetector GameObject
2. Drag all 3 fire alarm objects into `Fire Alarms` list
3. Or leave empty - script auto-finds FireAlarm components

## 🧪 Testing the System

### Test Fire Detection
1. **Start Play Mode**
2. **Cause Fire**: Spill chemical container to create fire
3. **Check Particles**: Fire particles should be visible and moving
4. **Verify Detection**: When particles reach detector, alarms should trigger
5. **Debug Console**: Should show "Fire Detector: Particle collision detected!"

### Test Fire Extinguishing  
1. **Pull Pin**: Remove fire extinguisher pin first
2. **Spray Fire**: Use trigger to spray fire particles
3. **Check Suppression**: Fire intensity should decrease
4. **Complete Extinguish**: Continue spraying until fire is out

## 🔧 Troubleshooting

### Fire Detection Not Working

**Check Fire Particles:**
- Are fire particles visible and moving?
- Is Collision module enabled on fire ParticleSystem?
- Is "Send Collision Messages" checked?
- Is detector within particle spread range?

**Check Detector Setup:**
- Does detector have SphereCollider (trigger)?
- Is detection radius large enough?
- Are fire alarms properly assigned?

**Debug Steps:**
1. Enable Gizmos in Scene view to see detection radius
2. Check Console for debug messages
3. Test with smaller detection radius closer to fire

### Fire Extinguisher Not Working

**Check Spray Particles:**
- Is spray ParticleSystem collision enabled?
- Is spray GameObject named with "FireEX_PS" or tagged "Smoke"?
- Are particles reaching the fire?

**Check Fire Objects:**
- Do fire objects have FireExtinguisher.cs script?
- Is OnParticleCollision() method present?
- Check suppression rate settings

## 📋 Particle System Settings Reference

### Fire Particles (Detection Source)
```
Collision Module:
✅ Collision: Enabled  
Type: World
Collision Quality: Medium/High
✅ Send Collision Messages: Enabled
Bounce: 0.0
Lifetime Loss: 0.0
```

### Spray Particles (Suppression Source)
```
Collision Module:
✅ Collision: Enabled
Type: World  
Collision Quality: Medium/High
✅ Send Collision Messages: Enabled
Bounce: 0.0
Lifetime Loss: 1.0 (particles die on impact)
```

## 🎯 Key Points

1. **Same Technology**: Both systems use `OnParticleCollision(GameObject other)`
2. **Bidirectional**: Fire particles trigger detector, spray particles suppress fire
3. **Automatic**: Once particles are configured, collision detection is automatic  
4. **Performance**: Unity handles particle-object collision efficiently
5. **Scalable**: Works with any number of fire sources and detectors

## 🚀 Advanced Features

### Multiple Detectors
- Create multiple FireDetector objects for different zones
- Each can trigger different sets of fire alarms
- Useful for large buildings or separate areas

### Fire Suppression Systems
- Extend concept to ceiling sprinkler systems
- Use same particle collision for automatic fire suppression
- Combine with fire detection for realistic fire safety simulation

Your particle collision system creates a realistic fire safety simulation where:
- **Fire particles naturally trigger smoke detectors** 
- **Spray particles realistically extinguish fires**
- **Both use the same proven collision technology**