using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TileManager : MonoBehaviour
{
    [SerializeField] int _width, _height;

    List<List<Tile>> _Tiles = new List<List<Tile>>();

    [SerializeField] Tile prefabTile;
    [SerializeField] GridLayoutGroup _gridLayoutGroup;

    [SerializeField] LineRenderer _line;
    [SerializeField] Text _selectedTile;
    //int[,] A = new int[3, 5];
    //private Dictionary<Vector2, Tile> _tiles = new Dictionary<Vector2, Tile>();

    // Start is called before the first frame update
    void Start()
    {
        _line = this.GetComponent<LineRenderer>();
        //In case use UI
        _gridLayoutGroup.constraint = GridLayoutGroup.Constraint.FixedRowCount;
        _gridLayoutGroup.constraintCount = _height;
        //In case use UI

        int totalTile = (_width -2) * (_height-2) / 2;
        Debug.LogError(totalTile);
        Stack<int> existNumber = new Stack<int>();
        for (int i = 0; i< totalTile; i++)
        {
            existNumber.Push(i);
        }

        for (int y = 0; y< _height; y++)
        {
            List<Tile> row = new List<Tile>();
            for (int x = 0; x < _width; x++)
            {
                Tile tmp = Instantiate<Tile>(prefabTile, new Vector2(x, -y), Quaternion.identity, _gridLayoutGroup.transform);
                row.Add(tmp);
                tmp.setX(x);
                tmp.setY(y);

                //tmp.isObstacle =  Random.Range(0, 100) <= 30;

                if (x == _width - 1 || y == _height - 1 || x == 0 || y == 0)
                {
                    tmp.GetComponent<Image>().enabled = false;
                    tmp.transform.GetChild(0).gameObject.SetActive(false);
                    continue;
                }

                tmp.GetComponent<Button>().onClick.AddListener(()=> {

                    CallSetPoint(tmp);
                });

                tmp.gameObject.name = string.Format("Tile [{0},{1}]", x, y);


                //_tiles.Add(new Vector2(x,y), tmp);
            }

            _Tiles.Add(row);
        }

        while (existNumber.Count != 0)
        {
            int tmp = existNumber.Pop();

            Tile choose = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            while (choose.ID != -1 || choose.GetComponent<Image>().enabled == false)
            {
                choose = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            }

            choose.ID = tmp;
            choose.IDText.text = tmp.ToString();

            choose = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            while (choose.ID != -1 || choose.GetComponent<Image>().enabled == false)
            {
                choose = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            }
            choose.ID = tmp;
            choose.IDText.text = tmp.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            foreach (List<Tile> t in _Tiles)
            {
                foreach (Tile t2 in t)
                    Destroy(t2.gameObject);
            }
            _Tiles.Clear();
            points.Clear();
            Start();
        }
       
    }

    List<Tile> points = new List<Tile>(); 
    public void CallSetPoint(Tile selected)
    {
        if(points.Count >= 2)
        {
            foreach (Tile t in points)
            {
                t.GetComponent<Image>().color = Color.white;
            }
            points.Clear();
        }

        _selectedTile.text = selected.ID.ToString();
        points.Add(selected);
        if(points.Count == 1)
        {
            selected.GetComponent<Image>().color = Color.red;
            
        }
        else
        {
            selected.GetComponent<Image>().color = Color.green;

            if(points[0].ID == points[1].ID)
            {


                if (checkPathFinding())
                    return;
            }

            Debug.LogError("Wrong!");
            foreach (Tile t in points)
            {
                t.GetComponent<Image>().color = Color.white;
            }
            points.Clear();
        }
    }

  
    bool checkPathFinding()
    {

        List<Tile> path = pathFinding(points[0], points[1]);
        if (path == null)
            return false;
        Debug.LogError("Match!");
        int i = 0;
        while (i < path.Count)
        {
            Tile tmp = path[i];

            int j = 0;

            List<Tile> equalX = new List<Tile>();
            List<Tile> equalY = new List<Tile>();
            while (j < path.Count)
            {
                if (!tmp.Equals(path[j]))
                {
                    if (tmp.X == path[j].X)
                        equalX.Add(path[j]);

                    if(tmp.Y == path[j].Y)
                        equalY.Add(path[j]);
                }

                j++;
            }

           
            if (equalX.Count > 2)
               for(int k = 0; k < equalX.Count-1; k++)
                {

                    path.Remove(equalX[k]);
                }

          
            if (equalY.Count > 2)
                for (int k = 0; k < equalY.Count-1; k++)
                {

                    path.Remove(equalY[k]);
                }

            i++;
        }

        if (path.Count - 1 > 3)
            return false;


        _line.positionCount = path.Count;
        for (i = 0; i < path.Count; i++)
        {
            _line.SetPosition(i, path[i].transform.position);
        }

        StartCoroutine(DelayTurnOff(path[0], path[path.Count - 1]));

        return true;
    }

    IEnumerator DelayTurnOff(Tile start, Tile end)
    {
        yield return new WaitForSeconds(1);

        start.GetComponent<Image>().enabled = false;
        start.transform.GetChild(0).gameObject.SetActive(false);

        end.GetComponent<Image>().enabled = false;
        end.transform.GetChild(0).gameObject.SetActive(false);

        _line.positionCount = 0;
    }

    public List<Tile> pathFinding(Tile start, Tile target)
    {
        Debug.LogError("Start path finding!");
        List<Tile> toSearch = new List<Tile>() { start };
        List<Tile> searchDone = new List<Tile>();

        while (toSearch.Count != 0)
        {
            Tile current = toSearch[0];
            //foreach (Tile t in toSearch)
            //{
            //    if (current.F < t.F )
            //    {
            //        current = t;
            //        continue;
            //    }

            //    if (current.F == t.F && t.H < current.H)
            //    {
            //        current = t;
            //        continue;
            //    }
            //}

            toSearch.Remove(current);
            searchDone.Add(current);

           

            if (current.Equals(target))
            {
                Debug.LogError("Reach target");
                Tile tmp = current;
                List<Tile> path = new List<Tile>();
                while (!tmp.Equals(start))
                {
                    path.Add(tmp);
                    tmp = tmp.connect;
                    tmp.GetComponent<Image>().color = Color.yellow;
                    
                }
                path.Add(start);
                return path;
            }

            

            foreach (Vector2 v in current.get_neirbought())
            {
                if ((int)v.y < 0 || (int)v.x < 0)
                    continue;

                if ((int)v.y >= _height || (int)v.x >= _width)
                    continue;

                Tile neightbor = _Tiles[(int)v.y][(int)v.x];

                if (!neightbor.Equals(start) && !neightbor.Equals(target) && neightbor.GetComponent<Image>().enabled == true)
                {
                    continue;
                }

                if (searchDone.Contains(neightbor))
                    continue;

                bool isDone = toSearch.Contains(neightbor);

                int distance = current.G + current.calDistance(neightbor);


                //neightbor.GetComponent<Image>().color = Color.blue;

                if (!isDone || distance < neightbor.G)
                {
                    neightbor.setG(distance);
                    neightbor.setH(neightbor.calDistance(target));
                    neightbor.connect = current;

                    if (!isDone)
                        toSearch.Add(neightbor);
                    
                }
            }
        }

        return null;
    }

    public void ShowHint()
    {
        List<Tile> path;
        Tile t1, t2;
        int count = 0;
        int totalTile = 0;

        foreach (List<Tile> tmp1 in _Tiles)
        {
            foreach (Tile tmp2 in tmp1)
            {
                if (tmp2.GetComponent<Image>().enabled == true)
                    totalTile++;
            }
                
        }


        do
        {
            t1 = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            while (t1.GetComponent<Image>().enabled == false)
            {
                t1 = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            }

             t2 = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            while ( t2.ID != t1.ID || t2.Equals(t1))
            {
                t2 = _Tiles[Random.Range(0, _height)][Random.Range(0, _width)];
            }
           
            path = pathFinding(t1, t2);
            count++;


        } while (path == null || count > totalTile /2 );

        if(path != null)
        {
            t1.GetComponent<Image>().color = Color.blue;
            t2.GetComponent<Image>().color = Color.blue;
        }
        else
        {
            reRandom();
        }
       
    }


    public void reRandom()
    {
        List<Tile> remainTiles = new List<Tile>();
        Queue<int> remainIDs = new Queue<int>();

        foreach (List<Tile> L1 in _Tiles)
        {
            foreach (Tile T in L1)
            {
                if (!T.GetComponent<Image>().enabled)
                    continue;

                remainTiles.Add(T);
                if (!remainIDs.Contains(T.ID))
                    remainIDs.Enqueue(T.ID);

                T.ID = -1;
            }
        }


        while (remainIDs.Count != 0)
        {
            int tmp = remainIDs.Dequeue();

            Tile choose = remainTiles[Random.Range(0, remainTiles.Count)];
            while (choose.ID != -1 || choose.GetComponent<Image>().enabled == false)
            {
                choose = remainTiles[Random.Range(0, remainTiles.Count)];
            }

            choose.ID = tmp;
            choose.IDText.text = tmp.ToString();

            choose = remainTiles[Random.Range(0, remainTiles.Count)];
            while (choose.ID != -1 || choose.GetComponent<Image>().enabled == false)
            {
                choose = remainTiles[Random.Range(0, remainTiles.Count)];
            }
            choose.ID = tmp;
            choose.IDText.text = tmp.ToString();
        }

    }
}
