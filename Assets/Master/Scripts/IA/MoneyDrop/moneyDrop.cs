using UnityEngine;

public class moneyDrop : MonoBehaviour
{
    public Grid grid;
    public GameObject gold;

    private void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();    
    }

    // Start is called before the first frame update
    void Start()
    {
        GridLayout gridlayout = grid.GetComponent<GridLayout>();
        Vector3Int cellPosition = gridlayout.WorldToCell(transform.position);
        if (gameObject.tag != "Monster_Phase")
        {
            // 0 to 20 -> higher probability of not having anything
            int coinToDrop = Random.Range(0, 20);
            switch (coinToDrop)
            {
                case 1:
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y, 0), Quaternion.identity);
                    break;
                case 2:
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x+1, cellPosition.y, 0), Quaternion.identity);
                    break;
                case 3:
                    Instantiate(gold, new Vector3(cellPosition.x-1, cellPosition.y, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x + 1, cellPosition.y, 0), Quaternion.identity);
                    break;
                case 4:
                    Instantiate(gold, new Vector3(cellPosition.x - 1, cellPosition.y, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y+1, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x + 1, cellPosition.y, 0), Quaternion.identity);
                    break;
                case 5:
                    Instantiate(gold, new Vector3(cellPosition.x - 1, cellPosition.y, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y + 1, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x, cellPosition.y-1, 0), Quaternion.identity);
                    Instantiate(gold, new Vector3(cellPosition.x + 1, cellPosition.y, 0), Quaternion.identity);
                    break;
            }
        }
    }
}
