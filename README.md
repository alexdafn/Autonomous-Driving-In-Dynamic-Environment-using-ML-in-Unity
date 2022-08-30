# Thesis: Navigation of Self-Driving Vehicle in Controlled Environment, made with Unity

This project was my thesis, made for my computer science studies.

## Abstract

The subject of this thesis is the development of a driving simulator for an autonomous vehicle, in a controlled environment, using the Unity game engine and machine learning algorithms for training. This problem concerns the realistic creation of the driving environment, where the path will be rendered at runtime, with increasing difficulty, as the agent progresses further. The agent gets trained, so it stays on top of the road, without getting out of the boundaries, driving at the highest speed possible, for as long as it can. More specifically, 3 types of ways were used, one short straight line and two long turns, one for left and one for right, at 90 degrees, with the proper objects on them. One more important aspect, that it had to be taken into consideration, was the right tuning of the car settings, so the movement becomes realistic and with a lot of detail. This way, as the agent drives itself, it learned to successfully solve the scenario, with the results being good enough, establishing the smooth operation of the algorithm that was used.

![Demo-Photo-Autodriving](https://user-images.githubusercontent.com/32633615/187473556-1ba48de6-47a2-41c8-a22d-385e7a5fae68.jpg)

## CarAgentScript.cs

## CarController.cs

## RoadSpawner.cs

## RoadSpawnerManager.cs

## DrivingNeuralNetwork.nn and Training Process