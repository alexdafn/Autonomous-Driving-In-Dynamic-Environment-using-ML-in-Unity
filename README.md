# Thesis: Navigation of Self-Driving Vehicle in Controlled Environment, made with Unity

This project was my thesis, made for my computer science studies. There is more detail in greek, on my official project report and short presentation.

[Thesis - Navigation of Self-Driving Vehicle in Controlled Environment, made with Unity.pdf](https://github.com/alexdafn/Autonomous-Driving-In-Dynamic-Environment-using-ML-in-Unity/files/9457343/Thesis.-.Navigation.of.Self-Driving.Vehicle.in.Controlled.Environment.made.with.Unity.pdf)

## Abstract

The subject of this thesis is the development of a driving simulator for an autonomous vehicle, in a controlled environment, using the Unity game engine and machine learning algorithms for training. This problem concerns the realistic creation of the driving environment, where the path will be rendered at runtime, with increasing difficulty, as the agent progresses further. The agent gets trained, so it stays on top of the road, without getting out of the boundaries, driving at the highest speed possible, for as long as it can. More specifically, 3 types of ways were used, one short straight line and two long turns, one for left and one for right, at 90 degrees, with the proper objects on them. One more important aspect, that it had to be taken into consideration, was the right tuning of the car settings, so the movement becomes realistic and with a lot of detail. This way, as the agent drives itself, it learned to successfully solve the scenario, with the results being good enough, establishing the smooth operation of the algorithm that was used.

![Demo-Photo-Autodriving](https://user-images.githubusercontent.com/32633615/187473556-1ba48de6-47a2-41c8-a22d-385e7a5fae68.jpg)

## CarAgentScript.cs

The main training script. It combines and uses every other script for the training process. It uses the neural network that has been produced after the training, to auto-drive the vehicle.

In this script, agent-car collects its environmental observations (speed and car's direction) with `CollectObservations(VectorSensor sensor)`. The car uses the *RayCasting 3D* component that Unity offers from the editor, to track the objects of its environment, with 2 sets of Rays, one on top of the car (tracks environmental objects) and one on the bottom (tracks the side parts of the road). It also randomly selects an action (UpKey-acceleration, DownKey-brake, LeftKey-Left steer, RightKey-Right steer), which will reward it, according to the policy that has been given to it, using `OnActionReceived(float[] vectorAction)`. In the same function, the road is rendered dynamicly, placing a new piece at the end of the path, while deleting the first road piece, as the agents progresses further. The function `Heuristic(float[] actionsOut)` is used for testing the car's movements with the keyboard, in combination with *OnActionReceived()*. Finally, the function `OnEpisodeBegin()` is used at the beginning of each episode, to reset every value before the episode ends and places the car at the initial position. On every new episode, the training cicle described before, starts again.

## CarController.cs

This script is responsible for the car's realistic movement. The function `MakeCarAction(float actionNumber)`, which is called from *CarAgentScript.cs* and *OnActionReceived()*, gets a random number (a random action from the agent) and makes the car move, according to that selected action. In this script, with the use of the functions `OnTriggerEnter(Collider other)` and `OnTriggerExit(Collider other)`, we can make certain events happen when the agent collides with an object or when it gets out the area of an object. One use of these methods is the detection of a collision with an environmental object (tree, house, lamp), that sends a signal to end the episode. We can also be notified whether or not the car steps on the side part of a road piece.

## RoadSpawner.cs

In this script, there is a list of all the possible road pieces to be placed (start, short straight lane, long left turn, long right turn). The function `SpawnRoadPiece(int roadIndex = 1, int rotationValue=0)` places a given road piece (*roadIndex*) on the right angle (*rotationValue*). It also randomises the existance of two houses and two trees, one on each side of the road. The lamps are excluded from the previous procedure and are always present, on every road piece.

## RoadSpawnerManager.cs

This script is responsible for managing the creation of the dynamic road system. In combination with *RoadSpawner.cs*, the function `SpawnTriggerEntered()` is called from the method *OnTriggerEnter()* from *CarController.cs*, when the car enters an invisible wall-TriggerSpot, at the end of a road piece, so the creation of a new one will occur. Road pieces will be placed (function *SpawnRoadPiece()* is called) at increasing difficulty (after every 30 road pieces being passed), with bigger likelihood of turns to be spawned at max difficulty.

## UIScript.cs

In this script helpful information reaches the game screen, for better understanding of the training process and the driving progression. On top right, it displays the time of the current episode, on bottom left the road pieces that have been passed and on bottom right the current difficulty (5=easiest, 2=hardest). During the training, the current reward of the agent is being displayed as well.

## DrivingNeuralNetwork.nn and Training Process

The training process has been executed according to the Unity ML-Agents toolkit standards at "[ML-Agents Unity](https://github.com/Unity-Technologies/ml-agents/blob/release_2_verified_docs/docs/Getting-Started.md)". In this project, **Soft-Actor-Critic** Machine Learning algorithm has been used (details of the parameters used, are  in "*sac_trainer_config.yaml*" file). With a lot of trial and error and many reconstructions of the policy, the training has succeeded. The produced neural network "*DrivingNeuralNetwork.nn*" is being dragged and dropped to the car object, on Unity's editor and the agent is ready to drive itself autonomously, on every possible road combination that is being rendered every time. 

## DEMO 1: Easy road path

https://user-images.githubusercontent.com/32633615/187568178-bb1e1d4d-f5db-4d8d-9d9c-7b5ac37c91da.mp4

## DEMO 2: Hard road path

https://user-images.githubusercontent.com/32633615/187568607-680bee47-20ca-4994-9727-293e2746cbba.mp4
