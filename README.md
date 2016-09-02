# Construction Mode - Gesture Ver.
**Author :** Yuan Yao  
**Last Update :** 2016.09.02

#### Introduction :
This is the gesture version of the game **Construction Mode**. It aims to help people practice their spatial reasoning skills. We used **Leap Motion** to detect the movement of user's hands and created a set of gestures as the input of the game.   
You can check the [demo](https://www.youtube.com/watch?v=Uku7eHwejqo) to briefly see how it works.

#### Operating Guide :
The set of gestures can be separated into two class : **one hand** and **two hands**. 

***Attention :*** The **one hand** class gesture ++only can be detected  when there is only one hand on the screen++, similarly **two hands** class gesture ++only can be detected when two hands appear on the screen++. The reason is because of the codes(HandController.cs) :

```C++
protected virtual void FixedUpdate()
{
    if (frame.Hands.Count == 2){
        ...
    }
    else if (frame.Hands.Count == 1){
        ...
    }
    else{
        ...
    }
}

```
Now here are the list of all the gestures.
#####  One Hand :  
1. **Swipe Gesture**  

**
All the swipe gestures should be parallel to the corresponding axis because of such codes :

```C++
if (
    (
    Math.Abs(swipe_direction_world.x) > 3 * Math.Abs(swipe_direction_world.y)
    ) && (
    Math.Abs(swipe_direction_world.x) > 3 * Math.Abs(swipe_direction_world.z)
    )
)

```
This condition sentence check whether your hand's movement is along the X-axis, therefore if the component of the movement on Y-axis or Z-axis is more that 1/3 of the component of that on X-axis, the movement won't be regarded as an X-axis swipe gesture. The swipe gestures on other directions are the same. To make it more generalizable, you can replace 3 with 2 or 1.5 .  



    1. Up or Down

2. **Feast Gesture**

#### Implementation :