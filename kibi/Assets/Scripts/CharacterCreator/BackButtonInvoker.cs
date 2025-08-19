using UnityEngine;
using UnityEngine.UI;
using CharacterCreator; // <- aquí está EditCategory

public class BackButtonInvoker : MonoBehaviour
{
    [SerializeField] private TabManager tabManager;
    [SerializeField] private bool disableWhenNoPanel = true;

    // Opcional: volver a una pestaña por defecto tras cerrar
    [SerializeField] private bool goToDefaultAfterClose = false;
    [SerializeField] private EditCategory defaultCategory = EditCategory.Face;

    private Button btn;

    void Awake()
    {
        if (!tabManager)
            tabManager = FindFirstObjectByType<TabManager>(FindObjectsInactive.Include);

        btn = GetComponent<Button>();
    }

    void OnEnable()
    {
        if (disableWhenNoPanel && btn && tabManager != null)
            btn.interactable = tabManager.HasOpenPanel;
    }

    public void InvokeBack()
    {
        if (!tabManager) { Debug.LogError("[Back] Falta TabManager"); return; }

        tabManager.CloseCurrent();

        if (goToDefaultAfterClose)
            tabManager.Show(defaultCategory);

        if (disableWhenNoPanel && btn)
            btn.interactable = tabManager.HasOpenPanel;
    }

    // (Opcional) cerrar con ESC
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && tabManager != null && tabManager.HasOpenPanel)
            InvokeBack();
    }
}
