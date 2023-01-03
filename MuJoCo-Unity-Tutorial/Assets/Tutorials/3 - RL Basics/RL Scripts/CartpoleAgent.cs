using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Mujoco;

namespace Cartpole
{
    public class CartpoleAgent : Agent
    {
        // We can assign references to these joints from the Editor
        [SerializeField] 
        MjHingeJoint hinge;

        [SerializeField]
        MjSlideJoint slide;

        // Start is called before the first frame update
        void Start()
        {

        }

        public override void Initialize()
        {
            base.Initialize();
        }

        // Since we are accessing memory shared with the MuJoCo simulation we have to do it in an "unsafe" context (You may need to enable this).
        public unsafe override void OnEpisodeBegin()
        {
            base.OnEpisodeBegin();
            // In case this is the first frame and the MuJoCo simulation didn't start yet, 
            // we know we will start in the correct state so we can skip it.
            if (!(MjScene.InstanceExists && MjScene.Instance.Data!=null)) return;
            
            // Get the reference to the bindings of the mjData structure https://mujoco.readthedocs.io/en/latest/APIreference.html#mjdata
            var data = MjScene.Instance.Data;
            
            // Reset kinematics to 0
            data->qpos[hinge.QposAddress] = 0;
            data->qpos[slide.QposAddress] = 0;

            data->qvel[hinge.DofAddress] = 0;
            data->qvel[slide.DofAddress] = 0;
            
            data->qacc[hinge.DofAddress] = 0;
            data->qacc[slide.DofAddress] = 0;
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
            base.CollectObservations(sensor);
            // If you wanted to collect observations from the Agent class, you can add them one by one to the sensor
            // Note that if you do this, and not via separate SensorComponents, you will have to update the BehaviourParameter's observation size
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
          // You could process your agent's actions directly here, and assign reward as well
        }
    }
}
