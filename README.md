# Volatile Hordes
An experimental mod to override the ambient outside Zombie spawning and AI behavior.

Heavily inspired by WalkerSim, which provided an essential template to experiment with and work off of.

https://github.com/ZehMatt/7dtd-WalkerSim

# General Features
- AI Package system that can control 1 or more zombies to wander towards specific directions
- Timing systems (heavy usage of Reactive Extensions) allow for complex timings of logic
- Several types of AI packages, including wandering hordes, small seeker cells of zombies that know where you are, ambient zombies with more crazy patterns
- Better reaction to gunshot noises.  Since all zombies are tracked to control their behaviors, probability systems have been put in place to draw zombies in a larger area than the base game to the source of gunshots.  Attraction is based on probability based on distance, to where you have to shoot a lot in a short span of time to result in zombies far away from finding you, but nearby zombies will head straight for the noise.
- A UI that uses server/client side system for testing and display of zombie location and their planned movements

An example of a horde spawned and heading towards the player:
https://i.imgur.com/pwP9lka.gif

An example of the player shooting a gun and causing zombies to attract
https://i.imgur.com/YU4geJs.gif

# State of Development
The AI package system that controls specific zombies and "hordes" of zombies has had the most work done and is fairly mature and allows for pretty specific control both in the movement of the zombies and the specific timings.

What needs to be ironed out before it's a "playable" mod is the overall director.  The components and horde types to be placed are mostly complete, but there is no overall logic of -when- these events should occur.   Currently in development to create a director that works off grid sections that refill over time and "spend" to create a horde.  So revisiting a recently cleared area will have less zombies, along with other features that work off that system.

# Development Videos
Some videos captured during development
https://www.youtube.com/playlist?list=PLWitGmFXM6rjJnBaj12LgXLsxAQ6xekgS

Mostly of testing specific AI package behaviors.  So I'll be typing in specific commands to trigger a specific horde style, analyzing results. 
