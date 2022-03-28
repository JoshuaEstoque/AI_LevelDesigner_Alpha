using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using System.IO;

public class StageBuider : Agent
{
	public int left=0, right=0, up=0, down=0, placeTile=0, placeEmpty=0, placeGoal=0, placePlayer=0, moveNothing=0, placeNothing=0;

    public int StageLength;
    public int StageHeight;

    private Vector3Int previous;

    public Tile tile1;
    public Tile tile2;
    public Tile tile3;
    public Tile tile4;
    public Tile tile5;
	
    public Tilemap highlightMap;
	public Tilemap debugMap;
	public Tilemap voidMap;
	public Tilemap borderMap;
	
    public Grid gridLayout;

    public Transform goalPosition;
    private bool flagCheck = false;
	
	public Transform spawnPosition;
	public Transform playerPosition;
	
	public Transform topLeft;

    private int[] stageGrid = new int[20 * 10];

    Vector3Int[,] tilePositions = new Vector3Int[99, 99];
    Vector3Int currentCell;
    public Vector3Int initialCell;
    private int xCount = 0, yCount = 0, testcount = 0;

    //public Tilemap debugMap;

    private int count = 1;
	private int maxStep = 50000;
	
    public int AstarDistance = 0;
	public bool flagPlaced = false, playerPlaced = false;
	public int totalTiles = 200, old_totalTiles = 200;
	public float Astar_ratio;
	private string screenshotName;
	public int deathCount = 0;
	
	private bool isActive = true;
	
	public int loadStageIndex = 0;
	
	[SerializeField]
	private int timer = 0;
	
	private int randomIndex = 0;
	
	[SerializeField]
    public GameObject my_Astar;
    Astar myAstar_script;
	
	[SerializeField]
	public float tileType = -1f;
	
	[SerializeField]
    public GameObject my_PlayerMove_Test;
    PlayerMove_Test myPlayerMove_Test_script;
	
	string folderPath = Directory.GetCurrentDirectory() + "/Screenshots/";
	public bool isMapping = true;
	
	[SerializeField]
	public Transform selector_pos;
	
	public float currentCumulativeReward;

    // Start is called before the first frame update
    void Start()
    {
		myAstar_script = my_Astar.GetComponent<Astar>();
		myPlayerMove_Test_script = my_PlayerMove_Test.GetComponent<PlayerMove_Test>();
        //initialCell = gridLayout.WorldToCell(transform.position);
        //getGrid();
        myPlayerMove_Test_script.freezePlayer(true);
		ClearMap();
		randomIndex = (Random.Range(1,1000));
		
		//timescale for testing
		Time.timeScale = 15f;
    }
	

    // Update is called once per frame
    void Update()
    {
		currentCumulativeReward = GetCumulativeReward();
		if(isMapping){
			myPlayerMove_Test_script.freezePlayer(true);
		}
		timer++;
		if(timer>5000){
			
			timer = 0;
			AddReward(-0.1f);
			Debug.Log("Reward updated (time out): "+GetCumulativeReward());
			Debug.Log("++++++++++++++++TIME OUT++++++++++++++++      FILLING EMPTY TILES Reward: "+GetCumulativeReward());
			fillEmpty();
		}
		totalTiles = updateTileCount();
		Vector3Int currentCell = gridLayout.WorldToCell(selector_pos.localPosition);
		AstarDistance = (myAstar_script.Algorithm(transform.position, goalPosition.position));
	
	
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            ClearMap();
        }
		if(Input.GetKeyDown(KeyCode.L)){
			myPlayerMove_Test_script.loadStage(loadStageIndex);
			loadStageIndex++;
		}
		if(Input.GetKeyDown(KeyCode.K)){
			screenshotName = (randomIndex+"___TestStage_"+loadStageIndex);
			ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+".png");
		}
		
		
        //start player after map is filled out
		if(totalTiles==0 && isMapping){
			Debug.Log("***************************************************************** TotalTiles = "+totalTiles+" IsMapping = "+isMapping);
		// || !flagPlaced || !playerPlaced
			if(AstarDistance > 500){
				Debug.Log("===NO PATH FOUND - MAP INVALID===   Astar = "+AstarDistance+" flag/player: "+flagPlaced+"/"+playerPlaced);
				AddReward(-0.5f);
				Debug.Log("Reward updated (no path found): "+GetCumulativeReward());
				EndEpisode();
			}else{
				//TakeScreenshot(false, screenshotName, folderPath, GetCumulativeReward(), (Random.Range(1,1000)));
				screenshotName = (randomIndex+"___RewardIs"+GetCumulativeReward());
				ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+".png");
				
				//gameObject.SetActive(false);
				isActive = false;
				Time.timeScale = 1f;
				
				highlightMap.SetTile(new Vector3Int((int)spawnPosition.position.x, (int)spawnPosition.position.y, 0), tile1);
				highlightMap.SetTile(new Vector3Int((int)playerPosition.position.x, (int)playerPosition.position.y, 0), tile1);
				highlightMap.SetTile(new Vector3Int((int)goalPosition.position.x, (int)goalPosition.position.y, 0), tile1);
				
				
				isMapping = false;
				myPlayerMove_Test_script.freezePlayer(false);
				//Debug.Log("Astar_ratio reward = "+(1-Astar_ratio));
				transform.position = topLeft.position;
				//transform.position = new Vector3(currentCell.x+999.5f, currentCell.y+999.5f, 0);
				AddReward(1-(Astar_ratio*4));
				Debug.Log("Reward updated (A* ratio): "+GetCumulativeReward());
				}
			
		}else{
			isActive = true;
		}

        
    }

    public void arrayGridEditor()
    {
        int count2 = 0;
        for(int y = 0; y < StageHeight; y++)
        {
            for (int x = 0; x < StageLength; x++)
            {
                switch(stageGrid[count2])
                {
                    case 0:
                        break;
                    case 1:
                        break;
                }
                count2++;
            }
        }
    }

    public int[] getGridArray()
    {
        BoundsInt bounds = highlightMap.cellBounds;
        TileBase[] allTiles = highlightMap.GetTilesBlock(bounds);
        string arrayStr = "";
        int count1 = 0;
        for (int y = 0; y < bounds.size.y; y++)
        {
            arrayStr = "[";
            for (int x = 0; x < bounds.size.x; x++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile != null)
                {
                    switch (tile.name)
                    {
                        case "Tile":
                            stageGrid[count1] = 1;
                            break;
                        case "Empty":
                            stageGrid[count1] = 0;
                            break;
                    }
                    tilePositions[x,y] = new Vector3Int(bounds.size.x, bounds.size.y, 0);
                    arrayStr += stageGrid[count1] + ", ";
                    count1++;
                    //Debug.Log(tilePositions[x, y]);
                }
                else
                {
                    Debug.Log("ERROR ---------- NULL DETECTED ---------- ERROR");
                }
            }
            arrayStr += "]";
            //print(arrayStr);
            arrayStr = "";
            foreach (int i in stageGrid)
            {
                testcount++;
                arrayStr += (i+", ");
                if (testcount%20 == 0)
                {
                    arrayStr += ("\n");
                }
            }
            print(arrayStr);

        }
        arrayStr = "ARRAY SIZE: x=" + bounds.size.x + " y=" + bounds.size.y;
        print(arrayStr);
        return stageGrid;
    }

    public void setGridArray(int[] stageGrid, Tilemap highlightMap, Transform goalPosition, Vector3 tempPosition, Transform startPosition)
    {
        //tempPosition.x = (int)Mathf.Floor(tempPosition.x);
        //tempPosition.y = (int)Mathf.Floor(tempPosition.y);
        int randIndex = 0;
        int[] goalArray = new int[5];
        int goalCount = 0;
        flagCheck = false;
        int count3 = 0;
        //BoundsInt bounds = highlightMap.cellBounds;
		BoundsInt bounds = voidMap.cellBounds;
        TileBase[] tileArray = new TileBase[bounds.size.x * bounds.size.y * bounds.size.z];
        int index = 0;
        string temptemp = "THE BOUNDS SIZE X="+bounds.size.x+" Y="+bounds.y;
        //Debug.Log(temptemp);
            for (int y = 0; y < bounds.size.y; y++)
            {
                for (int x = 0; x < bounds.size.x; x++)
                {
                //Debug.Log(count3);
                    if (stageGrid[count3] == 1)
                    {
                        tileArray[index] = tile2;
                    }
                    else if(stageGrid[count3] == 0)
                    {
                        tileArray[index] = tile1;
                    }

                //MOVING GOALPOSTS
                if (stageGrid[count3] == 2)
                {
                    temptemp = "FLAG FOUND AT INDEX #" + index;
                    //Debug.Log(temptemp);
                    goalArray[goalCount] = index;
                    goalCount++;
                    tileArray[index] = tile1;
                }
                if (goalCount >= 3){
                    //Debug.Log("tempPosition.x = " + tempPosition.x + " tempPosition.y = "+tempPosition.y);
                    randIndex = (int)Random.Range(1, 5);
                    //Debug.Log("goalArray = " + goalArray[randIndex - 1]);
                    goalPosition.position = new Vector3(tempPosition.x + (goalArray[randIndex - 1] % 20), tempPosition.y + (int)(goalArray[randIndex - 1]/20), 0);
                    //Debug.Log("Variables: randIndex = " + randIndex + " tempPosition.y = " + tempPosition.y + " goalArray[randIndex - 1] = " + goalArray[randIndex - 1] + "(goalArray[randIndex - 1] % 20) = " + ((goalArray[randIndex - 1] % 20)));
                    //Debug.Log("======tempPosition.y = "+tempPosition.y+ " (int)(goalArray[randIndex - 1]/20) = "+((int)(goalArray[randIndex - 1] / 20)));
                    /*
                    switch (randIndex)
                    {
                        case 1:
                            goalPosition.position = new Vector3(tempPosition.x + (goalArray[0] % 20), tempPosition.y - ((goalArray[0] - (goalArray[0] % 20)) / 10), 0);
                            goalPosition.position = new Vector3(tempPosition.x + (goalArray[0] % 20), tempPosition.y - ((int)Mathf.Floor(goalArray[0] / 20)), 0);
                            
                            break;
                        case 2:
                            goalPosition.position = new Vector3(tempPosition.x + (goalArray[1] % 20), tempPosition.y - ((int)Mathf.Floor(goalArray[1] / 20)), 0);
                            
                            break;
                        case 3:
                            goalPosition.position = new Vector3(tempPosition.x + (goalArray[2] % 20), tempPosition.y - ((int)Mathf.Floor(goalArray[2] / 20)), 0);
                            
                            break;
                        case 4:
                            goalPosition.position = new Vector3(tempPosition.x + (goalArray[3] % 20), tempPosition.y - ((int)Mathf.Floor(goalArray[3] / 20)), 0);
                            
                            break;
                    }*/
                }
                //MOVING STARTING POSITION
                if (stageGrid[count3] == 3)
                {
                    tileArray[index] = tile1;
                    //startPosition.position = new Vector3(tempPosition.x + (index % 20), tempPosition.y - ((index - (index % 20)) / 10), 0);
                    startPosition.position = new Vector3(tempPosition.x + (index % 20), tempPosition.y + (int)(index / 20) + 0.5f, 0);
                }

                index++;
                count3++;
            }
                    
            }


        goalCount = 0;
        highlightMap.SetTilesBlock(bounds, tileArray);
    }

    public bool checkBoundsX(int x, bool direction)
    {
        if(x > 0 && !direction)
        {
            return true;
        }

        if(x < StageLength && direction)
        {
            return true;
        }
        return false;
    }

    public bool checkBoundsY(int y, bool direction)
    {
        if (y < 0 && direction)
        {
            return true;
        }

        if (y > -StageHeight && !direction)
        {
            return true;
        }

        return false;
    }


    public void moveSelection(int x, int y)
    {
        //string temp = "xCount: " +xCount+ "yCount: " +yCount;
        //Debug.Log(temp);
        if(xCount < StageLength)
        {
            xCount++;
            transform.position = gridLayout.CellToWorld(gridLayout.WorldToCell(transform.position) + Vector3Int.right);
            //Debug.Log("IT MOVED LOL<<<<<<<<<<<<<<<<<<");
        }
        else
        {
            yCount++;
            xCount = 0;
            transform.position = gridLayout.CellToWorld(initialCell);
            for (int i = 0; i < yCount; i++)
            {
                transform.position = gridLayout.CellToWorld((gridLayout.WorldToCell(transform.position)) + (Vector3Int.down));
            }
        }
        if(yCount >= StageHeight+1)
        {
            xCount = 0;
            yCount = 0;
            //Debug.Log("REACHED END");
            //debugMap.ClearAllTiles();
            transform.position = initialCell;
            //Astar.MyAstarInstance.Algorithm();
            //ClearMap();
        }
        /*
        if (x < StageLength)
        {
            
        }
        else
        {
            if (y > -StageHeight)
            {
                transform.position = gridLayout.CellToWorld(initialCell);
                for (int i = 0; i<count; i++)
                {
                    transform.position = gridLayout.CellToWorld((gridLayout.WorldToCell(transform.position)) + (Vector3Int.down));
                }
                count++;
                Debug.Log("Count is " + count);
            }
            else
            {
                count = 0;
                Debug.Log("REACHED END");
                //debugMap.ClearAllTiles();
                transform.position = initialCell;
                Astar.MyAstarInstance.Algorithm();
                //ClearMap();
                

            }
        }
        */
    }
	
    public void ClearMap()
    {
		voidMap.ClearAllTiles();
		BoundsInt bounds = borderMap.cellBounds;
		foreach(Vector3Int pos in bounds.allPositionsWithin){
			Tile tile = borderMap.GetTile<Tile>(pos);
			if(tile != null)
			{
				voidMap.SetTile(pos, tile3);
			}
		}
		
        highlightMap.ClearAllTiles();
		
        count = 1;
    }

	public override void Initialize()
    {
        if (!Academy.Instance.IsCommunicatorOn)
        {
            this.MaxStep = 0;
        }
        else
        {
            maxStep = 100000;
        }
    }
	
	//Builder code
	public override void OnActionReceived(float[] vectorAction)
    {
		if(!isActive){
			Debug.Log("not active");
			return;
		}
		
		if(!isMapping){
			timer = 0;
			vectorAction[0] = 4;
			vectorAction[1] = 4;
		}
		
		//vectorAction[0] =  (Random.Range(0,4));
		//vectorAction[1] =  (Random.Range(0,4));
		
		//Debug.Log("================   Action is: "+vectorAction[0]);
		switch(vectorAction[0]){
		
			case 0:
				//right
				right++;
				if (checkBoundsX(xCount, true))
				{
					transform.position = gridLayout.CellToWorld(gridLayout.WorldToCell(transform.position) + Vector3Int.right);
					xCount++;
				}
				break;
			case 1:
				//up
				up++;
				if (checkBoundsY(yCount, true))
				{
					transform.position = gridLayout.CellToWorld(gridLayout.WorldToCell(transform.position) + Vector3Int.up);
					yCount++;
				}
				break;
			case 2:
				//left
				left++;
				if (checkBoundsX(xCount,false)) 
				{
					transform.position = gridLayout.CellToWorld(gridLayout.WorldToCell(transform.position) + Vector3Int.left);
					xCount--;
				}
				break;
			case 3:
				//down
				down++;
				if (checkBoundsY(yCount, false))
				{
					transform.position = gridLayout.CellToWorld(gridLayout.WorldToCell(transform.position) + Vector3Int.down);
					yCount--;
				}
				break;
			case 4:
				moveNothing++;
				//do nothing
				break;
		
		}
		
		Vector3Int currentCell = gridLayout.WorldToCell(transform.position);
		switch(vectorAction[1]){
		
			case 0:
				//place empty
				placeEmpty++;
				highlightMap.SetTile(currentCell, tile1);
				voidMap.SetTile(currentCell, null);
				break;
			case 1:
				//place tile
				placeTile++;
				highlightMap.SetTile(currentCell, tile2);
				voidMap.SetTile(currentCell, null);
				break;
			case 2:
				//place goal
				placeGoal++;
				if(flagPlaced){
					AddReward(-0.01f); 
					//Debug.Log("Reward updated (goal replaced): "+GetCumulativeReward());
				}
				
				
				if(spawnPosition.position == new Vector3(currentCell.x+0.5f, currentCell.y+0.5f, 0)){
					//AddReward(-0.1f);
				}else{
				voidMap.SetTile(currentCell, null);
				highlightMap.SetTile(currentCell, tile1);
				goalPosition.position = new Vector3(currentCell.x+0.5f, currentCell.y+0.5f, 0);
				flagPlaced = true;
				}
				break;
			case 3:
				//place player
				placePlayer++;
				if(playerPlaced){
					AddReward(-0.01f);
					//Debug.Log("Reward updated (player replaced): "+GetCumulativeReward());
				}
				
				if(goalPosition.position == new Vector3(currentCell.x+0.5f, currentCell.y+0.5f, 0)){
					//AddReward(-0.1f);
				}else{
				voidMap.SetTile(currentCell, null);
				highlightMap.SetTile(currentCell, tile1);
				spawnPosition.position = new Vector3(currentCell.x+0.5f, currentCell.y+0.5f, 0);
				playerPosition.position = new Vector3(currentCell.x+0.5f, currentCell.y+0.5f, 0);
				if(!playerPlaced) myPlayerMove_Test_script.showPlayer(true);
				playerPlaced = true;
				}
				break;
			case 4:
				//do nothing
				placeNothing++;
				break;
		}
		
		if(totalTiles < old_totalTiles){
			AddReward(0.1f);
		}
		old_totalTiles = totalTiles;
		if(AstarDistance < 200){
			Astar_ratio = (float)(Mathf.Abs(20 - AstarDistance))/20;
		}
		
		//AddReward(-0.0001f);
		
	}
	
	
	public override void OnEpisodeBegin()
    {
		Time.timeScale = 15f;
		SetReward(0f);
		randomIndex = (Random.Range(1,1000));
		isActive = true;
		resetValues();
		
		ClearMap();
		//gameObject.SetActive(true);
		myPlayerMove_Test_script.freezePlayer(true);
		myPlayerMove_Test_script.showPlayer(false);
        Debug.Log("<<<<<<<<<<<<<<<<<<<<NEW EPISODE BEGIN>>>>>>>>>>>>>>>>>>>>>>>>");
		
    }
	
	public void resetValues(){
		timer = 0;
		transform.position = topLeft.position;
		
		//goalPosition.position = new Vector3(currentCell.x+999.5f, currentCell.y+999.5f, 0);
		//spawnPosition.position = new Vector3(currentCell.x+995.5f, currentCell.y+995.5f, 0);
		//playerPosition.position = new Vector3(currentCell.x+995.5f, currentCell.y+995.5f, 0);
		
		playerPlaced = false;
		flagPlaced = false;
		totalTiles = 200;
		old_totalTiles = 200;
		xCount = 0; 
		yCount = 0; 
		testcount = 0;
		isMapping = true;
		
	}
	
	public override void Heuristic(float[] actionsOut)
    {
        //Player Input
        actionsOut[0] = 4;
		
        if (Input.GetKeyDown(KeyCode.D))
        {
            actionsOut[0] = 0;
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            actionsOut[0] = 1;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            actionsOut[0] = 2;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            actionsOut[0] = 3;
        }
		
		actionsOut[1] = 4;
		if (Input.GetKeyDown(KeyCode.Z))
        {
            actionsOut[1] = 0;
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            actionsOut[1] = 1;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            actionsOut[1] = 2;
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            actionsOut[1] = 3;
        }
		
    }
	
	public override void CollectObservations(VectorSensor sensor)
    {
		Tile tile = highlightMap.GetTile<Tile>(Vector3Int.FloorToInt(transform.position));
		tileType = -1f;
		if(tile == tile1) tileType = 0f;
		if(tile == tile2) tileType = 1f;
		//get current tile it is hovering over
		sensor.AddObservation(tileType);
		//total Tiles in map
		sensor.AddObservation((float)totalTiles/200);
		//Distance between player and goal
		sensor.AddObservation(((float)AstarDistance/30));
		//if flag was placed
		sensor.AddObservation(flagPlaced);
		//if player was placed
		sensor.AddObservation(playerPlaced);
		//if builder is active
		sensor.AddObservation(isActive);
		//timer counting up
		sensor.AddObservation(timer/3000);
		
		
        float currentPosObs_X = (transform.localPosition.x - (-15)) / (5 - (-15));
        float currentPosObs_Y = (transform.localPosition.y - (-5)) / (5 - (-5));
		//current position normalized
		sensor.AddObservation(currentPosObs_X);
		sensor.AddObservation(currentPosObs_Y);
		
		float goalPosObs_X = (goalPosition.localPosition.x - (-15)) / (5 - (-15));
        float goalPosObs_Y = (goalPosition.localPosition.y - (-5)) / (5 - (-5));
		//goal position normalized
		sensor.AddObservation(goalPosObs_X);
		sensor.AddObservation(goalPosObs_Y);
		
		float spawnPosObs_X = (spawnPosition.localPosition.x - (-15)) / (5 - (-15));
        float spawnPosObs_Y = (spawnPosition.localPosition.y - (-5)) / (5 - (-5));
		//spawn position normalized
		sensor.AddObservation(spawnPosObs_X);
		sensor.AddObservation(spawnPosObs_Y);
		
	}
	

	
	public int updateTileCount(){
		int tileCount = 0;
		BoundsInt bounds = borderMap.cellBounds;
		foreach(Vector3Int pos in bounds.allPositionsWithin){
			Tile tile = voidMap.GetTile<Tile>(pos);
			if(tile != null)
			{
				tileCount++;
			}
		}
		return tileCount;
	}
	
	public void fillEmpty(){
	voidMap.ClearAllTiles();
	int tileCount = 0;
		BoundsInt bounds = borderMap.cellBounds;
		foreach(Vector3Int pos in bounds.allPositionsWithin){
			Tile tile = highlightMap.GetTile<Tile>(pos);
			if(tile == null)
			{
				highlightMap.SetTile(pos, tile1);
			}
		}
	}
	
	public void onClear(bool clear){
		if(clear){
			myPlayerMove_Test_script.freezePlayer(true);
			AddReward(3.0f);
			Debug.Log("Reward updated (player clears): "+GetCumulativeReward());
			Debug.Log("++Player Has Cleared The Map++");
			
			//TakeScreenshot(true, screenshotName, folderPath, GetCumulativeReward(), (Random.Range(1,1000)));
			screenshotName = (randomIndex+"___RewardIs"+GetCumulativeReward());
			ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+"______CLEARED.png");
			waiter();
		}else{
			AddReward(-1.0f);
			Debug.Log("Reward updated (player failed): "+GetCumulativeReward());
			Debug.Log("--Player Failed The Map--");
		}
		
		Debug.Log("++=======EPISODE ENDED=======++  Reward: "+GetCumulativeReward());
		
		for(int counter = 0; counter < 500; counter++){
			myPlayerMove_Test_script.freezePlayer(true);
		}
		EndEpisode();
		
	}
	
	public void onDeath(){
		Debug.Log("Player has died    Reward: "+GetCumulativeReward());
		deathCount++;
		if(deathCount>10){
			deathCount = 0;
			AddReward(-1.0f);
			Debug.Log("Reward updated (player died): "+GetCumulativeReward());
			Debug.Log("--Player Failed The Map--");
			Debug.Log("++=======EPISODE ENDED=======++  Reward: "+GetCumulativeReward());
			EndEpisode();
		}
		//AddReward(-0.10f);
	}

	/*
	private TakeScreenshot(bool cleared, string screenshotName, string folderPath, float cumulativeReward, int randomIndex){
		screenshotName = (randomIndex+"___RewardIs"+cumulativeReward);
		if(cleared){
			ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+"_CLEARED.png");
		}else{
			ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+".png");
		}
		
		for (int i = 0; i < 5; i++)
		{
			yield return null;
		}
		
	}
	*/
	
	
    IEnumerator waiter()
    {
        //Wait for 1 seconds
        yield return new WaitForSecondsRealtime(1);
    }
	
	
	
	
	/*
	private static IEnumerator TakeScreenshot(bool cleared, string screenshotName, string folderPath, float cumulativeReward, int randomIndex)
	{
		//wait for be draw
		
		screenshotName = (randomIndex+"___RewardIs"+cumulativeReward);
		
		
		yield return new WaitForEndOfFrame();
		
		int width = Screen.width;
		int height = Screen.height;
		Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
		// Read screen contents into the texture
		tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
		tex.Apply();
		//Encode to PNG
		byte[] screenshot = tex.EncodeToPNG();
		
		File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", screenshot);
		
		if(cleared){
			ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+"_CLEARED.png");
			File.WriteAllBytes(System.IO.Path.Combine(folderPath, screenshotName)+"_CLEARED.png", screenshot);
			Debug.Log("Screenshot saved to: "+folderPath+screenshotName);
		}else{
			File.WriteAllBytes(System.IO.Path.Combine(folderPath, screenshotName)+".png", screenshot);
			ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+".png");
			//ScreenCapture.CaptureScreenshot(System.IO.Path.Combine(folderPath, screenshotName)+".png");
		}
	}
	*/

}
