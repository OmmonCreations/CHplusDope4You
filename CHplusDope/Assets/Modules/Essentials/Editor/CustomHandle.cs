using System;
using UnityEditor;
using UnityEngine;

namespace Modules.Essentials.Editor
{
    public static class CustomHandle
    {
        // internal state for DragHandle()
        static int s_DragHandleHash = "DragHandleHash".GetHashCode();
        static Vector2 s_DragHandleMouseStart;
        static Vector2 s_DragHandleMouseCurrent;
        static Vector3 s_DragHandleWorldStart;
        static float s_DragHandleClickTime = 0;
        static int s_DragHandleClickID;
        static float s_DragHandleDoubleClickInterval = 0.5f;
        static bool s_DragHandleHasMoved;

        // externally accessible to get the ID of the most resently processed DragHandle
        public static int lastDragHandleID;

        public enum DragHandleResult
        {
            none = 0,

            LMBPress,
            LMBClick,
            LMBDoubleClick,
            LMBDrag,
            LMBRelease,

            RMBPress,
            RMBClick,
            RMBDoubleClick,
            RMBDrag,
            RMBRelease,
        };
        
        public static Vector3 DragHandle(Vector3 position, float handleSize, Handles.CapFunction capFunc, 
            Func<Vector2,Vector2,Vector3> screenToWorld, Color colorSelected, out DragHandleResult result)
        {
            var id = GUIUtility.GetControlID(s_DragHandleHash, FocusType.Passive);
            lastDragHandleID = id;

            var screenPosition = Handles.matrix.MultiplyPoint(position);
            var cachedMatrix = Handles.matrix;

            result = DragHandleResult.none;

            var eventType = Event.current.GetTypeForControl(id);
            switch (eventType)
            {
                case EventType.MouseDown:
                    if (HandleUtility.nearestControl == id && (Event.current.button == 0 || Event.current.button == 1))
                    {
                        GUIUtility.hotControl = id;
                        s_DragHandleMouseCurrent = s_DragHandleMouseStart = Event.current.mousePosition;
                        s_DragHandleWorldStart = position;
                        s_DragHandleHasMoved = false;
                        
                        Event.current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);

                        if (Event.current.button == 0)
                            result = DragHandleResult.LMBPress;
                        else if (Event.current.button == 1)
                            result = DragHandleResult.RMBPress;
                    }

                    break;

                case EventType.MouseUp:
                    if (GUIUtility.hotControl == id && (Event.current.button == 0 || Event.current.button == 1))
                    {
                        GUIUtility.hotControl = 0;
                        Event.current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(0);

                        if (Event.current.button == 0)
                            result = DragHandleResult.LMBRelease;
                        else if (Event.current.button == 1)
                            result = DragHandleResult.RMBRelease;

                        if (Event.current.mousePosition == s_DragHandleMouseStart)
                        {
                            var doubleClick = (s_DragHandleClickID == id) &&
                                              (Time.realtimeSinceStartup - s_DragHandleClickTime <
                                               s_DragHandleDoubleClickInterval);

                            s_DragHandleClickID = id;
                            s_DragHandleClickTime = Time.realtimeSinceStartup;

                            if (Event.current.button == 0)
                                result = doubleClick ? DragHandleResult.LMBDoubleClick : DragHandleResult.LMBClick;
                            else if (Event.current.button == 1)
                                result = doubleClick ? DragHandleResult.RMBDoubleClick : DragHandleResult.RMBClick;
                        }
                    }

                    break;

                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == id)
                    {
                        s_DragHandleMouseCurrent = Event.current.mousePosition;
                        position = screenToWorld(s_DragHandleMouseStart, s_DragHandleMouseCurrent);

                        if (Event.current.button == 0)
                            result = DragHandleResult.LMBDrag;
                        else if (Event.current.button == 1)
                            result = DragHandleResult.RMBDrag;

                        s_DragHandleHasMoved = true;

                        GUI.changed = true;
                        Event.current.Use();
                    }

                    break;

                case EventType.Repaint:
                    var currentColour = Handles.color;
                    if (id == GUIUtility.hotControl && s_DragHandleHasMoved)
                        Handles.color = colorSelected;

                    Handles.matrix = Matrix4x4.identity;
                    capFunc(id, screenPosition, Quaternion.identity, handleSize, eventType);
                    Handles.matrix = cachedMatrix;

                    Handles.color = currentColour;
                    break;

                case EventType.Layout:
                    Handles.matrix = Matrix4x4.identity;
                    HandleUtility.AddControl(id, HandleUtility.DistanceToCircle(screenPosition, handleSize));
                    Handles.matrix = cachedMatrix;
                    break;
            }

            return position;
        }
    }
}