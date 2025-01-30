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

- `Game Over``

> Final de la partida, permite reiniciar con la misma configuración o volver al Menú.

---

### Root

`root` es el nodo base, de tipo `Windows`

![alt text](image-3.png)

---

### GLobal

``Godot`` tiene una función llamada Autoload que permite cargar un script en un nodo hijo del nodo ``root``  durante todo el proceso del juego:

![alt text](image-2.png)

---

### Menú

Al abrir el juego, se presentará por el Menu junto al título y dos opciones:

- `New Game`
> Inicia un nuevo juego.

- `Quit`
> Abandona el juego.

Si tu decisión fue iniciar un nuevo juego, entonces te enviará al ``Editor``.

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

En la escena están

![alt text](image-5.png)

## Generador de laberintos

El código que detrás de la generación de laberintos es [Maze Generator](). En el script [Setting]() existe una instancia:

`public MazeGenerator MazeGenerator { get; private set; }`



Laberinto generado a partir de una semilla. Si se eligió una semilla "aleatoria",

## Trampas électricas, pegajosas y de portales.

A su vez en los espacios vacíos del laberinto pueden aparecer uno de los tres tipos trampas, provocando al jugador que las pisan ciertos efectos:

- Électrica 

> Baja la velocidad en un 10% por 10 segundos.

- Portal

> Teletransporta a un espacio vacío vecino aleatorio.

- Pegajosa

> Detiene el movimiento, al menos de que intente moverse 10 veces.

Añadidas para entorpecer el movimiento de los jugadores.
Los jugadores tienen la opción de poseer habilidades 

### Errores a solucionar:

1. Cuando se juega una nueva partida después de haber ganado, el juego no resetea las habilidades.

[darsaveli´s Readme Markdown Syntax](https://github.com/darsaveli/Readme-Markdown-Syntax)

[Basic Writing And Formatting Syntax](https://docs.github.com/en/get-started/writing-on-github/getting-started-with-writing-and-formatting-on-github/basic-writing-and-formatting-syntax)