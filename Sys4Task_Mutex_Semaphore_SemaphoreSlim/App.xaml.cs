using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Sys4Task_Mutex_Semaphore_SemaphoreSlim
{
    public partial class App : Application
    {
        private Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            string appName = "Sys4Task";
            bool createdNew;

            _mutex = new Mutex(true, appName, out createdNew);

            if (!createdNew)
            {
                MessageBox.Show("App is already running", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                App.Current.Shutdown();
            }

            base.OnStartup(e);
        }
    }
}
