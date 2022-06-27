## ModGUIElement

#### Description
`public abstract class GUIElement` is base class for all GUI elements.

Its derived classes are used as containers that contain everything you need to create and modify interfaces.

------------

#### Fields
| Name  | Type | Description |
| :------------: | :------------: | :------------: |
| gameObject  | GameObject | GameObject of GUI element |
| rectTransform  | RectTransform  | RectTransform of GUI element |
| Size (get; set;) | Vector2 | Size of element |
| Position (get; set;) | Vector2 | Local position of element |
| Active (get; set;) | bool | Active of gameObject |

------------

#### Methods

| Method  | Description |
| :------------: | :------------: |
| ` public abstract void Init(GameObject self, GameObject parent)`  | Used by Builder to create elements |
