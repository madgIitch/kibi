using System.Collections.Generic;
using UnityEngine;
using CharacterCreator;



public class TabManager : MonoBehaviour
{


    public int DebugCount() => map != null ? map.Count : -1;
    public string DebugKeys()
    {
        if (map == null) return "null";
        var s = "";
        foreach (var k in map.Keys) s += k + " ";
        return s.Trim();
    }

    [System.Serializable]
    public class PanelEntry
    {
        public EditCategory category;
        public GameObject panelRoot;
    }

    [Header("Paneles registrados (Inspector)")]
    public List<PanelEntry> panels = new();

    private Dictionary<EditCategory, GameObject> map = new();
    private EditCategory current;
    private GameObject currentGo;

    void Awake() => BuildMap("[Awake]");
    void OnEnable() => BuildMap("[OnEnable]"); // útil si usas Enter Play Mode sin domain reload
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying) BuildMap("[OnValidate]");
    }
#endif

    private void BuildMap(string from)
    {
        map.Clear();
        Debug.Log($"[TabManager] {from} → reconstruyendo mapa. panels={panels.Count}");

        foreach (var p in panels)
        {
            if (p == null) continue;

            // Apaga todos al construir
            if (p.panelRoot != null) p.panelRoot.SetActive(false);

            // Registra (aunque panelRoot sea null, lo dejamos constar)
            map[p.category] = p.panelRoot;

            Debug.Log($"[TabManager]   • {p.category} -> {(p.panelRoot ? p.panelRoot.name : "NULL")}");
        }
    }

    public bool HasOpenPanel => currentGo != null;

    public void CloseCurrent()
    {
        if (currentGo != null)
        {
            Debug.Log($"[TabManager] Cerrando {current} -> {currentGo.name}");
            currentGo.SetActive(false);
            currentGo = null;
        }
        else
        {
            Debug.Log("[TabManager] No hay panel activo que cerrar.");
        }
    }
    


    public bool HasPanel(EditCategory cat)
        => map != null && map.TryGetValue(cat, out var go) && go != null;

    public void Show(EditCategory cat)
    {
        Debug.Log($"[TabManager] Show({cat}) solicitado.");

        if (currentGo != null)
        {
            Debug.Log($"[TabManager]   ocultando actual: {current} -> {currentGo.name}");
            currentGo.SetActive(false);
        }

        if (map != null && map.TryGetValue(cat, out var go))
        {
            if (go != null)
            {
                current = cat;
                currentGo = go;
                currentGo.SetActive(true);
                Debug.Log($"[TabManager]   mostrando: {cat} -> {go.name}");
            }
            else
            {
                Debug.LogWarning($"[TabManager]   categoría '{cat}' registrada pero su panel es NULL.");
                currentGo = null;
            }
        }
        else
        {
            Debug.LogWarning($"[TabManager] No hay panel registrado para '{cat}'. Ignorando.");
            currentGo = null;
        }
    }
}
