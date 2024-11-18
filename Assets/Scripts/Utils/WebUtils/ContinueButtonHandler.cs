using UnityEngine;
using UnityEngine.UI;

public class ContinueButtonHandler : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private string baseUrl = "https://www.example.com";

    private void Start()
    {
        continueButton.onClick.AddListener(HandleContinue);
    }

    private void HandleContinue()
    {
        string query = WebUtils.GetQueryStringWhole();
        Application.OpenURL(baseUrl + query);
    }

    private void OnDestroy()
    {
        continueButton.onClick.RemoveListener(HandleContinue);
    }
}