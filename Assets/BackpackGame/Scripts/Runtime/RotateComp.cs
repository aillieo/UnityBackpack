namespace AillieoTech.Game
{
    using UnityEngine;

    public class RotateComp : MonoBehaviour
    {
        private bool isRotating = false;
        private float targetRotation = 0f;
        private float rotationSpeed = 90f;

        public void Rotate()
        {
            if (isRotating)
            {
                return;
            }

            // 设置旋转相关参数
            targetRotation = transform.rotation.eulerAngles.z + 90f;

            // 启动旋转协程
            StartCoroutine(RotateCoroutine());
        }

        private System.Collections.IEnumerator RotateCoroutine()
        {
            isRotating = true;
            float startRotation = transform.rotation.eulerAngles.z;
            float timer = 0f;

            while (timer < 1f)
            {
                // 计算当前的旋转角度
                float currentRotation = Mathf.Lerp(startRotation, targetRotation, timer);
                transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);

                // 更新计时器
                timer += Time.deltaTime * rotationSpeed;

                yield return null;
            }

            // 完成旋转后调用回调方法（如果有）
            this.isRotating = false;
            this.OnDidRotate();
        }

        private void OnDidRotate()
        {
        }
    }
}
