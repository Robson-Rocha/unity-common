# Unity Common Scripts and Extensions

A Unity package containing common utility scripts and extension methods for Unity projects.

## Repository Summary

This package provides reusable runtime helpers focused on gameplay utilities, math/vector helpers, event invocation helpers, and physics/collision convenience components.

### Runtime Types Overview

- `ColorExtensions`: Color rounding and alpha manipulation helpers.
- `DelegateExtensions`: Safe multicast delegate invocation helpers with aggregated results.
- `Destructible`: Damageable pooled component with optional flash, fade, fragment, and knockback effects.
- `Direction`: Flag enum for cardinal and diagonal directions.
- `DirectionExtensions`: Direction checks, flips, and conversions to angles/vectors.
- `FloatExtensions`: Float tolerance checks and timer decrement helper.
- `IEnumerableOfBooleanExtensions`: Boolean sequence `All` and `Any` helpers.
- `IgnoreCollisionsFromDirection`: Runtime component to ignore collisions by incoming direction and layer.
- `MonoBehaviourExtensions`: Component initialization and visibility helpers for MonoBehaviours.
- `Rigidbody2DExtensions`: Velocity assignment helpers with optional per-axis multipliers.
- `Vector2Extensions`: Vector direction checks, near-equality checks, and angle helpers.

### Delegates

- `BeforeDestroyHandler`
- `BeforeTakeDamageHandler`
- `CollisionEntering`
- `AddingTempIncomingDirectionToIgnore`

## Installation

### Install via Git URL

You can install this package in your Unity project using the Package Manager:

1. Open Unity Package Manager (Window > Package Manager)
2. Click the "+" button in the top-left corner
3. Select "Add package from git URL..."
4. Enter: `https://github.com/Robson-Rocha/unity-common.git`

### Install via manifest.json

Alternatively, you can add the following line to your `Packages/manifest.json` file:

```json
{
  "dependencies": {
    "com.robsonrocha.unity-common": "https://github.com/Robson-Rocha/unity-common.git"
  }
}
```

## Package Structure

- **Runtime/**: Contains runtime scripts and extension classes
- **Editor/**: Contains editor-specific utilities

## Usage

Place your extension classes and utility scripts in the `Runtime` folder. They will be automatically included in your Unity project once the package is installed.

## Contributing

Feel free to submit issues and pull requests to improve this package.
