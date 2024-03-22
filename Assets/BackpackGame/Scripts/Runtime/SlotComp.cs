// -----------------------------------------------------------------------
// <copyright file="SlotComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(DroppableComp))]
    [DisallowMultipleComponent]
    public class SlotComp : MonoBehaviour
    {
        private DroppableComp droppableCompValue;

        private DroppableComp droppableComp
        {
            get
            {
                if (this.droppableCompValue == null)
                {
                    this.droppableCompValue = this.gameObject.GetComponent<DroppableComp>();
                }

                return this.droppableCompValue;
            }
        }

        private void OnEnable()
        {
            this.droppableComp.OnDrop += this.OnDrop;
            BackpackManager.Instance.OnChangeSlotVisibilityRequested += this.SetRenererVisible;

            this.SetRenererVisible(false);
        }

        private void OnDisable()
        {
            this.droppableComp.OnDrop -= this.OnDrop;
            BackpackManager.Instance.OnChangeSlotVisibilityRequested -= this.SetRenererVisible;
        }

        private void OnDrop(MouseEventData eventData)
        {
        }

        [SerializeField]
        private Renderer slotRenderer;

        private Coroutine coroutine;

        public void SetRenererVisible(bool visible)
        {
            if (this.slotRenderer == null)
            {
                return;
            }

            if (this.coroutine != null)
            {
                this.StopCoroutine(this.coroutine);
                this.coroutine = null;
            }

            this.coroutine = this.StartCoroutine(this.SetRendererAlpha(visible ? 1 : 0));
        }

        private IEnumerator SetRendererAlpha(float alpha)
        {
            var currentAlpha = this.slotRenderer.material.color.a;
            while (Mathf.Abs(currentAlpha - alpha) > 0.01f)
            {
                currentAlpha = Mathf.Lerp(currentAlpha, alpha, Time.deltaTime * 10);
                var color = this.slotRenderer.material.color;
                color.a = currentAlpha;
                this.slotRenderer.material.color = color;
                yield return null;
            }

            this.coroutine = null;
        }
    }
}
