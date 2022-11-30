using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Mujoco;
using System.Linq;

namespace Cartpole
{
    public class SlideObservations : SensorComponent
    {
        // SensorComponents don't provide the readings themselves, instead they provide the Agent with a collection of ISensors
        // A SensorComponent can group multiple separate sensors, but in this case we only need one.
        public override ISensor[] CreateSensors()
        {
          slideSensor = new SlideSensor(slide);
          return new ISensor[] { slideSensor }; 
        }

        // Instances of our custom ISensors
        private SlideSensor slideSensor; 
        public List<float> Observations => slideSensor.Observations; // Exposed in case needed for visualisation

        [SerializeField]
        MjSlideJoint slide;

        private class SlideSensor : ISensor
        {

            MjSlideJoint slide;

            float[] Velocities { get => new[] { slide.Velocity}; } // Note that velocity is already in radians in the Plugin

            float[] Positions { get => new[] { slide.Configuration}; } // We Configuration is scaled to degrees, even for slide joints...

            public List<float> Observations { get => Positions.Concat(Velocities).ToList(); } //Combine positions and velocities
            
            public int Write(ObservationWriter writer)
            {
                writer.AddList(Observations);
                return 2; // Return number of written observations (1 for slider pos, 1 for slider vel)
            }
            
            // Only needed for visual observations (e.g. camera feed)
            public byte[] GetCompressedObservation()
            {
                throw new NotImplementedException();
            }

            public CompressionSpec GetCompressionSpec()
            {
                return CompressionSpec.Default();
            }

            public string GetName()
            {
                return "SlideSensor";
            }

            ObservationSpec observationSpec;

            public SlideSensor(MjSlideJoint slide)
            {
                this.slide = slide;
                // Need to tell the shape so the networks can be configured correctly
                observationSpec = new ObservationSpec(shape: new InplaceArray<int>(2),
                                                      dimensionProperties: new InplaceArray<DimensionProperty>(DimensionProperty.None));
            }

            public ObservationSpec GetObservationSpec()
            {
                return observationSpec;
            }

            public void Reset(){}

            public void Update(){}

            
        }
    }
}