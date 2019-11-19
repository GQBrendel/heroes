using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils 
{
    static public List<Vector2> GetAdjacentPoints(Vector2 point)
    {
        List<Vector2> result = new List<Vector2>();
             Vector2 TopLeft = new Vector2(point.x - 1, point.y + 1); 
             Vector2 Top = new Vector2(point.x, point.y + 1); 
             Vector2 TopRight = new Vector2(point.x + 1, point.y + 1);
                       
             Vector2 Right = new Vector2(point.x + 1, point.y); 
                                                   
             Vector2 BottonRight = new Vector2(point.x + 1, point.y - 1); 
             Vector2 Botton = new Vector2(point.x, point.y - 1); 
             Vector2 BottonLeft = new Vector2(point.x - 1, point.y - 1); 
                                                   
             Vector2 Left = new Vector2(point.x - 1, point.y);

            result.Add(TopLeft);
            result.Add(Top);
            result.Add(TopRight);
            result.Add(Right);
            result.Add(BottonRight);
            result.Add(Botton);
            result.Add(BottonLeft);
            result.Add(Left);

            return result;
    }


}
