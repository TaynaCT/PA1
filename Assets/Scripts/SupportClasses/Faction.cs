using Assets.Scripts.SupportClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.SupportClasses
{
    public class Faction
    {
        private string name;

        public string Name { get { return name; } }

        public static Faction Player0 { get { return new Faction() { name = "Player0" }; } }
        public static Faction Player1 { get { return new Faction() { name = "Player1" }; } }
        public static Faction World0Enemy { get { return new Faction() { name = "World0Enemy" }; } }

        // Remove ability to generate a new Faction that is not one of the existing ones.
        private Faction() { }

        public FactionRelationship GetRelationship(Faction f)
        {
            return GetRelationship(this, f);
        }

        public static FactionRelationship GetRelationship(Faction a, Faction b)
        {
            if (a == b)
            {
                return FactionRelationship.Friendly;
            }

            switch (a.Name)
            {
                case "Player0":
                    switch (b.Name)
                    {
                        case "World0Enemy":
                            return FactionRelationship.Hostile;
                        case "Player1":
                            return FactionRelationship.Hostile;
                        default:
                            return FactionRelationship.Neutral;
                    }
                case "Player1":
                    switch (b.Name)
                    {
                        case "Player0":
                            return FactionRelationship.Hostile;
                        default:
                            return FactionRelationship.Neutral;
                    }
                default:
                    return GetRelationship(b, a);
            }
        }

        public override bool Equals(object obj)
        {
            var faction = obj as Faction;
            return faction != null &&
                    name == faction.name;
        }

        public override int GetHashCode()
        {
            var hashCode = 629881564;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(name);
            return hashCode;
        }

        public override string ToString()
        {
            return "[Faction " + name + "]";
        }

        public static bool operator ==(Faction left, Faction right) { return left.Name == right.Name; }

        public static bool operator !=(Faction left, Faction right) { return !(left == right); }
    }
}