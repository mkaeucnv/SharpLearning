﻿using System;
using System.Linq;
using System.Collections.Generic;
using SharpLearning.Containers.Tensors;
using SharpLearning.Neural.Initializations;

namespace SharpLearning.Neural.LayersNew
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class NeuralNet2
    {
        /// <summary>
        /// The layers in the network
        /// </summary>
        public readonly List<ILayerNew> Layers;

        readonly NeuralNetStorage Storage;

        readonly Initialization m_initialization;

        /// <summary>
        /// 
        /// </summary>
        public NeuralNet2(Initialization initialization = Initialization.GlorotUniform)
        {
            m_initialization = initialization;
            Layers = new List<ILayerNew>();
            Storage = new NeuralNetStorage();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        public void Add(ILayerNew layer)
        {
            Layers.Add(layer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observations"></param>
        /// <param name="targets"></param>
        public void SetNextBatch(Tensor<double> observations, Tensor<double> targets)
        {
            // inputs are assinged to the first layer.
            var input = Layers.First().Input;
            Storage.AssignTensor(input, observations.Data);

            // targets are stored as the gradients of the final layer.
            var output = Layers.Last().Output;
            Storage.AssignGradient(output, targets.Data);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Tensor<double> BatchPredictions()
        {
            var prediction = Storage.GetTensor(Layers.Last().Output);
            return prediction;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="observations"></param>
        /// <returns></returns>
        public Tensor<double> Predict(Tensor<double> observations)
        {
            var input = Layers.First().Input;

            if (input.Shape != observations.Shape)
            {
                var newInput = Variable.Create(observations.Dimensions.ToArray());
                UpdateDimensions(newInput);

                // set observation
                Storage.AssignTensor(newInput, observations.Data);
            }
            else
            {
                // set observation
                Storage.AssignTensor(input, observations.Data);
            }

            // pedict
            Forward(training: false);

            // return prediction
            var output = Layers.Last().Output;
            return Storage.GetTensor(output);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Forward()
        {
            Forward(true);
        }

        void Forward(bool training)
        {
            Layers.ForEach(l => l.Forward(Storage, training));
        }

        /// <summary>
        /// 
        /// </summary>
        public void Backward()
        {
            for (int i = Layers.Count; i-- > 0;)
            {
                Layers[i].Backward(Storage);
            }
        }

        /// <summary>
        /// Initializes the layers in the neural net (Instantiates members and creates random initialization of weights). 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="random"></param>
        public void Initialize(Variable input, Random random)
        {
            Layers.First().Initialize(input, Storage, random, m_initialization);

            for (int i = 1; i < Layers.Count; i++)
            {
                var previousLayer = Layers[i - 1];
                Layers[i].Initialize(previousLayer.Output, Storage, random, m_initialization);
            }           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameters"></param>
        public void GetTrainableParameters(List<Data<double>> parameters)
        {
            Storage.GetTrainableParameters(parameters);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        public void UpdateDimensions(Variable input)
        {
            Storage.ClearNonPreservables();

            Layers.First().UpdateDimensions(input);

            for (int i = 1; i < Layers.Count; i++)
            {
                var previousLayer = Layers[i - 1];
                Layers[i].UpdateDimensions(previousLayer.Output);
            }
        }
    }
}
