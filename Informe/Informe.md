# INFORME

Proyecto de la asignatura de Programación de Ciencia de la Computación.

> Hecho en Godot v4.3.stable.mono.official [77dcf97d8].

## Resumen

El juego solo se puede jugar para dos personas, enemigos entre sí. El objetivo es llegar a la salida antes que el oponente, en un laberinto generado por partida. Habrá trampas que van entorpecer a los jugadores, quitandoles ventaja y dominio sobre el laberinto. Contrarestarlas es el uso principal de las habilidades de las fichas a elegir.

Las habilidades son únicas por ficha, y hay 5 para elegir. Los jugadores las usan para esquivar contra las trampas o desfavorecer al oponente.

## Godot Engine

Godot usa un sistema de nodos para administrar cada elemento del proyecto

---

### Escenas

El proyecto consiste en las escenas:

- `Menú`

  > Menú del juego , permite la elección de iniciar o abandonar el juego.

- `Editor`

  > Editor de la partida, permite elegir las fichas y la generación del laberinto.

- `Game`

  > Donde ocurre la partida

- `Player`

  > Base de los jugadores.

- `Player UI`

  > Base de la interfaz visual de cada jugador.

- `Game Over`

  > Final de la partida, permite reiniciar con la misma configuración o volver al Menú.

---

### Root

**root** es el nodo base.

![alt text](image-3.png)

---

### Global

_Godot_ tiene una función llamada _Autoload_ que permite cargar un _script_ en un nodo hijo del nodo _root_ durante todo el proceso del juego. Por eso en todo script basado en Godot va a existir una referencia al [Global](../Scripts/Global.cs). En este se guardará las variables a usar entre escenas.

![alt text](image-2.png)

![alt text](image-6.png)

---

### Menú

Al abrir el juego, se presentará por el Menu junto al título. Si la decisión es iniciar un nuevo juego, entonces te enviará al `Editor`.
Los botones funcionan gracias a una funcionalidad de Godot llamada Signals, que permite provocar eventos según ocurre una acción.

- `New Game`

  > Inicia el juego.

- `Quit`

  > Abandona el juego.

![alt text](image-7.png)

---

Esta escena contiene un solo _script_ [Menu](../Scripts/Menu.cs).

![alt text](image-11.png)

`OnPlayButtonDown()` y `OnQuitButtonDown()` están suscritos a la señal de presionar sus respectivos botones. De igual manera `ChangeSceneToFile()` y `Quit()` permiten cambiar a una escena y cerrar la ventana del juego.

![alt text](image-10.png)

---

### Editor

Su función es poder configurar la partida. Sus opciones son:

1. Cambiar el nombre del jugador 1, escribiendo sobre el panel "Player One".

> Si no se elige un nombre, se le asignará al jugador 1 y 2: "Player One" y "Player Two" respectivamente.

2. Elegir o no la habilidad del jugador 1, seleccionando una de las opciones del control "No Skill".

> Los puntos 1 y 2 son iguales para el jugador 2, en los controles de "Player Two".

3. Elegir tamaño del laberinto, escribiendo sobre el control "Size".

4. ELegir el laberinto generado por una semilla aleatoria o predefinida, sobre los controles "Random" y "Seed".

> Para elegir una semilla predefinida, tienes que escribirla sobre el panel "Seed" y desactivar el botón "Random".

5. Iniciar la partida, presionando el botón "Start".

> Con las opciones elegidas en los puntos 1, 2 , 3 y 4 iniciarás una partida.

![alt text](image-4.png)

---

La escena contiene un script en su nodo base [Editor](../Scripts/Editor.cs).



![alt text](image-5.png)

## Generador de laberintos

La script que define al generación de laberintos es [Maze Generator](../Scripts/Logic/MazeGenerator.cs). Es instaciado por el script Global cada vez que se abre el juego.

Al iniciar una partida, el script [Global](../Scripts/Global.cs) instancia el generador laberinto con los valores asignados en [Editor](../Scripts/Editor.cs).

![alt text](../Informe/image-8.png)

La clase MazeGenerator tiene las propiedades:

- `Size`

> El tamaño del laberinto.

- `Seed`,

> la semilla, el valor con el que se inicializa la generación pseudoaleatoria.

- `_random`

> Generador de números pseudoaleatorios.

Si la semilla es aleatoria, entonces se toma el tiempo en ticks y se los asigna a `Seed`; si no entonces toma el valor predefinido en el parámetro `seed`. Luego se instancia `_random` con el valor asignado a `Seed`.

![alt text](../Informe/image-9.png)

> El arreglo de direcciones (arriba, abajo, derecha e izquierda).

> Es un arreglo bidimensional de casillas, en él se guarda y manipula el laberinto.

## Trampas électricas, pegajosas y de portales.

A su vez en los espacios vacíos del laberinto pueden aparecer uno de los tres tipos trampas, provocando al jugador que las pisan ciertos efectos:

- `A!`

> Baja la velocidad en un 10% por 10 segundos.

- `Portal`

> Teletransporta a un espacio vacío vecino aleatorio.

- `Paralysis`

> Detiene el movimiento, al menos de que intente moverse 10 veces.

Añadidas para entorpecer el movimiento de los jugadores.s

## Errores a solucionar:

1. Cuando se juega una nueva partida después de haber ganado, el juego no resetea las habilidades.

[darsaveli´s Readme Markdown Syntax](https://github.com/darsaveli/Readme-Markdown-Syntax)

[Basic Writing And Formatting Syntax](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)
