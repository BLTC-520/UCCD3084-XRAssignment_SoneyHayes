# Fire Alarm System Setup Guide

## Overview
This fire alarm system consists of:
- **FireDetector.cs** - Detects fire particles and triggers alarms
- **FireAlarm.cs** - Individual alarm behavior with sound, lights, and effects

## Setup Instructions

### 1. Fire Detector Setup
1. **Create Detector GameObject**:
   - Create empty GameObject named "FireDetector"
   - Add `FireDetector.cs` script
   - Position where you want fire detection (usually ceiling-mounted)

2. **Configure Detection**:
   - Set `Detection Radius` (default: 5 units)
   - The script automatically adds a SphereCollider trigger

3. **Assign Fire Alarms**:
   - Drag your 3 FireAlarm GameObjects into the `Fire Alarms` list
   - Or leave empty - script will auto-find all FireAlarm components

4. **Optional Components**:
   - Add `Light` component for visual indicator
   - Add `AudioSource` for detector beep sound

### 2. Fire Alarm Setup (Repeat for each of your 3 alarms)
1. **Create Alarm GameObjects**:
   - Create GameObject named "FireAlarm_01", "FireAlarm_02", "FireAlarm_03"
   - Add `FireAlarm.cs` script to each

2. **Audio Setup**:
   - Add `AudioSource` component (auto-created if missing)
   - Drag alarm sound clips into `Alarm Clips` array
   - Set volume and loop settings

3. **Visual Effects**:
   - Add `Light` component for flashing alarm light
   - Set `Alarm Color` (default: red) and `Flash Speed`
   - Optional: Add `ParticleSystem` for warning effects

4. **Optional Animation**:
   - Add `Animator` component for alarm movement/rotation
   - Create animation with trigger named "AlarmTriggered"

### 3. Particle Collision Detection

For fire particles to trigger the detector, you need to enable collision on your fire particle systems:

#### Method 1: Particle System Collision
1. **Select Fire Particle System** (in ChemicalContainer or FlammableObject)
2. **Enable Collision Module**:
   - In Particle System Inspector, check "Collision"
   - Set Type to "World"
   - Enable "Send Collision Messages"
3. **Set Layer/Tag**:
   - Set particle GameObject layer or tag to "Fire" (optional)

#### Method 2: Automatic Detection (Recommended)
The FireDetector automatically detects:
- Objects with "fire" in the name
- Objects tagged as "Fire" 
- Objects with `ChemicalContainer.isOnFire = true`
- Objects with `FlammableObject.isOnFire = true`

### 4. Testing the System

1. **Test Individual Alarms**:
   - Right-click FireAlarm component → "Test Activate Alarm"
   - Should play sound, flash lights, start particles

2. **Test Full System**:
   - Start a fire (spill chemical container)
   - Fire particles should trigger detector within detection radius
   - All 3 fire alarms should activate simultaneously

3. **Debug Information**:
   - Check Console for debug messages
   - FireDetector shows detection radius as yellow wireframe sphere
   - Lines connect detector to linked fire alarms

### 5. Customization Options

#### FireDetector Settings:
- `Detection Radius` - How far detector can sense fire
- `Normal Color` / `Alert Color` - Detector light colors
- `Blink Speed` - How fast detector light blinks when triggered

#### FireAlarm Settings:
- `Alarm Duration` - How long alarm stays active (-1 = infinite)
- `Volume` - Alarm sound volume
- `Flash Speed` - How fast alarm light flashes
- `Alarm Color` - Color of alarm light when active

### 6. Integration with Existing Fire System

The system automatically integrates with your existing:
- `ChemicalContainer.cs` - Detects when `isOnFire = true`
- `FlammableObject.cs` - Detects when `isOnFire = true`
- Fire extinguisher can be enhanced to reset alarms when fire is extinguished

## Troubleshooting

**Detector not triggering:**
- Check detection radius covers fire source
- Ensure fire particles have collision enabled OR objects have fire components
- Verify fire alarms are assigned in detector's Fire Alarms list

**Alarms not sounding:**
- Check AudioSource has clip assigned
- Verify volume settings
- Test individual alarms with context menu

**No visual effects:**
- Ensure Light components are added to alarms
- Check alarm colors are different from background
- Verify ParticleSystem is assigned if using particle effects