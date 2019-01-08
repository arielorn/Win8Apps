using System;
using System.Collections.Generic;

namespace Win8Apps.Model
{
    public class App
    {
        public string Id { get; set; }
        public string FriendlyUrl { get; set; }

        public Guid AppId { get; set; }

        public string PackageFamilyName { get; set; }

        public Guid R { get; set; }

        public Guid B { get; set; }

        public string Language { get; set; }

        public string Title { get; set; }

        public string AgeRating { get; set; }

        public string Icon { get; set; }

        public string Background { get; set; }

        public string Foreground { get; set; }

        public float ScoreRating { get; set; }

        public Price Price { get; set; }
        public Price PreviousPrice { get; set; }

        public Category Category { get; set; }

        public Category SubCategory { get; set; }

        public int Type { get; set; }

        public bool Accessibility { get; set; }

        public bool Dca { get; set; }

        public bool Trial { get; set; }

        public DateTime LastUpdated { get; set; }

        public int S { get; set; }

        public string Description { get; set; }

        public List<string> Features { get; set; }

        public string ReleaseNotes { get; set; }

        public int RatingCount { get; set; }

        public string Oc { get; set; }

        public List<string> SupportedLanguages { get; set; }

        public string Developer { get; set; }

        public long Version { get; set; }

        public string Copyrights { get; set; }

        public string Website { get; set; }

        public string SupportWebsite { get; set; }

        public string PrivacyPolicy { get; set; }

        public string Eula { get; set; }

        public long DownloadSize { get; set; }

        public List<string> SupportedPlatforms { get; set; }

        public List<AppCapabilities> Permissions { get; set; }

        public List<string> RecommendedHardware { get; set; }

        public List<Screenshot> Screenshots { get; set; }

        public List<string> Countries { get; set; }

        public List<Channel> Channels { get; set; }

        public float ReviewsRating { get; set; }

        public bool IsActive { get; set; }
    }
}