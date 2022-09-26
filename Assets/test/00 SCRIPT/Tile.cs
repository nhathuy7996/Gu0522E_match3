using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [SerializeField]
    int _X, _Y, _G = 0, _H = 0, _F = 0;
     public int ID = -1;
    [SerializeField] public Text IDText;

    
    public int G => _G;
    public int H => _H;
    public int F => _F;

    public int X => _X;
    public int Y => _Y;

    public Tile connect = null;

    public bool isObstacle = false;

    List<Vector2> _neirbought = new List<Vector2>() {
        new Vector2(0,1),
       // new Vector2(1,1),
        new Vector2(1,0),
       // new Vector2(1,-1),
        new Vector2(0,-1),
        //new Vector2(-1,-1),
        new Vector2(-1,0),
       // new Vector2(-1,1),

    };

    public List<Vector2> get_neirbought()
    {
        List<Vector2> tmp = new List<Vector2>();

        foreach (Vector2 v in this._neirbought)
        {
            tmp.Add(new Vector2(_X, _Y) + v);
        }

        return tmp;
    }

    public void setX(int newX)
    {
        _X = newX;
    }

    public void setY(int newY)
    {
        _Y = newY;
    }


    public void setG(int newG)
    {
        _G = newG;
        _F = _G + _H;
    }

    public void setH(int newH)
    {
        _H = newH;
        _F = _G + _H;
    }


    public int calDistance(Tile target)
    {
        //ko cho phep di chuyen cheo
        return Mathf.Abs(target.X - this.X) * 10 + Mathf.Abs(target.Y - this.Y) * 10;

        //cho phep di chuyen cheo

        //int X = Mathf.Abs(target.X - this.X);
        //int Y = Mathf.Abs(target.Y - this.Y);

        //return Mathf.Abs(X - Y) * 10 + (X < Y ? X : Y) * 14;
    }


    public void setColor(Color c)
    {
        this.GetComponent<Image>().color = c;
    }

}
