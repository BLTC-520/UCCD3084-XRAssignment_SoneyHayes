
# XR Chemical Factory - VR Safety Training Simulation

A **Virtual Reality training simulation** for chemical factory safety procedures, built in Unity for VR platforms including **PICO, Oculus, and OpenXR devices**.

## Project Scope & Overview

This VR application provides immersive safety training in a chemical factory environment. Scenarios include:

* Fire detection and alarm response
* Fire suppression using interactive extinguishers
* Chemical spill management
* Emergency evacuation procedures
* Safe handling of hazardous materials

The layout is designed with realistic **hazard placement** (e.g., chemical storage zones) and strategically placed **extinguishers** to reflect industry-standard safety protocols.

## Project Structure

```
Assets/
├── AssignmentsScripts/     # Core gameplay and safety scripts
├── AssignmentsAudio/       # Audio assets and sound effects
├── ChemicalFactory/        # 3D models and factory environment
├── DSFireEx/              # Fire extinguisher systems
├── Vefects/               # Visual effects and particles
├── Scenes/                # VR training scenarios
└── _Course Library/       # Shared assets and utilities
```

## Key Scripts

- `FireDetector.cs` - Fire detection and alarm triggering
- `FireExtinguisher.cs` - VR fire suppression mechanics
- `ChemicalContainer.cs` - Chemical handling procedures
- `LiquidSpawner.cs` - Chemical spill simulation
- `SmoothGrabInteractable.cs` - Enhanced VR object interaction


## Pyrophoric Substance Simulation

To enhance scientific accuracy and immersion, the simulation incorporates **pyrophoric substances**—materials that ignite spontaneously on contact with air, passing fires to the boxes. (The boxes surface are flammable.)

We model **Triethylaluminum \[Al(C₂H₅)₃]**, a real-world aerospace ignition chemical.

## Technical Stack

* **Engine**: Unity 2022.3+ (URP)
* **Framework**: XR Interaction Toolkit 2.6.4
* **Platforms**: PICO (Android), PC VR (OpenXR)
* **Physics**: Unity Physics for object interactions
* **Audio**: 3D spatial sound effects

## Development & Assets (Reference)

1. **Fire VFX**: [Free Fire VFX URP](https://assetstore.unity.com/packages/vfx/particles/fire-explosions/free-fire-vfx-urp-266226)
2. **Factory Models**: [3D66 Factory Environment](https://3d.3d66.com) & [Industrial Pipeline](https://3d.3d66.com)
3. **Sound Effects**: [Pixabay – Metal Container Impact](https://pixabay.com/sound-effects/hitting-big-metal-container-361821/)

## Conclusion

By combining **VR interaction** with **real-world chemical phenomena**, this project trains users in hazard awareness, emergency response, and chemical safety—bridging **education and engagement** in a factory setting.

