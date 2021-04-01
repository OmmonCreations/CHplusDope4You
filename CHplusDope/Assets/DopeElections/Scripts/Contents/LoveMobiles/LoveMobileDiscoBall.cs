using UnityEngine;

namespace DopeElections.LoveMobiles
{
    public class LoveMobileDiscoBall : MonoBehaviour
    {
        [SerializeField] private float _rotationSpeed = 10;
        [SerializeField] private Transform _transform = null;
        
        private void Update()
        {
            _transform.localEulerAngles += new Vector3(0, 0, _rotationSpeed * Time.deltaTime);
        }
    }
}