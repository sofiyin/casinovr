# Casino VR

Simulación de un entorno de casino en Realidad Virtual desarrollada en Unity para Meta Quest 2.

**Curso:** CS2H01 — Human Computer Interaction  
**Universidad:** UTEC Lima, Perú  
**Profesor:** Teófilo Chambilla  

**Equipo:**
| Nombre | Código |
|---|---|
| Max Bryam Antúnez Alfaro | 202110013 |
| Sofía Brenda Herrera Salazar | 202210167 |
| Yared Riveros Rodriguez | 202110310 |

---

## Stack tecnológico

- **Motor:** Unity (XR Interaction Toolkit)
- **Hardware objetivo:** Meta Quest 2
- **Lenguaje:** C#
- **Audio:** Unity Audio Mixer
- **Deploy:** Aplicación standalone sobre Meta Quest 2

---

## Avances por semana

### Semana 10 — E1 (15%)

Esta entrega establece la base del proyecto: entorno de desarrollo configurado, escena principal del casino construida y navegación por teleportación funcional.

---

#### RF-06 — Navegación por teleportación

> **Descripción del requisito:** El usuario se desplaza dentro del espacio del casino virtual mediante teleportación, apuntando con el controlador al suelo para moverse a la zona elegida. Este método es el estándar para usuarios novatos en VR porque elimina el riesgo de cybersickness asociado al desplazamiento continuo.

**Cómo se abordó:**  
Se implementó el sistema de teleportación utilizando el componente `Teleportation Provider` del XR Interaction Toolkit. Se definieron `Teleportation Area` en el suelo de cada zona de la escena (sala de máquinas y sala de mesas), marcadas visualmente para que el usuario identifique los puntos válidos de desplazamiento. Al apuntar con el ray interactor del controlador hacia el suelo, se activa una línea de previsualización de destino; al soltar el gatillo, la cámara se teletransporta instantáneamente a esa posición. Esta configuración no requiere ningún servicio externo ni red.

---

#### RNF-03 — Operación 100% offline

> **Descripción del requisito:** La aplicación debe funcionar completamente sin dependencia de servicios externos ni conectividad de red, dado que el contexto de demostración académica no garantiza acceso a internet.

**Cómo se abordó:**  
Se configuró el proyecto Unity con el XR Interaction Toolkit como capa de abstracción para el hardware Meta Quest 2, partiendo de la plantilla ValemVR como base. Toda la escena — sala principal del casino, iluminación (dorado/rojo estilo casino clásico), audio ambiente y assets 3D — se empaquetó localmente dentro del proyecto. El build se despliega directamente sobre el Meta Quest 2 como aplicación standalone, sin dependencias de red en tiempo de ejecución. Se verificó que la aplicación carga y opera sin conexión activa.
