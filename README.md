# MuJoCo + Unity ML-Agents Tutorials

![A human model clinging to a pendulum in motion](./images/cart_pole_human.png?raw=true "Optional Title")

A set of introductory, interactive tutorials for using the [MuJoCo](https://mujoco.org/) physics engine inside Unity, and learning control policies with the [ML-Agents](https://github.com/Unity-Technologies/ml-agents) framework. Unity's [Tutorial Authoring Tools](https://docs.unity3d.com/Packages/com.unity.learn.iet-framework.authoring@1.0/manual/index.html) were used to embed the steps and explanations into the Editor itself.



This project is still under development, so we welcome contributions in both editing/fixing existing tutorials or adding new lessons. 

### Current content:
- Importing and editing MuJoCo scenes to unity
- Setting up Reinforcement Learning with ML-Agents to control a cart-pole environment
- Guidelines for extending a scene with the example of a double-pendulum cart-pole

### Planned content:
- Learning a locomotion policy with ML-Agents
- Quick introduction to editing scenes in Unity and the available tools to speed it up
- Quick introduction to MuJoCo components and data structure


### Before you start:
This project has only been tested on Windows so far. Currently the repository includes the Windows binary for MuJoCo version 2.2.0 . The project already references a fork of the Unity plugin, so the MuJoCo C# scripts does not need to be downloaded. However, the binary may need to be replaced for your own system. Follow the guidance from the [documentation](https://mujoco.readthedocs.io/en/latest/unity.html#).

