# Global Component System (GCS)

Code-generated global access system for Unity.

## Motivation
In Unity, accessing global objects (such as game managers, UI managers, or audio systems) is usually done the same way as accessing local scene objects. This becomes inconvenient when these objects exist across all scenes.

At some point, you end up manually storing references to these global objects so they can be accessed from anywhere. This package automates that process.

**Global Component System (GCS)** generates a static class that provides easy access to your global components. It works across different scenes and automatically finds the required objects at runtime.
## Usage
1. Import the package (Window → Package Manager → + → Add package from Git URL)
2. Create GCS settings (Create → GCS → GlobalComponentSystem)
3. Add your scripts that should be global to the Global Components list
4. Press the Update button to generate the code
5. Access your global components in code:
```cs
void Awake()
{
    GCS.GameManager.LoadPlayer(); // example
}
```
