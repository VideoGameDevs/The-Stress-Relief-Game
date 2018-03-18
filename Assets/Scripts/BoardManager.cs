using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour {

	public static BoardManager board;    
	public List<Sprite> gemTypes = new List<Sprite>();   
	public GameObject gem;      
	public int xSize, ySize;     

	private GameObject[,] tiles;     

	public bool IsShifting { get; set; }    

	void Start () {
		board = GetComponent<BoardManager>();    

		Vector2 offset = gem.GetComponent<SpriteRenderer>().bounds.size;
		CreateBoard(offset.x, offset.y);    
	}

	private void CreateBoard (float xOffset, float yOffset) {
		tiles = new GameObject[xSize, ySize];    

		float startX = transform.position.x;     
		float startY = transform.position.y;

		Sprite[] previousLeft = new Sprite[ySize];
		Sprite previousBelow = null;

		for (int x = 0; x < xSize; x++) {      
			for (int y = 0; y < ySize; y++) {
				GameObject newTile = Instantiate(gem, new Vector3(startX + (xOffset * x), startY + (yOffset * y), 0), gem.transform.rotation);
				tiles[x, y] = newTile;
				newTile.transform.parent = transform;

				List<Sprite> possibleCharacters = new List<Sprite>(); 
				possibleCharacters.AddRange(gemTypes); 

				possibleCharacters.Remove(previousLeft[y]); 
				possibleCharacters.Remove(previousBelow);

				Sprite newSprite = possibleCharacters[Random.Range(0, possibleCharacters.Count)];
				newTile.GetComponent<SpriteRenderer>().sprite = newSprite;

				previousLeft[y] = newSprite;
				previousBelow = newSprite;
			}
		}
	}
}
