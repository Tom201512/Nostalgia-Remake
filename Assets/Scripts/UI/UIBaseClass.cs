using UnityEngine;

public class UIBaseClass : MonoBehaviour
{
    public void ToggleUI(bool value) => gameObject.SetActive(value);
}