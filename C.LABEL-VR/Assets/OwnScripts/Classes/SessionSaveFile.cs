using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SessionSaveFile
{
    public struct _labelsession
    {
        public static Dictionary<int, PointCloud> _pointClouds { get; set; }
        public static int _currentCLoud { get; set; }
    }

    public struct _ingameOptions
    {
        public static Util.MovementMode _movementMode { get; set; }
        public static float _fFreeFly_MaxSpeedTrans { get; set; }
        public static float _fFreeFly_AccelerationTrans { get; set; }
        public static float _fFreeFly_MaxSpeedRot { get; set; }
        public static float _fFreeFly_AccelerationRot { get; set; }
        public static float _fSicknessPrevention_TeleportDistance { get; set; }
        public static float _fSicknessPrevention_TurnAngle { get; set; }
        public static bool _bSicknessPrevention_TeleportWithBlink { get; set; }
        public static bool _bAttachOptionsToCamera { get; set; }
        public static bool _bDecreasePointsWhenMoving { get; set; }
    }

    public struct _exportMetaData
    {
        public struct Hdf5_DaimlerLidar
        {
            public static int _ID { get; set; }
            public static Tuple<int, int> _hdf5TableIndex { get; set; }
            public static float _distance { get; set; }
            public static float _intensity { get; set; }
            public static float _labelPropability { get; set; }
            public static int _pointValid { get; set; }
            public static Vector3 _position_Sensor { get; set; }
        }
    }
}
