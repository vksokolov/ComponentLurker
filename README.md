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
