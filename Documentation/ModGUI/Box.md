## Box : [GUIElement](Element.md)

#### Description
`public class Box : GUIElement` is container for box.

------------

![Preview](https://i.imgur.com/YTPPu08.png)

Box with some elements inside

------------

#### Fields
| Name  | Type | Description |
| :------------: | :------------: | :------------: |
| gameObject  | GameObject | GameObject of box |
| rectTransform  | RectTransform  | RectTransform of box |
| Size (get; set;) | Vector2 | Size of box |
| Position (get; set;) | Vector2 | Local position of box |
| Active (get; set;) | bool | Active of gameObject |
| Color (get; set;) | Color | Box color |
| Opacity (get; set;) | float | Opacity of box |

------------

#### Methods

| Method  | Description |
| :------------: | :------------: | 
| ` public ovveride void Init(GameObject self, GameObject parent)`  | Used by Builder to create box |
| `public HorizontalOrVerticalLayoutGroup CreateLayoutGroup(LayoutType type)` | Creates new layout group or return existing |

------------

#### Usage

You can create box by using Bulder.CreateBox method
