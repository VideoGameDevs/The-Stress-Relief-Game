using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);	//a dark filter color to use when a gem is selected
	private static Gem previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

	//need this for checking neighbours
	private Vector2[] adjacentDirections = new Vector2[] { Vector2.up, Vector2.down, Vector2.left, Vector2.right };
	private bool matchFound = false;

	void Awake() {
		render = GetComponent<SpriteRenderer>();
	}

	private void Select() {
		isSelected = true;
		render.color = selectedColor;
		previousSelected = gameObject.GetComponent<Gem>();
	}

	private void Deselect() {
		isSelected = false;
		render.color = Color.white;
		previousSelected = null;
	}

	void OnMouseDown() {
		//if there is no sprite in the gem or when the board is shifting do nothing
		if (render.sprite == null || BoardManager.board.IsShifting) {
			return;
		}
			
		if (isSelected) { 		//if you select an already selected gem, deselect
			Deselect();
		} else {
			//else if you don't have a previously selected gem and selected a new gem, the gem gets selected
			if (previousSelected == null) {  	
				Select();
			} else {
				//else if you have a previously selected gem and it is a neighbour of the currently selected gem, they swap. 
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject)) {
					//they swap their sprite not the gameObject but looks like they are actually swapping!
					SwapSprite(previousSelected.render); 
					previousSelected.ClearAllMatches();
					previousSelected.Deselect();
					ClearAllMatches();
				} else { 
					//else if you have a previously selected gem but select non neighbour gem, 
					//previous gem gets deselected and the current one gets selected
					previousSelected.GetComponent<Gem>().Deselect();
					Select();
				}
			}
		}
	}

	public void SwapSprite(SpriteRenderer render2) {
		if (render.sprite == render2.sprite) { 
			return;
		}

		Sprite tempSprite = render2.sprite;
		render2.sprite = render.sprite; 
		render.sprite = tempSprite; 
	}

	private GameObject GetAdjacent(Vector2 castDir) {
		//check the documentation of Physics2D.Raycast to understand this function
		RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + (castDir * this.gameObject.GetComponent<BoxCollider2D>().size.x), castDir);
		if (hit.collider != null) {
			return hit.collider.gameObject;
		}
		return null;
	}
		
	private List<GameObject> GetAllAdjacentTiles() {
		List<GameObject> adjacentTiles = new List<GameObject>();
		for (int i = 0; i < adjacentDirections.Length; i++) {
			adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
		}
		return adjacentTiles;
	}

	private List<GameObject> FindMatch(Vector2 castDir) { 
		List<GameObject> matchingTiles = new List<GameObject>(); 
		RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position + (castDir * this.gameObject.GetComponent<BoxCollider2D>().size.x), castDir);
		while (hit.collider != null && hit.collider.GetComponent<SpriteRenderer>().sprite == render.sprite) {
			matchingTiles.Add(hit.collider.gameObject);
			hit = Physics2D.Raycast((Vector2)hit.collider.transform.position + (castDir * hit.collider.gameObject.GetComponent<BoxCollider2D>().size.x), castDir);
		}
		return matchingTiles; 
	}

	private void ClearMatch(Vector2[] paths) // 1
	{
		List<GameObject> matchingTiles = new List<GameObject>(); // 2
		for (int i = 0; i < paths.Length; i++) // 3
		{
			matchingTiles.AddRange(FindMatch(paths[i]));
		}
		if (matchingTiles.Count >= 2) // 4
		{
			for (int i = 0; i < matchingTiles.Count; i++) // 5
			{
				matchingTiles[i].GetComponent<SpriteRenderer>().sprite = null;
			}
			matchFound = true; // 6
		}
	}

	public void ClearAllMatches() {
		if (render.sprite == null)
			return;

		ClearMatch(new Vector2[2] { Vector2.left, Vector2.right });
		ClearMatch(new Vector2[2] { Vector2.up, Vector2.down });
		if (matchFound) {
			render.sprite = null;
			matchFound = false;
			StopCoroutine(BoardManager.board.FindNullTiles());
			StartCoroutine(BoardManager.board.FindNullTiles());
		}
	}
}
