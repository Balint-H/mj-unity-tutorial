using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Mujoco;
using System.Linq;

public class HingeObservations : SensorComponent
{
    // SensorComponents don't provide the readings themselves, instead they provide the Agent with a collection of ISensors
    // A SensorComponent can group multiple separate sensors, but in this case we only need one.
    public override ISensor[] CreateSensors()
    {
        hingeSensor = new HingeSensor(slide);
        return new ISensor[] { hingeSensor };
    }

    // Instances of our custom ISensors
    private HingeSensor hingeSensor;
    public List<float> Observations => hingeSensor.Observations; // Exposed in case needed for visualisation

    [SerializeField]
    MjHingeJoint slide;


    private class HingeSensor : ISensor
    {

        MjHingeJoint hinge;

        float[] Velocities { get => new[] { hinge.Velocity }; } // Note that velocity is already in radians in the Plugin

        float[] ContinuousHingePositions // We separate the angle into 2 components so it is a continuous and doesn't jump from 359 deg to 0 deg.
        {
            get
            {
                float angle = hinge.Configuration * Mathf.Deg2Rad;
                var sin = Mathf.Sin(angle);
                var cos = Mathf.Cos(angle);
                return new[] { sin, cos };
            }
        }

        float[] Positions { get => ContinuousHingePositions; } // We combine the hinge and slider position

        public List<float> Observations { get => Positions.Concat(Velocities).ToList(); } //Combine positions and velocities

        public int Write(ObservationWriter writer)
        {
            writer.AddList(Observations);
            return 3; // Return number of written observations (2 for hinge pos, 1 for hinge vel)
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
            return "HingeSensor";
        }

        ObservationSpec observationSpec;

        public HingeSensor(MjHingeJoint hinge)
        {
            this.hinge = hinge;
            // Need to tell the shape so the networks can be configured correctly
            observationSpec = new ObservationSpec(shape: new InplaceArray<int>(3),
                                                  dimensionProperties: new InplaceArray<DimensionProperty>(DimensionProperty.None));
        }

        public ObservationSpec GetObservationSpec()
        {
            return observationSpec;
        }

        public void Reset() { }

        public void Update() { }


    }

}
