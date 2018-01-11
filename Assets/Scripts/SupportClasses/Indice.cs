using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.SupportClasses
{
    public class Indice
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Indice(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void SetIndice(int x, int y)
        {
            X = x;
            Y = y;
        }

        public bool Equals(Indice other)
        {
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object other)
        {
            return (other is Indice) ? Equals((Indice)other) : false;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }

        public static bool operator ==(Indice left, Indice right) { return left.X == right.X && left.Y == right.Y; }

        public static bool operator !=(Indice left, Indice right) { return !(left == right); }

        public override string ToString()
        {
            return string.Format("{{X:{0} Y:{1}}}", X, Y);
        }
    }
}
