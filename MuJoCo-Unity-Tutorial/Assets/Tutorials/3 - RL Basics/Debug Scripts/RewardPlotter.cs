using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cartpole
{

    public class RewardPlotter : MonoBehaviour
    {
        [SerializeField]
        CartpoleReward rewardProvider;

        //[DebugGUIGraph] Uncomment if DebugGUIGraph is available
        float reward;
        private void FixedUpdate()
        {
            reward = rewardProvider.Reward;
        }
    }
}