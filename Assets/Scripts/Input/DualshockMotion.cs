using UnityEngine;

namespace Input
{
    public static class DualshockMotion
    {
        private static float xGyroInfluence;
        private static bool invertZ;

        public static bool IsUsed { get; set; }

        /// <summary>
        /// Uncalibrated gyro euler axes
        /// </summary>
        public static Vector3 RawGyro { get; private set; }

        public static Vector3 Accelerometer { get; private set; }

        public static void ReportJoyshock(int handle, JSL.JOY_SHOCK_STATE state, JSL.JOY_SHOCK_STATE laststate, JSL.IMU_STATE imustate, JSL.IMU_STATE lastimustate, float deltatime)
        {
            xGyroInfluence = imustate.accelY;
            invertZ = imustate.accelZ > 0;
            RawGyro = new Vector3(imustate.gyroX, imustate.gyroY, imustate.gyroZ) * deltatime;
            Accelerometer = new Vector3(imustate.accelX, imustate.accelY, imustate.accelZ);
        }
    }
}