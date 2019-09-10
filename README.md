# Prototyping Player-tailored levels in Gameplay using Procedural Content Generation via Machine Learning
This prototype was created as part of the requirements for Project 4 in the B Tech (I.T.) programme at CPUT.

## Install the following software:
- [Unity 3D 2019.2.0f1 - Personal](https://public-cdn.cloud.unity3d.com/hub/nuo/UnityHubSetup.exe?button=onboarding-download-btn-windows)
- [Git for Windows](https://github.com/git-for-windows/git/releases/download/v2.23.0.windows.1/Git-2.23.0-64-bit.exe)
- [Jupyter Notebook](https://jupyter.org/install.html)

## Setup
- Follow the Unity ML-Agents Toolkit setup [here](https://github.com/Unity-Technologies/ml-agents/blob/master/docs/Installation-Windows.md).
  - [Anaconda3 2018.12](https://repo.continuum.io/archive/Anaconda2-2018.12-Windows-x86_64.exe) and Unity ML-Agents V0.9.x was used for this research paper.
  - Do not setup for GPU training.
- Copy the contents of the (1. ml-agents config) folder into your (C:\Users\<username>\ml-agents\ml-agents\config) directory which was created during the Unity ML-Agents setup. This directory might be different if changed during the setup steps.
- Copy the rest of the folders and files into the (C:\Users\<username>\ml-agents\UnitySDK\Assets) folder and overwrite all if prompted.

### Open the UnitySDK project in Unity3D and open the scene you want:

- Demo Scenes
  - DemoMazePlayer
  - DemoMazeGenerator

- Learning Scenes (Anaconda3 commands)
  - LearningMazePlayer
  - LearningMazeGenerator

### Anaconda3 Training Commands
Open Anaconda3 and enter the following commands in order:
- *activate ml-agents*
- *cd C:\Users\<username>\ml-agents*

Now ml-agents should be active in Anaconda3 and you can run one of the following commands to train either the Maze Player or Maze Generator if you have the appropriate scene open in Unity3D.
- **Weak Maze Player:** *mlagents-learn config/trainer_config.yaml --curriculum=config/curricula/mazePlayerWeak/ --run-id=mazePlayerWeak --train*
- **Average Maze Player:** *mlagents-learn config/trainer_config.yaml --curriculum=config/curricula/mazePlayerAverage/ --run-id=mazePlayerAverage --train*
- **Strong Maze Player:** *mlagents-learn config/trainer_config.yaml --curriculum=config/curricula/mazePlayerStrong/ --run-id=mazePlayerStrong --train*
- **Maze Generator:** *mlagents-learn config/trainer_config.yaml --run-id=mazeGenerator --train*