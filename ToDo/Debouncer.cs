﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ToDo
{
    public class Debouncer
    {
        private List<CancellationTokenSource> StepperCancelTokens = new List<CancellationTokenSource>();
        private readonly int MillisecondsToWait;

        public Debouncer(int millisecondsToWait = 300)
        {
            this.MillisecondsToWait = millisecondsToWait;
        }

        public void Debouce(Action func)
        {
            CancelAllStepperTokens(); // Cancel all api requests;
            var newTokenSrc = new CancellationTokenSource();
            StepperCancelTokens.Add(newTokenSrc);
            Task.Delay(MillisecondsToWait, newTokenSrc.Token).ContinueWith(task => // Create new request
            {
                if (!newTokenSrc.IsCancellationRequested) // if it hasn't been cancelled
                {
                    func(); // run
                    CancelAllStepperTokens(); // Cancel any that remain (there shouldn't be any)
                    StepperCancelTokens = new List<CancellationTokenSource>(); // set to new list
                }
            });
        }

        private void CancelAllStepperTokens()
        {
            foreach (var token in StepperCancelTokens)
            {
                if (!token.IsCancellationRequested)
                {
                    token.Cancel();
                }
            }
        }
    }
}
