using UnityEngine;

public class DisableAtStart : MonoBehaviour
{
    public GameObject[] a;

    void Awake()
    {
        foreach (GameObject obj in a)
            obj.SetActive(false);
    }
}