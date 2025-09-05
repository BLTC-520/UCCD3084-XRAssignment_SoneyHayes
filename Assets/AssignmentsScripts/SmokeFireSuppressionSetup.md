# Smoke Fire Suppression System Setup Guide

## 🔥 System Overview

Your fire suppression system now uses **particle collision detection** to realistically extinguish fires with smoke particles from your fire extinguisher.

### How It Works
```
Fire Extinguisher Smoke Particles → Fire Objects → OnParticleCollision() → Reduce Fire Intensity → Extinguish
```

## 🧰 Components

### Updated Scripts

1. **ChemicalContainer.cs** - Enhanced with fire intensity system
2. **DetectFire.cs** - Updated to use particle collision detection  
3. **SmokeVR.cs** - Your existing fire extinguisher spray system (unchanged)

## ⚙️ Unity Editor Setup

### Step 1: Enable Smoke Particle Collision

**On your Fire Extinguisher smoke ParticleSystem:**

1. **Find the Smoke ParticleSystem**:
   - Usually under your fire extinguisher GameObject
   - Referenced in `SmokeVR.cs` as `public ParticleSystem smoke`

2. **Enable Collision Module**:
   - In ParticleSystem Inspector, find **"Collision"** module
   - ✅ Check **"Collision"** to enable
   - Set **Type** to **"World"** 
   - Set **Quality** to **"Medium"** or **"High"**
   - ✅ Check **"Send Collision Messages"**
   - Set **Bounce** to 0.0
   - Set **Lifetime Loss** to 0.8 (particles fade on impact)

3. **Name/Tag the Smoke GameObject**:
   - Ensure name contains "smoke", "Smoke", or "FireEX"
   - OR set Tag to "Smoke"
   - This helps scripts identify it as suppression agent

### Step 2: Setup Fire Objects

**For each ChemicalContainer that can catch fire:**

1. **Add DetectFire Component**:
   - Add `DetectFire.cs` script to ChemicalContainer GameObject
   - Or add as child object positioned where fire particles appear

2. **Configure Settings** in DetectFire.cs Inspector:
   ```
   Suppression Rate: 20 (intensity reduced per particle hit)
   Suppression Cooldown: 0.08 (seconds between hits)
   Show Debug Messages: ✅ (for testing)
   Enable Continuous Suppression: ✅ 
   Continuous Suppression Rate: 8 (per second in trigger zone)
   ```

3. **ChemicalContainer Settings**:
   ```
   Max Fire Intensity: 100
   Suppression Rate: 15 (per particle collision)
   Suppression Cooldown: 0.1
   Show Suppression Effects: ✅
   ```

## 🎮 How to Use

### Fire Extinguisher Operation
1. **Pull Pin**: Grab pin and pull it out (realistic physics)
2. **Aim at Fire**: Point nozzle towards burning object
3. **Pull Trigger**: Hold VR trigger or 'F' key (keyboard testing)
4. **Spray Fire**: Smoke particles will collide with fire
5. **Watch Suppression**: Fire intensity decreases gradually
6. **Complete Extinguish**: Fire goes out when intensity reaches 0

### Visual Feedback
- **Fire gets smaller** as intensity decreases
- **Fire color changes** briefly when suppressed (white/cyan tint)
- **Fire sound gets quieter** as intensity reduces
- **Console messages** show suppression progress

## 🧪 Testing the System

### Test Sequence
1. **Start Fire**: 
   - Drop/spill ChemicalContainer to create fire
   - Wait for fire to ignite and spread

2. **Test Detection**:
   - Console should show fire-related debug messages
   - Fire should have full intensity initially

3. **Test Suppression**:
   - Pull fire extinguisher pin
   - Spray smoke at fire
   - Console should show: "Smoke suppression applied to [name]! Fire intensity: XX/100"
   - Fire should visually reduce in size

4. **Complete Extinguish**:
   - Continue spraying until intensity reaches 0
   - Fire should completely stop
   - Console shows: "chemical fire completely extinguished!"

### Debug Console Messages
```
✅ Good: "Fire particles on [name] don't have collision enabled" (setup warning)
✅ Good: "煙霧粒子開始滅火！從 [smoke_source]" (particle collision detected)
✅ Good: "Smoke suppression applied to [fire]! Fire intensity: 85.0/100" (suppression working)
✅ Good: "[name] chemical fire completely extinguished!" (fire out)
```

## 🔧 Troubleshooting

### Smoke Not Extinguishing Fire

**Check Smoke Particles:**
- Is Collision module enabled on smoke ParticleSystem?
- Is "Send Collision Messages" checked?
- Are particles reaching the fire? (increase emission rate)
- Is GameObject named with "smoke" or tagged "Smoke"?

**Check Fire Objects:**
- Does fire object have DetectFire.cs script?
- Is fire object actually on fire? (check `isOnFire = true`)
- Are there console error messages?

**Check Collision Settings:**
- Collision Quality set to Medium/High
- Particles have enough velocity to reach target
- No barriers blocking particle path

### Fire Not Starting
- Drop ChemicalContainer with enough force (spillThreshold = 3f)
- Wait for fire delay (fireDelay = 1.5s)
- Check if fire particles are playing

### Performance Issues
- Reduce particle count in emission
- Lower collision quality to "Low"
- Increase suppression cooldown time

## 🎯 Advanced Features

### Dual Detection Methods
The system uses **two detection methods** for reliability:

1. **Primary**: `OnParticleCollision()` - Direct particle hits
2. **Backup**: `OnTriggerStay()` - Continuous suppression in zone

### Fire Intensity System
- Fire starts at 100% intensity
- Each smoke particle hit reduces intensity by 15-20 points
- Fire visuals scale down as intensity decreases
- Fire completely extinguished at 0 intensity

### Multiple Fire Types
System supports:
- **ChemicalContainer** fires (with intensity system)
- **FlammableObject** fires (instant extinguish)
- **FireExtinguisher** component fires (legacy support)

## 📋 Required Particle Settings Summary

### Smoke ParticleSystem (Fire Extinguisher)
```
Collision Module:
✅ Collision: Enabled
Type: World
Quality: Medium/High
✅ Send Collision Messages: Enabled
Bounce: 0.0
Lifetime Loss: 0.8

GameObject:
Name: Contains "smoke", "Smoke", or "FireEX"
OR Tag: "Smoke"
```

### Fire ParticleSystem (Optional - for fire detection)
```
Collision Module:
✅ Collision: Enabled (if using for alarm system)
Type: World
✅ Send Collision Messages: Enabled
```

Your fire suppression system now provides realistic particle-based fire extinguishing with visual feedback and intensity-based suppression!