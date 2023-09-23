# ComponentLurker

"ComponentLurker" is a Unity tool designed to enhance your prefab and component search experience. Instead of manually inspecting each prefab, use ComponentLurker to query based on component fields and their values.

# Features

- **Component Selection:** Drag and drop a MonoScript and the tool will auto-detect its fields.
- **Field Processors:** Select from available field types if a class-processor exists for them.
- **Dynamic Comparison:** Add specific fields and choose comparison functions like "IsEqual", "Greater", and more.
- **Precise Results:** Fetch all prefabs matching your criteria in one click.

# Quick Start

1. **Drag a MonoScript:** For example, drag the built-in "Image" script.
2. **Choose Fields:** If there are class-processors, select from available field types like Sprite or int.
3. **Add Criteria:** Select a specific field by name, and choose a comparison function.
4. **Search:** Click "Find" to see all prefabs that match your criteria.

# Example

To find all prefabs with `Test` components that don't have a sprite set:

1. Drag the `Test` component to ComponentLurker.
2. Choose the Sprite field type.
3. Add the `SpriteVal` field.
4. Set the comparison to "IsEqual" and leave the sprite field blank.
5. Click "Find".

![Unity_0n2ZxfSrjI](https://github.com/vksokolov/ComponentLurker/assets/25208150/93e140e6-5a8b-476d-ae0d-233c1b8d320f)

# Custom Types

ComponentLurker provides a robust system to create custom drawers for specific types, ensuring a tailored and seamless integration with your unique components and classes.

## BaseDrawer: The Core
At the heart of this customization lies the BaseDrawer<T> abstract class. By extending this class, you can craft a drawer that's specifically designed for a particular type.

```
public abstract class BaseDrawer<T> : BaseDrawer
```

## Example: SpriteDrawer

For instance, if you want to have a custom drawer for the Sprite type, you'd extend the BaseDrawer<Sprite> like so:
[*SpriteDrawer.cs*](https://github.com/vksokolov/ComponentLurker/blob/main/ComponentLurker/Editor/Drawers/SpriteDrawer.cs)

```
public class SpriteDrawer : BaseDrawer<Sprite>
{
    // Define which operations are allowed
    public override ComparisonOperations AllowedOperations => 
        ComparisonOperations.Equal | 
        ComparisonOperations.NotEqual;

    // Specify how the Sprite type is drawn in the editor
    private static readonly Func<Sprite, GUILayoutOption[], Sprite> DrawFunc = (value, options) =>
    {
        var sprite = EditorGUILayout.ObjectField(value, typeof(Sprite), false, options) as Sprite;
        return sprite;
    };
    
    // ... rest of the SpriteDrawer class
}
```

This SpriteDrawer class is now equipped to draw a Sprite in the Unity Editor and decide how it should be compared with other sprites.

## Crafting Your Own

1. Select Your Type: Determine which type you want to customize the drawer for.
2. Extend BaseDrawer: Extend the BaseDrawer<T> class, replacing T with your chosen type.
3. Define Draw Functions: Specify how your type should be drawn and compared in the Unity Editor.
4. Use Your Custom Drawer: Once defined, your custom drawer will integrate seamlessly with ComponentLurker, enhancing your search capabilities.

With custom drawers, you're not limited by the built-in types and comparisons. ComponentLurker grows and adapts with your project's needs.

## Flexibility with File Placement

Good news for developers who like to organize their files their way: custom drawers created for ComponentLurker are found using reflection. This means you **don't have to place them in any specific directory**. Organize your project as you see fit, and ComponentLurker will still seamlessly identify and integrate your custom drawers.
