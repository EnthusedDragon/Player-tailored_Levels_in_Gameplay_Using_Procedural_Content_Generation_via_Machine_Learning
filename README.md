# player-tailored_levels_PCGML_Prototype
This prototype was created as part of the requirements for Project 4 in the B Tech (I.T.) programme at CPUT.

Install the following software:
Unity 3D 2019.2
Anaconda 3
Jupyter Notebook
MS Office Excel

Follow the Unity ML-Agents Toolkit setup here.
Clone this repository to any location on your computer.
Copy the contents of the (1. ml-agents config) folder into your (ml-agents/config) directory which was created during the Unity ML-Agents setup.
Copy the rest of the folders and files into the (ml-agents/UnitySDK/Assets) folder and overwrite all if prompted.

Open the UnitySDK project in Unity3D and open the scene you want:

- Demo Scenes
-- 0. DemoMazePlayer
-- 0. DemoMazeGenerator

- Learning Scenes (Anaconda3 commands)
-- 1. LearningMazePlayer
-- 1. LearningMazeGenerator

- Anaconda3 Training Commands
-- Weak Maze Player: mlagents-learn config/trainer_config.yaml --curriculum=config/curricula/mazePlayerWeak/ --run-id=mazePlayerWeak --train
-- Average Maze Player: mlagents-learn config/trainer_config.yaml --curriculum=config/curricula/mazePlayerAverage/ --run-id=mazePlayerAverage --train
-- Strong Maze Player: mlagents-learn config/trainer_config.yaml --curriculum=config/curricula/mazePlayerStrong/ --run-id=mazePlayerStrong --train
-- Maze Generator: mlagents-learn config/trainer_config.yaml --run-id=mazeGenerator --train