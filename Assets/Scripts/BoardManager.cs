//this class is a singleton class. Only one object of this class should exist

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public static BoardManager board;    
	public List<Sprite> gemTypes = new List<Sprite>();   
	public GameObject gem;      
	public int xGrid, yGrid;
	public bool IsShifting { get; set; }

	private GameObject[,] tiles;

	void Start () {
		if (board == null) {
			board = GetComponent<BoardManager>();
		}		    

		Vector2 offset = gem.GetComponent<SpriteRenderer> ().size;	//getting the size of the gem sprite
		CreateBoard(offset.x, offset.y);		
	}

	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xGrid, yGrid];				//making a 2Darray of GameObject to store the gameObjects of gem there

		//x and y position of the board that is to be displayed.
		//this is not the center pos. This is the pos of lower left most pos.
		//this is because the array starts assigning from there. 
		//Play the game and press the gems on the scene to understand
		float startX = transform.position.x;				
		float startY = transform.position.y;															

		//will be used later to make sure there is no 3/more combinations in the beginning
		Sprite[] previousLeft = new Sprite[yGrid];
		Sprite previousBelow = null;

		for (int x = 0; x < xGrid; x++) {      
			for (int y = 0; y < yGrid; y++) {
				//instantiating a new duplicate gem gameobject and assigning in the 2darray tile 
				GameObject newGem = Instantiate(gem, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), gem.transform.rotation);
				tiles[x, y] = newGem;
				newGem.transform.parent = transform;	//making the board it's parent

				//now will assign color (different sprite) to the gem
				List<Sprite> possibleCharacters = new List<Sprite>(); 
				possibleCharacters.AddRange(gemTypes); 

				//removing the left and the down sprite from the list to avoid combinations
				possibleCharacters.Remove(previousLeft[y]); 
				possibleCharacters.Remove(previousBelow);

				//assigning one of the leftover sprites to the newGem
				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
				newGem.GetComponent<SpriteRenderer>().sprite = newSprite;

				//updateing the list
				previousLeft[y] = newSprite;
				previousBelow = newSprite;
			}
		}
	}

	public IEnumerator FindNullTiles() {
		for (int x = 0; x < xGrid; x++) {
			for (int y = 0; y < yGrid; y++) {
				if (tiles[x, y].GetComponent<SpriteRenderer>().sprite == null) {
					yield return StartCoroutine(ShiftTilesDown(x, y));
					break;
				}
			}
		}
		for (int x = 0; x < xGrid; x++) {
			for (int y = 0; y < yGrid; y++) {
				tiles[x, y].GetComponent<Gem>().ClearAllMatches();
			}
		}
	}

	private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f) {
		IsShifting = true;
		List<SpriteRenderer>  renders = new List<SpriteRenderer>();
		int nullCount = 0;

		for (int y = yStart; y < yGrid; y++) {  // 1
			SpriteRenderer render = tiles[x, y].GetComponent<SpriteRenderer>();
			if (render.sprite == null) { // 2
				nullCount++;
			}
			renders.Add(render);
		}

		for (int i = 0; i < nullCount; i++) { // 3
			yield return new WaitForSeconds(shiftDelay);// 4
			for (int k = 0; k < renders.Count - 1; k++) { // 5
				renders[k].sprite = renders[k + 1].sprite;
				renders[k + 1].sprite = GetNewSprite(x, yGrid - 1);
			}
		}
		IsShifting = false;
	}

	private Sprite GetNewSprite(int x, int y) {
		List<Sprite> possibleCharacters = new List<Sprite>();
		possibleCharacters.AddRange(gemTypes);

		if (x > 0) {
			possibleCharacters.Remove(tiles[x - 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (x < xGrid - 1) {
			possibleCharacters.Remove(tiles[x + 1, y].GetComponent<SpriteRenderer>().sprite);
		}
		if (y > 0) {
			possibleCharacters.Remove(tiles[x, y - 1].GetComponent<SpriteRenderer>().sprite);
		}

		return possibleCharacters[Random.Range(0, possibleCharacters.Count)];
	}
}
