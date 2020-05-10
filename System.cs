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

        public List<Planet> planets = new List<Planet>();

        public static List<System> systems = new List<System>();

        public System(Vector2 position)
        {
            this.Position = position;

            Random rnd = new Random();

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
                addPlanets(2);
            } 
            else if (planetRnd < 0.3)
            {
                addPlanets(3);
            } 
            else if (planetRnd < 0.6)
            {
                addPlanets(4);
            }
            else if (planetRnd < 0.8)
            {
                addPlanets(5 + (int)(3*rnd.NextDouble()));
            }
        }

        private void addPlanets(int numPlanets)
        {
            for (int i = 0; i < numPlanets; i++)
            {
                String name = "Planet " + (i + 1);
                Planet planet = new Planet(name);
                planets.Add(planet);
            }
        }

    }
}
