// -----------------------------------------------------------------------
// <copyright file="WallComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System.Collections;
    using UnityEngine;

    [RequireComponent(typeof(GridDataComp))]
    [DisallowMultipleComponent]
    public class WallComp : MonoBehaviour
    {
        private GridDataComp gridDataValue;

        public GridDataComp gridData
        {
            get
            {
                if (this.gridDataValue == null)
                {
                    this.gridDataValue = this.gameObject.GetComponent<GridDataComp>();
                }

                return this.gridDataValue;
            }
        }

        private void OnEnable()
        {
            this.SetRenererVisible(false);
        }

        [SerializeField]
        private Renderer wallRenderer;

        private Coroutine coroutine;

        public void SetRenererVisible(bool visible)
        {
            if (this.wallRenderer == null)
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
            var currentAlpha = this.wallRenderer.material.color.a;
            while (Mathf.Abs(currentAlpha - alpha) > 0.01f)
            {
                currentAlpha = Mathf.Lerp(currentAlpha, alpha, Time.deltaTime * 10);
                var color = this.wallRenderer.material.color;
                color.a = currentAlpha;
                this.wallRenderer.material.color = color;
                yield return null;
            }

            this.coroutine = null;
        }
    }
}
