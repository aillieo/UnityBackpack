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

        private float forceMagnitude = 10f;
        private float overlapThreshold = 0.8f;

        private void OnCollisionEnter2D(Collision2D other)
        {
        }

        private ContactPoint2D[] contacts = new ContactPoint2D[10];

        private void OnCollisionStay2D(Collision2D other)
        {
            // 施加力 将物体移动到target的位置
            if (other.gameObject.TryGetComponent(out Rigidbody2D rb))
            {
                var overlapMax = float.MinValue;
                var count = other.GetContacts(this.contacts);
                for (var i = 0; i < count; i++)
                {
                    var overlap = -this.contacts[i].separation;
                    if (overlap > overlapMax)
                    {
                        overlapMax = overlap;
                    }
                }

                if (overlapMax > this.overlapThreshold)
                {
                    var gravity = Physics2D.gravity;
                    Vector2 direction = (this.targetPosition - other.transform.position).normalized;
                    var directionForce = direction * this.forceMagnitude;
                    var force = (-gravity + directionForce) * rb.mass;
                    rb.AddForce(force, ForceMode2D.Impulse);
                }
            }
        }
    }
}
