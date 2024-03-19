// -----------------------------------------------------------------------
// <copyright file="Physics2DComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using UnityEngine;

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(Collider2D))]
    [DisallowMultipleComponent]
    public class Physics2DComp : MonoBehaviour
    {
        private Collider2D colliderValue;

        private new Collider2D collider
        {
            get
            {
                if (colliderValue == null)
                {
                    colliderValue = this.gameObject.GetComponent<Collider2D>();
                }

                return colliderValue;
            }
        }

        private Rigidbody2D rigidbodyValue;

        private new Rigidbody2D rigidbody
        {
            get
            {
                if (rigidbodyValue == null)
                {
                    rigidbodyValue = this.gameObject.GetComponent<Rigidbody2D>();
                }

                return rigidbodyValue;
            }
        }

        public void SwitchSimulation(bool isOn)
        {
            UnityEngine.Debug.Log("SwitchSimulation " + this.name + " " + isOn);

            this.rigidbody.velocity = Vector2.zero;
            this.rigidbody.angularVelocity = 0f;

            if (isOn)
            {
                this.rigidbody.bodyType = RigidbodyType2D.Dynamic;
                this.collider.isTrigger = false;
            }
            else
            {
                this.rigidbody.bodyType = RigidbodyType2D.Kinematic;
                this.collider.isTrigger = true;
            }
        }
    }
}
