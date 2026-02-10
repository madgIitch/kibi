# Kibi - Simulador Social con IA Generativa

## 🎯 Visión del Proyecto

**Kibi** es un simulador social 2D donde los jugadores crean NPCs (Personajes No Jugadores) con personalidades únicas basadas en sistemas psicológicos. Los NPCs interactúan entre sí de forma autónoma, generando diálogos y acciones dinámicas mediante Inteligencia Artificial.

### Concepto Principal

- **Editor de Personajes**: Sistema para crear y personalizar NPCs
- **Personalidades Psicológicas**: Cada NPC tiene una personalidad única (MBTI, Big Five, etc.)
- **IA Generativa**: Genera diálogos y comportamientos dinámicos en tiempo real
- **Simulación Autónoma**: Los NPCs interactúan entre sí sin intervención del jugador
- **Mundo Viviente**: Los NPCs se mueven, conversan y realizan acciones en diferentes zonas

---

## 🏗️ Arquitectura Técnica

### Motor y Tecnologías

- **Unity 2D** con Universal Render Pipeline (URP)
- **NavMeshPlus**: Sistema de navegación 2D para movimiento de NPCs
- **TextMeshPro**: Renderizado de texto y diálogos
- **LLM Local**: Generación de contenido mediante modelos de lenguaje locales

### Estructura del Proyecto

```
kibi/
├── My project/
│   ├── Assets/
│   │   ├── Scenes/
│   │   │   ├── Plaza.unity          # Escena principal (zonas de interacción)
│   │   │   └── SampleScene.unity    # Escena de prueba
│   │   ├── Sripts/
│   │   │   ├── ZoneGizmo.cs         # Visualización de zonas en editor
│   │   │   └── WorldZoneFitToCamera.cs  # Ajuste de zonas a cámara
│   │   └── Settings/
│   │       └── URP Configuration     # Configuración de renderizado 2D
│   └── Packages/
│       └── NavMeshPlus              # Sistema de navegación 2D
```

---

## 🤖 Sistema de IA - LLM Local

### ¿Por qué LLM Local?

**Ventajas sobre APIs externas:**

| Aspecto | LLM Local | API Externa |
|---------|-----------|-------------|
| **Costos** | ✅ Gratuito tras instalación | ❌ Pago por token (muy caro con múltiples NPCs) |
| **Privacidad** | ✅ Datos en máquina del jugador | ❌ Datos enviados a servidores externos |
| **Offline** | ✅ Funciona sin internet | ❌ Requiere conexión constante |
| **Latencia** | ✅ Sin delays de red | ❌ Depende de conexión y servidores |
| **Control** | ✅ Fine-tuning y personalización | ❌ Limitado a lo que ofrece la API |

### Solución Recomendada: Ollama

**Ollama** es la mejor opción para este proyecto:
- Instalación simple y gestión fácil de modelos
- API REST local (`localhost:11434`)
- Soporta múltiples modelos optimizados (GGUF)
- Integración directa con Unity mediante HTTP requests
- Comunidad activa y buena documentación

### Modelos Sugeridos

| Modelo | Parámetros | Velocidad | RAM | Uso Recomendado |
|--------|------------|-----------|-----|-----------------|
| **Llama 3.2** | 1B - 3B | ⚡⚡⚡ Muy rápido | 2-4 GB | NPCs secundarios, diálogos simples |
| **Phi-3 Mini** | 3.8B | ⚡⚡ Rápido | 4 GB | Balance ideal para conversaciones |
| **Qwen 2.5** | 7B | 🟡 Moderado | 8 GB | Diálogos complejos, NPCs principales |
| **Mistral 7B** | 7B | 🟡 Moderado | 8 GB | Versatilidad general |

**Estrategia de uso:**
- Modelos pequeños (1B-3B) para NPCs de fondo o fuera de cámara
- Modelos medianos (7B) para NPCs con los que el jugador interactúa directamente

---

## 🏛️ Arquitectura del Sistema de IA

### Flujo de Comunicación

```
┌─────────────────────────────────────────────────────┐
│                    Unity (C#)                       │
│                                                     │
│  ┌──────────────────────────────────────────────┐ │
│  │         NPCDialogueManager                    │ │
│  │  - Gestiona conversaciones                    │ │
│  │  - Sistema de turnos                          │ │
│  │  - Contexto de personalidades                 │ │
│  └───────────────┬──────────────────────────────┘ │
│                  │                                  │
│  ┌───────────────▼──────────────────────────────┐ │
│  │         OllamaClient (HTTP)                   │ │
│  │  - UnityWebRequest                            │ │
│  │  - Serialización JSON                         │ │
│  │  - Manejo de prompts                          │ │
│  └───────────────┬──────────────────────────────┘ │
└──────────────────┼──────────────────────────────────┘
                   │ HTTP POST
                   │ localhost:11434/api/generate
┌──────────────────▼──────────────────────────────────┐
│              Ollama Server                          │
│  - Gestión de modelos                               │
│  - Optimización de inferencia                       │
│  - Cache de contexto                                │
└──────────────────┬──────────────────────────────────┘
                   │
┌──────────────────▼──────────────────────────────────┐
│         LLM Model (Llama/Phi/Qwen)                  │
│  - Generación de texto                              │
│  - Basado en personalidad del NPC                   │
└─────────────────────────────────────────────────────┘
```

### Componentes a Implementar

#### 1. **OllamaClient.cs**
```csharp
// Cliente HTTP para comunicarse con Ollama
public class OllamaClient : MonoBehaviour
{
    private const string OLLAMA_URL = "http://localhost:11434/api/generate";

    public async Task<string> GenerateResponse(string prompt, string model = "llama3.2:1b")
    {
        // Implementación de UnityWebRequest
    }
}
```

#### 2. **NPCPersonality.cs**
```csharp
// Sistema de personalidades psicológicas
public class NPCPersonality : ScriptableObject
{
    public string mbtiType;      // MBTI: INTJ, ENFP, etc.
    public float openness;       // Big Five
    public float conscientiousness;
    public float extraversion;
    public float agreeableness;
    public float neuroticism;

    public string GetPersonalityPrompt() { }
}
```

#### 3. **NPCDialogueManager.cs**
```csharp
// Gestiona las conversaciones entre NPCs
public class NPCDialogueManager : MonoBehaviour
{
    public async Task<string> GenerateDialogue(NPC speaker, NPC listener, string context)
    {
        // Construye prompt basado en personalidades
        // Llama a OllamaClient
        // Retorna diálogo generado
    }
}
```

#### 4. **NPCMemorySystem.cs**
```csharp
// Sistema de memoria para que NPCs recuerden interacciones
public class NPCMemorySystem
{
    private List<Interaction> shortTermMemory;
    private List<Interaction> longTermMemory;

    public void RememberInteraction(NPC npc, string context, string response) { }
    public string GetRelevantMemories(NPC npc) { }
}
```

---

## 🎮 Sistemas Implementados (Estado Actual)

### ✅ Completado

1. **Configuración de Unity 2D**
   - Universal Render Pipeline configurado
   - Sistema de iluminación 2D

2. **Sistema de Navegación**
   - NavMeshPlus integrado
   - NavMesh horneado para escena Plaza
   - NPCs pueden navegar por el mundo 2D

3. **Sistema de Zonas**
   - `ZoneGizmo.cs`: Visualización de zonas en el editor
   - `WorldZoneFitToCamera.cs`: Ajuste dinámico de zonas
   - Escena Plaza dividida en zonas de interacción

4. **Interfaz Estática**
   - Paneles de UI básicos
   - Character Editor (estático)

### 🚧 En Desarrollo

- **NavMesh Surface y Horneado** (Rama actual: `4-s13-navmeshsurface-y-horneado`)
- Optimización de zonas en Plaza

---

## 📋 Próximos Pasos

### Fase 1: Integración de IA Local

1. **Instalación y Configuración**
   - [ ] Instalar Ollama en la máquina de desarrollo
   - [ ] Descargar modelos recomendados (Llama 3.2 1B, Phi-3 Mini)
   - [ ] Probar API local con requests básicos

2. **Integración con Unity**
   - [ ] Crear `OllamaClient.cs` para comunicación HTTP
   - [ ] Implementar serialización JSON de requests/responses
   - [ ] Sistema de manejo de errores y timeouts

3. **Sistema de Prompts**
   - [ ] Plantillas de prompts para diferentes tipos de diálogo
   - [ ] Sistema de inyección de personalidad en prompts
   - [ ] Contexto de situación y memoria

### Fase 2: Sistema de Personalidades

1. **Definición de Personalidades**
   - [ ] Crear `NPCPersonality.cs` (ScriptableObject)
   - [ ] Implementar sistema MBTI o Big Five
   - [ ] Editor de personalidades en Unity Inspector

2. **Generación de Prompts**
   - [ ] Convertir rasgos de personalidad a descripciones textuales
   - [ ] Templates específicos por tipo de personalidad
   - [ ] Sistema de coherencia de respuestas

### Fase 3: Sistema de Diálogos

1. **NPCDialogueManager**
   - [ ] Gestor de conversaciones entre NPCs
   - [ ] Sistema de turnos de conversación
   - [ ] Detección de contexto (ubicación, NPCs cercanos)

2. **UI de Diálogos**
   - [ ] Burbujas de diálogo sobre NPCs
   - [ ] Sistema de log de conversaciones
   - [ ] Animaciones de texto

### Fase 4: Sistema de Memoria

1. **Short-term Memory**
   - [ ] Buffer de conversaciones recientes
   - [ ] Inyección en contexto de prompts

2. **Long-term Memory**
   - [ ] Persistencia de relaciones entre NPCs
   - [ ] Sistema de eventos importantes
   - [ ] Recuperación de memorias relevantes

### Fase 5: Comportamientos y Acciones

1. **Sistema de Acciones**
   - [ ] NPCs pueden decidir moverse a zonas
   - [ ] Iniciar conversaciones con otros NPCs
   - [ ] Realizar actividades (leer, comer, etc.)

2. **Generación de Decisiones**
   - [ ] IA decide qué acción tomar basado en personalidad
   - [ ] Sistema de necesidades/motivaciones

### Fase 6: Optimización

1. **Performance**
   - [ ] Cache de respuestas comunes
   - [ ] Pool de inferencias pre-generadas
   - [ ] Pausa de NPCs fuera de cámara
   - [ ] Sistema de prioridad de procesamiento

2. **Calidad de IA**
   - [ ] Fine-tuning de modelos (opcional)
   - [ ] A/B testing de diferentes modelos
   - [ ] Sistema de feedback para mejorar prompts

---

## 🔧 Consideraciones Técnicas

### Rendimiento

**Estrategias de Optimización:**

1. **NPCs por Prioridad**
   ```
   Alta Prioridad (7B model):  NPCs en pantalla, interactuando con jugador
   Media Prioridad (3B model): NPCs en pantalla, interacciones entre ellos
   Baja Prioridad (1B model):  NPCs fuera de pantalla, background
   Pausa:                       NPCs muy lejos de la acción
   ```

2. **Cache Inteligente**
   - Respuestas a saludos genéricos pre-generadas
   - Diálogos comunes por tipo de personalidad
   - Refresh periódico para evitar repetición

3. **Procesamiento Asíncrono**
   - Generación de diálogos no bloquea el juego
   - NPCs "piensan" antes de hablar (naturalidad + tiempo de procesamiento)
   - Sistema de cola para requests a Ollama

### Requisitos del Sistema (Jugador)

| Componente | Mínimo | Recomendado |
|------------|--------|-------------|
| **CPU** | 4 cores | 8+ cores |
| **RAM** | 8 GB | 16 GB |
| **GPU** | Integrada | Dedicada (opcional para GPU inference) |
| **Disco** | 10 GB | 20 GB (para múltiples modelos) |
| **SO** | Windows 10/11, macOS, Linux | Cualquiera |

---

## 📚 Recursos y Referencias

### Ollama
- [Sitio oficial](https://ollama.ai/)
- [Modelos disponibles](https://ollama.ai/library)
- [API Documentation](https://github.com/ollama/ollama/blob/main/docs/api.md)

### Modelos de Personalidad
- **MBTI**: Myers-Briggs Type Indicator
- **Big Five**: Openness, Conscientiousness, Extraversion, Agreeableness, Neuroticism
- **Enneagram**: Sistema de 9 tipos de personalidad

### Unity + AI
- [UnityWebRequest Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html)
- [NavMeshPlus GitHub](https://github.com/h8man/NavMeshPlus)

---

## 🎨 Visión a Largo Plazo

### Features Futuras

1. **Relaciones Dinámicas**
   - NPCs forman amistades, enemistades, romances
   - Sistema de reputación y opiniones

2. **Eventos del Mundo**
   - Noticias que afectan conversaciones
   - Celebraciones, crisis, etc.

3. **Exportación de Historias**
   - Guardar conversaciones interesantes
   - Compartir NPCs creados por la comunidad

4. **Modo Sandbox vs. Modo Historia**
   - Sandbox: Observar interacciones libremente
   - Historia: Objetivos y misiones basados en NPCs

5. **Multiplayer/Compartir**
   - Compartir NPCs con otros jugadores
   - Mundo persistente compartido

---

## 👥 Equipo y Contribución

**Desarrollo actual**: Proyecto personal/pequeño equipo

**Tecnologías principales**:
- Unity 2D
- C#
- Ollama + LLMs locales
- Git para control de versiones

---

## 📄 Licencia

*Por definir*

---

**Última actualización**: Febrero 2026
**Versión del documento**: 1.0