using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace forgotten.Desktop
{
    public class System
    {
        public Vector2 Position { get; set; }
        public String Name { get; set; }
        public Color Color { get; set; }
        public float Difficulty;

        public List<Planet> Planets = new List<Planet>();
        public List<System> Neighbors = new List<System>();

        public static List<System> Systems = new List<System>();

        public System(Vector2 position)
        {
            this.Position = position;

            GameRandom rnd = GameRandom.Instance;

            float rads = (float)(Math.Atan2(position.Y, position.X) + Math.PI);

            rads += rnd.NextFloat() * 0.5f;
            rads -= rnd.NextFloat() * 0.5f;

            if (rads > Math.PI*1.5f && rads < Math.PI * 2) {
                Color = new Color(255, 50, 50);
            } else if (rads > Math.PI && rads < Math.PI * 2) {
                Color = new Color(255, 255, 50);
            } else if (rads > Math.PI* 0.5f && rads < Math.PI * 2) {
                Color = new Color(50, 255, 255);
            } else { // A
                Color = new Color(255, 50, 255);
            }

            float planetRnd = (float)rnd.NextDouble();
            if (planetRnd < 0.1)
            {
                AddPlanets(2);
            } 
            else if (planetRnd < 0.3)
            {
                AddPlanets(3);
            } 
            else if (planetRnd < 0.6)
            {
                AddPlanets(4);
            }
            else if (planetRnd < 0.8)
            {
                AddPlanets(5 + (int)(3*rnd.NextDouble()));
            }
        }


        public void SetHomeSystem()
        {
            Name = "Terragon";
            Planets.RemoveRange(0, Planets.Count);
            Planets.Add(new Planet("Mercury"));
            Planets.Add(new Planet("Terra"));

        }

        private void AddPlanets(int numPlanets)
        {
            for (int i = 0; i < numPlanets; i++)
            {
                String name = GeneratePlanetName();
                Planet planet = new Planet(name);
                Planets.Add(planet);
            }
        }

        private String GeneratePlanetName()
        {
            var rnd = GameRandom.Instance;

            String[] prefixes =
            {
                "Ab",
                "Ghrub",
                "Endo",
                "Tres",
                "Urs"
            };

            String[] mids =
            {
                "ghar",
                "mit",
                "rab",
                "xel"
            };

            String[] suffixes =
            {
                "nion",
                "erion",
                "vi"
            };

            String name = rnd.Choose(prefixes);
            if (rnd.NextFloat() < 0.2)
            {
                name += rnd.Choose(mids);
            }
            name += rnd.Choose(suffixes);

            return name;
        }
    }
}
