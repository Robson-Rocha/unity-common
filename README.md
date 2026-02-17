# Unity Common Scripts and Extensions

A Unity package containing common utility scripts and extension methods for Unity projects.

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
- **Tests/**: Contains unit tests for the package
- **Documentation~/**: Contains package documentation

## Usage

Place your extension classes and utility scripts in the `Runtime` folder. They will be automatically included in your Unity project once the package is installed.

## Contributing

Feel free to submit issues and pull requests to improve this package.
