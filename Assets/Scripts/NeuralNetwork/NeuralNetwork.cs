using System.Collections.Generic;
using System.Linq;
using System;

namespace NN{
    [System.Serializable]
    public class NeuralNetwork{
        public List<Neuron> inputLayer = new List<Neuron>();
        public List<List<Neuron>> hiddenLayers = new List<List<Neuron>>();
        public List<Neuron> outputLayer = new List<Neuron>();

        double learnRate;
        double momentum;

        public NeuralNetwork(){
        }

        public NeuralNetwork(int inputs, int[] hidden, int output, double learnRate, double momentum){
            this.learnRate = learnRate;
            this.momentum = momentum;

            CreateLayers(inputs, hidden, output);
            ConnectAllLayers();
        }

        public List<double> Train(DataSet dataSet, List<double> errors) {
            ForwardPropagate(dataSet.Values);
            BackPropagate(dataSet.Targets);
            errors.Add(CalculateError(dataSet.Targets));

            List<double> output = outputLayer.Select(a => a.Value).ToList();
            string s = "output: ";
            foreach (double d in output) {
                s += string.Format("{0:0.00000}", d) + " ";
            }
            s += " (" + output.IndexOf(output.Max()) + ")";
            s += "\ntargets: ";
            foreach (double t in dataSet.Targets) {
                s += string.Format("{0:0.00000}", t) + " ";
            }
            s += " (" + dataSet.Targets.ToList().IndexOf(dataSet.Targets.Max()) + ")";
            s += " (error: " + errors.Average() + ")\n";
            System.Console.WriteLine(s);

            return errors;
        }

		public double[] Compute(double[] inputs) {
			ForwardPropagate(inputs);

            double[] output = new double[outputLayer.Count];
            int count = 0;
            foreach(Neuron neuron in outputLayer){
                output[count] = neuron.Value;
                count++;
            }

			return output;
		}

        void ForwardPropagate(double[] inputs){
            var i = 0;

            foreach(Neuron neuron in inputLayer) {
				neuron.Value = inputs[i];
				i++;
            }

            foreach(List<Neuron> layer in hiddenLayers) {
                foreach(Neuron neuron in layer) {
					neuron.CalculateValue();
                }
            }

            foreach(Neuron neuron in outputLayer) {
				neuron.CalculateValue();
            }
        }

        void BackPropagate(params double[] targets)
		{
			var i = 0;

			foreach (Neuron neuron in outputLayer) {
                neuron.CalculateError(targets[i]);
				neuron.CalculateGradient();
				i++;
			}

			hiddenLayers.Reverse();
			foreach (List<Neuron> layer in hiddenLayers) {
				foreach (Neuron neuron in layer) {
                    neuron.CalculateError();
					neuron.CalculateGradient();
				}
			}
			hiddenLayers.Reverse();

			foreach (List<Neuron> layer in hiddenLayers) {
				foreach (Neuron neuron in layer) {
					neuron.UpdateWeights(learnRate, momentum);
                    neuron.UpdateBias(learnRate, momentum);
				}
			}

			foreach (Neuron neuron in outputLayer) {
				neuron.UpdateWeights(learnRate, momentum);
                    neuron.UpdateBias(learnRate, momentum);
			}
		}
        
        double CalculateError(params double[] targets) {
			int i = 0;
			return outputLayer.Sum(a => Math.Abs(a.CalculateError(targets[i++])));
		}

        void CreateLayers(int inputs, int[] hidden, int output){
            for(int i = 0; i < inputs; i++){
                inputLayer.Add(new Neuron());
            }

            foreach (int hiddenLayerSize in hidden){
                List<Neuron> hiddenLayer = new List<Neuron>();
                for(int i = 0; i < hiddenLayerSize; i++){
                    hiddenLayer.Add(new Neuron());
                }
                hiddenLayers.Add(hiddenLayer);
            }

            for(int i = 0; i < output; i++){
                outputLayer.Add(new Neuron());
            }
        }

        void ConnectAllLayers(){
            if(hiddenLayers.Count == 0){
                return;
            }

            List<Neuron> firstHiddenLayer = hiddenLayers[0];
            List<Neuron> lastHiddenLayer = hiddenLayers[hiddenLayers.Count -1];

            ConnectLayers(inputLayer, firstHiddenLayer);

            for(int i = 0; i < hiddenLayers.Count-1; i++){
                ConnectLayers(hiddenLayers[i], hiddenLayers[i+1]);
            } 

            ConnectLayers(lastHiddenLayer, outputLayer);
        }

        void ConnectLayers(List<Neuron> fromLayer, List<Neuron> toLayer){
            foreach(Neuron input in fromLayer){
                foreach(Neuron output in toLayer){
                    Synapse synapse = new Synapse(input, output);
                    input.OutputSynapses.Add(synapse);
                    output.InputSynapses.Add(synapse);
                }
            }  
        }

        public override string ToString(){
            string s = "";
            s += "-- Input Layer ----------\n";
            foreach(Neuron inputNeuron in inputLayer){
                s += inputNeuron.Value + " ----> ";
                foreach(Synapse inputNeuronSynapse in inputNeuron.OutputSynapses){
                    s += inputNeuronSynapse.Weight + " | ";
                }
            }
            s += "\n";

            int hiddenLayerCount = 1;
            foreach(List<Neuron> hiddenLayer in hiddenLayers){
                s += "-- Hidden Layer " + hiddenLayerCount + " ----------\n";
                foreach(Neuron hiddenNeuron in hiddenLayer){
                    s += hiddenNeuron.Value + " ----> ";
                    foreach(Synapse hiddenNeuronSynapse in hiddenNeuron.OutputSynapses){
                        s += hiddenNeuronSynapse.Weight + " | ";
                    }
                    s += "\n";
                }
                hiddenLayerCount++;
            }
            s += "\n";

            // s += "-- Output Layer ----------\n";
            // foreach(Neuron outputNeuron in outputLayer){
            //     s += outputNeuron.Value + ": ";
            //     foreach(Synapse outputNeuronSynapse in outputNeuron.InputSynapses){
            //         s += outputNeuronSynapse.Weight + " | ";
            //     }
            // }

            return s;
        }
    }
}