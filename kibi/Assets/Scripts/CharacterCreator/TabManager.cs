using System.Collections.Generic;
using UnityEngine;

public class TabManager : MonoBehaviour
{
    [System.Serializable]
    public class PanelEntry
    {
        public EditCategory category;
        public GameObject panelRoot;
    }

    public List<PanelEntry> panels = new();
    Dictionary<EditCategory, GameObject> map;
    EditCategory current;

    void Awake()
    {
        map = new Dictionary<EditCategory, GameObject>();
        foreach (var p in panels)
        {
            if (p.panelRoot) p.panelRoot.SetActive(false);
            map[p.category] = p.panelRoot;
        }
    }

    public void Show(EditCategory cat)
    {
        if (map != null && map.ContainsKey(current) && map[current])
            map[current].SetActive(false);

        current = cat;
        if (map != null && map.ContainsKey(cat) && map[cat])
            map[cat].SetActive(true);
    }
}
