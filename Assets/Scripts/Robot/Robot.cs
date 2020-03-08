using UnityEngine;
using Robot.functionality;

namespace Robot
{
    public class Robot : MonoBehaviour
    {
        private Engine _engine;
        private DepthSensor _depthSensor;
        private Accelerometer _accelerometer;
        private BallGrapper _ballGrapper;
        private Torpedo _torpedo;

        public Engine Engine { get { return _engine; } }
        public DepthSensor DepthSensor { get { return _depthSensor; } }
        public Accelerometer Accelerometer { get { return _accelerometer; } }
        public BallGrapper BallGrapper { get { return _ballGrapper; } }
        public Torpedo Torpedo { get { return _torpedo; } }

        void Start()
        {
            _engine = GetComponentInChildren<Engine>();
            _depthSensor = GetComponentInChildren<DepthSensor>();
            _accelerometer = GetComponentInChildren<Accelerometer>();
            _ballGrapper = GetComponentInChildren<BallGrapper>();
            _torpedo = GetComponentInChildren<Torpedo>();

        }
    }
}