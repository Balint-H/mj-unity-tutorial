using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cartpole {

public class RewardPlotter : MonoBehaviour {
  [SerializeField]
  CartpoleReward rewardProvider;

  //[DebugGUIGraph] // Uncomment if Unity package DebugGUIGraph is available (can be downloaded from Asset store)
  float reward;

  private void FixedUpdate() {
    reward = rewardProvider.Reward;
  }
}
}