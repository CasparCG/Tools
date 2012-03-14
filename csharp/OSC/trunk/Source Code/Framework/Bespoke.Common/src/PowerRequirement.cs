using System;
using System.Runtime.InteropServices;

namespace Bespoke.WindowsMobile
{
#if PocketPC
    /// <summary>
    /// Power states.
    /// </summary>
    public enum PowerRequirementState
    {
        /// <summary>
        /// The system state is unspecified. 
        /// </summary>
        Unspecified = -1,

        /// <summary>
        /// Full On. This is the state in which the device is on and running. It is receiving full power from the system and is delivering full functionality to the user.
        /// </summary>
        D0 = 0,

        /// <summary>
        /// Low On. This is the state in which the device is fully functional at a lower power state than D0, a lower performance state than D0, or both. This state is applicable when the device is being used, but where peak performance is unnecessary and power is at a premium.
        /// </summary>
        /// 
        D1 = 1,

        /// <summary>
        /// Standby. This is the state in which the device is partially powered with automatic wakeup on request. The device is effectively standing by.
        /// </summary>
        /// 
        D2 = 2,

        /// <summary>
        /// Sleep. This is the state in which the device is partially powered with device-initiated wakeup if available. A device in state D3 is sleeping but capable of raising the System Power State on its own. It consumes only enough power to be able to do so; which must be less than or equal to the amount of power used in state D2.
        /// </summary>
        D3 = 3,

        /// <summary>
        /// Off. This is the state in which the device is not powered. A device in state D4 should not be consuming any significant power. Some peripheral busses require static terminations that intrinsically use non-zero power when a device is physically connected to the bus; a device on such a bus can still support D4.
        /// </summary>
        D4 = 4
    }

    /// <summary>
    /// 
    /// </summary>
    public class PowerRequirement : IDisposable
    {
        private static class CoreDll
        {
            [DllImport("coredll.dll")]
            internal static extern IntPtr SetPowerRequirement(String pvDevice, PowerRequirementState deviceState, Int32 DeviceFlags, IntPtr pvSystemState, Int32 StateFlags);

            [DllImport("coredll.dll")]
            internal static extern Int32 ReleasePowerRequirement(IntPtr hPowerReq);

            internal const Int32 SUCCESS = 0;
            internal const Int32 POWER_NAME = 1;
            internal const Int32 POWER_FORCE = 2;
        }

        /// <summary>
        /// Gets whether power requirement is set.
        /// </summary>
        public bool IsSet
        {
            get
            {
                return (mHandle != IntPtr.Zero);
            }
        }

        /// <summary>
        /// Creates new instance.
        /// </summary>
        public PowerRequirement(string deviceName)
        {
            mDeviceName = deviceName;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~PowerRequirement()
        {
            Dispose(false);
        }

        /// <summary>
        /// Sets power requirement.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Device already open. -or- Cannot open device.</exception>
        public void Set(PowerRequirementState powerState)
        {
            if (mHandle != IntPtr.Zero)
            {
                throw new System.InvalidOperationException("Device already open.");
            }

            mHandle = CoreDll.SetPowerRequirement(mDeviceName, powerState, CoreDll.POWER_NAME, IntPtr.Zero, 0);
            if(mHandle == IntPtr.Zero)
            {
                throw new System.InvalidOperationException("Cannot open device.");
            }
        }

        /// <summary>
        /// Releases power requirement.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Cannot close device.</exception>
        public void Release()
        {
            if (mHandle != IntPtr.Zero)
            {
                if (CoreDll.ReleasePowerRequirement(mHandle) != CoreDll.SUCCESS)
                {
                    throw new InvalidOperationException("Cannot close device.");
                }

                mHandle = IntPtr.Zero;
            }
        }

        #region IDisposable Members

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">True if managed resources should be disposed; otherwise, false.</param>
        protected virtual void Dispose(bool disposing)
        {
            Release();
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion

        /// <summary>
        /// First audio device.
        /// </summary>
        public static readonly string AudioDevice = "WAV1:";

        /// <summary>
        /// First backlight device.
        /// </summary>
        public static readonly string BacklightDevice = "BKL1:";

        /// <summary>
        /// First serial port device.
        /// </summary>
        public static readonly string SerialPortDevice = "COM1:";

        private IntPtr mHandle;
        private string mDeviceName;
    }
#endif
}
