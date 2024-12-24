using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Compression;

namespace MazeRunner.Scripts.Data
{
    public class Tile
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Empty : Tile
    {
        public Empty(int x, int y) : base(x, y)
        {
        }
    }

    public class Wall : Tile
    {
        public Wall(int x, int y) : base(x, y)
        {
        }
    }

    public class Ability
    {
        public string Name
        {
            get;
            set;
        }
        public string Description
        {
            get;
            set;
        }
        public Ability(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public class Token : Tile
    {
        int Speed
        {
            get;
            set;
        }
        public Token(int x, int y) : base(x, y)
        {
        }


        public static bool IsValidMove(Tile[,] map, (int x, int y) direction, (int x, int y) initialPos)
        {
            (int x, int y) newPos = (initialPos.x + direction.x, initialPos.y + direction.y);
            if (newPos.x >= 0 && newPos.y >= 0 && newPos.y < map.GetLength(1) && newPos.x < map.GetLength(0) && map[newPos.x, newPos.y].GetType() == typeof(Empty))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}