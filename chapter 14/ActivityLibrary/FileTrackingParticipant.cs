//--------------------------------------------------------------------------------
// This file is part of the downloadable code for the Apress book:
// Pro WF: Windows Workflow in .NET 4.0
// Copyright (c) Bruce Bukovics.  All rights reserved.
//
// This code is provided as is without warranty of any kind, either expressed
// or implied, including but not limited to fitness for any particular purpose. 
// You may use the code for any commercial or noncommercial purpose, and combine 
// it with your own code, but cannot reproduce it in whole or in part for 
// publication purposes without prior approval. 
//--------------------------------------------------------------------------------      
using System;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;

namespace ActivityLibrary
{
    public class FileTrackingParticipant : TrackingParticipant
    {
        private Queue<TrackingRecord> _records = new Queue<TrackingRecord>();
        private AutoResetEvent _recordsToProcess = new AutoResetEvent(false);
        private Thread _processingThread;
        private Boolean _isThreadRunning;

        public FileTrackingParticipant()
        {
            _processingThread = new Thread(ProcessingThreadProc);
            _isThreadRunning = true;
            _processingThread.Start();
        }

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            lock (_records)
            {
                _records.Enqueue(record);
            }
            _recordsToProcess.Set();
        }

        public void Stop()
        {
            _isThreadRunning = false;
            _processingThread.Join(5000);
        }

        private void ProcessingThreadProc()
        {
            while (_isThreadRunning)
            {
                if (_recordsToProcess.WaitOne(2000))
                {
                    Int32 count = 0;
                    lock (_records)
                    {
                        count = _records.Count;
                    }

                    while (count > 0)
                    {
                        TrackingRecord record = null;
                        lock (_records)
                        {
                            record = _records.Dequeue();
                            count = _records.Count;
                        }

                        if (record != null)
                        {
                            PersistRecord(record);
                        }
                    }
                }
            }
        }

        private void PersistRecord(TrackingRecord tr)
        {
            try
            {
                String path = Path.Combine(
                    Environment.CurrentDirectory, "tracking");
                String fileName = String.Format("{0}.{1}",
                    tr.EventTime.ToString("yyyyMMdd.HHmmss.fffffff"),
                    tr.RecordNumber);
                String fullPath = Path.Combine(path, fileName + ".xml");

                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                using (FileStream stream =
                    new FileStream(fullPath, FileMode.Create))
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding = Encoding.UTF8;
                    using (XmlWriter writer = XmlWriter.Create(stream, settings))
                    {
                        writer.WriteRaw(TrackingRecordSerializer.Serialize(tr));
                    }
                }
            }
            catch (IOException exception)
            {
                Console.WriteLine(
                    "PersistRecord Exception: {0}", exception.Message);
            }
        }
    }
}
