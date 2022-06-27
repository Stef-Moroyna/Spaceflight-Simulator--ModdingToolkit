using UnityEngine.UI;

public static class Text_Static
{
    public static void SetLines(this Text text, params string[] lines)
    {
        text.Clear();

        foreach (string line in lines)
            text.text += text.text.Length == 0? line : "\n" + line;
    }
    public static void AddLines(this Text text, params string[] lines)
    {
        foreach (string line in lines)
            text.text += text.text.Length == 0 ? line : "\n" + line;
    }
    public static void Clear(this Text text)
    {
        text.text = "";
    }
}