using UnityEngine;
using System.Collections;
using AssemblyCSharp;
namespace AssemblyCSharp
{
    public class SizeChanger : MonoBehaviour
    {
        [SerializeField]
        private float sizeMultiplier = 1.2f;
        [SerializeField]
        private float increaseTime = 0.02f;
        [SerializeField]
        private float increaseHoldTime = 0.02f;
        [SerializeField]
        private float rotateSpeed = 1000;
        protected bool isChangingSize = false;
        protected bool isSpinning = false;
        Vector3 baseScale;
        Quaternion baseRotation;

        public virtual void Start()
        {
            baseScale = transform.localScale;
            transform.localRotation = new Quaternion();
            baseRotation = transform.localRotation;
        }

        protected void TryChangingSize()
        {
            if (!isChangingSize)
            {
                StartCoroutine("IncreaseSize");
                isChangingSize = true;
            }
            else
            {
                StopCoroutine("IncreaseSize");
                isChangingSize = true;
                transform.localScale = baseScale;
                StartCoroutine("IncreaseSize");
            }
        }

        private IEnumerator IncreaseSize()
        {
            Vector3 desiredScale = transform.localScale * sizeMultiplier;

            while (transform.localScale.magnitude < desiredScale.magnitude)
            {
                Vector3 step = (desiredScale - baseScale) * Time.deltaTime / increaseTime;
                transform.localScale += step;
                yield return 0;
            }
            transform.localScale = desiredScale;
            if (increaseHoldTime > 0)
            {
                yield return new WaitForSeconds(increaseHoldTime);
            }
            while (transform.localScale.magnitude > baseScale.magnitude)
            {
                Vector3 step = (desiredScale - baseScale) * Time.deltaTime / increaseTime;
                transform.localScale -= step;
                yield return 0;
            }
            transform.localScale = baseScale;
            isChangingSize = false;

        }

        protected void TrySpinning(float time)
        {
            if (!isSpinning)
            {
                StartCoroutine("Spin", time);
                isSpinning = true;
            }
            else
            {
                StopCoroutine("Spin");
                transform.rotation = baseRotation;
                isSpinning = true;
                StartCoroutine("Spin", time);
            }
        }

        private IEnumerator Spin(float time)
        {
            while (time > 0)
            {
                transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * rotateSpeed);
                time -= Time.deltaTime;
                yield return 0;
            }
            transform.localRotation = baseRotation;
        }

        protected void ReverseRotation()
        {
            rotateSpeed = -rotateSpeed;
        }
    }
}

