namespace AillieoTech.Game
{
    using System;
    using System.Collections;
    using UnityEngine;

    [DisallowMultipleComponent]
    public class RotateComp : MonoBehaviour
    {
        public event Action OnRotationIndexChanged;

        private bool isRotating = false;
        private float targetRotation = 0f;
        private float rotationSpeedFactor = 2.4f;

        private int currentRotationIndexValue;

        public int currentRotationIndex
        {
            get { return currentRotationIndexValue; }

            set
            {
                if (currentRotationIndexValue != value)
                {
                    currentRotationIndexValue = value;
                    OnRotationIndexChanged?.Invoke();
                }
            }
        }

        public void Rotate()
        {
            if (isRotating)
            {
                return;
            }

            // 设置旋转相关参数
            var rotationIndex = GridUtils.AngleToRotationIndex(this.transform.localEulerAngles.z);
            rotationIndex = rotationIndex + 1;
            rotationIndex = rotationIndex % 4;
            targetRotation = GridUtils.RotationIndexToAngle(rotationIndex);

            // 启动旋转协程
            StartCoroutine(RotateCoroutine());
        }

        public void FixRotation()
        {
            if (isRotating)
            {
                return;
            }

            // 设置旋转相关参数
            var eulerAngles = GridUtils.SnapAngle(this.transform.localEulerAngles);
            targetRotation = eulerAngles.z;

            // 启动旋转协程
            StartCoroutine(RotateCoroutine());
        }

        private IEnumerator RotateCoroutine()
        {
            isRotating = true;
            float startRotation = transform.localEulerAngles.z;
            float rotateRatio = 0f;

            while (rotateRatio < 1f)
            {
                // 计算当前的旋转角度
                float currentRotation = Mathf.LerpAngle(startRotation, targetRotation, rotateRatio);
                transform.localEulerAngles = new Vector3(0f, 0f, currentRotation);

                this.currentRotationIndex = GridUtils.AngleToRotationIndex(currentRotation);

                // 更新计时器
                rotateRatio += Time.deltaTime * rotationSpeedFactor;

                yield return null;
            }

            transform.localEulerAngles = new Vector3(0f, 0f, targetRotation);

            // 完成旋转后调用回调方法（如果有）
            this.isRotating = false;
            this.OnDidRotate();
        }

        private void OnDidRotate()
        {
        }
    }
}
