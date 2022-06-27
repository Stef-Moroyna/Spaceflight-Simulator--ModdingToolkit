## Button : [GUIElement](Element.md)

#### Description
`public class Button : GUIElement` is container for button.

------------

![Preview](https://i.imgur.com/W03Sm18.png)

------------

#### Fields
| Name  | Type | Description |
| :------------: | :------------: | :------------: |
| gameObject  | GameObject | GameObject of button |
| rectTransform  | RectTransform  | RectTransform of button |
| Size (get; set;) | Vector2 | Size of button |
| Position (get; set;) | Vector2 | Local position of button |
| Active (get; set;) | bool | Active of gameObject |
| OnClick (get; set;) | Action | Action that will be invoked on click |
| Text (get; set;) | string | Button title text |
| TextColor (get; set;) | Color | Button title text color |
| TextOpacity (get; set;) | float | Button title text opacity |

------------

#### Methods

| Method  | Description |
| :------------: | :------------: | 
| ` public ovveride void Init(GameObject self, GameObject parent)`  | Used by Builder to create button |

------------

#### Usage

You can create button by using Bulder.CreateButton method
