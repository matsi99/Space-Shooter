using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Scripts.AI
{
    [Serializable]
    public class WorkingNeuron : Neuron
    {
        public WorkingNeuron() :base()
        {
            value = null;
            connections = new List<Connection>();
        }

        public float? value { get; private set; }
        public List<Connection> connections { get; private set; }

        public void AddNeuronConnection(Neuron n, float weight)
        {
            AddNeuronConnection(new Connection(n, weight));
        }

        public void AddNeuronConnection(Connection connection)
        {
            connections.Add(connection);
        }

        public void Invalidate()
        {
            this.value = null;
        }

        public List<Connection> GetConnections()
        {
            return connections;
        }

        private void calculate()
        {
            float value = 0;
            foreach (Connection c in connections)
            {
                value += c.GetValue();
            }
            value = Neuron.Sigmoid(value);
            this.value = value;
        }

        public override float GetValue()
        {
            if (value == null)
            {
                calculate();
            }
            return (float)this.value;
        }

        public override Neuron NameCopy()
        {
            WorkingNeuron clone = new WorkingNeuron();
            clone.SetName(GetName());
            return clone;
        }

        public void RandomizeWeights(float factor = 1.0f)
        {
            foreach (Connection c in connections)
            {
                c.weight = UnityEngine.Random.Range(-factor, factor);
            }
        }

        public float GetStrongestConnection()
        {
            float strongest = 0;
            foreach (Connection c in connections)
            {
                float val = Math.Abs(c.weight);
                if (val > strongest) strongest = val;
            }
            return strongest;
        }
    }
}
