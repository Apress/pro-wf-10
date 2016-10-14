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
using System.Configuration;
using System.Linq;
using System.ServiceModel.Activities.Tracking.Configuration;

namespace ActivityLibrary
{
    public class TrackingProfileLoader
    {
        public TrackingProfile Profile { get; set; }

        public TrackingProfileLoader(String profileName)
        {
            LoadConfig(profileName);
        }

        private void LoadConfig(String profileName)
        {
            TrackingSection ts =
                (TrackingSection)ConfigurationManager.GetSection(
                    "system.serviceModel/tracking");
            if (ts != null && ts.TrackingProfiles != null)
            {
                TrackingProfile profile =
                    (from tp in ts.TrackingProfiles
                     where tp.Name == profileName
                     select tp).SingleOrDefault();
                if (profile != null)
                {
                    Profile = profile;
                }
            }

            if (Profile == null)
            {
                throw new ArgumentException(String.Format(
                    "Tracking Profile {0} not found in app.config",
                    profileName));
            }
        }
    }
}
