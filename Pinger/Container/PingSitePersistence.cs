using System;
using Pinger.Util;

// ReSharper disable AutoPropertyCanBeMadeGetOnly.Global
// Used by Jot when loading persisted object

namespace Pinger.Container {
    public class PingSitePersistence {
        public string ChartColorHex { get; set; }
        public string RawSiteUri { get; set; }
        public bool Selected { get; set; }

        // ReSharper disable once UnusedMember.Global
        // Used by Jot when loading persisted object
        public PingSitePersistence() {}

        public PingSitePersistence(PingSite site, bool selected) {
            ChartColorHex = ColorUtil.ToHex(site.ChartColor);
            RawSiteUri = site.Location.AbsoluteUri;
            Selected = selected;
        }

        public PingSite GetInstance() {
            return new PingSite(new Uri(RawSiteUri), ColorUtil.FromHex(ChartColorHex));
        }
    }
}