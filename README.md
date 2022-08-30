# Thesis: Navigation of Self-Driving Vehicle in Controlled Environment, made with Unity

This project was my thesis, made for my computer science studies.

## Abstract

The subject of this thesis is the development of a driving simulator for an autonomous vehicle, in a controlled environment, using the Unity game engine and machine learning algorithms for training. This problem concerns the realistic creation of the driving environment, where the path will be rendered at runtime, with increasing difficulty, as the agent progresses further. The agent gets trained, so it stays on top of the road, without getting out of the boundaries, driving at the highest speed possible, for as long as it can. More specifically, 3 types of ways were used, one short straight line and two long turns, one for left and one for right, at 90 degrees, with the proper objects on them. One more important aspect, that it had to be taken into consideration, was the right tuning of the car settings, so the movement becomes realistic and with a lot of detail. This way, as the agent drives itself, it learned to successfully solve the scenario, with the results being good enough, establishing the smooth operation of the algorithm that was used.

![Demo-Photo-Autodriving](https://user-images.githubusercontent.com/32633615/187473556-1ba48de6-47a2-41c8-a22d-385e7a5fae68.jpg)

## CarAgentScript.cs

The main training script. It combines and uses every other script for the training process. It uses the neural network that has been produced after the training, to auto-drive the vehicle.

In this script, agent-car collects its environmental observations (speed and car's direction) with `CollectObservations(VectorSensor sensor)`. It also randomly selects an action (UpKey-acceleration, DownKey-brake, LeftKey-Left steer, RightKey-Right steer), which will reward it, according to the policy that has been given to it, using `OnActionReceived(float[] vectorAction)`. In the same function, the roads is rendered dynamicly, creating a new piece at the end of the path, while deleting the first road piece, as the agents progresses further. The function `Heuristic(float[] actionsOut)` is used for testing the car's movements with the keyboard, in combination with OnActionReceived. Finally, the function `OnEpisodeBegin()` is used at the beginning of each episode, to reset every previous value before the episode ends and places the car at the initial position.

## CarController.cs

This script is responsible for the car's realistic movement.

## RoadSpawner.cs



## RoadSpawnerManager.cs



## DrivingNeuralNetwork.nn and Training Process