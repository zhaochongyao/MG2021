// using ScriptableObjects;
// using UnityEngine;
//
// namespace MiniInteraction
// {
//     public class StraightDragGizmos : MonoBehaviour
//     {
//         [SerializeField] private RectTransform _dragRectTrans;
//
//         [SerializeField] private StraightDrag _straightDrag;
//
//         [SerializeField] private GizmosColorSettingSO _gizmosColorSetting;
//         
//         private void OnDrawGizmos()
//         {
//             Gizmos.color = _gizmosColorSetting._straightDragColor;
//             Vector3 origin = _dragRectTrans.position;
//             float dragAmount = 5;
//             Vector3 to = _straightDrag.DragTowards switch
//             {
//                 StraightDrag.DragDirection.Up => origin + new Vector3(0f, dragAmount, 0f),
//                 StraightDrag.DragDirection.Down => origin + new Vector3(0f, -dragAmount, 0f),
//                 StraightDrag.DragDirection.Left => origin + new Vector3(-dragAmount, 0f, 0f),
//                 StraightDrag.DragDirection.Right => origin + new Vector3(dragAmount, 0f, 0f),
//                 _ => new Vector3()
//             };
//             Gizmos.DrawLine(origin, to);
//         }
//     }
// }
