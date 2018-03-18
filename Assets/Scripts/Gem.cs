using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour {

	private static Color selectedColor = new Color(.5f, .5f, .5f, 1.0f);
	private static Gem previousSelected = null;

	private SpriteRenderer render;
	private bool isSelected = false;

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
		if (render.sprite == null || BoardManager.board.IsShifting) {
			return;
		}

		if (isSelected) { 
			Deselect();
		} else {
			if (previousSelected == null) { 
				Select();
			} else {
				if (GetAllAdjacentTiles().Contains(previousSelected.gameObject)) { 
					Debug.Log("1");
					SwapSprite(previousSelected.render); 
					previousSelected.Deselect();
				} else { 
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
		RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
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
