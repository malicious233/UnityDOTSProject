# UnityDOTSProject
Trying out the Unity DOTS/ECS package
Uses Unity version 2023.3.13.f1

The aim of this project was to make a simple game that utilizes Unity DOTS to handle many moving objects at the same time.
In data oriented fashion, we're omitting several object oriented strategies. Entities, which would be an object, have components which store data but they don't do anything by themselves.
Instead Systems will find the components, and then very quickly iterate through all the relevant entity data with multithreading.

Normally this can be an issue with object oriented approaches due to how data is scattered all over memory.
Unity's DOTS forces you to use components (which are really just keys for primitive type/blittable struct data which is far more sorted in memory) due to this.

From a development standpoint it also is more modular since it's more about the actual data instead of objects and their implementation, which can get in the way when working in larger projects, since where-
you find the implementation and where you find the data gets muddier. In ECS you reuse Components as one sees fit instead of having to worry about what kind of implementation Monobehaviors may come with, and instead prioritize thinking about what the Component data will do for the Systems that act upon it instead.
