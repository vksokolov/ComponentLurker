# ComponentLurker

Finds prefabs containing a target component with field values (not) equal to requested
![image](https://user-images.githubusercontent.com/25208150/166845932-32a4d26e-2d09-4d91-88d9-1a168daeded0.png)

# How to use
Open the ComponentLurker window
![image](https://user-images.githubusercontent.com/25208150/166846056-c9a0a015-4e25-41bf-b3b5-af70597dbd6b.png)

Drag a target script to the field
![image](https://user-images.githubusercontent.com/25208150/166846430-99597583-dfbb-4128-b0c7-d7a942bf6b85.png)

Select necessary fields (1), enter a target value (2), select comparison function (3)
![image](https://user-images.githubusercontent.com/25208150/166846984-6d7a869b-432f-4cf4-8e3d-5b47dfc1fa6d.png)

# Tech stuff
The tool uses Reflection to find fields
_type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
