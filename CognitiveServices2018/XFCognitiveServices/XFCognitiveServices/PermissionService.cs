using System;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace XFCognitiveServices
{
    public static class PermissionService
    {
        /// <summary>
        /// カメラとストレージのパーミッションを確認
        /// </summary>
        /// <returns>パーミッションが許可されているか？</returns>
        public static async Task<bool> CheckPermissionGranted()
        {
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);

            if (cameraStatus != PermissionStatus.Granted || storageStatus != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Camera, Permission.Storage });
                cameraStatus = results[Permission.Camera];
                storageStatus = results[Permission.Storage];
            }

            if (cameraStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
                return true;
            else
                return false;
        }

        public static bool CheckFeaturesAvalable() =>
            CrossMedia.Current.IsTakePhotoSupported &&
            CrossMedia.Current.IsPickPhotoSupported;
    }
}
