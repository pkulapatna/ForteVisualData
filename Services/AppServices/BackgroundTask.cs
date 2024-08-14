using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AppServices
{
    public  class BackgroundTask
    {

        private  Task? _timerTask;
        private readonly PeriodicTimer _timer;
        private readonly CancellationTokenSource _cts = new();


        public BackgroundTask(TimeSpan interval)
        {
            _timer =  new(interval);
        }

        public void Start() 
        {
            _timerTask = DoWorkAsync();
        }

        private async Task DoWorkAsync()
        {

            try
            {

                while(await _timer.WaitForNextTickAsync(_cts.Token)) 
                { 

                }


            }
            catch (OperationCanceledException)
            {
               
            }
        }

        public async Task StopAsync() 
        {
            if( _timer is null ) 
            {
                return;
            }

            _cts?.Cancel();
            await _timerTask;
            _cts?.Dispose();
        }
    }
}
