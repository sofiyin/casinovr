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

### Semana 10 — E1 (15%) ✅

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

---

### Semana 11 — E2 (30%) ✅

Esta entrega incorpora la tragamonedas 3D en la escena, la tabla de pagos integrada en la máquina y el sistema de fichas con inventario visible en tiempo real.

---

#### RNF-02 — Modelo 3D de la tragamonedas con zonas de colisión ampliadas

> **Descripción del requisito:** Todos los objetos interactuables deben tener zonas de colisión de al menos 8 cm de radio virtual, ya que los controladores VR tienen menor precisión que un cursor y zonas pequeñas generan frustración en usuarios novatos.

**Cómo se abordó:**  
El modelo de la tragamonedas fue descargado de Sketchfab bajo licencia Creative Commons e importado a Unity como prefab. Se configuraron colisiones ampliadas en los elementos interactuables (botón de giro, ranura de monedas) superando los 8 cm de radio virtual exigidos, garantizando que los controladores del Meta Quest 2 puedan activarlos sin fallos de tracking.

---

#### RF-02 — Tabla de pagos visible en la máquina

> **Descripción del requisito:** Las reglas y combinaciones ganadoras deben mostrarse de forma visual dentro del entorno, sin necesidad de salir de la experiencia.

**Cómo se abordó:**  
Se integró un panel físico en el lateral de la tragamonedas que muestra las combinaciones ganadoras y sus multiplicadores directamente en el entorno 3D. La tabla es visible en todo momento sin interrumpir la inmersión, siguiendo el principio de interfaz diegética del framework de diseño: la información del estado del sistema está siempre dentro del mundo virtual, no en menús externos.

---

#### RF-04 — Sistema de fichas con inventario visual

> **Descripción del requisito:** El usuario debe disponer de un contador de saldo permanente y visible que se actualice en tiempo real tras cada jugada, para que pueda comprender la mecánica de apuesta observando cómo varían sus fichas.

**Cómo se abordó:**  
Se implementó un contador de fichas que se muestra en el panel frontal de la máquina. El saldo se actualiza en tiempo real tras cada interacción, permitiendo al usuario entender de forma inmediata la relación entre sus acciones y sus recursos disponibles.

---

### Próximas semanas

| Semana | Entregable | Avance | Tareas principales |
|---|---|---|---|
| 12 | E3 | 50% | Mecánica de giro (botón 3D), lógica de rodillos, retroalimentación luces + sonido, optimización de carga |
| 13 | E4 | 70% | Tutorial guiado en VR, pantalla de resumen, pruebas con ≥5 usuarios + métricas HEART |
| 14 | E5 | 100% | Correcciones post-evaluación, optimización ≥72 FPS Quest 2, redacción Hito 2 |
| 15 | E6 | Pres. | Slides + demo en vivo, ensayo general, entrega informe LaTeX |

---

## Requisitos del sistema

- Meta Quest 2 con Developer Mode activado
- Unity 2022.3 LTS o superior
- XR Interaction Toolkit 2.5+
- Android Build Support (para deploy en Quest)

## Créditos

- Modelo 3D tragamonedas: [Sketchfab](https://sketchfab.com/) — licencia Creative Commons
- Template base VR: [ValemVR](https://github.com/ValemVR/VR-Game-Jam-Template)
