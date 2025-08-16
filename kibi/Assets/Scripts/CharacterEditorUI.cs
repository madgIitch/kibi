using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class CharacterEditorUI : MonoBehaviour
{
    [Header("Refs")]
    public CharacterCustomizer customizer;   // Arrastra el Character con CharacterCustomizer
    public TMP_InputField nameInput;

    [Header("Sliders Apariencia (0..1)")]
    public Slider eyeSize;
    public Slider mouthWidth;
    public Slider noseSize;
    public Slider hairHue;

    [Header("Sliders Personalidad (0..1)")]
    public Slider sHonesty;
    public Slider sEmotionality;
    public Slider sExtraversion;
    public Slider sAgreeableness;
    public Slider sConscientiousness;
    public Slider sOpenness;

    // (Opcional) Textos que muestren el valor 0..100 al lado de cada slider
    [Header("Value Labels (opcional)")]
    public TMP_Text vEyeSize, vMouthWidth, vNoseSize, vHairHue;
    public TMP_Text vHonesty, vEmotionality, vExtraversion, vAgreeableness, vConscientiousness, vOpenness;

    CharacterConfig cfg = new CharacterConfig();

    void Start()
    {
        // Apariencia
        SetupSlider(eyeSize,     v => { cfg.eyeSize = v;     Apply(); UpdateLabel(vEyeSize, v); });
        SetupSlider(mouthWidth,  v => { cfg.mouthWidth = v;  Apply(); UpdateLabel(vMouthWidth, v); });
        SetupSlider(noseSize,    v => { cfg.noseSize = v;    Apply(); UpdateLabel(vNoseSize, v); });
        SetupSlider(hairHue,     v => { cfg.hairHue = v;     Apply(); UpdateLabel(vHairHue, v); });

        // Personalidad (HEXACO sin Humor en UI)
        SetupSlider(sHonesty,           v => { cfg.Honesty           = v; UpdateLabel(vHonesty, v); });
        SetupSlider(sEmotionality,      v => { cfg.Emotionality      = v; UpdateLabel(vEmotionality, v); });
        SetupSlider(sExtraversion,      v => { cfg.eXtraversion      = v; UpdateLabel(vExtraversion, v); });
        SetupSlider(sAgreeableness,     v => { cfg.Agreeableness     = v; UpdateLabel(vAgreeableness, v); });
        SetupSlider(sConscientiousness, v => { cfg.Conscientiousness = v; UpdateLabel(vConscientiousness, v); });
        SetupSlider(sOpenness,          v => { cfg.Openness          = v; UpdateLabel(vOpenness, v); });

        if (nameInput) nameInput.onValueChanged.AddListener(t => cfg.displayName = t);

        LoadUI(cfg);
        Apply();
    }

    void SetupSlider(Slider s, System.Action<float> onVal)
    {
        if (!s) return;
        s.minValue = 0f; s.maxValue = 1f;
        s.onValueChanged.AddListener(v => onVal(v));
    }

    void UpdateLabel(TMP_Text t, float v)
    {
        if (t) t.text = Mathf.RoundToInt(v * 100f).ToString();
    }

    void LoadUI(CharacterConfig c)
    {
        if (nameInput) nameInput.text = c.displayName;

        if (eyeSize)     eyeSize.value     = c.eyeSize;
        if (mouthWidth)  mouthWidth.value  = c.mouthWidth;
        if (noseSize)    noseSize.value    = c.noseSize;
        if (hairHue)     hairHue.value     = c.hairHue;

        if (sHonesty)           sHonesty.value           = c.Honesty;
        if (sEmotionality)      sEmotionality.value      = c.Emotionality;
        if (sExtraversion)      sExtraversion.value      = c.eXtraversion;
        if (sAgreeableness)     sAgreeableness.value     = c.Agreeableness;
        if (sConscientiousness) sConscientiousness.value = c.Conscientiousness;
        if (sOpenness)          sOpenness.value          = c.Openness;

        // Labels (si existen)
        UpdateLabel(vEyeSize, eyeSize ? eyeSize.value : c.eyeSize);
        UpdateLabel(vMouthWidth, mouthWidth ? mouthWidth.value : c.mouthWidth);
        UpdateLabel(vNoseSize, noseSize ? noseSize.value : c.noseSize);
        UpdateLabel(vHairHue, hairHue ? hairHue.value : c.hairHue);

        UpdateLabel(vHonesty, c.Honesty);
        UpdateLabel(vEmotionality, c.Emotionality);
        UpdateLabel(vExtraversion, c.eXtraversion);
        UpdateLabel(vAgreeableness, c.Agreeableness);
        UpdateLabel(vConscientiousness, c.Conscientiousness);
        UpdateLabel(vOpenness, c.Openness);
    }

    void Apply() => customizer?.Apply(cfg);

    // --- Botones ---
    public void OnRandomize()
    {
        // Apariencia
        cfg.eyeSize = Random.value;
        cfg.mouthWidth = Random.value;
        cfg.noseSize = Random.value;
        cfg.hairHue = Random.value;

        // Personalidad (HEXACO)
        cfg.Honesty = Random.value;
        cfg.Emotionality = Random.value;
        cfg.eXtraversion = Random.value;
        cfg.Agreeableness = Random.value;
        cfg.Conscientiousness = Random.value;
        cfg.Openness = Random.value;

        // Humor se mantiene interno/dinámico (no tocar aquí)

        LoadUI(cfg);
        Apply();
    }

    public void OnReset()
    {
        cfg = new CharacterConfig();
        LoadUI(cfg);
        Apply();
    }

    public void OnSaveJSON()
    {
        string json = JsonUtility.ToJson(cfg, true);
        string path = Path.Combine(Application.persistentDataPath, $"{SafeName(cfg.displayName)}.json");
        File.WriteAllText(path, json);
        Debug.Log($"[Creator] Guardado en: {path}");
    }

    public void OnLoadJSON()
    {
        var dir = new DirectoryInfo(Application.persistentDataPath);
        var file = System.Array.Find(dir.GetFiles("*.json"), f => true);
        if (file == null)
        {
            Debug.LogWarning("[Creator] No hay JSON para cargar en persistentDataPath.");
            return;
        }

        string json = File.ReadAllText(file.FullName);
        cfg = JsonUtility.FromJson<CharacterConfig>(json);
        LoadUI(cfg);
        Apply();
        Debug.Log($"[Creator] Cargado: {file.FullName}");
    }

    string SafeName(string s)
    {
        foreach (var c in Path.GetInvalidFileNameChars())
            s = s.Replace(c, '_');
        return string.IsNullOrWhiteSpace(s) ? "character" : s.Trim();
    }
}
