using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.AI
{
    [Serializable]
    public abstract class Neuron
    {

        public String name { get; private set; }

        public Neuron()
        {
            name = "NO NAME";
        }


        public abstract float GetValue();
        public abstract Neuron NameCopy();

        public static float Sigmoid(float x)
        {
            float et = (float)Math.Pow(Math.E, x);
            return (et / (1 + et)) * 2 - 1;
        }

        public string GetName()
        {
            return name;
        }

        public void SetName(string name)
        {
            this.name = name;
        }
    }

}
