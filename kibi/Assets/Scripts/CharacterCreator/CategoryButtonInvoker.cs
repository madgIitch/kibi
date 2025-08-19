using UnityEngine;
using UnityEngine.UI;
using CharacterCreator;

public class CategoryButtonInvoker : MonoBehaviour
{
    [SerializeField] private TabManager tabManager;
    [SerializeField] private EditCategory category;

    void Awake()
    {
        if (!tabManager) tabManager = FindFirstObjectByType<TabManager>(FindObjectsInactive.Include);
    }

    public void InvokeShow()
    {
        if (!tabManager) { Debug.LogError("[Invoker] TabManager NULL en " + name); return; }
        Debug.Log($"[Invoker] Click {category} -> TM:{tabManager.name} id:{tabManager.GetInstanceID()} map:{tabManager.DebugCount()} keys:{tabManager.DebugKeys()}");
        tabManager.Show(category);
    }
}
