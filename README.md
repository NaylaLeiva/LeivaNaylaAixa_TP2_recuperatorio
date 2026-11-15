# Recuperatorio Trabajo Práctico N°2 - Programación de Videojuegos 1

* **Estudiante:** Leiva, Nayla Aixa
* **Carrera:** Tecnicatura Universitaria en Diseño Integral de Videojuegos (TUDIVJ)
* **Libreta Universitaria:** TUV000028
* **Institución:** Facultad de Ingeniería - Universidad Nacional de Jujuy (UNJu)
* **Versión de Unity:** 6000.3.27f1

---

## Descripción del Proyecto
Prototipo jugable 3D desarrollado en Unity bajo el escenario simulado para **CubeGame Studios**, integrando sistemas de detección e interacción por Raycast con filtrado de capas (`LayerMask`), inteligencia artificial enemiga mediante `NavMesh`, modelado de escenarios con `Terrain Tool`, gestión centralizada del flujo de juego (`GameManager`) y una Estación de Curación con recarga temporizada.

---

## Controles
* **W, A, S, D:** Desplazamiento del personaje
* **Shift Izquierdo:** Correr (mientras se avanza)
* **Mouse:** Rotación horizontal de cámara/personaje
* **Espacio:** Saltar (validado mediante Raycast al suelo)
* **F:** Recolectar ítems de puntuación en rango
* **Clic Izquierdo:** Disparar arma equipada
* **R:** Revivir en la Estación de Curación tras entrar en estado `GameOver`
* **P / Escape:** Pausar y reanudar la partida

---

## Correcciones y Mejoras Aplicadas
* **Desacoplamiento con `GameManager` (Singleton):** Se centralizó el control de los estados del juego (`Start`, `Playing`, `Paused`, `Victory`, `GameOver`), la gestión del puntaje y la pausa (`Time.timeScale = 0f`) mediante eventos (`OnGameStateChanged`, `OnScoreChanged`) hacia el `UIManager`, liberando de lógica innecesaria al script del jugador.
* **Raycasts con `LayerMask` y Prevención de Autodaño:** Se implementó un rayo hacia el suelo (`groundLayer`) para validar el salto y un rayo frontal desde la cámara (`interactableLayer`) para activar objetos a distancia, asegurando además que los disparos del jugador (`PlayerAttack`) ignoren su propio colisionador (`targetHealth.gameObject != gameObject`).
* **Sistema de Salud e Inmunidad:** Se configuró `HealthSystem` con un tiempo de inmunidad (`playerImmunityCooldown = 1.2s`) para evitar que el contacto continuo con enemigos o zonas de peligro reduzca toda la vida en un solo frame.
* **Navegación IA con `NavMeshAgent`:** Se configuró `EnemyAI` con validación inicial en la malla (`NavMesh.SamplePosition`), patrullaje por puntos de ruta (`patrolPoints`), persecución al detectar al jugador y ataque con enfriamiento.

---

## Estación de Curación (Requisito Extra)
Se diseñó una plataforma de curación identificable visualmente mediante luz puntual y sistema de partículas (`HealingStation.cs`):
* **Curación Progresiva:** Al ingresar el jugador herido al área trigger, restaura **+10 HP cada 1.5 segundos**.
* **Límite de Carga y Cooldown:** Posee una capacidad máxima de **50 HP**. Al agotarse su carga, entra en estado deshabilitado durante **8 segundos** antes de volver a estar operativa.
* **Retroalimentación Visual y por Consola:** Durante el tiempo de enfriamiento apaga automáticamente su luz y partículas, notificando por consola el estado *"Estación en Recarga"*.

---

## Requisitos Opcionales Implementados
**Opción D - Zona de Desaceleración / Barro (`MudZone.cs`):** Área delimitada por un collider trigger que reduce la velocidad de desplazamiento del jugador al 50% (`SetSpeedMultiplier(0.5f)`) al ingresar y la restablece a su valor original al salir.
**Opción E - Spawner / Generador Temporizado de Enemigos (EnemySpawner.cs): Punto de generación de refuerzos enemigos que instancia unidades de forma periódica mediante InvokeRepeating utilizando puntos de referencia en la escena (spawnPoints), con validación de posición sobre el NavMesh y sincronización con el GameManager para pausarse al morir el jugador y reanudarse al revivir.
**Opción F - Zona de Gas Tóxico / Peligro (ToxicGasZone.cs): Zona de riesgo complementada con un sistema visual de humo (Particle System), la cual descuenta salud de forma paulatina por tics (damagePerTick) mediante un intervalo de tiempo controlado (damageInterval) mientras el jugador permanece dentro del área.
---

## Capturas del Juego en Ejecución
<img width="1575" height="817" alt="image" src="https://github.com/user-attachments/assets/0a338910-1150-40c4-bdbe-14789cccda15" />
<img width="1520" height="801" alt="image" src="https://github.com/user-attachments/assets/92e2beb0-4100-4a36-9b2f-a7a007a9176f" />
<img width="1462" height="837" alt="image" src="https://github.com/user-attachments/assets/ec774ded-c779-40af-8882-35c675a131cb" />
