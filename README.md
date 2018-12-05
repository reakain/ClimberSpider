ClimberSpider
======
Unity project to develop grasping and motion planning of a climbing 6-legged robot. Initial work focuses on development of single arm grasp selection and motion, then spread to double arm separate and parallelized actions, before expanding out to the final six leg concept. 

Initial single and double arm build out is part of a term project for a motion planning class.

Unity Script Definitions
------
#### ```ArmPlanner.cs```
The Arm Planner handles the primary RRT implementation, and is fed information about possible goal states from the assigned object's Grasp Region class. The Arm Planner also calls the IK Solver functions to determine if the arm can find a motion to a CFree point. 

#### ```GraspRegion.cs```
In addition to the scripts added to the joints, hand, and arm of the robot, each graspable object has a C# component. This component handles the PCA-based grasp region definitions, and the secondary set of adding potential goal poses to the pose tree defined in the Arm Planner.

#### ```MultiArmPlanner.cs```
The Multi-Arm Planner handles interactions between the two arms. All Arm objects in Unity3D are added as children to a root-level object who's only components are the transform, which each object has, and the Multi-Arm Planner script. When each Arm Planner finds a solution, the solution is fed to the Multi-Arm Planner, which then handles deadlock and inter-arm collision checks.

#### ```ArmController.cs```
When an acceptable multi-arm solution is found, the appropriate arm solutions are then piped from the script to each arm's specific Arm Controller class for implementation. Unity3D's Update method in-built to their scripting components then allows each arm step to implement simultaneously.

#### Arm Directory
This directory contains scripts that attach to child objects on the Arm prefab object.

###### ```RobotJoint.cs, ArmJoint.cs, WristJoint.cs, FingerJoint.cs```

The primary support class is the base Robot Joint class which is inherited by both the Finger Joint and Arm Joint classes. This class handles all joint rotations and angle checks, and is an underpinning of the gradient descent based IK Solver used in this RRT implementation. Information stored in this component class is an individual joint's rotation angle range from the "home" position, and the axis of rotation. This base class is then separated into inherited finger, wrist, and arm joints. This split allows for simplified inverse kinematic solutions, and more focus spent on movements that only impact the actual object grasp.

###### ```IKSolver.cs, Arm.cs, Wrist.cs, Finger.cs```

The inverse kinematics of the robot are handled by a set of classes that inherit from the IK Solver class. The IK Solver class is a gradient descent based inverse kinematics solver. It solves the inverse kinematics to move the last joint position in a joint chain to a specified point. This solver uses the location of the object it is assigned to as the static root location. This solver class is the inherited class for the Finger, Wrist, and Arm classes, which solve the inverse kinematics for each associated joint chain.

#### Infrastructure Directory
Contains utility class definitions and functions for building node trees, hand configuration poses, and movement solutions.

Unity Prefabs
------
* Arm
* Hand
* Gripper

Third-Party Packages
------
#### ProCore
Package for Mesh building and rendering

#### RockVR
Package for recording. Used to make clips to shaaaare

#### Accord
Contains statistical modeling tools, in this case specifically PCA, which is attempted to use for grasp region planning

Improvement/Correction To-Do List
------
* Item numero uno
* Item numero dos
* Item numero tres