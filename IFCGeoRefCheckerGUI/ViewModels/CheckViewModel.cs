﻿using IFCGeoRefCheckerGUI.Messaging;
using IFCGeorefShared;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

using Xbim.Ifc;
using Serilog;

namespace IFCGeoRefCheckerGUI.ViewModels
{
    public class CheckViewModel : BaseViewModel
    {
        IEventAggregator eventAggregator;
        List<Subscription>? eventSubscriptions;
        ITranslator translator = new ResxConverter();

        private GeoRefCheckerResult checkerResults;
        public GeoRefCheckerResult CheckerResults
        {
            get => checkerResults;
            set
            {
                if (checkerResults != value)
                {
                    checkerResults = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private string? selectedPath;
        public string? SelectedPath
        {
            get => selectedPath;
            set
            {
                if (selectedPath != value)
                {
                    selectedPath = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private string? workingDir;

        public string? WorkingDir
        {
            get => workingDir;
            set
            {
                if (workingDir != value)
                {
                    workingDir = value;
                    this.RaisePropertyChanged();
                }
            }
        }
        
        private bool isChecking { get; set; }
        public bool IsChecking
        {
            get => isChecking;
            set
            {
                if (isChecking != value)
                {
                    isChecking = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private IDictionary<string, GeoRefChecker> checkerDict = new Dictionary<string, GeoRefChecker>();
        public IDictionary<string, GeoRefChecker> CheckerDict
        {
            get => checkerDict;
            set
            {
                if (checkerDict != value)
                {
                    checkerDict = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private int nrOfChecks;

        public int NrOfChecks
        {
            get => nrOfChecks;
            set
            {
                if (nrOfChecks != value)
                {
                    nrOfChecks = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        public DelegateCommand CheckIFC { get; set; }
        public DelegateCommand ShowLog { get; set; }

        public event EventHandler? NoFileSelected;
        public event EventHandler? NoWorkingDirSelected;
        public event EventHandler? FileNotYetChecked;

        public CheckViewModel(IEventAggregator eventAggregator) 
        {
            this.eventAggregator = eventAggregator;

            this.eventSubscriptions = new List<Subscription>
            {
                eventAggregator.Subscribe<SelectedPathMessageObject>(spmo => NewPathReceived(spmo.SelectedPath)),
                eventAggregator.Subscribe<SelectedWorkingDirMessageObject>(swdmo => NewWorkingDirReceived(swdmo.WorkingDir))
            };
            //this.eventSubscriptions = eventAggregator.Subscribe<SelectedPathMessageObject>(spmo => NewPathReceived(spmo.SelectedPath));

            this.checkerResults = new GeoRefCheckerResult();
            this.CheckIFC = new DelegateCommand((o) => ExecCheck(o));
            this.ShowLog = new DelegateCommand((o) => ExecShowLog(o));
        }

        private void NewPathReceived(string? selectedPath)
        {
            this.selectedPath = selectedPath;

            if (!String.IsNullOrEmpty(selectedPath) && this.checkerDict.ContainsKey(selectedPath))
            {
                this.CheckerResults = this.checkerDict[selectedPath].getCheckResults();
            }
            else
            {
                this.CheckerResults = new GeoRefCheckerResult();
            }
        }

        private void NewWorkingDirReceived(string? workingDir)
        {
            this.WorkingDir = workingDir;
        }

        private async void ExecCheck(object o)
        {
                await IfcCheckService();
        }
        
        private async Task IfcCheckService()
        {
            var task = Task.Run(() =>
            {
                if (String.IsNullOrEmpty(selectedPath))
                {
                    NoFileSelected?.Invoke(this, EventArgs.Empty);
                }
                else if (String.IsNullOrEmpty(WorkingDir) || !System.IO.Directory.Exists(workingDir))
                {
                    NoWorkingDirSelected?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    this.IsChecking = true;
                    Log.Information($"Starting to check {selectedPath}");
                    using (var model = IfcStore.Open(selectedPath))
                    {
                        var checker = new GeoRefChecker(model, translator);
                        CheckerResults = checker.getCheckResults();
                        if (CheckerDict.ContainsKey(selectedPath))
                        {
                            CheckerDict[selectedPath] = checker;
                        }
                        else
                        {
                            CheckerDict.Add(selectedPath, checker);
                            this.NrOfChecks = CheckerDict.Count;
                        }
                        Log.Information($"Writing check protocol to {WorkingDir}");
                        checker.WriteProtocoll(WorkingDir!);
                    }
                    this.IsChecking = false;
                    Log.Information($"Finished checking {selectedPath}");
                }
            });
            await task;
        }

        private void ExecShowLog(object o)
        {
            if (String.IsNullOrEmpty(SelectedPath))
            {
                NoFileSelected?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                if (this.checkerDict.ContainsKey(this.SelectedPath))
                {
                    var fileName = this.checkerDict[this.SelectedPath].ProtocollPath;
                    if (!String.IsNullOrEmpty(fileName))
                    {
                        Process p = new Process();
                        p.StartInfo.UseShellExecute = true;
                        p.StartInfo.FileName =fileName;
                        p.Start();
                    }   
                }
                else
                {
                    FileNotYetChecked?.Invoke(this, EventArgs.Empty);
                }
            }
        }

    }
}
