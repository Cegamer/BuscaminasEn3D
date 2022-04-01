using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    private ModeSelection gameMode;
    public GameObject[] celdasRelleno;
    public GameObject grassPrefab, cameraParent, grid, gridParent, loseCanvas, winCanvas, timerTXT,flagsTXT;
    public int gridSize, mines,flagsPlaced;

    public CameraMovement camMove;
    public float timer = 0;
    public Cell[,] grasSquares;
    public Material gridMaterial,touchedMaterial;
    public List<Cell> mineList;
    public LayerMask laymask;
    public bool playing = true;
    public Transform cameraObjective;
    public Vector3 cameraStartPosition;
    public Sprite[] faceGUI;
    void Start()
    {
        gameMode = GameObject.FindGameObjectWithTag("noDestroy").GetComponent<ModeSelection>();
        gridSize = gameMode.getMapSize();
        mines = gameMode.getMinesNumber();

        gridGeneration();
    }
    void Update()
    {
        if (playing) 
        rayCasting();
        timerFunc();
    }
    public void gotoMenu() {

        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    
    }
    void timerFunc()
    {
        timer += 1 * Time.deltaTime;
        var mins = (int)timer / 60;
        var segs = (int)timer % 60;

        timerTXT.GetComponent<Text>().text = text(mins, segs);

    }
    string text(int mins, int segs)
    {
        if (segs < 10 && mins > 9)
            return $"{mins}:0{segs}";

        else if (segs < 10 && mins < 10)
            return $"0{mins}:0{segs}";

        else if (segs > 10 && mins < 10)
            return $"0{mins}:{segs}";

        else
            return $"{mins}:{segs}";
    }
    
    void gridGeneration()
    {
        mineList = new List<Cell>();
        gridParent = new GameObject("gridParent");
        gridParent.transform.position = new Vector3(0, 0, 0);
        grasSquares = new Cell[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                grasSquares[x, z] = new Cell(x, z, gridParent, grassPrefab,this,touchedMaterial);
            }
        }

        //calculates grid X and Z position
        float gridposition = (gridSize / 2f) - 0.5f;

        //Defines grid Position, tiling and Scale
        grid.transform.position = new Vector3(gridposition, 0.01f, gridposition);
        grid.transform.localScale = new Vector3(gridSize, 0.0000001f, gridSize);
        gridMaterial.mainTextureScale = new Vector2(gridSize, gridSize);
        grid.GetComponent<Renderer>().material = gridMaterial;

        //Define Camera Starting Position
        cameraStartPosition = new Vector3(gridposition, gridSize, -0.5f);

        minesGeneration();
        if (gridSize < 50)
        {
            fillingBorders(-gridSize / 2, 0, 0, gridSize);
            fillingBorders(gridSize, gridSize + gridSize / 2, 0, gridSize);
            fillingBorders(-gridSize / 2, gridSize + gridSize / 2, gridSize, gridSize * 2);
        }
        flagsTXT.GetComponent<Text>().text = $"{flagsPlaced}/{mines}";

    }

    void minesGeneration()
    {
        for(int countmines = 0;countmines < mines; countmines++)
        {
            var x = Random.Range(0, gridSize);
            var z = Random.Range(0, gridSize);

  
            grasSquares[x, z].hasMine = true;
            mineList.Add(grasSquares[x, z]);
        }

        foreach(Cell actual in grasSquares)
        {
            actual.calculateAdyacentCells();
        }
    }
    void fillingBorders(int startX, int endX, int startZ, int endZ)
    {
        for (int x = startX; x < endX; x++)
        {
            for (int z = startZ; z < endZ; z++)
            {
                int rand = Random.Range(0, celdasRelleno.Length);
                var gameObject = Instantiate(celdasRelleno[rand], new Vector3(x, 0, z), Quaternion.identity, gridParent.transform);
                gameObject.name = $"filling {x},{z}";
            }
        }
    }
   
    private void rayCasting()
    {
        RaycastHit raycastHit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out raycastHit, Mathf.Infinity, laymask))
        {
            Debug.DrawLine(Input.mousePosition, raycastHit.transform.position, Color.red);
            if (raycastHit.transform != null)
            {
                cameraObjective = raycastHit.transform;
                var cont = raycastHit.transform.gameObject.GetComponent<SquareController>();

                if (Input.GetMouseButtonDown(0))
                    grasSquares[cont.x, cont.z].revealCell();
                
                else if (Input.GetMouseButtonDown(1))
                    grasSquares[cont.x, cont.z].flagCell();
            }
        }
    }
    public void winGame()
    {
        playing = false;
        winCanvas.SetActive(true);
    }
    IEnumerator endGame()
    {
        playing = false;
        camMove.restoreCamera();
        foreach (Cell c in mineList)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            c.mine.SetActive(true);
        }
        loseCanvas.SetActive(true);
    }
    public void restart()
    {
        flagsPlaced = 0;
        timer = 0;
        Destroy(gridParent);
        StopAllCoroutines();
        gridGeneration();
        loseCanvas.SetActive(false);
        winCanvas.SetActive(false);

        playing = true;
    }

    public class Cell
    {
        // state: 0 grass | 1 dirt | 2 Flagged
        public int xPosition, zPosition, adyacentMines, state = 0;
        public bool hasMine = false;
        public string name;
        MapGenerator mapGenerator;
        public GameObject gameObject,text3D, mine, flag;
        public SquareController squareController;
        public List<Cell> adyacentCells = new List<Cell>();
        public Material touchedMaterial;
        public Cell(int x, int z, GameObject gridParent, GameObject grassPrefab, MapGenerator mapgen, Material touchedMat)
        {
            mapGenerator = mapgen;
            xPosition = x;
            zPosition = z;
            name = $"{x},{z}";
            touchedMaterial = touchedMat;

            Vector3 position = new Vector3(x, 0, z);
            GameObject actualGrass = Instantiate(grassPrefab, position, Quaternion.identity);

            squareController = actualGrass.GetComponent<SquareController>();
            squareController.x = x;
            squareController.z = z;

            actualGrass.name = name;
            actualGrass.transform.parent = gridParent.transform;
            gameObject = actualGrass;
            text3D = gameObject.transform.Find("text3D").gameObject;
            mine = gameObject.transform.Find("mine").gameObject;
            flag = gameObject.transform.Find("flag").gameObject;

        }


        public void calculateAdyacentCells()
        {
            int xMin = xPosition - 1;
            int zMin = zPosition - 1;
            int xMax = xPosition + 1;
            int zMax = zPosition + 1;

            for (int x = xMin; x <= xMax; x++)
            {
                for (int z = zMin; z <= zMax; z++)
                {
                    try
                    {
                        if (mapGenerator.grasSquares[x, z] != this)
                        {
                            adyacentCells.Add(mapGenerator.grasSquares[x, z]);
                            if (mapGenerator.grasSquares[x, z].hasMine == true)
                                adyacentMines++;
                        }
                    }
                    catch { }
                }
                text3D.GetComponent<TextMesh>().text = adyacentMines.ToString();
            }
        }
        public void flagCell()
        {
            if (mapGenerator.flagsPlaced <= mapGenerator.mines)
            {
                if (state == 0)
                {
                    flag.SetActive(true);
                    state = 2;
                    mapGenerator.flagsPlaced++;

                    if (checkFlagedMines())
                        mapGenerator.winGame();
                }
                else if (state == 2)
                {
                    flag.SetActive(false);
                    state = 0;
                    mapGenerator.flagsPlaced--;
                }
                mapGenerator.flagsTXT.GetComponent<Text>().text = $"{mapGenerator.flagsPlaced}/{mapGenerator.mines}";
            }
        }
        bool checkFlagedMines()
        {
            foreach (Cell mine in mapGenerator.mineList)
                if (mine.state == 0)
                    return false;
            
            return true;
        }

        public void revealCell()
        {
            if (!hasMine && state == 0)
            {
                text3D.SetActive(true);
                gameObject.GetComponent<Renderer>().material = touchedMaterial; 
                
                state = 1; // <---- No Tocar Esta wea de aqui

                if (adyacentMines == 0)
                {
                    text3D.GetComponent<TextMesh>().text = "";
                    foreach (Cell c in adyacentCells)
                        if (c.state == 0)
                            c.revealCell();
                }
            }
            else if (hasMine && state == 0)
            {
                mine.SetActive(true);

                IEnumerator coroutine = mapGenerator.endGame();
                mapGenerator.StartCoroutine(coroutine);     
            }
        }
    }
}

