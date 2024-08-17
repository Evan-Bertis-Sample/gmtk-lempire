using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Curly.EntityGrid;
using Curly.InputWrapper;
using Curly.Turns;

namespace Curly.Player
{
    [RequireComponent(typeof(GridEntityComponent))]   
    public class PlayerController : MonoBehaviour
    {
        private GridEntityComponent _entity;
        private Vector2Int _targetDirection;

        private void Start()
        {
            _entity = GetComponent<GridEntityComponent>();
            RegisterTurnEvents();
        }

        private void Update()
        {
            // listen for input
            Vector2 move = InputMapping.Default.Movement.GetAxis();
            _targetDirection = new Vector2Int((int)move.x, (int)move.y);
        }

        private void RegisterTurnEvents()
        {
            TurnManager.Instance.RegisterTurnEvent("PlayerMove", 1, 0, MovePlayer);
        }

        private void MovePlayer()
        {
            Debug.Log("Moving player");
            Vector2Int newPosition = _entity.GridPosition + _targetDirection;
            _entity.GridComponent.MoveEntity(_entity.GridEntity, newPosition);
        }

    }
}