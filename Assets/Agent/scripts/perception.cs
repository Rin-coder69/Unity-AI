using UnityEngine;

public class perception : MonoBehaviour
{
    public string tagName;
    public float maxDistance;
    public float maxAngle;

    public abstract GameObject[] GetGameObjects();
}
