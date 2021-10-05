using UnityEngine;

namespace utils
{
    public class CommonUtil
    {
        public static bool CheckPointInColliderArea(Vector3 point, Collider2D collider)
        {
            var colliderPosition = collider.transform.position;
            var halfWidth = collider.GetComponent<SpriteRenderer>().size.x / 2f;
            if(point.x > (colliderPosition.x - halfWidth) &&
               point.x < (colliderPosition.x + halfWidth))
                Debug.Log(halfWidth);
            return point.x > (colliderPosition.x - halfWidth) &&
                   point.x < (colliderPosition.x + halfWidth);
        }
    }
}