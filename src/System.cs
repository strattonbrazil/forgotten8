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

        public static List<System> Systems = new List<System>();

        public System(Vector2 position)
        {
            this.Position = position;

            GameRandom rnd = GameRandom.Instance;

            float colorSpace = (float)rnd.NextDouble();
            if (colorSpace < .3)
            {
                Color = new Color(150, 250, 250);
            }
            else if (colorSpace < .7)
            {
                Color = new Color(250, 150, 250);
            }
            else
            {
                Color = new Color(250, 250, 150);
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
