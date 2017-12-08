using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Assets.Scripts.AI
{
    [Serializable]
    public class NeuronalNetwork
    {
        public float AiScore = 0;

        private bool fullMeshGenerated = false;
        private List<InputNeuron> inputNeurons = new List<InputNeuron>();
        public List<InputNeuron> InputNeurons {
            get { return inputNeurons; }
        }
        private List<WorkingNeuron> hiddenNeurons = new List<WorkingNeuron>();
        public List<WorkingNeuron> HiddenNeurons {
            get { return hiddenNeurons; }
        }
        private List<WorkingNeuron> outputNeurons = new List<WorkingNeuron>();
        public List<WorkingNeuron> OutputNeurons {
            get { return outputNeurons; }
        }

        public void AddInputNeuron(InputNeuron neuron)
        {
            inputNeurons.Add(neuron);
        }

        public void AddHiddenNeuron(WorkingNeuron neuron)
        {
            hiddenNeurons.Add(neuron);
        }

        public void AddOutputNeuron(WorkingNeuron neuron)
        {
            outputNeurons.Add(neuron);
        }

        public void GenerateHiddenNeurons(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var wn = new WorkingNeuron();
                wn.SetName("HIDDEN_" + i);
                hiddenNeurons.Add(wn);
            }
        }

        public void GenerateFullMesh()
        {
            foreach (WorkingNeuron hidden in hiddenNeurons)
            {
                foreach (InputNeuron input in inputNeurons)
                {
                    hidden.AddNeuronConnection(input, 1);
                }
            }

            foreach (WorkingNeuron output in outputNeurons)
            {
                foreach (WorkingNeuron hidden in hiddenNeurons)
                {
                    output.AddNeuronConnection(hidden, 1);
                }
            }
            fullMeshGenerated = true;
        }

        public void Invalidate()
        {
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                wn.Invalidate();
            }
            foreach (WorkingNeuron wn in outputNeurons)
            {
                wn.Invalidate();
            }
        }

        public void RandomizeAllWeights()
        {
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                wn.RandomizeWeights();
            }
            foreach (WorkingNeuron wn in outputNeurons)
            {
                wn.Invalidate();
            }
        }

        public WorkingNeuron GetOutputNeuronFromIndex(int i)
        {
            if(outputNeurons.Count <= i)
            {
                throw new IndexOutOfRangeException();
            }

            return outputNeurons.ElementAt(i);
        }

        public NeuronalNetwork CloneFullMesh()
        {
            //if(fullMeshGenerated == false)
            //{
            //    throw new Exception("Cannot clone before full mesh not generated!");
            //}

            NeuronalNetwork clone = new NeuronalNetwork();
            clone.AiScore = this.AiScore;
            foreach(InputNeuron input in inputNeurons)
            {
                clone.AddInputNeuron((InputNeuron)input.NameCopy());
            }
            foreach (WorkingNeuron wn in hiddenNeurons)
            {
                clone.AddHiddenNeuron((WorkingNeuron)wn.NameCopy());
            }
            foreach (WorkingNeuron wn in outputNeurons)
            {
                clone.AddOutputNeuron((WorkingNeuron)wn.NameCopy());
            }

            clone.GenerateFullMesh();
            
            for (int i = 0; i < hiddenNeurons.Count; i++)
            {
                List<Connection> connectionsOriginal = hiddenNeurons[i].GetConnections();
                List<Connection> connectionsClone = clone.hiddenNeurons[i].GetConnections();
                if (connectionsOriginal.Count != connectionsClone.Count)
                {
                    throw new Exception("Not same amount of Neurons!");
                }
                for (int k = 0; k < connectionsOriginal.Count; k++)
                {
                    connectionsClone[k].weight = connectionsOriginal[k].weight;
                }
            }
            for (int i = 0; i < outputNeurons.Count; i++)
            {
                List<Connection> connectionsOriginal = outputNeurons[i].GetConnections();
                List<Connection> connectionsClone = clone.outputNeurons[i].GetConnections();
                if (connectionsOriginal.Count != connectionsClone.Count)
                {
                    throw new Exception("Not same amount of Neurons!");
                }
                for (int k = 0; k < connectionsOriginal.Count; k++)
                {
                    connectionsClone[k].weight = connectionsOriginal[k].weight;
                }
            }

            return clone;
        }

        public String GetNeuronsAsString()
        {
            String output = "";

            foreach(var n in hiddenNeurons)
            {
                output += n.GetName()+": ";
                var connections = n.GetConnections();
                foreach(var c in connections)
                {
                    output += c.weight+";";
                }
                output += "\n";
            }

            return output;
        }

        public void Serialize()
        {
            using (var writer = new System.IO.StreamWriter("NeuronalNetwork.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(NeuronalNetwork));
                serializer.Serialize(writer, this);
                writer.Flush();
            }
        }

        public static NeuronalNetwork Deserialize()
        {
            try
            {
                using (var stream = System.IO.File.OpenRead("NeuronalNetwork.xml"))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(NeuronalNetwork));
                    return serializer.Deserialize(stream) as NeuronalNetwork;
                }
            }
            catch (Exception)
            {
                return new NeuronalNetwork();
            }
        }
    }
}
