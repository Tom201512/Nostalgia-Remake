using UnityEngine;

public abstract class UIBaseClass : MonoBehaviour
{
    public void ToggleUI(bool value) => gameObject.SetActive(value);
}