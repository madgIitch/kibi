using UnityEngine;


/// <summary>
/// Aplica una CharacterConfig al personaje:
/// - Face: blendshapes (ojos, boca, nariz) si existen.
/// - Hair: color vía Hue (HSV -> RGB).
/// - Body: reservado para futuras personalizaciones (piel/ropa).
/// </summary>
public class CharacterCustomizer : MonoBehaviour
{
    [Header("Refs (asignar en el prefab/escena)")]
    [Tooltip("SkinnedMeshRenderer con blendshapes de la cara (opcional).")]
    public SkinnedMeshRenderer face;
    [Tooltip("Renderer del pelo (obligatorio para ver el color).")]
    public Renderer hair;
    [Tooltip("Renderer del cuerpo (opcional, por ahora no se modifica).")]
    public Renderer body;

    [Header("Blendshape names (deben existir en la malla de 'face')")]
    public string eyesSizeBS   = "Eyes_Size";
    public string mouthWidthBS = "Mouth_Width";
    public string noseSizeBS   = "Nose_Size";

    // Índices cacheados
    int _eyesIdx  = -1;
    int _mouthIdx = -1;
    int _noseIdx  = -1;

    // Instancias de materiales para no mutar materiales compartidos
    Material _hairMatInstance;
    Material _bodyMatInstance;

    void Awake()
    {
        // Cachea índices de blendshapes si hay face y malla válida
        if (face && face.sharedMesh)
        {
            var mesh = face.sharedMesh;
            _eyesIdx  = mesh.GetBlendShapeIndex(eyesSizeBS);
            _mouthIdx = mesh.GetBlendShapeIndex(mouthWidthBS);
            _noseIdx  = mesh.GetBlendShapeIndex(noseSizeBS);
        }

        // Crea instancias de materiales (seguro para edición en runtime)
        if (hair) _hairMatInstance = hair.material;
        if (body) _bodyMatInstance = body.material;
    }

    /// <summary>
    /// Punto de entrada: aplica toda la config al personaje.
    /// Llama a esto desde CharacterEditorUI cada vez que cambie un slider.
    /// </summary>
    public void Apply(CharacterConfig cfg)
    {
        if (cfg == null) return;

        ApplyFace(cfg);
        ApplyHair(cfg);
        // Reserva: aquí podrás añadir ApplyBody(cfg) cuando metas más opciones
    }

    void ApplyFace(CharacterConfig cfg)
    {
        if (!face) return;

        // Los blendshapes usan 0..100
        if (_eyesIdx  >= 0) face.SetBlendShapeWeight(_eyesIdx,  cfg.eyeSize    * 100f);
        if (_mouthIdx >= 0) face.SetBlendShapeWeight(_mouthIdx, cfg.mouthWidth * 100f);
        if (_noseIdx  >= 0) face.SetBlendShapeWeight(_noseIdx,  cfg.noseSize   * 100f);
    }

    void ApplyHair(CharacterConfig cfg)
    {
        if (_hairMatInstance == null) return;

        // Hue 0..1 -> Color RGB (saturación y valor fijos para buen look)
        var rgb = Color.HSVToRGB(Mathf.Repeat(cfg.hairHue, 1f), 0.7f, 0.9f);

        // Soporta shaders que usen _BaseColor (URP) o _Color (Standard)
        if (_hairMatInstance.HasProperty("_BaseColor"))
            _hairMatInstance.SetColor("_BaseColor", rgb);
        else if (_hairMatInstance.HasProperty("_Color"))
            _hairMatInstance.SetColor("_Color", rgb);
        // Si tu shader usa otro nombre, añádelo aquí.
    }

    // --- Helpers opcionales ---

    /// <summary>
    /// Permite reasignar renderers en runtime si cambias de peinado/cuerpo.
    /// </summary>
    public void SetHairRenderer(Renderer newHair)
    {
        hair = newHair;
        _hairMatInstance = hair ? hair.material : null;
    }

    public void SetFaceRenderer(SkinnedMeshRenderer newFace)
    {
        face = newFace;
        _eyesIdx = _mouthIdx = _noseIdx = -1;
        if (face && face.sharedMesh)
        {
            var mesh = face.sharedMesh;
            _eyesIdx  = mesh.GetBlendShapeIndex(eyesSizeBS);
            _mouthIdx = mesh.GetBlendShapeIndex(mouthWidthBS);
            _noseIdx  = mesh.GetBlendShapeIndex(noseSizeBS);
        }
    }

#if UNITY_EDITOR
    // Calidad de vida en editor: botón para auto-asignar por nombre
    [ContextMenu("Auto-assign by child names")]
    void AutoAssign()
    {
        if (!face)
            face = transform.Find("Face")?.GetComponent<SkinnedMeshRenderer>();
        if (!hair)
            hair = transform.Find("Hair")?.GetComponent<Renderer>();
        if (!body)
            body = transform.Find("Body")?.GetComponent<Renderer>();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
