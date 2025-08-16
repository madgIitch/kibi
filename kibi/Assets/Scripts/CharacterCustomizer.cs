using UnityEngine;

public class CharacterCustomizer : MonoBehaviour
{
    [Header("Refs")]
    public SkinnedMeshRenderer face; // opcional (si no tienes blendshapes aún)
    public Renderer hair;            // renderer del pelo (obligatorio para probar)

    [Header("Blendshape names")]
    public string eyesSizeBS = "Eyes_Size";
    public string mouthWidthBS = "Mouth_Width";
    public string noseSizeBS = "Nose_Size";

    int EyesIdx = -1, MouthIdx = -1, NoseIdx = -1;
    Material hairMatInstance;

    void Awake()
    {
        if (face && face.sharedMesh)
        {
            var mesh = face.sharedMesh;
            EyesIdx  = mesh.GetBlendShapeIndex(eyesSizeBS);
            MouthIdx = mesh.GetBlendShapeIndex(mouthWidthBS);
            NoseIdx  = mesh.GetBlendShapeIndex(noseSizeBS);
        }
        if (hair) hairMatInstance = hair.material; // instancia segura
    }

    public void Apply(CharacterConfig cfg)
    {
        if (face)
        {
            if (EyesIdx  >= 0) face.SetBlendShapeWeight(EyesIdx,  cfg.eyeSize    * 100f);
            if (MouthIdx >= 0) face.SetBlendShapeWeight(MouthIdx, cfg.mouthWidth * 100f);
            if (NoseIdx  >= 0) face.SetBlendShapeWeight(NoseIdx,  cfg.noseSize   * 100f);
        }

        if (hairMatInstance)
        {
            Color rgb = Color.HSVToRGB(Mathf.Repeat(cfg.hairHue,1f), 0.7f, 0.9f);
            if (hairMatInstance.HasProperty("_BaseColor")) hairMatInstance.SetColor("_BaseColor", rgb);
            else if (hairMatInstance.HasProperty("_Color")) hairMatInstance.SetColor("_Color", rgb);
        }
    }
}
