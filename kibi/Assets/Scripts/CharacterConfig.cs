using System;
using UnityEngine;

[Serializable]
public class CharacterConfig
{
    public string id = Guid.NewGuid().ToString();
    public string displayName = "Nuevo";

    // Apariencia
    public float eyeSize = 0.5f;
    public float mouthWidth = 0.5f;
    public float noseSize = 0.5f;
    public float hairHue = 0.0f; // 0..1 (Hue)

    // Personalidad (HEXACO)
    [Range(0, 1)] public float Honesty = 0.5f;  // Honesty–Humility
    [Range(0, 1)] public float Emotionality = 0.5f;  // Emotionality
    [Range(0, 1)] public float eXtraversion = 0.5f;  // eXtraversion
    [Range(0, 1)] public float Agreeableness = 0.5f;  // Agreeableness
    [Range(0, 1)] public float Conscientiousness = 0.5f;  // Conscientiousness
    [Range(0, 1)] public float Openness = 0.5f;  // Openness
    [Range(0,1)] public float Humor = 0.5f; // Extra: Humor

}
