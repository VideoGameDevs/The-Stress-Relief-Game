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
					previousSelected.Deselect();
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
			Debug.Log (hit.collider.gameObject.GetComponent<SpriteRenderer>().sprite);
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
}
