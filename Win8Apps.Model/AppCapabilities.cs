using System.ComponentModel;

namespace Win8Apps.Model
{
    public enum AppCapabilities
    {
        [Description("Your Internet connection.")]
        APPX_CAPABILITY_INTERNET_CLIENT = 0x0000000000000001,
        [Description("Your Internet connection.")]
        APPX_CAPABILITY_INTERNET_CLIENT_SERVER = 0x0000000000000002,
        [Description("A home or work network.")]
        APPX_CAPABILITY_PRIVATE_NETWORK_CLIENT_SERVER = 0x0000000000000004,
        [Description("Your documents library.")]
        APPX_CAPABILITY_DOCUMENTS_LIBRARY = 0x0000000000000008,
        [Description("Your pictures library.")]
        APPX_CAPABILITY_PICTURES_LIBRARY = 0x0000000000000010,
        [Description("Your videos library.")]
        APPX_CAPABILITY_VIDEOS_LIBRARY = 0x0000000000000020,
        [Description("Your music library and playlists.")]
        APPX_CAPABILITY_MUSIC_LIBRARY = 0x0000000000000040,
        [Description("Your Windows credentials.")]
        APPX_CAPABILITY_ENTERPRISE_AUTHENTICATION = 0x0000000000000080,
        [Description("Software and hardware certificates or a smart card .")]
        APPX_CAPABILITY_SHARED_USER_CERTIFICATES = 0x0000000000000100,
        [Description("Removable storage.")]
        APPX_CAPABILITY_REMOVABLE_STORAGE = 0x0000000000000200
    }
}