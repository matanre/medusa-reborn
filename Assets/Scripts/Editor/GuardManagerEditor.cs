//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor;

//[CustomEditor(typeof(FieldOfView))]
//public class GuardManagerEditor : Editor
//{
//    private void OnSceneGUI()
//    {
//        FieldOfView _guard = (FieldOfView)target;

//        Color c = Color.green;
//        if (_guard.alertStage == AlertStage.Intrigued)
//            c = Color.Lerp(Color.green, Color.red, _guard.alertLevel / 100f);
//        else if (_guard.alertStage == AlertStage.Alerted)
//            c = Color.red;

//        Handles.color = new Color(c.r, c.g, c.b, 0.3f);
//        Handles.DrawSolidArc(
//            _guard.transform.position,
//            _guard.transform.up,
//            Quaternion.AngleAxis(-_guard.angle / 2f, _guard.transform.up) * _guard.transform.forward,
//            _guard.angle,
//            _guard.radius);

//        Handles.color = c;
//        _guard.radius = Handles.ScaleValueHandle(
//            _guard.radius,
//            _guard.transform.position + _guard.transform.forward * _guard.radius,
//            _guard.transform.rotation,
//            3,
//            Handles.SphereHandleCap,
//            1);
//    }

//}
