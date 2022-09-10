# PT-Clone
![alt text](https://publiczeus.com/devlin/PTIntro.gif "Gameplay")

A Unity Project inspired by the playable teaser for SIlent Hills.

This is a looped experience which combines atmosphere and a Sadistic AI that constantly tries to spook you until inevaitably; it kills you.

## The Story
TBD

## Core Components
These are key parts of the game containing both design and functionally.

### Sadistic AI
The Sadistic AI is a class that controls the experience of the player responding to the players movements and postion, adjusting the experience accordingly. The AI isn't really AI, its a complex random event manager.
#### Algorithm
The AI makes desicions based on a random bumber (1-100), a cruelty integer (0 - 100), a danger integer (0 - 100), the total loops tha player has made and the thinking timer. Using these if the the random number generated is less than the (cruelty divided by total loops) then when the thinking timer completes the AI will do something based on which trigger the player is in.
#### Triggers
Throughout the path there are triggers that when entered the AI notices and when it desides to act, it will act within those areas of the map.
#### Interactions
Objects within the world must be interacted with by the AI, these interactables allow the AI to communicate to those objects.
#### Scares
These are the functions used to scare the player, usually corrisponding to certain areas of the map.
#### Dangers
These are functions desinged to harm the player, ending the experience.

### Path Manager
This is the class responsible for manageing the path the player takes, this includes spawning/despawing "path" prefabs based on the player entering/leaving triggers installed on those prefabs.
####Triggers
The Path Manager uses triggers to detect when the player enters and exits the paths, later this will be replaces with interaction. It spawns and despawns the paths based on these triggers.
### Player Controller
The player controller is designed to manage the players interaction with the game, this includes managing the movement and camera along with the flashlight movement. Player rotation is handled by a lerp function to smoothly and with delay move the players veiw of the world.
#### Debug Camera (F1)
Pressing F1 allows for the Debug Free Camera which has the abbility to fly around the level, this disables the player collider allowing for "noclip" and preventing triggers firing. You can use "q" & "e" to go up and down and Left Shift & Left Control to increase/decrease your speed.
#### Toggle Cruely (F2)
Toggles the Sadistic AI's cruelty from 10 to 100.
#### Debug info (F3)
Toggle the debug info to the on screen UI.
