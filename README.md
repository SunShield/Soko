![](images/SokoLogo_Intro.png)
Hi! I'm Herman, a passionate game developer from Ukraine. This is my portfolio game, Soko! Soko is a reimagination of a classic Sokoban game, packed with extra elements for the even greater challenge and more varied player experience.

<details><summary>Some example levels</summary>
<img align="left" width="218" src="images/Lv1.png" /><img align="left" width="250" src="images/Lv2.png" /><img align="left" width="250" src="images/Lv3.png" />
<br></br><br></br><br></br><br></br><br></br>
</details>

### Main Features:
- More than 10 different gameplay elements, colors and groups!
- No progression stucking - play every level you want!
- A Turn-revert feature: don't make a wrong click to end your level completion.
- Tutorial, explaining every new element.

Below, there is a more detailed explanation of every game aspect and system.

![](images/SokoLogo_WhatsInside.png)
While maintaining the classic gameplay loop of Sokoban game, Soko adds a whole new sets of conceptions and elements to diversify player's game experience. Below there's a small breakdown of all the concepts alongside with a comprehensive table of all gameplay elements currently present in game.

![](images/Elements.png)
Currently, Soko features 12 gameplay elements (13 with empty spaces included). Some elements can have colors or be grouped to extend level variety even further. New elements can be easily created with Soko's flexible Component system.
<details><summary>Full table of gameplay elements featured in Soko</summary>

  <table>
    <tr>
      <th>Name</th>
      <th>Icon</th>
      <th>Description</th>
      <th>Name</th>
      <th>Icon</th>
      <th>Description</th>
    </tr>
    <tr>
      <td>Player</td>
      <td><img width="28" src="Assets/Graphics/Sprites/Hero/Hero_New.png"/></td>
      <td>User-controlled game entity.</td>
      <td>Wall</td>
      <td><img width="28" src="Assets/Graphics/Sprites/Level/Wall.png"/></td>
      <td>Object unpassable by anything.</td>
    </tr>
    <tr>
        <td>Box</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/Crate.png"/></td>
        <td>Pushable box. Can activate spots.</td>
        <td>Spot</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/Spot.png"/></td>
        <td>Activate all to complete the level.</td>
      </tr>
      <tr>
        <td>Sliding Box</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/Slippery Crate.png"/></td>
        <td>This box, while pushed, moves in push direction until something stops it.</td>
        <td>Lock Spot</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/Lock Spot.png"/></td>
        <td>While activated, locks a box in self, preventing it from further movement.</td>
      </tr>
      <tr>
        <td>Fence</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/Fence.png"/></td>
        <td>Allows player movement, but blocks boxes.</td>
        <td>Water</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/Water.png"/></td>
        <td>Allows box movement, but blocks player.</td>
      </tr>
      <tr>
        <td>Gate Toggle Button</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/GateToggleButton.png"/></td>
        <td>When entered, toggles all Togglable Gate states (open to close and vice versa).</td>
        <td>Togglable Gate</td>
        <td><img width="28" src="images/TogglableGate.png"/></td>
        <td>Can be open or closed. Whle open, acts like empty spot, if closed, acts like a wall</td>
      </tr>
      <tr>
        <td>Color Push Button</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/ColorPushButton.png"/></td>
        <td>Always colored. When entered, moves all the boxes of corresponding color in button's direction.</td>
        <td>Teleporter</td>
        <td><img width="28" src="Assets/Graphics/Sprites/Level/Teleporter.png"/></td>
        <td>Always colored. When player enters teleporter's spot, it is teleported to another teleporter of the same color.</td>
      </tr>
  </table>
</details>

<img align="left" width="100" src="images/Colors.png" />
Colors both contribute to the aestetics and gameplay. The idea is simple: only elements of the same color interact with each other.
Not every element can be colored. Walls, Fences, Water and Player do not have colors.
For elements like Color Push Button color is mandatory. Other elements (like Boxes) can have or not have a color.
White color is a color wildcard and can be used to substitute any of them.
<br/><br/>
<br/><br/>

<img align="left" width="100" src="images/Groups.png" />
Groups is a concept related to simultaneous movement of objects. Objects united in a same group will move if any of these objects are about to move. Also, movement possibility will be checked for the whole group.	
	Even objects of different type can be a part of the group. Rules for these hybrid sets are something player is meant to discover, however each individal element always behaves consistently, disregarding of being part of the group.
 <br/><br/>
<br/><br/>

![](images/SokoLogo_TechDecisions.png)

# <ins>Movement System</ins>
Movement rules for all the game elements are consistent and centralized in MoveManager, with a minimum amount of hardcode and corner-cases. Movement system is designed to be easily expandable with new elements and is capable of handling even complex cases of movement (like, player is pushing a boxes group and entering a portal).

### <ins>A brief breakdown of movement rules</ins>

Before movement starts, all objects participating in movement are found. The **first participant** is always an object whose movement was caused by user (Player in our case, but another ones could be added in future). Then, the movement tile is checked and if there's a movable object, it is also registered as a participant. Then, both moved object and **first participant** call their ```GetSubsequentObjects``` to find all objects the movement of which will be caused by the movement of the main object (now, only the Group Movement is a case).

For each object, a ```MoveAction``` is generated. If object cannot move due to various reasons (edge/wall reached, grouped object movement is blocked etc), it's movement action's ```IsInterrupted``` is set to true. **If all object's MoveActions have their IsInterrupted set to true, movement is considered finished**. 

Movement is split into iterations, each iteration handling movement for for tile in any direction. On each iteration, ```OnMoveStarted``` is called for each object which ```MoveAction.Started``` is false, and ```OnMoveFinished``` is called for each object, which ```MoveAction.Interrupted``` is true and ```MoveAction.Finished``` is false.

There are several overridable methods to customize behaviour of the objects: ```MovementRulesComponent.CheckCanMove```, ```MovementRulesComponent.CheckBoundObjectsAllowMove```, ```LevelObjectComponent.CheckObjectEnter``` and others.

There are three types of movement on the code level: **Regular Movement**, **Teleportation** and **Delayed Movement**.
- **Regular Movement** is the simple movement of objects. Player, Boxes etc are using regular movement when moved/pushed.
- **Teleportation** is the unique case of movement, always executed after the all regular movement was finished.
- **Delayed Movement** is used if something during regular movement execution led to another objects movenent. For example, if player pushes a box on the Color Push Button, he will enter the button cell, causing all the colored boxes to move in the direction button shows. This movement will be executed _after_ all the movement caused by player pushing box (including all the grouped boxes) finishes.

# Level Editor
<img align="left" width="250" src="images/LevelEditor.png">
Level creation is a process, requiring a fast implementation of changes and clear, immediate vision of changed result. To ensure this is possible, I've created a simple yet powerful level editor able to cover every design situation for the entire game.

Level Editor is a simple ```OdinValueDrawer``` (class provided by ```OdinInspector``` framework) for the ```LevelData``` class. This means, in every MonoBehaviour or ScriptableObject where LevelData is mentioned, this editing interface can be used.

In general, Level Editor fetches objects data on its own from the application. However, for some complicated objects manual drawing logic override is required.
<br></br>

# Reversion System
<img align="right" width="250" src="Assets/Graphics/Sprites/Tutorials/Tutorial0Controls3.png">

A very often case in a puzzle games is a wrong user input (misclick) leading to the loss situation and requiring level to be restarted. To avvoid this frustrating situation, a full turn reversion system was implemented in Soko.

Every time movement is executed, an Imprint is created for every object and every it's component with ```IImprintableComponent``` interface. Components of this type have ```CreateComponentImprint``` and ```RestoreFromImprint``` methods where developer can define what is imprinted and how to revert a component to it's previous state base on imprinted information.

# Tutorial System

<img align="left" width="250" src="images/Tutorial.png">
Sometimes a new element's behaviour can be unintuitive and require an explicit explanation. To make this possible, a compound tutorial system was implemented. As user can start any level he wants (and also levels can be changed by GD), tutorial system relies on a conditions rather than the exact levels. Each time user encounters a new element, that element is focused and an information popup appears, explaining that element's behaviour.
