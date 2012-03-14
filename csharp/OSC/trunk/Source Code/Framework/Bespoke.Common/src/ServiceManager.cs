using System;
using System.ServiceProcess;

namespace Bespoke.Common
{
    public static class ServiceManager
    {
        public static bool IsServiceRunning(string serviceName)
        {
            bool isServiceRunning;

            try
            {

                ServiceController service = new ServiceController(serviceName);
                isServiceRunning = (service.Status == ServiceControllerStatus.Running);
            }
            catch
            {
                isServiceRunning = false;
            }

            return isServiceRunning;
        }

        public static bool StartService(string serviceName)
        {
            return StartService(serviceName, DefaultTimeout);
        }

        public static bool StartService(string serviceName, int timeoutMilliseconds)
        {
            bool serviceStarted;

            try
            {
                ServiceController service = new ServiceController(serviceName);
                if (service.Status == ServiceControllerStatus.Stopped)
                {
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                    serviceStarted = true;
                }
                else
                {
                    serviceStarted = false;
                }
            }
            catch
            {
                serviceStarted = false;
            }

            return serviceStarted;
        }

        public static bool StopService(string serviceName)
        {
            return StopService(serviceName, DefaultTimeout);
        }

        public static bool StopService(string serviceName, int timeoutMilliseconds)
        {
            bool serviceStopped;

            ServiceController service = new ServiceController(serviceName);
            if (service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                serviceStopped = true;
            }
            else
            {
                serviceStopped = false;
            }

            return serviceStopped;
        }

        public static bool RestartService(string serviceName)
        {
            return RestartService(serviceName, DefaultTimeout);
        }

        public static bool RestartService(string serviceName, int timeoutMilliseconds)
        {
            bool serviceRestarted;

            ServiceController service = new ServiceController(serviceName);
            if (service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(timeoutMilliseconds));
            }

            if (service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(timeoutMilliseconds));
                serviceRestarted = true;
            }
            else
            {
                serviceRestarted = false;
            }

            return serviceRestarted;
        }

        public static readonly int DefaultTimeout = 5000;
    }
}
