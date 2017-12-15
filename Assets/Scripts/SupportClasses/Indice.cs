using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.SupportClasses
{
    public class Indice : IEquatable<Indice>
    {
        private int _indiceX;
        private int _indiceY;

        public Indice()
        {            
        }

        public Indice(int x, int y)
        {
            _indiceX = x;
            _indiceY = y;
        }

        public int X
        {
            get { return _indiceX; }
        }
        public int Y
        {
            get { return _indiceY; }
        }

        public void SetIndice(int x, int y)
        {
            _indiceX = x;
            _indiceY = y;
        }

        public bool Equals(Indice other)
        {
            return _indiceX == other.X && _indiceY == other.Y;
        }
    }
}
