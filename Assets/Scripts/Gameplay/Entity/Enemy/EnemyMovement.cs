using System;
using System.Collections;
using Pathfinding;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay.Entity.Enemy
{
    public class EnemyMovement : MonoBehaviour
    {
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Transform _destination;
        [SerializeField] private Movable _movable;
        [SerializeField] private float _stopDistance;
        [SerializeField] private float _maxSpeed;

        private void Awake()
        {
            
        }

        private IEnumerator Traversed(RichSpecial arg)
        {
            transform.position = arg.second.position;
            yield break;
        }

        private void FixedUpdate()
        {
            _agent.destination = _destination.position;
            Move();
        }

        private void Move()
        {
            Vector3 direction = _agent.desiredVelocity;
            if (_agent.remainingDistance < _stopDistance)
                direction = Vector3.zero;
            _movable.SetHorizontalVelocity(direction * _maxSpeed);
        }
    }
}