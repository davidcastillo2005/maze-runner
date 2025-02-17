# INFORME

Proyecto de la asignatura de Programación de Ciencia de la Computación.

## Resumen

El proyecto es un juego multijugador para dos personas, contrarios entre sí. El objetivo de cada jugador es llegar a la salida antes que el oponente en un laberinto generado para cada partida. Habrá trampas que entorpecerán a los jugadores, quitándoles ventaja y dominio sobre el laberinto. Contrarrestarlas es el uso principal de las habilidades a elegir.

Las habilidades son únicas por ficha, y hay 5 para elegir. Los jugadores las usan para esquivar trampas o desfavorecer al oponente.

## Dependencias y ejecución

Hecho en Godot v4.3.stable.mono.official [77dcf97d8].

En [Game](../Game/) habrá archivos del juego exportado para _Windows_ y _Linux_.

## Flujo e interacciones

### Menú

Al abrir el juego, se presentará el Menú junto al título. Si decides iniciar un nuevo juego, te enviará al `Editor`.

- `New Game`

  > Inicia el juego.

- `Quit`

  > Abandona el juego.

![alt text](image-7.png)

### Editor

Su función es configurar la partida. Sus opciones son:

1. Cambiar el nombre del jugador 1, escribiendo sobre el panel "Player One".

   > Si no se elige un nombre, se asignará a los jugadores 1 y 2: "Player One" y "Player Two" respectivamente.

2. Elegir o no la habilidad del jugador 1, seleccionando una de las opciones del control "No Skill".

   > Los puntos 1 y 2 son iguales para el jugador 2, en los controles de "Player Two".

3. Elegir tamaño del laberinto, escribiendo sobre el control "Size".

4. Elegir el laberinto generado por una semilla aleatoria o predefinida, sobre los controles "Random" y "Seed".

   > Para elegir una semilla predefinida, tienes que escribirla sobre el panel "Seed" y desactivar el botón "Random".

5. Iniciar la partida, presionando el botón "Start".
   > Con las opciones elegidas en los puntos 1, 2, 3 y 4 iniciarás una partida.

![alt text](image-4.png)

#### Controles

Las teclas son detectadas mediante el _Input Map_ de _Godot_.

_Player One_

- Moverse:

  - Derecha: `A`.

  - Izquierda: `D`.

  - Arriba: `W`.

  - Abajo: `S`.

- Cambiar cámara: `E`.

- Habilidad: `F`.

_Player Two_

- Moverse:

  - Derecha: `I`.

  - Izquierda: `L`.

  - Arriba: `J`.

  - Abajo: `K`.

- Cambiar cámara: `U`.

- Habilidad: `H`.

  > None: No usas habilidades, presionar `H` no tendrá efecto.

  > Shield: Atraviesa trampas presionando `H`.

  > Portal Gun: Presionando una tecla direccional + `H`, atraviesa paredes .

  > Blind: Provoca ceguera presionando `H`.

  > Mute: Silencia otras habilidades, presionando `H` dentro de un radio de 10 casillas.

  > Glare: Petrifica, presionando `H` dentro de un radio de 10 casillas.

## Estructura

### Clases

El código está separado en partes: [Visual](../Scripts), [Lógica](../Scripts/Logic/) y de [Datos](../Scripts/Data/).

#### Casillas

- [Tile](../Scripts/Data/Tile.cs#L3): Clase casilla del laberinto. El laberinto es un array bidimensional de casillas. En esta clase están definidos los componentes de la coordenada de las casillas.

- [Empty](../Scripts/Data/Tile.cs#L15) y [Wall](../Scripts/Data/Tile.cs#L58): Derivan de la clase Tile, representan el camino que puede recorrer el jugador y el que no puede atravesar respectivamente.

- [Trap](../Scripts/Data/Tile.cs#L30): Clase abstracta derivada de Empty, representa una trampa. Tiene una propiedad de tipo bool `IsActive` que define si la trampa está activada o no, junto a los métodos `Activate()` y `Deactivate()`.

- [Spikes](../Scripts/Data/Tile.cs#L40): Derivada de Trap, trampa de púas. Reduce la velocidad del jugador en un 10% por 10 segundos.

  > Posee una propiedad `Timer` de 10000 milisegundos.

- [Portal](../Scripts/Data/Tile.cs#L47): Derivada de Trap, trampa de portal. Traslada al jugador a una casilla vacía vecina.

- [Shock](../Scripts/Data/Tile.cs#L52): Derivada de Trap, trampa eléctrica. Paraliza al jugador y para liberarse tendrá que tratar de moverse 10 veces.

  > Posee una propiedad `Struggle` de 10.

#### Habilidades

- [Skill](../Scripts/Data/Skill.cs#L3): Clase abstracta habilidad del jugador.

  > `Radius`: Radio de efecto.

  > `BatteryLife`: Tiempo de enfriamiento para volver usar la habilidad.

- [Shield](../Scripts/Data/Skill.cs#L12): Derivada de la clase Skill, escudo contra trampas. Permite al jugador atravesar trampas.

  > `BatteryLife = 20`

- [Portal Gun](../Scripts/Data/Skill.cs#L17): Derivada de la clase Skill, lanza-portales. Permite atravesar paredes.

  > `BatteryLife = 20`

- [Blind](../Scripts/Data/Skill.cs#L22): Derivada de la clase Skill, ceguera. Provoca ceguera al contrario.

  > `Radius = 10`, 10 cuadrículas.

  > `BatteryLife = 20`

  > `Timer = new(10000)`, 10000 milisegundos.

- [Mute](../Scripts/Data/Skill.cs#L29): Derivada de la clase Skill, silenciador. Prohíbe al contrario usar su habilidad por un tiempo predefinido.

  > `Radius = 10`, 10 cuadrículas.

  > `BatteryLife = 20`

  > `Timer = new(10000)`, 10000 milisegundos.

- [Glare](../Scripts/Data/Skill.cs#L36): Derivada de la clase Skill, intimidación. Paraliza al contrario por un tiempo predefinido.

  > `Radius = 10`, 10 cuadrículas.

  > `BatteryLife = 20`

  > `Timer = new(10000)`, 10000 milisegundos.


#### Generador de laberintos

Al instanciar [Maze Generator](../Scripts/Logic/MazeGenerator.cs#L6) se crea recibiendo de parámetros tamaño y semilla predefinida o aleatoria; para luego generar un array bidimensional de casillas con `GenerateMaze()`.

#### Global

[Global](../Scripts/Global.cs#L6) es un nodo que está presente durante el curso del juego. Tendrá propiedades que son usadas entre escenas.

#### Board

[Board](../Scripts/Board.cs#L6) es una clase derivada de TileMapLayer, pinta el laberinto en el mundo del juego junto a las colisiones de las paredes del laberinto.

#### Editor

[Editor](../Scripts/Editor.cs#L6) es una clase derivada de Control, maneja la escena para configurar el laberinto y los personajes. Permite elegir nombres a los jugadores, una habilidad de 5, tamaño del laberinto y una semilla predefinida o aleatoria.

#### World

[World](../Scripts/World.cs#L6) es una clase derivada de Node2D, es el mundo del juego. Cuando aparece en escena genera el laberinto.

#### Player

[Player](../Scripts/Player.cs#L8) es una clase derivada de CharacterBody2D, representa a los jugadores en el juego. Maneja el movimiento, habilidades y estados del jugador.

#### PlayerCamera

[PlayerCamera](../Scripts/PlayerCamera.cs#L7) es una clase derivada de Camera2D, maneja la cámara del jugador. Tiene tres estados:

- Player

  > Sigue al jugador.

- Free

  > Mueve libremente la camara.

- Extensive.

  > Activada cuando el laberinto es lo suficientemente pequeño (`Size < 12`).

#### PlayerUi

[PlayerUi](../Scripts/PlayerUi.cs#L6) es una clase derivada de Control, maneja la interfaz de usuario del jugador. Muestra el nombre del jugador, la habilidad y la energía.

#### PlayerTwoSubViewport

[PlayerTwoSubViewport](../Scripts/PlayerTwoSubViewport.cs#L5) es una clase derivada de SubViewport, maneja el subviewport del segundo jugador.

#### GameOver

[Game Over](../Scripts/GameOver.cs#L5) es una clase derivada de Node2D, cuando termina el juego muestra quién de los jugadores ganó o perdió.

Si presionas el botón `Restart`, cambiará a la escena Editor. Si presionas `Menu`, te cambiará a la escena del Menú.

## Extras

### Vídeos del desarrollo

[Generación por ruido en Godot 4.3](../Informe/Grabación%20de%20pantalla%202024-12-15%20234950.mp4)

[Intento de discretizar el movimiento](../Informe/Grabación%20de%20pantalla%202024-12-31%20212633.mp4)

[Probando las trampas](../Informe/Grabación%20de%20pantalla%202025-01-07%20084955.mp4)

[Probando las trampas 2](../Informe/Grabación%20de%20pantalla%202025-01-10%20022538.mp4)
