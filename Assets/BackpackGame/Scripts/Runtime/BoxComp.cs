// -----------------------------------------------------------------------
// <copyright file="BoxComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(Collider2D))]
    [DisallowMultipleComponent]
    public class BoxComp : MonoBehaviour
    {
        [SerializeField]
        private Vector3 targetPosition;

        private float forceMagnitude = 9f;
        private float overlapThreshold = 0.8f;

        private void OnCollisionEnter2D(Collision2D other)
        {
        }

        private void OnCollisionStay2D(Collision2D other)
        {
            // 施加力 将物体移动到target的位置
            if (other.gameObject.TryGetComponent(out Rigidbody2D rb))
            {
                var overlap = -other.GetContact(0).separation;
                if (overlap > this.overlapThreshold)
                {
                    var force = (this.targetPosition - other.transform.position).normalized * this.forceMagnitude;
                    rb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }
    }
}
