#!/bin/sh
echo -ne '\033c\033]0;Maze Runner\a'
base_path="$(dirname "$(realpath "$0")")"
"$base_path/MAZESCAPE.x86_64" "$@"
