namespace GSGD1
{
	using System.Collections;
	using System.Collections.Generic;
	using TMPro;
	using UnityEngine;
	using UnityEngine.UI;

	public enum State
	{
		Available = 0,
		GhostVisible
	}

	public class TowerSlotController : MonoBehaviour
	{
		[SerializeField]
		private TowerSlot[] _towerSlots = null;

		[System.NonSerialized]
		private State _state = State.Available;

		[System.NonSerialized]
		private TowerDescription _currentTowerDescription = null;

		//[SerializeField]
		//private PlayerGhostHandler _playerGhostHandler;

		public PlayerPickerController PlayerPickerController
		{
			get
			{
				return LevelReferences.Instance.PlayerPickerController;
			}
		}

		private void OnEnable()
		{
			for (int i = 0, length = _towerSlots.Length; i < length; i++)
			{
				_towerSlots[i].OnTowerSlotClicked -= TowerSlotController_OnTowerSlotClicked;
				_towerSlots[i].OnTowerSlotClicked += TowerSlotController_OnTowerSlotClicked;
			}
		}

		private void OnDisable()
		{
			for (int i = 0, length = _towerSlots.Length; i < length; i++)
			{
				_towerSlots[i].OnTowerSlotClicked -= TowerSlotController_OnTowerSlotClicked;
			}
		}

		private void TowerSlotController_OnTowerSlotClicked(TowerSlot sender)
		{
			//if (sender.TowerDescription.WoodCost > ResourceManager.Instance.Wood)
			//{
			//	Debug.Log("You don't have enough wood!");
			//	return;
			//}
            if (ResourceManager.Instance.FoundationStoneCost > ResourceManager.Instance.Stone)
            {
                Debug.Log("You don't have enough stone for a foundation!");
                return;
            }

            if (_state == State.Available)
			{
				_currentTowerDescription = sender.TowerDescription;
				ChangeState(State.GhostVisible);
			}
		}

		private void Update()
		{
			if (_state == State.GhostVisible)
			{
				if (Input.GetMouseButtonDown(0) == true)
				{
					if (PlayerPickerController.TrySetGhostAsCellChild() == true)
					{
						//Lose resources related to cost.

						//float closestDistanceToWood = float.MaxValue;
						//                  float closestDistanceToStone = float.MaxValue;
						//                  Resource_Stockpile chosenWoodStockpile = null;
						//Resource_Stockpile chosenStoneStockpile = null;

						//foreach(Resource_Stockpile stockpile in ResourceManager.Instance.Stockpiles)
						//{
						//	if (stockpile.ResourceType == ResourceManager.ResourceType.Wood)
						//	{		
						//		float distance = Vector3.Distance(PlayerPickerController.Ghost.GetTransform().position, stockpile.transform.position);
						//		if (distance < closestDistanceToWood)
						//		{
						//			closestDistanceToWood = distance;
						//			chosenWoodStockpile = stockpile;
						//		}
						//	}

						//                      if (stockpile.ResourceType == ResourceManager.ResourceType.Stone)
						//                      {
						//                          float distance = Vector3.Distance(PlayerPickerController.Ghost.GetTransform().position, stockpile.transform.position);
						//                          if (distance < closestDistanceToWood)
						//                          {
						//                              closestDistanceToStone = distance;
						//                              chosenStoneStockpile = stockpile;
						//                          }
						//                      }
						//                  }
						//if (chosenWoodStockpile != null)
						//{
						//                      chosenWoodStockpile.GetMined(_currentTowerDescription.WoodCost);
						//                  }
						//else
						//{
						//	Debug.Log("No chosen wood stockpile!");
						//}
						//                  if (chosenStoneStockpile != null)
						//                  {
						//                      chosenStoneStockpile.GetMined(_currentTowerDescription.StoneCost);
						//                  }
						//                  else
						//                  {
						//                      Debug.Log("No chosen stone stockpile!");
						//                  }

						//ResourceManager.Instance.OnResourceUpdate(ResourceManager.ResourceType.Wood, -_currentTowerDescription.WoodCost, 0);
						//ResourceManager.Instance.OnResourceUpdate(ResourceManager.ResourceType.Stone, -_currentTowerDescription.StoneCost, 0);
						foreach (Resource_Stockpile stockpile in ResourceManager.Instance.Stockpiles)
						{
							if (stockpile.ResourceQuantity >= ResourceManager.Instance.FoundationStoneCost && stockpile.ResourceType == ResourceManager.ResourceType.Stone)
							{
								stockpile.GetMined(ResourceManager.Instance.FoundationStoneCost);
                                ChangeState(State.Available);
                                return;
							}
							else
							{
								Debug.Log("You don't have enough stone!");
							}
						}
                        ChangeState(State.Available);
					}
				}
				if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape) == true)
				{
					ChangeState(State.Available);
				}
			}
		}

		public void ChangeState(State newState)
		{
			switch (newState)
			{
				case State.Available:
				{
					PlayerPickerController.DestroyGhost();
					PlayerPickerController.Activate(false);
					_currentTowerDescription = null;
				}
				break;
				case State.GhostVisible:
				{
					//Creates the 'ghost' of the tower. Would be interesting in using a ghostPrefab for a more 'ghostly' look.
					PlayerPickerController.ActivateWithGhost(_currentTowerDescription.Instantiate());
					//_playerGhostHandler.ActivateWithGhost(_currentTowerDescription.Instantiate());

                }
					break;
				default:
					break;
			}
			_state = newState;
		}
	}
}
