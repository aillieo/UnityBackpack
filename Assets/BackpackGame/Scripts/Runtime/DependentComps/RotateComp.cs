// -----------------------------------------------------------------------
// <copyright file="RotateComp.cs" company="AillieoTech">
// Copyright (c) AillieoTech. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

namespace AillieoTech.Game
{
    using System;
    using System.Collections;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class RotateComp : MonoBehaviour
    {
        public event Action OnRotationIndexChanged;

        public event Action OnDidRotate;

        private Coroutine coroutine;
        private float targetRotation = 0f;
        private const float rotationSpeedFactor = 2.4f;

        private int currentRotationIndexValue;

        public int currentRotationIndex
        {
            get
            {
                return this.currentRotationIndexValue;
            }

            private set
            {
                if (this.currentRotationIndexValue != value)
                {
                    this.currentRotationIndexValue = value;
                    this.OnRotationIndexChanged?.Invoke();
                }
            }
        }

        public void Rotate()
        {
            if (this.coroutine != null)
            {
                return;
            }

            // 设置旋转相关参数
            var rotationIndex = GridUtils.AngleToRotationIndex(this.transform.eulerAngles.z);
            rotationIndex = rotationIndex + 1;
            rotationIndex = rotationIndex % 4;
            this.targetRotation = GridUtils.RotationIndexToAngle(rotationIndex);

            // 启动旋转协程
            this.coroutine = this.StartCoroutine(this.RotateCoroutine());
        }

        public void FixRotation()
        {
            if (this.coroutine != null)
            {
                return;
            }

            // 设置旋转相关参数
            var eulerAngles = GridUtils.SnapAngle(this.transform.eulerAngles);
            this.targetRotation = eulerAngles.z;

            // 启动旋转协程
            this.coroutine = this.StartCoroutine(this.RotateCoroutine());
        }

        private IEnumerator RotateCoroutine()
        {
            var startRotation = this.transform.eulerAngles.z;
            var rotateRatio = 0f;

            while (rotateRatio < 1f)
            {
                // 计算当前的旋转角度
                var currentRotation = Mathf.LerpAngle(startRotation, this.targetRotation, rotateRatio);
                this.transform.eulerAngles = new Vector3(0f, 0f, currentRotation);

                // 更新计时器
                rotateRatio += Time.deltaTime * rotationSpeedFactor;

                yield return null;
            }

            this.transform.eulerAngles = new Vector3(0f, 0f, this.targetRotation);

            this.coroutine = null;
            this.OnDidRotate?.Invoke();
        }
    }
}
