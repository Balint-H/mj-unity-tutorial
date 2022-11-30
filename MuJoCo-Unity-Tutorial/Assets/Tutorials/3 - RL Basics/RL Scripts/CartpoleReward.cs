using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using System;
using System.Linq;
using Mujoco;

namespace Cartpole
{
    // A base class we inherit from. You can use this as well in your own rewards.
    public abstract class RewardProvider: MonoBehaviour
    {
        [SerializeField]
        Agent agent;

        // Note that there is no guarantee by default on the execution order of the FixedUpdate() of different components. 
        // This also affects the Agent; we don't know if the MuJoCo physics step happens first or the agent one, or the reward.
        // Additional information on guaraneeing synchronisation will be discussed in a later tutorial.
        private void FixedUpdate()
        {
            agent.AddReward(Reward);
        }

        public abstract float Reward { get; }
    }

    /// <summary>
    /// A C# adaptation of the non-sparse cartpole reward signal in the DM control suite.
    /// </summary>
    public class CartpoleReward : RewardProvider
    {
        

        [SerializeField]
        Transform pole;

        [SerializeField]
        MjActuator slideActuator;

        [SerializeField]
        MjHingeJoint hinge;

        public override float Reward { get => GetStateReward()*Regularization(); }

        // Our desired state is pointing upwards, with no velocity
        float GetStateReward()
        {
            var upright = (Mathf.Cos(pole.localEulerAngles.z * Mathf.Deg2Rad) + 1) / 2;
            var centered = Tolerance(pole.position.x, bounds:(0f, 0f), margin: 2f);
            centered = (1 + centered) / 2;
            var smallVelocity = Tolerance(hinge.Velocity, bounds: (0f, 0f), margin: 5);
            smallVelocity = (1 + smallVelocity) / 2;
            return upright * centered * smallVelocity; // Multiply, so we need to satisfy all three at the same time
        }

        // We want to use as little force as possible to get there.
        float Regularization()
        {
            float control = slideActuator.Control;
            var smallControl = Tolerance(control, bounds:(0f, 0f), margin: 1, valueAtMargin: 0, sigmoid: "quadratic");
            smallControl = (4 + smallControl) / 5;
            return smallControl;
        }

        // Non-linearity to process the actions that penalize being outside the given bounds
        private static float Tolerance(float x, (float, float) bounds, float margin=0, 
                                       string sigmoid="gaussian", float valueAtMargin=0.1f)
        {
            (float lower, float upper) = bounds;

            if(lower > upper)
            {
                throw new ArgumentException("Lower bound must be <= upper bound.");
            }

            if(margin < 0)
            {
                throw new ArgumentException("'margin' must be non-negative");
            }

            bool inBounds = (lower <= x) && (x <= upper);

            float value;
            if (margin == 0)
            {
                value = inBounds? 1 : 0;
            }
            else
            {
                float d = (x < lower ? lower - x : x - upper) / margin;
                value = inBounds ? 1 : Sigmoids(d, valueAtMargin, sigmoid);
            }

            return value;
        }

        public static float Sigmoids(float x, float valueAt1, string sigmoid)
        {
            if (new[] { "cosine", "linear", "quadratic" }.Contains(sigmoid))
            {
                if (!(0f <= valueAt1 && valueAt1 < 1f))
                {
                    throw new ArgumentException($"`value_at_1` must be nonnegative and smaller than 1, got{valueAt1}");
                }
                }
            else if(!(0<valueAt1 && valueAt1<1))
            {
                throw new ArgumentException($"`value_at_1` must be strictly between 0 and 1, got {valueAt1}");
            }

            switch(sigmoid)
            {
                case "gaussian":
                    var scale = Mathf.Sqrt(-2 * Mathf.Log(valueAt1));
                    return Mathf.Exp(-0.5f * x * x * scale * scale);

                case "quadratic":
                    scale = Mathf.Sqrt(1 - valueAt1);
                    var scaledX = x * scale;
                    return Mathf.Abs(scaledX) < 1 ? 1 - (scaledX * scaledX) : 0;

                default:
                    throw new NotImplementedException($"Unknown sigmoid type {sigmoid}");
            }

        }

        
    }
}